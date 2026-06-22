namespace FlowerShop.Tests;

public class InventoryServiceTests
{
    private static InventoryService BuildInventoryWith(params Flower[] flowers)
    {
        var repo = new InMemoryRepository<Flower>();
        foreach (var f in flowers)
            repo.Add(f);
        return new InventoryService(repo);
    }

    [Fact]
    public void CheckAvailability_AllInStock_ReturnsAvailable()
    {
        var inv = BuildInventoryWith(
            new Flower { Name = "Roses", Color = "Red", StemsOnHand = 60, LowStockThreshold = 30 });

        var result = inv.CheckAvailability(new[] { (1, 10) });

        Assert.True(result.IsAvailable);
        Assert.Empty(result.Shortages);
    }

    [Fact]
    public void CheckAvailability_RequestExceedsOnHand_ReturnsShortage()
    {
        var inv = BuildInventoryWith(
            new Flower { Name = "Roses", Color = "White", StemsOnHand = 18, LowStockThreshold = 30 });

        var result = inv.CheckAvailability(new[] { (1, 30) });

        Assert.False(result.IsAvailable);
        Assert.Single(result.Shortages);
        Assert.Equal(30, result.Shortages[0].Needed);
        Assert.Equal(18, result.Shortages[0].Available);
        Assert.Equal(12, result.Shortages[0].ShortBy);
    }

    [Fact]
    public void CheckAvailability_AfterReservation_ReducesAvailable()
    {
        var inv = BuildInventoryWith(
            new Flower { Name = "Roses", Color = "Red", StemsOnHand = 60 });
        inv.ReserveStems(new[] { (1, 55) });

        var result = inv.CheckAvailability(new[] { (1, 10) });

        Assert.False(result.IsAvailable);
        Assert.Equal(5, result.Shortages[0].Available);
        Assert.Equal(5, result.Shortages[0].ShortBy);
    }

    [Fact]
    public void ReserveStems_IncreasesCommittedCount()
    {
        var inv = BuildInventoryWith(
            new Flower { Name = "Roses", Color = "Red", StemsOnHand = 60 });

        inv.ReserveStems(new[] { (1, 10) });

        Assert.Equal(10, inv.GetById(1)!.StemsCommitted);
    }

    [Fact]
    public void DecrementStems_ReducesOnHandAndCommitted()
    {
        var inv = BuildInventoryWith(
            new Flower { Name = "Roses", Color = "Red", StemsOnHand = 60 });

        inv.ReserveStems(new[] { (1, 10) });
        inv.DecrementStems(1, 10);

        var f = inv.GetById(1)!;
        Assert.Equal(50, f.StemsOnHand);
        Assert.Equal(0,  f.StemsCommitted);
    }

    [Fact]
    public void GetLowStock_ReturnsOnlyFlowersBelowThreshold()
    {
        var inv = BuildInventoryWith(
            new Flower { Name = "Roses",      Color = "Red",   StemsOnHand = 60, LowStockThreshold = 30 },
            new Flower { Name = "Roses",      Color = "White", StemsOnHand = 18, LowStockThreshold = 30 },
            new Flower { Name = "Eucalyptus", Color = "Green", StemsOnHand = 12, LowStockThreshold = 24 });

        var low = inv.GetLowStock();

        Assert.Equal(2, low.Count);
        Assert.Contains(low, f => f.Color == "White");
        Assert.Contains(low, f => f.Name  == "Eucalyptus");
    }
}