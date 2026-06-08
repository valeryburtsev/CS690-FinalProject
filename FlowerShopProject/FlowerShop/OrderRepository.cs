namespace FlowerShop;
public class OrderRepository : JsonRepository<Order>
{
    public OrderRepository() : base("orders.json") { }
}