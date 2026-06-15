using FlowerShop;
using Spectre.Console;

const string InStoreOption  = "1. In Store";
const string DeliveryOption = "2. Delivery";

// --- Wire repositories ---
var staffRepo             = new StaffRepository();
var customerRepo          = new CustomerRepository();
var orderRepo             = new OrderRepository();
var arrangementRepo       = new ArrangementRepository();
var flowerRepo            = new FlowerRepository();
var flowerRequirementRepo = new FlowerRequirementRepository();
var pickupRepo            = new PickupRepository();
var deliveryRepo          = new DeliveryRepository();

// --- Wire services ---
var authService      = new AuthService(staffRepo);
var customerService  = new CustomerService(customerRepo);
var inventoryService = new InventoryService(flowerRepo);
var deliveryService  = new DeliveryService(deliveryRepo);
var pickupService    = new PickupService(pickupRepo, orderRepo, customerRepo);
var orderService     = new OrderService(orderRepo, arrangementRepo, flowerRequirementRepo,
                                          deliveryService, pickupService, inventoryService);
var endOfDayService  = new EndOfDayService(orderRepo, deliveryRepo, pickupRepo, flowerRepo);
var addressValidator = new AddressValidator();

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
        new DriverMenuController(user, deliveryService, orderService).Run();
    }
    else
    {
        new StoreMenuController(
        user,
        customerService,
        orderService,
        inventoryService,
        pickupService,
        deliveryService,
        endOfDayService,
        addressValidator
    ).Run();
    }
} else if(user.Role == Role.Florist) {
    AnsiConsole.Clear();
    new StoreMenuController(
        user,
        customerService,
        orderService,
        inventoryService,
        pickupService,
        deliveryService,
        endOfDayService,
        addressValidator
    ).Run();
} else if(user.Role == Role.Driver)
{
    AnsiConsole.Clear();
    new DriverMenuController(user, deliveryService, orderService).Run();
}
