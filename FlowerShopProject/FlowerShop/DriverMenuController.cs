using Spectre.Console;

namespace FlowerShop;

public class DriverMenuController
{
    private const string OptionViewRoute = "1. View today's route";
    private const string OptionMarkStop = "2. Mark stop status";
    private const string OptionViewStop = "3. View stop detail";
    private const string OptionExit = "4. Exit";

    private readonly StaffMember _user;
    private readonly DeliveryService _deliveries;
    private readonly OrderService _orders;

    public DriverMenuController(StaffMember user, DeliveryService deliveries, OrderService orders)
    {
        _user = user;
        _deliveries = deliveries;
        _orders = orders;
    }

    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            ConsoleUi.PrintHeader("Driver Menu");

            var route = _deliveries.GetRouteForDate(DateTime.Today);
            var completed = route.Count(d => d.Status == DeliveryStatus.Completed);
            var pending = route.Count - completed;

            AnsiConsole.WriteLine($"Signed in as {_user.Name}.");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine($"Today's stops: {route.Count}    Completed: {completed}    Pending: {pending}");
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please, select the action:")
                    .AddChoices(OptionViewRoute, OptionMarkStop, OptionViewStop, OptionExit));

            switch (choice)
            {
                case OptionViewRoute: HandleViewRoute(); break;
                case OptionMarkStop: HandleMarkStop(); break;
                case OptionViewStop: HandleViewStop(); break;
                case OptionExit: return;
            }
        }
    }

    private void HandleViewRoute()
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader("Today's Route");

        var route = _deliveries.GetRouteForDate(DateTime.Today);

        if (route.Count == 0)
        {
            AnsiConsole.WriteLine("(No deliveries scheduled for today.)");
        }
        else
        {
            var table = new Table();
            table.AddColumn("#");
            table.AddColumn("Window");
            table.AddColumn("Recipient");
            table.AddColumn("Address");
            table.AddColumn("Phone");
            table.AddColumn("Status");

            int i = 1;
            foreach (var d in route)
            {
                table.AddRow(
                    i.ToString(),
                    $"{d.WindowStart:hh\\:mm}-{d.WindowEnd:hh\\:mm}",
                    d.RecipientName,
                    ShortAddress(d.Address),
                    d.RecipientPhone,
                    d.Status.ToString());
                i++;
            }

            AnsiConsole.Write(table);
        }

        PromptReturn();
    }

    private void HandleMarkStop()
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader("Mark Stop Status");

        var route = _deliveries.GetRouteForDate(DateTime.Today);
        if (route.Count == 0)
        {
            AnsiConsole.WriteLine("(No deliveries scheduled for today.)");
            PromptReturn();
            return;
        }

        var stop = PromptForStop(route, "Pick a stop:");
        if (stop is null) return;

        const string StatusCompleted = "1. Completed";
        const string StatusAttempted = "2. Attempted (with reason)";
        const string StatusFailed = "3. Failed (with reason)";
        const string StatusCancel = "4. Cancel";

        var pick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Set status to:")
                .AddChoices(StatusCompleted, StatusAttempted, StatusFailed, StatusCancel));

        DeliveryStatus newStatus;
        string? reason = null;

        switch (pick)
        {
            case StatusCompleted:
                newStatus = DeliveryStatus.Completed;
                break;
            case StatusAttempted:
                newStatus = DeliveryStatus.Attempted;
                reason = AnsiConsole.Ask<string>("Reason:");
                break;
            case StatusFailed:
                newStatus = DeliveryStatus.Failed;
                reason = AnsiConsole.Ask<string>("Reason:");
                break;
            default:
                return;
        }

        _deliveries.MarkStop(stop.Id, newStatus, reason);

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Stop marked as {newStatus}.");
        PromptReturn();
    }

    private void HandleViewStop()
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader("Stop Detail");

        var route = _deliveries.GetRouteForDate(DateTime.Today);
        if (route.Count == 0)
        {
            AnsiConsole.WriteLine("(No deliveries scheduled for today.)");
            PromptReturn();
            return;
        }

        var stop = PromptForStop(route, "Pick a stop to view:");
        if (stop is null) return;

        var order = _orders.GetById(stop.OrderId);

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Order code: {order?.OrderCode ?? "—"}");
        AnsiConsole.WriteLine($"Recipient:  {stop.RecipientName}");
        AnsiConsole.WriteLine($"Phone:      {stop.RecipientPhone}");
        AnsiConsole.WriteLine("Address:");
        AnsiConsole.WriteLine($"  {stop.Address.Street}");
        if (!stop.Address.HasNoUnit && !string.IsNullOrWhiteSpace(stop.Address.Unit))
            AnsiConsole.WriteLine($"  Unit {stop.Address.Unit}");
        AnsiConsole.WriteLine($"  {stop.Address.City}, {stop.Address.State} {stop.Address.Zip}");
        AnsiConsole.WriteLine($"Window:     {stop.WindowStart:hh\\:mm}-{stop.WindowEnd:hh\\:mm}");
        AnsiConsole.WriteLine($"Status:     {stop.Status}");
        if (!string.IsNullOrWhiteSpace(stop.AttemptReason))
            AnsiConsole.WriteLine($"Reason:     {stop.AttemptReason}");

        PromptReturn();
    }

    // --- Helpers ---

    private static Delivery? PromptForStop(List<Delivery> route, string title)
    {
        const string Cancel = "(Cancel)";
        var labels = route
            .Select((d, i) => $"{i + 1}. {d.WindowStart:hh\\:mm}-{d.WindowEnd:hh\\:mm}  {d.RecipientName}  ({d.Status})")
            .ToList();
        labels.Add(Cancel);

        var pick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .AddChoices(labels));

        if (pick == Cancel) return null;

        var index = int.Parse(pick.Split('.')[0]) - 1;
        return route[index];
    }

    private static string ShortAddress(Address a)
    {
        var unit = a.HasNoUnit ? string.Empty : $", Unit {a.Unit}";
        return $"{a.Street}{unit}, {a.City}";
    }

    private static void PromptReturn()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Press any key to return...");
        Console.ReadKey(true);
    }
}