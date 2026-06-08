namespace FlowerShop;

public class FlowerRepository : JsonRepository<Flower>
{
    public FlowerRepository() : base("flowers.json")
    {
        SeedIfEmpty(new[]
        {
            new Flower { Name = "Roses",         Color = "Red",    StemsOnHand = 60, LowStockThreshold = 30, UnitCost = 1.50m, SuggestedPrice = 4.00m, LastStockArrivalDate = DateTime.Today },
            new Flower { Name = "Roses",         Color = "White",  StemsOnHand = 18, LowStockThreshold = 30, UnitCost = 1.50m, SuggestedPrice = 4.00m, LastStockArrivalDate = DateTime.Today },
            new Flower { Name = "Roses",         Color = "Pink",   StemsOnHand = 45, LowStockThreshold = 30, UnitCost = 1.50m, SuggestedPrice = 4.00m, LastStockArrivalDate = DateTime.Today },
            new Flower { Name = "Tulips",        Color = "Yellow", StemsOnHand = 40, LowStockThreshold = 20, UnitCost = 1.00m, SuggestedPrice = 3.00m, LastStockArrivalDate = DateTime.Today },
            new Flower { Name = "Lilies",        Color = "White",  StemsOnHand = 25, LowStockThreshold = 15, UnitCost = 2.00m, SuggestedPrice = 5.00m, LastStockArrivalDate = DateTime.Today },
            new Flower { Name = "Sunflowers",    Color = "Yellow", StemsOnHand = 30, LowStockThreshold = 15, UnitCost = 1.25m, SuggestedPrice = 3.50m, LastStockArrivalDate = DateTime.Today },
            new Flower { Name = "Eucalyptus",    Color = "Green",  StemsOnHand = 12, LowStockThreshold = 24, UnitCost = 0.50m, SuggestedPrice = 1.50m, LastStockArrivalDate = DateTime.Today },
            new Flower { Name = "Baby's Breath", Color = "White",  StemsOnHand = 20, LowStockThreshold = 10, UnitCost = 0.75m, SuggestedPrice = 2.00m, LastStockArrivalDate = DateTime.Today }
        });
    }
}