namespace FlowerShop;

public class OrderService
{
    private static readonly char[] Alphabet = "ABCDEFGHJKMNPQRSTUVWXYZ23456789".ToCharArray();
    private static readonly Random Random = new();

    private readonly OrderRepository _orders;
    private readonly ArrangementRepository _arrangements;
    private readonly FlowerRequirementRepository _flowerRequirements;
    private readonly DeliveryService _deliveryService;
    private readonly PickupService _pickupService;
    private readonly InventoryService _inventoryService;

    public OrderService(
        OrderRepository orders,
        ArrangementRepository arrangements,
        FlowerRequirementRepository flowerRequirements,
        DeliveryService deliveryService,
        PickupService pickupService,
        InventoryService inventoryService)
    {
        _orders = orders;
        _arrangements = arrangements;
        _flowerRequirements = flowerRequirements;
        _deliveryService = deliveryService;
        _pickupService = pickupService;
        _inventoryService = inventoryService;
    }

    public List<Order> GetOpenOrders() =>
        _orders.GetAll()
            .Where(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled)
            .OrderBy(o => o.PromisedTime)
            .ToList();

    public Order? GetById(int id) => _orders.GetById(id);

    public void UpdateStatus(int orderId, OrderStatus status)
    {
        var order = _orders.GetById(orderId);
        if (order is null) return;
        order.Status = status;
        _orders.Update(order);
    }

    // --- Order creation ---

    public Order CreateWalkInOrder(
        int customerId, int staffId,
        string arrangementDescription,
        List<(int FlowerId, int StemsRequired)> flowerRequirements)
    {
        var price = CalculatePrice(flowerRequirements);
        var order = CreateOrderShell(customerId, staffId, OrderType.WalkIn,
                                      DateTime.UtcNow, price);
        var arrangement = AddArrangement(order.Id, arrangementDescription, price);
        AddFlowerRequirements(arrangement.Id, flowerRequirements);

        // Walk-in: the flowers leave the shop right now, no reservation needed.
        foreach (var (flowerId, stems) in flowerRequirements)
            _inventoryService.DecrementStems(flowerId, stems);

        order.Status = OrderStatus.Completed;
        _orders.Update(order);
        return order;
    }

    public Order CreatePickupOrder(
        int customerId, int staffId,
        string occasion,
        DateTime pickupDate, TimeSpan windowStart, TimeSpan windowEnd,
        string arrangementDescription,
        List<(int FlowerId, int StemsRequired)> flowerRequirements)
    {
        var price = CalculatePrice(flowerRequirements);
        var order = CreateOrderShell(customerId, staffId, OrderType.Pickup,
                                      pickupDate.Add(windowStart), price);
        var arrangement = AddArrangement(order.Id, arrangementDescription, price);
        AddFlowerRequirements(arrangement.Id, flowerRequirements);

        // Reserve the stems for the future pickup.
        _inventoryService.ReserveStems(
            flowerRequirements.Select(r => (r.FlowerId, r.StemsRequired)));

        _pickupService.Create(order.Id, customerId, occasion, pickupDate, windowStart, windowEnd);
        return order;
    }

    public Order CreateDeliveryOrder(
        int customerId, int staffId,
        string recipientName, string recipientPhone, Address address,
        DateTime deliveryDate, TimeSpan windowStart, TimeSpan windowEnd,
        string arrangementDescription,
        List<(int FlowerId, int StemsRequired)> flowerRequirements)
    {
        var price = CalculatePrice(flowerRequirements);
        var order = CreateOrderShell(customerId, staffId, OrderType.Delivery,
                                      deliveryDate.Add(windowStart), price);
        var arrangement = AddArrangement(order.Id, arrangementDescription, price);
        AddFlowerRequirements(arrangement.Id, flowerRequirements);

        _inventoryService.ReserveStems(
            flowerRequirements.Select(r => (r.FlowerId, r.StemsRequired)));

        _deliveryService.Create(order.Id, recipientName, recipientPhone, address,
                                 deliveryDate, windowStart, windowEnd);
        return order;
    }

    // --- Helpers ---

    private decimal CalculatePrice(List<(int FlowerId, int StemsRequired)> requirements)
    {
        decimal total = 0;
        foreach (var (flowerId, stems) in requirements)
        {
            var flower = _inventoryService.GetById(flowerId);
            if (flower is null) continue;
            total += flower.SuggestedPrice * stems;
        }
        return total;
    }

    private Order CreateOrderShell(int customerId, int staffId, OrderType type,
                                     DateTime promisedTime, decimal totalPrice)
    {
        var order = new Order
        {
            OrderCode = GenerateOrderCode(),
            CustomerId = customerId,
            Type = type,
            Status = OrderStatus.New,
            PromisedTime = promisedTime,
            CreatedAt = DateTime.UtcNow,
            CreatedByStaffId = staffId,
            TotalPrice = totalPrice
        };
        return _orders.Add(order);
    }

    private Arrangement AddArrangement(int orderId, string description, decimal price)
    {
        var arrangement = new Arrangement
        {
            OrderId = orderId,
            Description = description,
            IsCustom = true,           // every order is built from a specific flower list now
            Price = price
        };
        return _arrangements.Add(arrangement);
    }

    private void AddFlowerRequirements(int arrangementId,
        List<(int FlowerId, int StemsRequired)> requirements)
    {
        foreach (var (flowerId, stems) in requirements)
        {
            _flowerRequirements.Add(new FlowerRequirement
            {
                ArrangementId = arrangementId,
                FlowerId = flowerId,
                StemsRequired = stems
            });
        }
    }

    private static string GenerateOrderCode() =>
        new string(Enumerable.Range(0, 4)
            .Select(_ => Alphabet[Random.Next(Alphabet.Length)])
            .ToArray());
}