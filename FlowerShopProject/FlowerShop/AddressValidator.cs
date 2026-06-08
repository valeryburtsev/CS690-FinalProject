namespace FlowerShop;

public class AddressValidator
{
    public AddressValidationResult Validate(Address address, string recipientName, string recipientPhone)
    {
        var missing = new List<string>();

        if (string.IsNullOrWhiteSpace(recipientName))  missing.Add("Recipient name");
        if (string.IsNullOrWhiteSpace(recipientPhone)) missing.Add("Recipient phone");
        if (string.IsNullOrWhiteSpace(address.Street)) missing.Add("Street");
        if (string.IsNullOrWhiteSpace(address.City))   missing.Add("City");
        if (string.IsNullOrWhiteSpace(address.State))  missing.Add("State");
        if (string.IsNullOrWhiteSpace(address.Zip))    missing.Add("ZIP");

        // Either a Unit value or an explicit "no unit" choice must be present.
        if (!address.HasNoUnit && string.IsNullOrWhiteSpace(address.Unit))
            missing.Add("Unit (or mark 'no unit')");

        return new AddressValidationResult
        {
            IsValid = missing.Count == 0,
            MissingFields = missing
        };
    }
}

public class AddressValidationResult
{
    public bool IsValid { get; set; }
    public List<string> MissingFields { get; set; } = new();
}