using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NeHive.Generator
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

                    var classSymbol = semanticModel.GetDeclaredSymbol(classDecl);
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
            return symbol.GetAttributes()
                .Any(attr => attr.AttributeClass?.Name == "StoreAttribute");
        }

        // ======================================================
        // 主要代码生成逻辑
        // ======================================================
        private string GenerateStoreClass(
            ClassDeclarationSyntax originalClass,
            SemanticModel semanticModel,
            INamedTypeSymbol classSymbol)
        {
            // 准备命名空间和新类名
            var namespaceName = originalClass.Parent is BaseNamespaceDeclarationSyntax ns ? ns.Name.ToString() : null;
            var className = originalClass.Identifier.Text;
            var newClassName = className + "Store";
            var usingNamespaces = new HashSet<string>
            {
                "System",
                "NeHive.Core"
            };
            CollectNameSpaces(classSymbol.BaseType, usingNamespaces);
            foreach (var i in classSymbol.Interfaces)
                CollectNameSpaces(i, usingNamespaces);
            var walker = new TypeCollector(semanticModel, usingNamespaces);

            // 收集需要处理的成员信息

            var properties = CollectProperties(classSymbol, originalClass);
            var fields = CollectFields(classSymbol);
            var methods = CollectMethods(classSymbol, originalClass);
            var ctors = originalClass.Members
                .OfType<ConstructorDeclarationSyntax>();

            var signalProps = properties.Where(p => !p.NoSignal && !p.IsDerived).ToList();
            var memoProps = properties.Where(p => p.IsComputedDerived).ToList();
            var needScope = memoProps.Count > 0;
            var scopeName = $"_{Util.LowerFirst(newClassName)}Scope";

            // 使用 SyntaxFactory 构建新类
            var newClass = ClassDeclaration(newClassName)
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(originalClass.ParameterList);

            // 收集所有要添加到类中的成员
            var members = new List<MemberDeclarationSyntax>();

            // scope 字段
            if (needScope)
            {
                var scopeType =
                    IdentifierName("Scope");
                var variable = VariableDeclarator(scopeName)
                    .WithInitializer(
                        EqualsValueClause(
                            ObjectCreationExpression(scopeType)
                                .WithArgumentList(
                                    ArgumentList()
                                )
                        )
                    );

                var scopeField =
                    FieldDeclaration(
                            VariableDeclaration(scopeType)
                                .WithVariables(
                                    SingletonSeparatedList(variable)
                                )
                        )
                        // private readonly
                        .WithModifiers(TokenList(
                            Token(SyntaxKind.PrivateKeyword),
                            Token(SyntaxKind.ReadOnlyKeyword)
                        ));

                members.Add(scopeField);
            }

            // 1. 信号字段
            foreach (var prop in signalProps)
            {
                var propSyntax = originalClass.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .First(p => p.Identifier.Text == prop.Name);
                walker.Visit(propSyntax);

                var signalType =
                    GenericName("Signal")
                        .WithTypeArgumentList(
                            TypeArgumentList(
                                SingletonSeparatedList(
                                    GetTypeSyntax(prop.Type, usingNamespaces)
                                )
                            )
                        );

                var variable = VariableDeclarator(prop.FieldName);

                // 处理 initializer
                // 优先使用属性 initializer
                var initExpr = propSyntax.Initializer?.Value
                               ?? LiteralExpression(SyntaxKind.DefaultLiteralExpression);

                variable = variable.WithInitializer(
                    EqualsValueClause(
                        ObjectCreationExpression(signalType)
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(initExpr)
                                    )
                                )
                            )
                    )
                );

                var signalField =
                    FieldDeclaration(
                            VariableDeclaration(signalType)
                                .WithVariables(
                                    SingletonSeparatedList(variable)
                                )
                        )
                        // private readonly
                        .WithModifiers(TokenList(
                            Token(SyntaxKind.PrivateKeyword),
                            Token(SyntaxKind.ReadOnlyKeyword)
                        ));

                members.Add(signalField);
            }

            // Computed字段
            foreach (var prop in memoProps)
            {
                CollectNameSpaces(prop.Type, usingNamespaces);
                var memoType = GenericName("Computed")
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList(
                                GetTypeSyntax(prop.Type, usingNamespaces)
                            )
                        )
                    );
                var nullableComputedType = NullableType(memoType);

                var variable = VariableDeclarator(prop.FieldName);
                var memoField = FieldDeclaration(
                        VariableDeclaration(nullableComputedType)
                            .WithVariables(SingletonSeparatedList(variable))
                    )
                    // private
                    .WithModifiers(TokenList(
                        Token(SyntaxKind.PrivateKeyword)
                    ));

                members.Add(memoField);
            }

            // 2. 原有字段（直接复制）
            foreach (var field in fields)
            {
                var fieldSyntax = originalClass.Members.OfType<FieldDeclarationSyntax>()
                    .FirstOrDefault(f => f.Declaration.Variables.First().Identifier.Text == field.Name);
                if (fieldSyntax == null) continue;
                walker.Visit(fieldSyntax);
                members.Add(fieldSyntax);
            }

            // 3. 信号属性（转换后的）
            foreach (var prop in signalProps)
            {
                var propSyntax = originalClass.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .First(p => p.Identifier.Text == prop.Name);

                walker.Visit(propSyntax);
                // 获取原属性的访问器
                AccessorDeclarationSyntax getter;
                AccessorDeclarationSyntax setter;

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
                var newProp = PropertyDeclaration(
                    propSyntax.AttributeLists,
                    propSyntax.Modifiers,
                    propSyntax.Type,
                    propSyntax.ExplicitInterfaceSpecifier,
                    propSyntax.Identifier,
                    AccessorList(
                        List(new[] { getter, setter }
                            .Where(a => a != null)
                        )
                    )
                );

                members.Add(newProp);
            }

            // Computed属性
            foreach (var prop in memoProps)
            {
                var propSyntax = originalClass.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .First(p => p.Identifier.Text == prop.Name);
                walker.Visit(propSyntax);

                var getter = propSyntax.AccessorList?.Accessors.FirstOrDefault(a =>
                    a.Kind() == SyntaxKind.GetAccessorDeclaration);
                var getterBlock = getter?.Body;

                if (getterBlock == null)
                {
                    var exp = getter is null
                        ? propSyntax.ExpressionBody?.Expression // 处理像：public int ScoreAndLevel => Score + Level; 这种情况
                        : getter.ExpressionBody
                            ?.Expression; // 处理像：public int ScoreAndLevel { get => Score + Level; } 这种情况
                    getterBlock = Block
                    (
                        ReturnStatement(exp)
                    );
                }

                var assignment =
                    AssignmentExpression(
                        SyntaxKind.CoalesceAssignmentExpression, // ??=
                        IdentifierName(prop.FieldName), // _scoreAndLevelSignal
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(scopeName),
                                IdentifierName("AddComputed")
                            )
                        ).WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList(
                                    Argument(
                                        ParenthesizedLambdaExpression(
                                            ParameterList(),
                                            getterBlock
                                        )
                                    )
                                )
                            )
                        ) // scope.AddComputed(() => store.Count * 2);
                    );

                var propertyGetter =
                    ArrowExpressionClause(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ParenthesizedExpression(assignment),
                            IdentifierName("Value")
                        )
                    );

                var newProp =
                    PropertyDeclaration(
                            propSyntax.Type,
                            propSyntax.Identifier
                        )
                        .WithAttributeLists(propSyntax.AttributeLists)
                        .WithModifiers(propSyntax.Modifiers)
                        .WithExplicitInterfaceSpecifier(propSyntax.ExplicitInterfaceSpecifier)
                        .WithExpressionBody(propertyGetter).WithSemicolonToken(
                            Token(SyntaxKind.SemicolonToken)
                        );

                members.Add(newProp);
            }

            // 4. 非信号属性（直接复制）
            var nonSignalProps = properties.Where(p => p.NoSignal).ToList();
            foreach (var prop in nonSignalProps)
            {
                var propSyntax = originalClass.Members.OfType<PropertyDeclarationSyntax>()
                    .FirstOrDefault(p => p.Identifier.Text == prop.Name);
                if (propSyntax == null) continue;

                walker.Visit(propSyntax);
                members.Add(propSyntax);
            }

            // 5. 非Computed派生属性（直接复制）
            var derivedProps = properties.Where(p => p.IsDerived && !p.IsComputedDerived).ToList();
            foreach (var prop in derivedProps)
            {
                var propSyntax = originalClass.Members.OfType<PropertyDeclarationSyntax>()
                    .FirstOrDefault(p => p.Identifier.Text == prop.Name);
                if (propSyntax == null) continue;

                walker.Visit(propSyntax);
                members.Add(propSyntax);
            }

            // 6. 构造函数（转换后的）
            foreach (var ctor in ctors)
            {
                var newCtor = ctor
                    // 🔥 改类名
                    .WithIdentifier(Identifier(newClassName));

                members.Add(newCtor);
            }

            // 7. 方法（包裹或直接复制）
            foreach (var method in methods)
            {
                var methodSyntax = originalClass.Members.OfType<MethodDeclarationSyntax>()
                    .FirstOrDefault(m => m.Identifier.Text == method.Name);
                if (methodSyntax == null) continue;
                walker.Visit(methodSyntax);

                var newMethod = method.ShouldWrap
                    ? GenerateWrappedMethod(methodSyntax)
                    : methodSyntax;
                members.Add(newMethod);
            }

            if (needScope)
            {
                var disposeScope =
                    InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(scopeName),
                            IdentifierName("Dispose")
                        )
                    ).WithArgumentList(ArgumentList());

                var methodSyntax =
                    MethodDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.VoidKeyword)
                            ), "Dispose"
                        )
                        .AddModifiers(
                            Token(SyntaxKind.PublicKeyword)
                        )
                        .WithBody(
                            Block()
                                .AddStatements(
                                    ExpressionStatement(disposeScope)
                                )
                        );

                members.Add(methodSyntax);
            }

            // 8. 其他成员（事件、索引器等）直接复制
            var otherMembers = originalClass.Members
                .Where(m => !(m is ConstructorDeclarationSyntax
                              || m is FieldDeclarationSyntax
                              || m is PropertyDeclarationSyntax
                              || m is MethodDeclarationSyntax));
            members.AddRange(otherMembers);

            // 组装类
            newClass = newClass.WithMembers(List(members));

            // 包装到命名空间
            CompilationUnitSyntax compilationUnit;
            if (namespaceName != null)
            {
                var namespaceDecl = NamespaceDeclaration(ParseName(namespaceName))
                    .AddMembers(newClass);
                compilationUnit = CompilationUnit()
                    .AddMembers(namespaceDecl);
            }
            else
            {
                compilationUnit = CompilationUnit()
                    .AddMembers(newClass);
            }

            // 添加 using 语句
            var currentNs = classSymbol.ContainingNamespace.ToDisplayString();
            compilationUnit = compilationUnit.AddUsings(
                usingNamespaces
                    .Where(space => space != currentNs && !string.IsNullOrWhiteSpace(space))
                    .Distinct()
                    .OrderBy(space => space)
                    .Select(spaces => UsingDirective(ParseName(spaces)))
                    .ToArray()
            );

            // 格式化并返回
            return compilationUnit.NormalizeWhitespace().ToFullString();
        }

        // ======================================================
        // 辅助类和方法
        // ======================================================
        private static void CollectNameSpaces(ITypeSymbol type, HashSet<string> usings)
        {
            if (type == null) return;

            var space = type.ContainingNamespace;
            if (space != null && !space.IsGlobalNamespace)
                usings.Add(space.ToDisplayString());

            switch (type)
            {
                case INamedTypeSymbol named:
                    foreach (var arg in named.TypeArguments)
                        CollectNameSpaces(arg, usings);
                    break;

                case IArrayTypeSymbol array:
                    CollectNameSpaces(array.ElementType, usings);
                    break;

                case IPointerTypeSymbol pointer:
                    CollectNameSpaces(pointer.PointedAtType, usings);
                    break;
            }
        }

        // 类型转换函数（智能简化）
        private static TypeSyntax GetTypeSyntax(ITypeSymbol type, HashSet<string> usings)
        {
            var space = type.ContainingNamespace?.ToDisplayString();

            var format = usings.Contains(space)
                ? SymbolDisplayFormat.MinimallyQualifiedFormat
                : SymbolDisplayFormat.FullyQualifiedFormat;

            return ParseTypeName(type.ToDisplayString(format));
        }

        private class PropertyInfo
        {
            public string Name;
            public ITypeSymbol Type;

            public bool IsAutoGetter;
            public bool GetterHasField;
            public bool IsAutoSetter;
            public bool SetterHasField;

            public bool NoSignal;
            public bool IsDerived; // ⭐ 业务
            public bool IsComputedDerived; // ⭐ 业务

            public string FieldName => $"_{Util.LowerFirst(Name)}Signal";
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
                var isComputed = false;
                if (isDerived)
                    isComputed = prop.GetAttributes()
                        .Any(a => a.AttributeClass?.Name == "ComputedAttribute");

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
                    Type = prop.Type,
                    NoSignal = noSignal,
                    IsAutoGetter = isAutoGet,
                    GetterHasField = getterHasField,
                    IsAutoSetter = isAutoSet,
                    SetterHasField = setterHasField,
                    IsDerived = isDerived,
                    IsComputedDerived = isComputed
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

        private static List<MethodInfo> CollectMethods(INamedTypeSymbol classSymbol,
            ClassDeclarationSyntax originalClass)
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
                    return ParseExpression($"{_fieldName}.Value");
                }

                return base.Visit(node);
            }
        }

        private static AccessorDeclarationSyntax GenerateSignalAccessor(PropertyDeclarationSyntax originalProp,
            string fieldName, SyntaxKind kind)
        {
            var accessor = originalProp.AccessorList?.Accessors.FirstOrDefault(a =>
                a.Kind() == kind);
            if (accessor is null) return null;
            if (kind == SyntaxKind.GetAccessorDeclaration)
            {
                return AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithModifiers(accessor.Modifiers)
                    .WithBody(Block(
                        ReturnStatement(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(fieldName),
                                IdentifierName("Value")
                            )
                        )
                    ));
            }

            if (kind == SyntaxKind.SetAccessorDeclaration)
            {
                return AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                    .WithModifiers(accessor.Modifiers)
                    .WithBody(Block(
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(fieldName),
                                    IdentifierName("Value")
                                ),
                                IdentifierName("value")
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
            var returnVoid = returnType is PredefinedTypeSyntax pts &&
                             pts.Keyword.IsKind(SyntaxKind.VoidKeyword);

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
            var lambdaBody = Block(bodyStatements);
            var batchInvocation = InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Reactive"),
                        IdentifierName("Batch")
                    )
                )
                .WithArgumentList(ArgumentList(
                    SingletonSeparatedList(
                        Argument(ParenthesizedLambdaExpression(lambdaBody))
                    )
                ));

            StatementSyntax statement;
            if (returnVoid) statement = ExpressionStatement(batchInvocation);
            else statement = ReturnStatement(batchInvocation);

            var newBody = Block(statement);

            // 构建新方法
            var newMethod = MethodDeclaration(
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

    internal class TypeCollector : CSharpSyntaxWalker
    {
        private readonly SemanticModel _semanticModel;
        private readonly HashSet<string> _namespaces;

        public TypeCollector(SemanticModel semanticModel, HashSet<string> namespaces)
        {
            _semanticModel = semanticModel;
            _namespaces = namespaces;
        }

        public override void Visit(SyntaxNode node)
        {
            if (node == null) return;

            // 类型信息
            var typeInfo = _semanticModel.GetTypeInfo(node).Type;
            Add(typeInfo);

            // 符号信息（方法 / 属性 / 静态调用）
            var symbol = _semanticModel.GetSymbolInfo(node).Symbol;

            switch (symbol)
            {
                case IMethodSymbol m:
                    Add(m.ContainingType);
                    break;

                case IPropertySymbol p:
                    Add(p.ContainingType);
                    break;

                case INamedTypeSymbol t:
                    Add(t);
                    break;
            }

            base.Visit(node);
        }

        private void Add(ITypeSymbol type)
        {
            if (type == null) return;

            var ns = type.ContainingNamespace;
            if (ns != null && !ns.IsGlobalNamespace)
            {
                _namespaces.Add(ns.ToDisplayString());
            }

            switch (type)
            {
                case INamedTypeSymbol named:
                    foreach (var arg in named.TypeArguments)
                        Add(arg);
                    break;

                case IArrayTypeSymbol array:
                    Add(array.ElementType);
                    break;

                case IPointerTypeSymbol pointer:
                    Add(pointer.PointedAtType);
                    break;
            }
        }
    }
}