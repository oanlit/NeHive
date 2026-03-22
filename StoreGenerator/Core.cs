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
    public class SignalPropertyGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // 查找带有 GenerateSignalPropertiesAttribute 的类声明
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (node, _) =>
                        node is ClassDeclarationSyntax classDecl && classDecl.AttributeLists.Count > 0,
                    transform: (ctx, _) => GetClassSymbol(ctx))
                .Where(c => c != null);

            // 收集所有需要生成的类
            var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

            context.RegisterSourceOutput(compilationAndClasses, (spc, source) =>
            {
                var (compilation, classes) = source;
                foreach (var classSymbol in classes)
                {
                    GenerateCode(spc, classSymbol);
                }
            });
        }

        private INamedTypeSymbol GetClassSymbol(GeneratorSyntaxContext context)
        {
            var classDecl = (ClassDeclarationSyntax)context.Node;
            // 检查是否有 GenerateSignalPropertiesAttribute
            foreach (var attributeList in classDecl.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var symbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol;
                    if (symbol is IMethodSymbol methodSymbol &&
                        methodSymbol.ContainingType.ToDisplayString() == "Lib.StoreAttribute")
                    {
                        return context.SemanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;
                    }
                }
            }

            return null;
        }

        private void GenerateCode(SourceProductionContext context, INamedTypeSymbol classSymbol)
        {
            // 获取命名空间
            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            // 获取类名
            var className = classSymbol.Name;

            // 只获取 public partial 属性
            var properties = classSymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.DeclaredAccessibility == Accessibility.Public &&
                            p.SetMethod != null && p.GetMethod != null &&
                            p.IsPartial()); // 需要自己实现 IsPartial 扩展方法

            if (!properties.Any())
                return;

            // 构建主构造函数的参数列表和 Signal 字段初始化
            var paramList = new List<string>();
            var fieldInitializers = new List<string>();

            foreach (var prop in properties)
            {
                var propType = prop.Type.ToDisplayString();
                var propName = prop.Name;
                var paramName = propName.ToLowerInvariant(); // 约定：参数名小写

                paramList.Add($"{propType} {paramName}");
                fieldInitializers.Add($"private readonly Signal<{propType}> _{paramName} = new({paramName});");
            }
            
            // 构建属性实现
            var propertyImpls = new List<string>();
            foreach (var prop in properties)
            {
                var propType = prop.Type.ToDisplayString();
                var propName = prop.Name;
                var paramName = propName.ToLowerInvariant();
                var getAccessibility = prop.GetMethod?.DeclaredAccessibility ?? Accessibility.NotApplicable;
                var setAccessibility = prop.SetMethod?.DeclaredAccessibility ?? Accessibility.NotApplicable;
                var isInitOnly = prop.SetMethod?.IsInitOnly ?? false;
                var propAccessibility = prop.DeclaredAccessibility;
                var getModifier = GetAccessibilityKeyword(getAccessibility, propAccessibility);
                var setModifier = GetAccessibilityKeyword(setAccessibility, propAccessibility);
                var setKeyword = isInitOnly ? "init" : "set";

                propertyImpls.Add($@"
    public partial {propType} {propName}
    {{
        {getModifier}get => _{paramName}.Value;
        {setModifier}{setKeyword} => _{paramName}.Value = value;
    }}");
            }

            // 生成完整的类代码
            var code = $@"using Lib;

namespace {namespaceName};

public partial class {className}({string.Join(", ", paramList)})
{{{string.Join("", propertyImpls)}

    {string.Join("\n    ", fieldInitializers)}
}}";

            // 输出源文件
            context.AddSource($"{className}_Store.g.cs", SourceText.From(code, Encoding.UTF8));
        }

        private string GetAccessibilityKeyword(Accessibility accessibility, Accessibility propertyAccessibility)
        {
            if (accessibility == propertyAccessibility) return ""; // 默认省略
            switch (accessibility)
            {
                case Accessibility.Public:
                    return "public ";
                case Accessibility.Private:
                    return "private ";
                case Accessibility.Protected:
                    return "protected ";
                case Accessibility.Internal:
                    return "internal ";
                case Accessibility.ProtectedOrInternal:
                    return "protected internal ";
                case Accessibility.ProtectedAndInternal:
                    return "protected internal ";
                default:
                    return "";
            }
        }
    }

    // 辅助扩展方法：判断属性是否为自动属性（通过检查是否有访问器体）
    public static class SymbolExtensions
    {
        public static bool IsPartial(this IPropertySymbol property)
        {
            var syntaxRef = property.DeclaringSyntaxReferences.FirstOrDefault();
            if (syntaxRef?.GetSyntax() is PropertyDeclarationSyntax propDecl)
            {
                return propDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
            }

            return false;
        }
    }
}