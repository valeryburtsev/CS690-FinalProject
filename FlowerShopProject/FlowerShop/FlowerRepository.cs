namespace FlowerShop;
public class FlowerRepository : JsonRepository<Flower>
{
    public FlowerRepository() : base("flowers.json") { }
}