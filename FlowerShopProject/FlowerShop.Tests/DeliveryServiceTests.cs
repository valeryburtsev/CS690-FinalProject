namespace FlowerShop.Tests;

public class DeliveryServiceTests
{
    private readonly InMemoryRepository<Delivery> _repo = new();
    private readonly DeliveryService _service;

    public DeliveryServiceTests()
    {
        _service = new DeliveryService(_repo);
    }

    private Delivery AddDelivery(DateTime date, int startHour, int endHour) =>
        _repo.Add(new Delivery
        {
            DeliveryDate   = date,
            WindowStart    = TimeSpan.FromHours(startHour),
            WindowEnd      = TimeSpan.FromHours(endHour),
            RecipientName  = "Test",
            RecipientPhone = "555-0000",
            Address        = new Address { Street = "1 Test St", HasNoUnit = true, City = "X", State = "Y", Zip = "00000" },
            Status         = DeliveryStatus.ReadyToLeave
        });

    [Fact]
    public void CountInWindow_NoDeliveries_ReturnsZero()
    {
        var count = _service.CountInWindow(
            new DateTime(2026, 6, 15),
            TimeSpan.FromHours(9),
            TimeSpan.FromHours(11));

        Assert.Equal(0, count);
    }

    [Fact]
    public void CountInWindow_OverlappingWindowsAreCounted()
    {
        var date = new DateTime(2026, 6, 15);
        AddDelivery(date,  9, 11);
        AddDelivery(date, 10, 12);

        Assert.Equal(2, _service.CountInWindow(date, TimeSpan.FromHours(9), TimeSpan.FromHours(11)));
    }

    [Fact]
    public void CountInWindow_DifferentDateIsNotCounted()
    {
        AddDelivery(new DateTime(2026, 6, 15), 9, 11);

        Assert.Equal(0, _service.CountInWindow(new DateTime(2026, 6, 16),
            TimeSpan.FromHours(9), TimeSpan.FromHours(11)));
    }

    [Fact]
    public void HasCapacity_BelowLimit_ReturnsTrue()
    {
        var date = new DateTime(2026, 6, 15);
        AddDelivery(date, 9, 11);
        AddDelivery(date, 9, 11);

        Assert.True(_service.HasCapacity(date, TimeSpan.FromHours(9), TimeSpan.FromHours(11)));
    }

    [Fact]
    public void HasCapacity_AtLimit_ReturnsFalse()
    {
        var date = new DateTime(2026, 6, 15);
        for (int i = 0; i < 5; i++)
            AddDelivery(date, 9, 11);

        Assert.False(_service.HasCapacity(date, TimeSpan.FromHours(9), TimeSpan.FromHours(11)));
    }

    [Fact]
    public void GetRouteForDate_OrdersByWindowStart()
    {
        var date = new DateTime(2026, 6, 15);
        AddDelivery(date, 15, 17);
        AddDelivery(date,  9, 11);
        AddDelivery(date, 13, 15);

        var route = _service.GetRouteForDate(date);

        Assert.Equal(TimeSpan.FromHours(9),  route[0].WindowStart);
        Assert.Equal(TimeSpan.FromHours(13), route[1].WindowStart);
        Assert.Equal(TimeSpan.FromHours(15), route[2].WindowStart);
    }

    [Fact]
    public void MarkStop_Completed_SetsStatusAndCompletedAt()
    {
        var delivery = AddDelivery(new DateTime(2026, 6, 15), 9, 11);

        _service.MarkStop(delivery.Id, DeliveryStatus.Completed);

        var updated = _repo.GetById(delivery.Id)!;
        Assert.Equal(DeliveryStatus.Completed, updated.Status);
        Assert.NotNull(updated.CompletedAt);
    }

    [Fact]
    public void MarkStop_Failed_RecordsReason()
    {
        var delivery = AddDelivery(new DateTime(2026, 6, 15), 9, 11);

        _service.MarkStop(delivery.Id, DeliveryStatus.Failed, "Customer refused");

        var updated = _repo.GetById(delivery.Id)!;
        Assert.Equal(DeliveryStatus.Failed, updated.Status);
        Assert.Equal("Customer refused", updated.AttemptReason);
    }
}