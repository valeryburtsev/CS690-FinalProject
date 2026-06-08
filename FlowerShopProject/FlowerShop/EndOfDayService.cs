namespace FlowerShop;

public class EndOfDayService
{
    private readonly OrderRepository _orders;
    private readonly DeliveryRepository _deliveries;
    private readonly PickupRepository _pickups;
    private readonly FlowerRepository _flowers;

    public EndOfDayService(
        OrderRepository orders,
        DeliveryRepository deliveries,
        PickupRepository pickups,
        FlowerRepository flowers)
    {
        _orders = orders;
        _deliveries = deliveries;
        _pickups = pickups;
        _flowers = flowers;
    }

    public DailySummary ComputeFor(DateTime date)
    {
        var dateOnly = date.Date;

        var orders     = _orders.GetAll().Where(o => o.CreatedAt.Date == dateOnly).ToList();
        var deliveries = _deliveries.GetAll().Where(d => d.DeliveryDate.Date == dateOnly).ToList();
        var pickups    = _pickups.GetAll().Where(p => p.PickupDate.Date == dateOnly).ToList();

        return new DailySummary
        {
            Date = dateOnly,
            WalkInCount        = orders.Count(o => o.Type == OrderType.WalkIn),
            PickupOrderCount   = orders.Count(o => o.Type == OrderType.Pickup),
            DeliveryOrderCount = orders.Count(o => o.Type == OrderType.Delivery),

            DeliveriesCompleted = deliveries.Count(d => d.Status == DeliveryStatus.Completed),
            DeliveriesAttempted = deliveries.Count(d => d.Status == DeliveryStatus.Attempted),
            DeliveriesFailed    = deliveries.Count(d => d.Status == DeliveryStatus.Failed),

            PickupsCollected   = pickups.Count(p => p.Status == PickupStatus.Collected),
            PickupsUncollected = pickups.Count(p => p.Status == PickupStatus.Pending),

            LowStockFlowers = _flowers.GetAll()
                .Where(f => f.StemsOnHand < f.LowStockThreshold)
                .Select(f => $"{f.Color} {f.Name} ({f.StemsOnHand} stems, threshold {f.LowStockThreshold})".Trim())
                .ToList()
        };
    }
}

public class DailySummary
{
    public DateTime Date { get; set; }

    public int WalkInCount { get; set; }
    public int PickupOrderCount { get; set; }
    public int DeliveryOrderCount { get; set; }
    public int TotalOrderCount => WalkInCount + PickupOrderCount + DeliveryOrderCount;

    public int DeliveriesCompleted { get; set; }
    public int DeliveriesAttempted { get; set; }
    public int DeliveriesFailed { get; set; }

    public int PickupsCollected { get; set; }
    public int PickupsUncollected { get; set; }

    public List<string> LowStockFlowers { get; set; } = new();
}