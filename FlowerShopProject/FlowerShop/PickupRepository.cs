namespace FlowerShop;
public class PickupRepository : JsonRepository<Pickup>
{
    public PickupRepository() : base("pickups.json") { }
}