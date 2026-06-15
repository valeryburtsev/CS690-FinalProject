namespace FlowerShop;

public class TimeSlot
{
    public TimeSpan Start { get; }
    public TimeSpan End { get; }

    public TimeSlot(TimeSpan start, TimeSpan end)
    {
        Start = start;
        End = end;
    }

    public override string ToString() => $"{Start:hh\\:mm} - {End:hh\\:mm}";
}

public static class TimeSlots
{
    public static readonly List<TimeSlot> Standard = new()
    {
        new TimeSlot(new TimeSpan( 9, 0, 0), new TimeSpan(11, 0, 0)),
        new TimeSlot(new TimeSpan(11, 0, 0), new TimeSpan(13, 0, 0)),
        new TimeSlot(new TimeSpan(13, 0, 0), new TimeSpan(15, 0, 0)),
        new TimeSlot(new TimeSpan(15, 0, 0), new TimeSpan(17, 0, 0)),
        new TimeSlot(new TimeSpan(17, 0, 0), new TimeSpan(18, 0, 0))
    };
}