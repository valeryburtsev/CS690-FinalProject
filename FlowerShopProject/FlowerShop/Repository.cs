namespace FlowerShop;

public abstract class Repository<T> where T : IEntity, new()
{
    protected List<T> Items = new();
    protected bool Loaded = false;

    public List<T> GetAll()
    {
        EnsureLoaded();
        return Items.ToList();
    }

    public T? GetById(int id)
    {
        EnsureLoaded();
        return Items.FirstOrDefault(x => x.Id == id);
    }

    public T Add(T item)
    {
        EnsureLoaded();
        item.Id = Items.Count == 0 ? 1 : Items.Max(x => x.Id) + 1;
        Items.Add(item);
        Save();
        return item;
    }

    public void Update(T item)
    {
        EnsureLoaded();
        var index = Items.FindIndex(x => x.Id == item.Id);
        if (index < 0)
            throw new InvalidOperationException($"Item with id {item.Id} not found");
        Items[index] = item;
        Save();
    }

    public void Delete(int id)
    {
        EnsureLoaded();
        Items.RemoveAll(x => x.Id == id);
        Save();
    }

    public void SeedIfEmpty(IEnumerable<T> seedItems)
    {
        EnsureLoaded();
        if (Items.Count > 0) return;

        int nextId = 1;
        foreach (var item in seedItems)
        {
            item.Id = nextId++;
            Items.Add(item);
        }
        Save();
    }

    protected virtual void EnsureLoaded() { Loaded = true; }
    protected abstract void Save();
}