namespace FlowerShop;

public class CustomerService
{
    private readonly CustomerRepository _customers;

    public CustomerService(CustomerRepository customers)
    {
        _customers = customers;
    }

    public Customer FindOrCreate(string name, string phoneNumber)
    {
        var existing = _customers.GetAll().FirstOrDefault(
            c => c.PhoneNumber.Equals(phoneNumber, StringComparison.OrdinalIgnoreCase));

        if (existing is not null) return existing;

        return _customers.Add(new Customer { Name = name, PhoneNumber = phoneNumber });
    }

    public Customer? GetById(int id) => _customers.GetById(id);
}