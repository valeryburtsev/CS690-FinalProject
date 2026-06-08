using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowerShop;

public class JsonRepository<T> where T : IEntity, new()
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _filePath;
    private List<T> _items = new();
    private bool _loaded;

    public JsonRepository(string fileName)
    {
        _filePath = Path.Combine("Data", fileName);
    }

    public List<T> GetAll()
    {
        EnsureLoaded();
        return _items.ToList();
    }

    public T? GetById(int id)
    {
        EnsureLoaded();
        return _items.FirstOrDefault(x => x.Id == id);
    }

    public T Add(T item)
    {
        EnsureLoaded();
        item.Id = _items.Count == 0 ? 1 : _items.Max(x => x.Id) + 1;
        _items.Add(item);
        Save();
        return item;
    }

    public void Update(T item)
    {
        EnsureLoaded();
        var index = _items.FindIndex(x => x.Id == item.Id);
        if (index < 0)
            throw new InvalidOperationException($"Item with id {item.Id} not found in {_filePath}");
        _items[index] = item;
        Save();
    }

    public void Delete(int id)
    {
        EnsureLoaded();
        _items.RemoveAll(x => x.Id == id);
        Save();
    }

    public void SeedIfEmpty(IEnumerable<T> seedItems)
    {
        EnsureLoaded();
        if (_items.Count > 0) return;

        int nextId = 1;
        foreach (var item in seedItems)
        {
            item.Id = nextId++;
            _items.Add(item);
        }
        Save();
    }

    private void EnsureLoaded()
    {
        if (_loaded) return;

        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            _items = JsonSerializer.Deserialize<List<T>>(json, JsonOptions) ?? new List<T>();
        }

        _loaded = true;
    }

    private void Save()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(_items, JsonOptions);
        File.WriteAllText(_filePath, json);
    }
}