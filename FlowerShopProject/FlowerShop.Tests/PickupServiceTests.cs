namespace FlowerShop.Tests;

public class PickupServiceTests
{
    private readonly InMemoryRepository<Pickup>   _pickupRepo   = new();
    private readonly InMemoryRepository<Order>    _orderRepo    = new();
    private readonly InMemoryRepository<Customer> _customerRepo = new();
    private readonly PickupService _service;

    public PickupServiceTests()
    {
        _service = new PickupService(_pickupRepo, _orderRepo, _customerRepo);
    }

    private Pickup AddPickup(DateTime date, int startHour, int endHour,
                              PickupStatus status = PickupStatus.Pending,
                              string occasion = "Birthday") {
        return _pickupRepo.Add(new Pickup
        {
            Occasion    = occasion,
            PickupDate  = date,
            WindowStart = TimeSpan.FromHours(startHour),
            WindowEnd   = TimeSpan.FromHours(endHour),
            Status      = status,
            LabelText   = "stub"
        });
    }

    public DateTime today     = new DateTime(2026, 6, 23);
    public DateTime yesterday = new DateTime(2026, 6, 22);
    // --- FR3.3: Show a list of ready-to-pickup orders ---

    [Fact]
    public void GetTodaysPending_NoPickups_ReturnsEmpty()
    {
        var result = _service.GetTodaysPending(today);

        Assert.Empty(result);
    }

    [Fact]
    public void GetTodaysPending_ReturnsOnlyTodaysPendingPickups()
    {

        AddPickup(today, 10, 12);
        AddPickup(today, 13, 15, status: PickupStatus.Collected);
        AddPickup(yesterday, 10, 12);

        var result = _service.GetTodaysPending(today);

        Assert.Single(result);
        Assert.Equal(today, result[0].PickupDate);
        Assert.Equal(PickupStatus.Pending, result[0].Status);
    }

    // --- FR3.4: Mark a pickup collected ---

    [Fact]
    public void MarkCollected_SetsStatusAndMetadata()
    {
        var pickup = AddPickup(today, 10, 12);

        _service.MarkCollected(pickup.Id, 0);

        var updated = _pickupRepo.GetById(pickup.Id)!;
        Assert.Equal(PickupStatus.Collected, updated.Status);
        Assert.NotNull(updated.CollectedAt);
    }

    [Fact]
    public void MarkCollected_NonexistentPickup_DoesNotThrow()
    {
        _service.MarkCollected(999, 0);

        Assert.Empty(_pickupRepo.GetAll());
    }

    // --- FR3.1 + FR3.2: Create captures customer name, occasion, date; label includes order code ---

    [Fact]
    public void Create_GeneratesLabelWithCustomerOccasionDateAndOrderCode()
    {
        var customer = _customerRepo.Add(new Customer { Name = "Sara Cohen", PhoneNumber = "555-1234" });
        var order    = _orderRepo.Add(new Order { OrderCode = "A4F7", CustomerId = customer.Id });

        var pickup = _service.Create(
            orderId:     order.Id,
            customerId:  customer.Id,
            occasion:    "Birthday",
            pickupDate:  new DateTime(2026, 6, 23),
            windowStart: TimeSpan.FromHours(10),
            windowEnd:   TimeSpan.FromHours(12));

        Assert.Contains("Sara Cohen", pickup.LabelText);
        Assert.Contains("Birthday",   pickup.LabelText);
        Assert.Contains("2026-06-23", pickup.LabelText);
        Assert.Contains("A4F7",       pickup.LabelText);
        Assert.Equal(PickupStatus.Pending, pickup.Status);
    }
}