using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text.Json;

namespace Serenity.Net.Services.Generators;

[Generator]
public class PropertyMetadataGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // no init required
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var propertyData = new List<Dictionary<string, object>>();
        foreach (var tree in context.Compilation.SyntaxTrees)
        {
            var semanticModel = context.Compilation.GetSemanticModel(tree);
            var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var cls in classes)
            {
                if (semanticModel.GetDeclaredSymbol(cls) is not INamedTypeSymbol symbol)
                    continue;

                bool hasAttribute = symbol.GetAttributes().Any(a => a.AttributeClass?.Name.EndsWith("FormScriptAttribute") == true ||
                                                                      a.AttributeClass?.Name.EndsWith("ColumnsScriptAttribute") == true);
                if (!hasAttribute)
                    continue;

                var props = symbol.GetMembers().OfType<IPropertySymbol>()
                    .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic);

                foreach (var prop in props)
                {
                    propertyData.Add(new Dictionary<string, object>
                    {
                        ["Type"] = symbol.ToDisplayString(),
                        ["Property"] = prop.Name,
                        ["PropertyType"] = prop.Type.ToDisplayString()
                    });
                }
            }
        }

        if (propertyData.Count == 0)
            return;

        var json = JsonSerializer.Serialize(propertyData);
        var source = $@"namespace Serenity.PropertyMetadata; internal static class GeneratedMetadata {{ public const string Json = @""{json}""; }}";
        context.AddSource("PropertyMetadata.g.cs", SourceText.From(source, Encoding.UTF8));
    }
}
