namespace FlowerShop;

public class Customer : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Notes { get; set; }
}

public class Order : IEntity
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = string.Empty;   // human-readable, e.g. "A4F7"
    public int CustomerId { get; set; }
    public OrderType Type { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime PromisedTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedByStaffId { get; set; }
    public decimal TotalPrice { get; set; }
}

public enum OrderType
{
    WalkIn,
    Pickup,
    Delivery
}

public enum OrderStatus
{
    New,
    InProgress,
    Ready,
    Completed,
    Cancelled
}

public class Arrangement : IEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsCustom { get; set; }
    public decimal Price { get; set; }
}

public class Flower : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int StemsOnHand { get; set; }
    public int StemsCommitted { get; set; }
    public int LowStockThreshold { get; set; }
    public DateTime LastStockArrivalDate { get; set; }
    public decimal UnitCost { get; set; }
    public decimal SuggestedPrice { get; set; }

    public int StemsAvailable => StemsOnHand - StemsCommitted;
}

public class FlowerRequirement : IEntity
{
    public int Id { get; set; }
    public int ArrangementId { get; set; }
    public int FlowerId { get; set; }
    public int StemsRequired { get; set; }
}

public class Pickup : IEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Occasion { get; set; } = string.Empty;
    public DateTime PickupDate { get; set; }
    public TimeSpan WindowStart { get; set; }
    public TimeSpan WindowEnd { get; set; }
    public string LabelText { get; set; } = string.Empty;
    public PickupStatus Status { get; set; }
    public DateTime? CollectedAt { get; set; }
    public int? CollectedByStaffId { get; set; }
}

public enum PickupStatus
{
    Pending,
    Collected,
    NoShow
}

public class Delivery : IEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public Address Address { get; set; } = new();
    public DateTime DeliveryDate { get; set; }
    public TimeSpan WindowStart { get; set; }
    public TimeSpan WindowEnd { get; set; }
    public DeliveryStatus Status { get; set; }
    public string? AttemptReason { get; set; }
    public int? RoutePosition { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public enum DeliveryStatus
{
    Draft,
    ReadyToLeave,
    OutForDelivery,
    Completed,
    Attempted,
    Failed
}

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public bool HasNoUnit { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
}