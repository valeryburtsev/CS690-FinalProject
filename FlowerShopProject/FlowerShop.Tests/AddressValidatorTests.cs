namespace FlowerShop.Tests;

public class AddressValidatorTests
{
    [Fact]
    public void Validate_ValidAddressWithUnit_ReturnsValid()
    {
        var validator = new AddressValidator();
        var address = new Address
        {
            Street    = "123 Main St",
            Unit      = "4B",
            HasNoUnit = false,
            City      = "Berkeley",
            State     = "CA",
            Zip       = "94704"
        };

        var result = validator.Validate(address, "John Doe", "555-1234");

        Assert.True(result.IsValid);
        Assert.Empty(result.MissingFields);
    }

    [Fact]
    public void Validate_MissingUnitWithoutNoUnitFlag_ReturnsInvalid()
    {
        var validator = new AddressValidator();
        var address = new Address
        {
            Street    = "123 Main St",
            Unit      = null,
            HasNoUnit = false,
            City      = "Berkeley",
            State     = "CA",
            Zip       = "94704"
        };

        var result = validator.Validate(address, "John Doe", "555-1234");

        Assert.False(result.IsValid);
        Assert.Contains("Unit (or mark 'no unit')", result.MissingFields);
    }

    [Fact]
    public void Validate_NoUnitFlagSet_IsValid()
    {
        var validator = new AddressValidator();
        var address = new Address
        {
            Street    = "123 Main St",
            Unit      = null,
            HasNoUnit = true,
            City      = "Berkeley",
            State     = "CA",
            Zip       = "94704"
        };

        var result = validator.Validate(address, "John Doe", "555-1234");

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_MissingRecipientPhone_ReturnsInvalid()
    {
        var validator = new AddressValidator();
        var address = new Address
        {
            Street    = "123 Main St",
            HasNoUnit = true,
            City      = "Berkeley",
            State     = "CA",
            Zip      = "94704"
        };

        var result = validator.Validate(address, "John Doe", "");

        Assert.False(result.IsValid);
        Assert.Contains("Recipient phone", result.MissingFields);
    }
}