using Pryanigram.ScopedBag.Contract;

namespace Pryanigram.Extensions;

public static class ScopedBagExtensions
{
    public static T? Get<T>(this IScopedBag bag, object key)
    {
        var value = bag.GetValue(key);

        return value is T unboxed ? unboxed : default;
    }
    
    public static void Set<T>(this IScopedBag bag, object key, T value)
    {
        bag.SetValue(key, value!);
    }
}
