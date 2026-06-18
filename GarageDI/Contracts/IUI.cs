namespace GarageDI.Contracts;

/// <summary>
/// Defines the user interface contract for the garage management system.
/// </summary>
public interface IUI
{
    /// <summary>
    /// Clears the console screen.
    /// </summary>
    void Clear();

    string GetString();
    string GetKey();

    /// <summary>
    /// Displays a menu with the specified options and title.
    /// </summary>
    /// <param name="isFull">Indicates if the garage is full.</param>
    /// <param name="options">The menu options to display.</param>
    /// <param name="title">The title of the menu.</param>

    void Menu(bool isFull, string options, string title);
    /// <summary>
    /// Prints a message to the console.
    /// </summary>
    /// <param name="message">The message to print.</param>
    
    void Print(string message);

    /// <summary>
    /// Gets the key information from the console.
    /// </summary>
    /// <returns>The console key information.</returns>
    ConsoleKeyInfo GetKeyInfo();

    void SetBackgroundColor(ConsoleColor color);
    void SetForegroundColor(ConsoleColor color);
    void ResetColor();

    void Pause(string message = "\nPress any key to go back to main menu");
}
