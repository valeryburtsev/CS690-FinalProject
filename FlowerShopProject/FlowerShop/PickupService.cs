namespace FlowerShop;

public class PickupService
{
    private readonly PickupRepository _pickups;
    private readonly OrderRepository _orders;
    private readonly CustomerRepository _customers;

    public PickupService(PickupRepository pickups, OrderRepository orders, CustomerRepository customers)
    {
        _pickups = pickups;
        _orders = orders;
        _customers = customers;
    }

    // UC3: counter view of pickups expected today.
    public List<Pickup> GetTodaysPending(DateTime today) =>
        _pickups.GetAll()
            .Where(p => p.PickupDate.Date == today.Date && p.Status == PickupStatus.Pending)
            .OrderBy(p => p.WindowStart)
            .ToList();

    public List<Pickup> GetTodaysAll(DateTime today) =>
    _pickups.GetAll()
        .Where(p => p.PickupDate.Date == today.Date)
        .OrderBy(p => p.WindowStart)
        .ToList();
    public Pickup? GetById(int id) => _pickups.GetById(id);

    public void MarkCollected(int pickupId, int staffId)
    {
        var pickup = _pickups.GetById(pickupId);
        if (pickup is null) return;
        pickup.Status = PickupStatus.Collected;
        pickup.CollectedAt = DateTime.UtcNow;
        pickup.CollectedByStaffId = staffId;
        _pickups.Update(pickup);
    }

    // Called by OrderService when creating a Pickup order
    internal Pickup Create(int orderId, int customerId, string occasion,
                            DateTime pickupDate, TimeSpan windowStart, TimeSpan windowEnd)
    {
        var customer = _customers.GetById(customerId);
        var orderCode = _orders.GetById(orderId)?.OrderCode ?? string.Empty;

        var pickup = new Pickup
        {
            OrderId = orderId,
            Occasion = occasion,
            PickupDate = pickupDate,
            WindowStart = windowStart,
            WindowEnd = windowEnd,
            LabelText = BuildLabel(customer?.Name ?? "Unknown", occasion, pickupDate, orderCode),
            Status = PickupStatus.Pending
        };
        return _pickups.Add(pickup);
    }

    private static string BuildLabel(string customerName, string occasion, DateTime date, string orderCode) =>
        $"{customerName} · {occasion} · {date:yyyy-MM-dd} · #{orderCode}";
}