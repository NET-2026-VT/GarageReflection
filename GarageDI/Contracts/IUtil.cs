namespace GarageDI.Contracts;

/// <summary>
/// Defines utility methods for user input and data handling.
/// </summary>
public interface IUtil
{
    /// <summary>
    /// Prompts the user for a string input.
    /// </summary>
    /// <param name="prompt">The prompt message to display.</param>
    /// <returns>The user's input string.</returns>
    string AskForString(string prompt);

    /// <summary>
    /// Prompts the user for an integer input.
    /// </summary>
    /// <param name="prompt">The prompt message to display.</param>
    /// <returns>The user's input integer.</returns>
    int AskForInt(string prompt);
}