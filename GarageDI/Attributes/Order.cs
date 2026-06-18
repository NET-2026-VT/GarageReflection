namespace GarageDI.Attributes;

/// <summary>
/// Anger visningsordningen för en egenskap i UI.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class Order : Attribute
{
    public int Value { get; }
    public Order(int value) => Value = value;
}
