using Spectre.Console;

namespace FlowerShop;

public class DriverMenuController
{
    private const string OptionViewRoute = "1. View today's route";
    private const string OptionMarkStop  = "2. Mark stop status";
    private const string OptionViewStop  = "3. View stop detail";
    private const string OptionExit      = "4. Exit";

    private readonly StaffMember _user;
    private readonly DeliveryService _deliveries;

    public DriverMenuController(StaffMember user, DeliveryService deliveries)
    {
        _user = user;
        _deliveries = deliveries;
    }

    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            ConsoleUi.PrintHeader("Driver Menu");

            var route     = _deliveries.GetRouteForDate(DateTime.Today);
            var completed = route.Count(d => d.Status == DeliveryStatus.Completed);
            var pending   = route.Count - completed;

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
                case OptionMarkStop:  HandleMarkStop();  break;
                case OptionViewStop:  HandleViewStop();  break;
                case OptionExit:      return;
            }
        }
    }

    private void HandleViewRoute() => ShowPlaceholder("Today's Route",    "Route view coming in Step 8.");
    private void HandleMarkStop()  => ShowPlaceholder("Mark Stop Status", "Mark stop coming in Step 8.");
    private void HandleViewStop()  => ShowPlaceholder("Stop Detail",      "Stop detail coming in Step 8.");

    private static void ShowPlaceholder(string title, string message)
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader(title);
        AnsiConsole.WriteLine(message);
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Press any key to return...");
        Console.ReadKey(true);
    }
}