using Spectre.Console;

namespace FlowerShop;

public class StoreMenuController
{
    private readonly StaffMember _user;

    public StoreMenuController(StaffMember user)
    {
        _user = user;
    }

    public void Run()
    {
        ConsoleUi.PrintHeader("Store Menu");

        AnsiConsole.WriteLine($"Signed in as {_user.Name}.");
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("");
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Press any key to exit...");
        Console.ReadKey(true);
    }
}