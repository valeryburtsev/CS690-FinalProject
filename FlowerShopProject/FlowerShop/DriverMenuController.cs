using Spectre.Console;

namespace FlowerShop;

public class DriverMenuController
{
    private readonly StaffMember _user;

    public DriverMenuController(StaffMember user)
    {
        _user = user;
    }

    public void Run()
    {
        ConsoleUi.PrintHeader("Driver Menu");

        AnsiConsole.WriteLine($"Signed in as {_user.Name}.");
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("");
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Press any key to exit...");
        Console.ReadKey(true);
    }
}