namespace FlowerShop;

public class InMemoryRepository<T> : Repository<T> where T : IEntity, new()
{
    protected override void Save() { }
}