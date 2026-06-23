namespace FlowerShop;

public class InventoryService
{
    private readonly Repository<Flower> _flowers;

    public InventoryService(Repository<Flower> flowers)
    {
        _flowers = flowers;
    }

    public List<Flower> GetAll() { return _flowers.GetAll(); }
    public Flower? GetById(int id) { return _flowers.GetById(id); }

    public List<Flower> GetLowStock() =>
        _flowers.GetAll().Where(f => f.StemsOnHand < f.LowStockThreshold).ToList();

    // UC2: check whether all required flowers are available. Returns shortages if any.
    public AvailabilityResult CheckAvailability(IEnumerable<(int FlowerId, int StemsRequired)> requirements)
    {
        var shortages = new List<FlowerShortage>();

        foreach (var (flowerId, stemsRequired) in requirements)
        {
            var flower = _flowers.GetById(flowerId);
            if (flower is null)
            {
                shortages.Add(new FlowerShortage
                {
                    FlowerName = $"Flower #{flowerId} (unknown)",
                    Needed = stemsRequired,
                    Available = 0
                });
                continue;
            }
            if (flower.StemsAvailable < stemsRequired)
            {
                shortages.Add(new FlowerShortage
                {
                    FlowerName = $"{flower.Color} {flower.Name}".Trim(),
                    Needed = stemsRequired,
                    Available = flower.StemsAvailable
                });
            }
        }

        return new AvailabilityResult
        {
            IsAvailable = shortages.Count == 0,
            Shortages = shortages
        };
    }

    // UC2: once the custom order is confirmed, commit the stems to it.
    public void ReserveStems(IEnumerable<(int FlowerId, int Stems)> reservations)
    {
        foreach (var (flowerId, stems) in reservations)
        {
            var flower = _flowers.GetById(flowerId);
            if (flower is null) continue;
            flower.StemsCommitted += stems;
            _flowers.Update(flower);
        }
    }

    public void DecrementStems(int flowerId, int count)
    {
        var flower = _flowers.GetById(flowerId);
        if (flower is null) return;
        flower.StemsOnHand -= count;
        if (flower.StemsCommitted >= count)
            flower.StemsCommitted -= count;
        _flowers.Update(flower);
    }

    // New stock arriving
    public void AddStock(int flowerId, int count)
    {
        var flower = _flowers.GetById(flowerId);
        if (flower is null) return;
        flower.StemsOnHand += count;
        flower.LastStockArrivalDate = DateTime.Today;
        _flowers.Update(flower);
    }
}

public class AvailabilityResult
{
    public bool IsAvailable { get; set; }
    public List<FlowerShortage> Shortages { get; set; } = new();
}

public class FlowerShortage
{
    public string FlowerName { get; set; } = string.Empty;
    public int Needed { get; set; }
    public int Available { get; set; }
    public int ShortBy => Needed - Available;
}