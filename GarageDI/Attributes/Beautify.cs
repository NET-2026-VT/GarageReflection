namespace GarageDI.Attributes;

/// <summary>
/// Ger en egenskap ett läsbart visningsnamn i UI och sökmenyer.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class Beautify : Attribute
{
    public string Text { get; }

    public Beautify(string text)
    {
        Text = text;
    }
}
