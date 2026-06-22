namespace FlowerShop;

public class DeliveryService
{
    private const int CapacityPerWindow = 5;

    private readonly Repository<Delivery> _deliveries;

    public DeliveryService(Repository<Delivery> deliveries)
    {
        _deliveries = deliveries;
    }

    // UC1:
    public int CountInWindow(DateTime date, TimeSpan windowStart, TimeSpan windowEnd) =>
        _deliveries.GetAll().Count(d =>
            d.DeliveryDate.Date == date.Date &&
            d.WindowStart < windowEnd &&
            d.WindowEnd > windowStart);

    public int CapacityForWindow() => CapacityPerWindow;

    public bool HasCapacity(DateTime date, TimeSpan windowStart, TimeSpan windowEnd) =>
        CountInWindow(date, windowStart, windowEnd) < CapacityPerWindow;

    // UC4: today's route in time order.
    public List<Delivery> GetRouteForDate(DateTime date) =>
        _deliveries.GetAll()
            .Where(d => d.DeliveryDate.Date == date.Date)
            .OrderBy(d => d.RoutePosition ?? int.MaxValue)
            .ThenBy(d => d.WindowStart)
            .ToList();

    public Delivery? GetById(int id) => _deliveries.GetById(id);

    public void MarkStop(int deliveryId, DeliveryStatus status, string? reason = null)
    {
        var delivery = _deliveries.GetById(deliveryId);
        if (delivery is null) return;

        delivery.Status = status;
        delivery.AttemptReason = reason;
        if (status == DeliveryStatus.Completed)
            delivery.CompletedAt = DateTime.UtcNow;

        _deliveries.Update(delivery);
    }

    // Called by OrderService when creating a Delivery order.
    internal Delivery Create(int orderId, string recipientName, string recipientPhone,
                              Address address, DateTime date,
                              TimeSpan windowStart, TimeSpan windowEnd)
    {
        var delivery = new Delivery
        {
            OrderId = orderId,
            RecipientName = recipientName,
            RecipientPhone = recipientPhone,
            Address = address,
            DeliveryDate = date,
            WindowStart = windowStart,
            WindowEnd = windowEnd,
            Status = DeliveryStatus.ReadyToLeave
        };
        return _deliveries.Add(delivery);
    }
}