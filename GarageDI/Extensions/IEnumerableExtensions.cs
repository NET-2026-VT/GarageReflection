namespace GarageDI.Extensions;

internal static class IEnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action?.Invoke(item);
        }
    }
}
