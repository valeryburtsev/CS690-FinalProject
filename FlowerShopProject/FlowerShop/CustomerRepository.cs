namespace FlowerShop;
public class CustomerRepository : JsonRepository<Customer>
{
    public CustomerRepository() : base("customers.json") { }
}