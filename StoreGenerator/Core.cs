using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StoreGenerator
{
    [Generator(LanguageNames.CSharp)]
    public class StoreIncrementalGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // 1. 获取所有标记了 [Store] 的类声明
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (node, _) =>
                        node is ClassDeclarationSyntax classDecl && classDecl.AttributeLists.Count > 0,
                    transform: (ctx, _) => GetClassWithStoreAttribute(ctx))
                .Where(c => c != null);

            // 2. 结合编译信息
            var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

            // 3. 生成代码
            context.RegisterSourceOutput(compilationAndClasses, (spc, source) =>
            {
                var (compilation, classes) = source;
                foreach (var classDecl in classes)
                {
                    var semanticModel = compilation.GetSemanticModel(classDecl.SyntaxTree);
                    if (semanticModel == null) continue;

                    var classSymbol = semanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
                    if (classSymbol == null) continue;

                    // 检查是否有 [Store] 特性（已由 predicate 过滤，但保留双重保险）
                    if (!HasStoreAttribute(classSymbol)) continue;

                    // 生成新类代码
                    var generatedCode = GenerateStoreClass(classDecl, semanticModel, classSymbol);
                    spc.AddSource($"{classDecl.Identifier.Text}Store.g.cs",
                        SourceText.From(generatedCode, Encoding.UTF8));
                }
            });
        }

        private static ClassDeclarationSyntax GetClassWithStoreAttribute(GeneratorSyntaxContext context)
        {
            var classDecl = (ClassDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(classDecl);
            if (symbol != null && HasStoreAttribute(symbol))
                return classDecl;
            return null;
        }

        private static bool HasStoreAttribute(ISymbol symbol)
        {
            return symbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "StoreAttribute");
        }

        // ======================================================
        // 主要代码生成逻辑
        // ======================================================
        private string GenerateStoreClass(ClassDeclarationSyntax originalClass, SemanticModel semanticModel,
            INamedTypeSymbol classSymbol)
        {
            // 准备命名空间和新类名
            var namespaceName = originalClass.Parent is BaseNamespaceDeclarationSyntax ns ? ns.Name.ToString() : null;
            var className = originalClass.Identifier.Text;
            var newClassName = className + "Store";

            // 收集需要处理的成员信息
            var properties = CollectProperties(classSymbol, originalClass);
            var fields = CollectFields(classSymbol);
            var methods = CollectMethods(classSymbol, originalClass);
            var constructors = classSymbol.Constructors.Where(c => !c.IsStatic).ToList();

            // 使用 SyntaxFactory 构建新类
            var newClass = SyntaxFactory.ClassDeclaration(newClassName)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)));

            // 收集所有要添加到类中的成员
            var members = new List<MemberDeclarationSyntax>();

            // 1. 信号字段
            var signalProps = properties.Where(p => !p.NoSignal && !p.IsDerived).ToList();
            foreach (var prop in signalProps)
            {
                var field = SyntaxFactory.FieldDeclaration(
                        SyntaxFactory.VariableDeclaration(
                                SyntaxFactory.GenericName("Signal")
                                    .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                            SyntaxFactory.ParseTypeName(prop.Type)))))
                            .WithVariables(SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.VariableDeclarator(prop.FieldName))))
                    .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                        SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)));
                members.Add(field);
            }

            // 2. 原有字段（直接复制）
            foreach (var field in fields)
            {
                var fieldSyntax = originalClass.Members.OfType<FieldDeclarationSyntax>()
                    .FirstOrDefault(f => f.Declaration.Variables.First().Identifier.Text == field.Name);
                if (fieldSyntax != null)
                    members.Add(fieldSyntax);
            }

            // 3. 构造函数（转换后的）
            foreach (var ctor in constructors)
            {
                var ctorSyntax = originalClass.Members.OfType<ConstructorDeclarationSyntax>()
                    .FirstOrDefault(c => c.Identifier.Text == className);
                if (ctorSyntax is null) continue;
                var newCtor = GenerateConstructor(ctorSyntax, signalProps, newClassName);
                members.Add(newCtor);
            }

            // 4. 信号属性（转换后的）
            foreach (var prop in signalProps)
            {
                var propSyntax = originalClass.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .First(p => p.Identifier.Text == prop.Name);

                PropertyDeclarationSyntax newProp;

                // 获取原属性的访问器
                AccessorDeclarationSyntax getter;
                AccessorDeclarationSyntax setter = null;

                if (prop.IsAutoGetter)
                {
                    getter = GenerateSignalAccessor(propSyntax, prop.FieldName, SyntaxKind.GetAccessorDeclaration);
                }
                else if (prop.GetterHasField)
                {
                    getter = RewriteFieldProperty(propSyntax, prop.FieldName, SyntaxKind.GetAccessorDeclaration);
                }
                else
                {
                    getter = propSyntax.AccessorList?.Accessors.FirstOrDefault(a =>
                        a.Kind() == SyntaxKind.GetAccessorDeclaration);
                }

                if (prop.IsAutoSetter)
                {
                    setter = GenerateSignalAccessor(propSyntax, prop.FieldName, SyntaxKind.SetAccessorDeclaration);
                }
                else if (prop.SetterHasField)
                {
                    setter = RewriteFieldProperty(propSyntax, prop.FieldName, SyntaxKind.SetAccessorDeclaration);
                }
                else
                {
                    setter = propSyntax.AccessorList?.Accessors.FirstOrDefault(a =>
                        a.Kind() == SyntaxKind.SetAccessorDeclaration);
                }

                // 组装新属性
                newProp = SyntaxFactory.PropertyDeclaration(
                    propSyntax.AttributeLists,
                    propSyntax.Modifiers,
                    propSyntax.Type,
                    propSyntax.ExplicitInterfaceSpecifier,
                    propSyntax.Identifier,
                    SyntaxFactory.AccessorList(
                        SyntaxFactory.List(new[] { getter, setter }
                            .Where(a => a != null)
                        )
                    )
                );

                members.Add(newProp);
            }

            // 5. 非信号属性（直接复制）
            var nonSignalProps = properties.Where(p => p.NoSignal).ToList();
            foreach (var prop in nonSignalProps)
            {
                var propSyntax = originalClass.Members.OfType<PropertyDeclarationSyntax>()
                    .FirstOrDefault(p => p.Identifier.Text == prop.Name);
                if (propSyntax != null)
                    members.Add(propSyntax);
            }

            // 6. 派生属性（直接复制）
            var derivedProps = properties.Where(p => p.IsDerived).ToList();
            foreach (var prop in derivedProps)
            {
                var propSyntax = originalClass.Members.OfType<PropertyDeclarationSyntax>()
                    .FirstOrDefault(p => p.Identifier.Text == prop.Name);
                if (propSyntax != null)
                    members.Add(propSyntax);
            }

            // 7. 方法（包裹或直接复制）
            foreach (var method in methods)
            {
                var methodSyntax = originalClass.Members.OfType<MethodDeclarationSyntax>()
                    .FirstOrDefault(m => m.Identifier.Text == method.Name);
                if (methodSyntax == null) continue;
                var newMethod = method.ShouldWrap
                    ? GenerateWrappedMethod(methodSyntax)
                    : methodSyntax;
                members.Add(newMethod);
            }

            // 8. 其他成员（事件、索引器等）直接复制
            var otherMembers = originalClass.Members
                .Where(m => !(m is ConstructorDeclarationSyntax
                              || m is FieldDeclarationSyntax
                              || m is PropertyDeclarationSyntax
                              || m is MethodDeclarationSyntax));
            members.AddRange(otherMembers);

            // 组装类
            newClass = newClass.WithMembers(SyntaxFactory.List(members));

            // 包装到命名空间
            CompilationUnitSyntax compilationUnit;
            if (namespaceName != null)
            {
                var namespaceDecl = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName))
                    .AddMembers(newClass);
                compilationUnit = SyntaxFactory.CompilationUnit()
                    .AddMembers(namespaceDecl);
            }
            else
            {
                compilationUnit = SyntaxFactory.CompilationUnit()
                    .AddMembers(newClass);
            }

            // 添加 using 语句
            compilationUnit = compilationUnit.AddUsings(
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Lib")));

            // 格式化并返回
            return compilationUnit.NormalizeWhitespace().ToFullString();
        }

        // ======================================================
        // 辅助类和方法
        // ======================================================

        private class PropertyInfo
        {
            public string Name;
            public string Type;

            public bool IsAutoGetter;
            public bool GetterHasField;
            public bool IsAutoSetter;
            public bool SetterHasField;

            public bool NoSignal;
            public bool IsDerived; // ⭐ 业务

            public string FieldName => $"_{Name.ToLowerInvariant()}Signal";
        }

        private static List<PropertyInfo> CollectProperties(
            INamedTypeSymbol classSymbol,
            ClassDeclarationSyntax classSyntax)
        {
            var result = new List<PropertyInfo>();

            // 👉 预先拿 backing field（高性能）
            var backingFields = classSymbol
                .GetMembers()
                .OfType<IFieldSymbol>()
                .Where(f => f.AssociatedSymbol is IPropertySymbol)
                .ToList();

            // 👉 建立 syntax 索引（避免重复查）
            var propSyntaxMap = classSyntax.Members
                .OfType<PropertyDeclarationSyntax>()
                .ToDictionary(p => p.Identifier.ValueText);

            foreach (var member in classSymbol.GetMembers())
            {
                if (!(member is IPropertySymbol prop) ||
                    prop.DeclaredAccessibility != Accessibility.Public)
                    continue;

                var propSyntax = propSyntaxMap[prop.Name];

                // ✅ 特性
                var noSignal = prop.GetAttributes()
                    .Any(a => a.AttributeClass?.Name == "NoSignalAttribute");

                // ✅ backing field
                var hasBackingField = backingFields
                    .Any(f => SymbolEqualityComparer.Default.Equals(f.AssociatedSymbol, prop));

                // ✅ 派生属性
                var isDerived = !hasBackingField;

                var getter =
                    propSyntax.AccessorList?.Accessors.FirstOrDefault(a =>
                        a.Kind() == SyntaxKind.GetAccessorDeclaration);
                var setter =
                    propSyntax.AccessorList?.Accessors.FirstOrDefault(a =>
                        a.Kind() == SyntaxKind.SetAccessorDeclaration);

                var isAutoGet = !(getter is null) && getter.Body is null && getter.ExpressionBody is null;
                var getterHasField = getter?.DescendantNodes()
                    .OfType<FieldExpressionSyntax>().Any() ?? false;

                var isAutoSet = !(setter is null) && setter.Body is null && setter.ExpressionBody is null;
                var setterHasField = setter?.DescendantNodes()
                    .OfType<FieldExpressionSyntax>().Any() ?? false;

                result.Add(new PropertyInfo
                {
                    Name = prop.Name,
                    Type = prop.Type.ToDisplayString(),
                    NoSignal = noSignal,
                    IsAutoGetter = isAutoGet,
                    GetterHasField = getterHasField,
                    IsAutoSetter = isAutoSet,
                    SetterHasField = setterHasField,
                    IsDerived = isDerived
                });
            }

            return result;
        }

        private static List<IFieldSymbol> CollectFields(INamedTypeSymbol classSymbol)
        {
            return classSymbol.GetMembers().OfType<IFieldSymbol>()
                .Where(f => !f.IsImplicitlyDeclared && f.DeclaredAccessibility == Accessibility.Public)
                .ToList();
        }

        private class MethodInfo
        {
            public string Name { get; set; }
            public bool ShouldWrap { get; set; }
        }

        private List<MethodInfo> CollectMethods(INamedTypeSymbol classSymbol, ClassDeclarationSyntax originalClass)
        {
            var result = new List<MethodInfo>();
            foreach (var member in classSymbol.GetMembers())
            {
                if (!(member is IMethodSymbol method)
                    || method.DeclaredAccessibility != Accessibility.Public) continue;
                var noBatch = method.GetAttributes().Any(attr => attr.AttributeClass?.Name == "NoBatchAttribute");
                // 默认包裹，除非标记了 NoBatch 或是抽象方法（没有方法体）
                var shouldWrap = !noBatch && !method.IsAbstract;
                result.Add(new MethodInfo
                {
                    Name = method.Name,
                    ShouldWrap = shouldWrap
                });
            }

            return result;
        }

        // ======================================================
        // 代码生成辅助方法（使用 SyntaxFactory）
        // ======================================================
        private AccessorDeclarationSyntax RewriteFieldProperty(
            PropertyDeclarationSyntax propSyntax,
            string fieldName, SyntaxKind kind)
        {
            var rewriter = new FieldRewriter(fieldName);
            var accessor = propSyntax.AccessorList?.Accessors.FirstOrDefault(a =>
                a.Kind() == kind);
            if (accessor is null) return null;

            return (AccessorDeclarationSyntax)rewriter.Visit(accessor);
        }

        private class FieldRewriter : CSharpSyntaxRewriter
        {
            private readonly string _fieldName;

            public FieldRewriter(string fieldName)
            {
                _fieldName = fieldName;
            }

            public override SyntaxNode Visit(SyntaxNode node)
            {
                if (node is FieldExpressionSyntax)
                {
                    return SyntaxFactory.ParseExpression($"{_fieldName}.Value");
                }

                return base.Visit(node);
            }
        }

        private static ConstructorDeclarationSyntax GenerateConstructor(ConstructorDeclarationSyntax originalCtor,
            List<PropertyInfo> signalProps, string newClassName)
        {
            // 保留原构造函数的修饰符、参数、基类调用
            var modifiers = originalCtor.Modifiers;
            var parameters = originalCtor.ParameterList;
            var initializer = originalCtor.Initializer;

            // 构建信号字段初始化语句
            var initStatements = new List<StatementSyntax>();
            foreach (var prop in signalProps)
            {
                string paramName = char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);
                initStatements.Add(
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.IdentifierName(prop.FieldName),
                            SyntaxFactory.ObjectCreationExpression(
                                SyntaxFactory.GenericName("Signal")
                                    .WithTypeArgumentList(SyntaxFactory.TypeArgumentList(
                                        SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                            SyntaxFactory.ParseTypeName(prop.Type)
                                        )
                                    )),
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                        SyntaxFactory.Argument(SyntaxFactory.IdentifierName(paramName)
                                        )
                                    )
                                ), null)
                        )
                    )
                );
            }

            // 复制原构造函数体
            var originalBodyStatements = originalCtor.Body?.Statements ?? new SyntaxList<StatementSyntax>();
            var newBody = SyntaxFactory.Block(initStatements.Concat(originalBodyStatements));

            // 使用新类名创建标识符
            var newIdentifier = SyntaxFactory.Identifier(newClassName);

            // 构建新构造函数，替换标识符
            return SyntaxFactory.ConstructorDeclaration(
                originalCtor.AttributeLists,
                modifiers,
                newIdentifier, // 关键修改：使用新类名
                parameters,
                initializer,
                newBody,
                originalCtor.SemicolonToken);
        }

        private static AccessorDeclarationSyntax GenerateSignalAccessor(PropertyDeclarationSyntax originalProp,
            string fieldName, SyntaxKind kind)
        {
            var accessor = originalProp.AccessorList?.Accessors.FirstOrDefault(a =>
                a.Kind() == kind);
            if (accessor is null) return null;
            if (kind == SyntaxKind.GetAccessorDeclaration)
            {
                return SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithModifiers(accessor.Modifiers)
                    .WithBody(SyntaxFactory.Block(
                        SyntaxFactory.ReturnStatement(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName(fieldName),
                                SyntaxFactory.IdentifierName("Value")
                            )
                        )
                    ));
            }

            if (kind == SyntaxKind.SetAccessorDeclaration)
            {
                return SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithModifiers(accessor.Modifiers)
                    .WithBody(SyntaxFactory.Block(
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(fieldName),
                                    SyntaxFactory.IdentifierName("Value")
                                ),
                                SyntaxFactory.IdentifierName("value")
                            )
                        )
                    ));
            }

            return null;
        }

        private static MethodDeclarationSyntax GenerateWrappedMethod(MethodDeclarationSyntax originalMethod)
        {
            // 保留原方法的所有修饰符、返回类型、参数、约束等
            var modifiers = originalMethod.Modifiers;
            var returnType = originalMethod.ReturnType;
            var identifier = originalMethod.Identifier;
            var typeParams = originalMethod.TypeParameterList;
            var parameters = originalMethod.ParameterList;
            var constraints = originalMethod.ConstraintClauses;

            // 获取原方法体语句（如果方法体为空，则不包裹）
            var bodyStatements = originalMethod.Body?.Statements ?? new SyntaxList<StatementSyntax>();
            if (bodyStatements.Count == 0 && originalMethod.ExpressionBody == null)
            {
                // 无方法体（抽象、extern 等），直接返回原方法
                return originalMethod;
            }

            // 构建 lambda 体
            var lambdaBody = SyntaxFactory.Block(bodyStatements);
            var batchInvocation = SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("Reactive"),
                        SyntaxFactory.IdentifierName("Batch")))
                .WithArgumentList(SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                        SyntaxFactory.Argument(SyntaxFactory.ParenthesizedLambdaExpression(lambdaBody)))));

            var newBody = SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(batchInvocation));

            // 构建新方法
            var newMethod = SyntaxFactory.MethodDeclaration(
                originalMethod.AttributeLists,
                modifiers,
                returnType,
                originalMethod.ExplicitInterfaceSpecifier,
                identifier,
                typeParams,
                parameters,
                constraints,
                newBody,
                null,
                originalMethod.SemicolonToken);

            return newMethod;
        }
    }
}