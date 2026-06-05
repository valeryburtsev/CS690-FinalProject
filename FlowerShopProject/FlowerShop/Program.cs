using FlowerShop;
using Spectre.Console;

const string InStoreOption  = "1. In Store";
const string DeliveryOption = "2. Delivery";

// --- Wire dependencies ---
var staffRepo = new StaffRepository();
var authService = new AuthService(staffRepo);

// --- Login screen ---
ConsoleUi.PrintHeader("Login");

StaffMember? user = null;
while (user is null)
{
    var username = AnsiConsole.Ask<string>("Username:");
    var password = AnsiConsole.Prompt(
        new TextPrompt<string>("Password:").Secret());

    user = authService.TryLogin(username, password);

    if (user is null)
    {
        AnsiConsole.WriteLine("Invalid username or password. Please try again.");
        AnsiConsole.WriteLine();
    }
}

AnsiConsole.WriteLine("Logging in...");
AnsiConsole.WriteLine();

// --- Mode selection ---

if(user.Role == Role.Owner)
{
    var mode = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Please, select the role:")
        .AddChoices(InStoreOption, DeliveryOption));

    AnsiConsole.Clear();

    if (mode == DeliveryOption)
    {
        new DriverMenuController(user).Run();
    }
    else
    {
        new StoreMenuController(user).Run();
    }
} else if(user.Role == Role.Florist) {
    AnsiConsole.Clear();
    new StoreMenuController(user).Run();
} else if(user.Role == Role.Driver)
{
    AnsiConsole.Clear();
    new DriverMenuController(user).Run();
}
