using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowerShop;

public class JsonRepository<T> : Repository<T> where T : IEntity, new()
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _filePath;

    public JsonRepository(string fileName, string? dataDirectory = null)
    {
        var dir = dataDirectory ?? "Data";
        _filePath = Path.Combine(dir, fileName);
    }

    protected override void EnsureLoaded()
    {
        if (Loaded) return;

        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            Items = JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new List<T>();
        }

        Loaded = true;
    }

    protected override void Save()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(Items, JsonOptions);
        File.WriteAllText(_filePath, json);
    }
}