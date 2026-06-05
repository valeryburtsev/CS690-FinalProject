using Spectre.Console;

namespace FlowerShop;

public static class ConsoleUi
{
    public static void PrintHeader(string subtitle)
    {
        AnsiConsole.WriteLine(new string('=', 50));
        AnsiConsole.WriteLine($"   Grace's Flower Shop — {subtitle}");
        AnsiConsole.WriteLine(new string('=', 50));
        AnsiConsole.WriteLine();
    }
}