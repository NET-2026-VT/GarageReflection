namespace GarageDI.UI;

public class ConsoleUI : IUI
{
    public void Clear() => Console.Clear();

    public string GetString() => Console.ReadLine() ?? string.Empty;

    public string GetKey() => Console.ReadKey(intercept: true).KeyChar.ToString();

    public void Menu(bool isFull, string options, string menuHeading) =>
        Console.WriteLine(isFull ? "No spots left" : menuHeading + "\n" + options);

    public void Print(string message) => Console.WriteLine(message);

    public ConsoleKeyInfo GetKeyInfo() => Console.ReadKey();

    public void SetBackgroundColor(ConsoleColor color) => Console.BackgroundColor = color;

    public void SetForegroundColor(ConsoleColor color) => Console.ForegroundColor = color;

    public void ResetColor() => Console.ResetColor();

    public void Pause(string message = "\nPress any key to go back to main menu")
    {
        Console.WriteLine(message);
        Console.ReadKey(true);
    }
}
