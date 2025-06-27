using System.Text.Json;
using Serenity.PropertyMetadata;

namespace Serenity.PropertyGrid;

public partial class DefaultPropertyItemProvider
{
    partial void LoadGeneratedMetadata(Type type, List<PropertyItem> list)
    {
        if (string.IsNullOrEmpty(GeneratedMetadata.Json))
            return;

        try
        {
            var all = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(GeneratedMetadata.Json);
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
