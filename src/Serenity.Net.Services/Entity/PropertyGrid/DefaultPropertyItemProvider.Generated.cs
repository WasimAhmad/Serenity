using System.Text.Json;
using System.Reflection;

namespace Serenity.PropertyGrid;

public partial class DefaultPropertyItemProvider
{
    partial void LoadGeneratedMetadata(Type type, List<PropertyItem> list)
    {
        var metadataType = Type.GetType("Serenity.PropertyMetadata.GeneratedMetadata");
        if (metadataType?.GetField("Json", BindingFlags.Public | BindingFlags.Static)?.GetValue(null) is not string json ||
            string.IsNullOrEmpty(json))
            return;

        try
        {
            var all = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);
            if (all == null)
                return;

            foreach (var entry in all)
            {
                if (entry.TryGetValue("Type", out var t) && t == type.FullName && entry.TryGetValue("Property", out var p))
                {
                    list.Add(new PropertyItem { Name = p });
                }
            }
        }
        catch
        {
        }
    }
}
