namespace GarageDI.Attributes;

/// <summary>
/// Markerar en egenskap som ska uteslutas från användarinmatning och visning.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class Exclude : Attribute
{
}
