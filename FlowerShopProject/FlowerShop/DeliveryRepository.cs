namespace FlowerShop;
public class DeliveryRepository : JsonRepository<Delivery>
{
    public DeliveryRepository() : base("deliveries.json") { }
}