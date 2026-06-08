using Spectre.Console;

namespace FlowerShop;

public class StoreMenuController
{
    private const string OptionOrders = "1. Orders";
    private const string OptionInventory = "2. Inventory";
    private const string OptionPickups = "3. Pickups";
    private const string OptionDeliveries = "4. Deliveries";
    private const string OptionEndOfDay = "5. End-of-Day (Owner only)";
    private const string OptionExit = "6. Exit";

    private readonly StaffMember _user;
    private readonly CustomerService _customers;
    private readonly OrderService _orders;
    private readonly InventoryService _inventory;
    private readonly PickupService _pickups;
    private readonly DeliveryService _deliveries;
    private readonly EndOfDayService _endOfDay;
    private readonly AddressValidator _addressValidator;

    public StoreMenuController(
        StaffMember user,
        CustomerService customers,
        OrderService orders,
        InventoryService inventory,
        PickupService pickups,
        DeliveryService deliveries,
        EndOfDayService endOfDay,
        AddressValidator addressValidator)
    {
        _user = user;
        _customers = customers;
        _orders = orders;
        _inventory = inventory;
        _pickups = pickups;
        _deliveries = deliveries;
        _endOfDay = endOfDay;
        _addressValidator = addressValidator;
    }

    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            ConsoleUi.PrintHeader("Store Menu");
            AnsiConsole.WriteLine($"Signed in as {_user.Name} ({_user.Role}).");
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please, select the action:")
                    .AddChoices(
                        OptionOrders,
                        OptionInventory,
                        OptionPickups,
                        OptionDeliveries,
                        OptionEndOfDay,
                        OptionExit));

            switch (choice)
            {
                case OptionOrders: HandleOrders(); break;
                case OptionInventory: HandleInventory(); break;
                case OptionPickups: HandlePickups(); break;
                case OptionDeliveries: HandleDeliveries(); break;
                case OptionEndOfDay: HandleEndOfDay(); break;
                case OptionExit: return;
            }
        }
    }

    private void HandleOrders()
    {
        const string OptionNewOrder = "1. New order";
        const string OptionListOpen = "2. List open orders";
        const string OptionBack = "3. Back";

        while (true)
        {
            AnsiConsole.Clear();
            ConsoleUi.PrintHeader("Orders");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please, select the action:")
                    .AddChoices(OptionNewOrder, OptionListOpen, OptionBack));

            switch (choice)
            {
                case OptionNewOrder: HandleNewOrder(); break;
                case OptionListOpen: HandleListOpenOrders(); break;
                case OptionBack: return;
            }
        }
    }

    private void HandleInventory()
    {
        const string OptionViewAll = "1. View all flowers";
        const string OptionViewLow = "2. View low-stock flowers";
        const string OptionAddStock = "3. Add new stock";
        const string OptionPullStems = "4. Pull stems";
        const string OptionBack = "5. Back";

        while (true)
        {
            AnsiConsole.Clear();
            ConsoleUi.PrintHeader("Inventory");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please, select the action:")
                    .AddChoices(OptionViewAll, OptionViewLow, OptionAddStock, OptionPullStems, OptionBack));

            switch (choice)
            {
                case OptionViewAll: ShowFlowers(_inventory.GetAll(), "All Flowers"); break;
                case OptionViewLow: ShowFlowers(_inventory.GetLowStock(), "Low-Stock Flowers"); break;
                case OptionAddStock: AddStock(); break;
                case OptionPullStems: PullStems(); break;
                case OptionBack: return;
            }
        }
    }

    private void HandlePickups() => ShowPlaceholder("Pickups", "Pickup management coming in Step 8.");
    private void HandleDeliveries() => ShowPlaceholder("Deliveries", "Delivery management coming in Step 8.");

    private void HandleEndOfDay()
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader("End-of-Day Summary");

        if (_user.Role != Role.Owner)
        {
            AnsiConsole.WriteLine("This view is restricted to Owner accounts.");
            PromptReturn();
            return;
        }

        AnsiConsole.WriteLine("(End-of-day summary coming in Step 8.)");
        PromptReturn();
    }

    private static void ShowPlaceholder(string title, string message)
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader(title);
        AnsiConsole.WriteLine(message);
        PromptReturn();
    }

    private static void PromptReturn()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Press any key to return...");
        Console.ReadKey(true);
    }

    private void ShowFlowers(List<Flower> flowers, string title)
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader(title);

        if (flowers.Count == 0)
        {
            AnsiConsole.WriteLine("(No flowers to show.)");
        }
        else
        {
            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Name");
            table.AddColumn("Color");
            table.AddColumn("On Hand");
            table.AddColumn("Committed");
            table.AddColumn("Available");
            table.AddColumn("Threshold");
            table.AddColumn("Status");

            foreach (var f in flowers)
            {
                var status = f.StemsOnHand < f.LowStockThreshold ? "LOW" : "OK";
                table.AddRow(
                    f.Id.ToString(),
                    f.Name,
                    f.Color,
                    f.StemsOnHand.ToString(),
                    f.StemsCommitted.ToString(),
                    f.StemsAvailable.ToString(),
                    f.LowStockThreshold.ToString(),
                    status);
            }

            AnsiConsole.Write(table);
        }

        PromptReturn();
    }

    private void AddStock()
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader("Add New Stock");

        var flower = PromptForFlower("Pick a flower:");
        if (flower is null) return;

        var count = AnsiConsole.Ask<int>("How many stems arriving?");
        _inventory.AddStock(flower.Id, count);

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Added {count} stems to {flower.Color} {flower.Name}.");
        PromptReturn();
    }

    private void PullStems()
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader("Pull Stems");

        var flower = PromptForFlower("Pick a flower:");
        if (flower is null) return;

        var count = AnsiConsole.Ask<int>("How many stems pulling?");
        _inventory.DecrementStems(flower.Id, count);

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Pulled {count} stems of {flower.Color} {flower.Name}.");
        PromptReturn();
    }

    private Flower? PromptForFlower(string title)
    {
        var flowers = _inventory.GetAll();
        if (flowers.Count == 0)
        {
            AnsiConsole.WriteLine("(No flowers in the catalog.)");
            PromptReturn();
            return null;
        }

        const string Cancel = "(Cancel)";
        var labels = flowers
            .Select(f => $"{f.Id}. {f.Color} {f.Name}".Trim())
            .ToList();
        labels.Add(Cancel);

        var pick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .AddChoices(labels));

        if (pick == Cancel) return null;

        var id = int.Parse(pick.Split('.')[0]);
        return flowers.First(f => f.Id == id);
    }

    private void HandleNewOrder()
    {
        const string TypeWalkIn = "1. Walk-in";
        const string TypePickup = "2. Pickup";
        const string TypeDelivery = "3. Delivery";
        const string TypeCancel = "4. Cancel";

        AnsiConsole.Clear();
        ConsoleUi.PrintHeader("New Order");

        var pick = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Pick order type:")
                .AddChoices(TypeWalkIn, TypePickup, TypeDelivery, TypeCancel));

        switch (pick)
        {
            case TypeWalkIn: HandleNewWalkInOrder(); break;
            case TypePickup: HandleNewPickupOrder(); break;
            case TypeDelivery: ShowPlaceholder("Delivery Order", "Coming in Step 8c."); break;
            case TypeCancel: return;
        }
    }
    private Customer PromptCustomer()
    {
        var name = AnsiConsole.Ask<string>("Customer name:");
        var phone = AnsiConsole.Ask<string>("Customer phone:");
        return _customers.FindOrCreate(name, phone);
    }

    private void HandleNewWalkInOrder()
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader("New Order — Walk-in");

        var customer = PromptCustomer();
        var description = AnsiConsole.Ask<string>("Arrangement description:");

        var requirements = CaptureAndCheckFlowers();
        if (requirements is null) return;

        var order = _orders.CreateWalkInOrder(customer.Id, _user.Id, description, requirements);

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Order {order.OrderCode} completed. Total: ${order.TotalPrice:F2}.");
        PromptReturn();
    }

    private void HandleNewPickupOrder()
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader("New Order — Pickup");

        var customer = PromptCustomer();
        var occasion = AnsiConsole.Ask<string>("Occasion:");

        var pickupDate = AnsiConsole.Ask<DateTime>("Pickup date (YYYY-MM-DD):");
        var windowStart = AnsiConsole.Ask<TimeSpan>("Window start (HH:MM):");
        var windowEnd = AnsiConsole.Ask<TimeSpan>("Window end (HH:MM):");

        var description = AnsiConsole.Ask<string>("Arrangement description:");

        var requirements = CaptureAndCheckFlowers();
        if (requirements is null) return;

        var order = _orders.CreatePickupOrder(
            customer.Id, _user.Id,
            occasion, pickupDate, windowStart, windowEnd,
            description, requirements);

        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine($"Order {order.OrderCode} created. Total: ${order.TotalPrice:F2}. Status: {order.Status}.");
        PromptReturn();
    }

    // Returns the requirements list to use, or null if the user cancelled.
    private List<(int FlowerId, int StemsRequired)>? CaptureAndCheckFlowers()
    {
        while (true)
        {
            var requirements = CaptureFlowerRequirements();
            if (requirements.Count == 0)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine("No flowers entered. Order cancelled.");
                PromptReturn();
                return null;
            }

            var availability = _inventory.CheckAvailability(requirements);

            if (availability.IsAvailable)
                return requirements;

            // Shortage path
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("SHORTAGE — the following flowers are not available:");
            var shortageTable = new Table();
            shortageTable.AddColumn("Flower");
            shortageTable.AddColumn("Needed");
            shortageTable.AddColumn("Available");
            shortageTable.AddColumn("Short by");
            foreach (var s in availability.Shortages)
                shortageTable.AddRow(s.FlowerName, s.Needed.ToString(), s.Available.ToString(), s.ShortBy.ToString());
            AnsiConsole.Write(shortageTable);
            AnsiConsole.WriteLine();

            const string ChoiceOverride = "1. Owner override (proceed anyway)";
            const string ChoiceModify = "2. Modify required flowers";
            const string ChoiceCancel = "3. Cancel order";

            var choices = new List<string>();
            if (_user.Role == Role.Owner) choices.Add(ChoiceOverride);
            choices.Add(ChoiceModify);
            choices.Add(ChoiceCancel);

            var pick = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("How would you like to proceed?")
                    .AddChoices(choices));

            if (pick == ChoiceOverride)
            {
                var reason = AnsiConsole.Ask<string>("Override reason:");
                AnsiConsole.WriteLine($"Proceeding with override. Reason: {reason}");
                return requirements;
            }
            if (pick == ChoiceModify)
                continue;

            // Cancel
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Order cancelled.");
            PromptReturn();
            return null;
        }
    }

    private List<(int FlowerId, int StemsRequired)> CaptureFlowerRequirements()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine("Enter required flowers (pick one, enter stems, repeat):");
        var requirements = new List<(int FlowerId, int StemsRequired)>();

        while (true)
        {
            var flower = PromptForFlower("Pick a flower (or Cancel to finish):");
            if (flower is null) break;

            var stems = AnsiConsole.Ask<int>($"How many stems of {flower.Color} {flower.Name}?");
            requirements.Add((flower.Id, stems));

            var addAnother = AnsiConsole.Confirm("Add another flower?", true);
            if (!addAnother) break;
        }

        return requirements;
    }
    private void HandleListOpenOrders()
    {
        AnsiConsole.Clear();
        ConsoleUi.PrintHeader("Open Orders");

        var orders = _orders.GetOpenOrders();
        if (orders.Count == 0)
        {
            AnsiConsole.WriteLine("(No open orders.)");
        }
        else
        {
            var table = new Table();
            table.AddColumn("Code");
            table.AddColumn("Type");
            table.AddColumn("Status");
            table.AddColumn("Promised");
            table.AddColumn("Customer ID");
            table.AddColumn("Price");

            foreach (var o in orders)
            {
                table.AddRow(
                    o.OrderCode,
                    o.Type.ToString(),
                    o.Status.ToString(),
                    o.PromisedTime.ToString("yyyy-MM-dd HH:mm"),
                    o.CustomerId.ToString(),
                    o.TotalPrice.ToString("F2"));
            }

            AnsiConsole.Write(table);
        }

        PromptReturn();
    }
}