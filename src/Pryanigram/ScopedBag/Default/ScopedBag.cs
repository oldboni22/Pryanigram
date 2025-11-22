using Pryanigram.ScopedBag.Contract;

namespace Pryanigram.ScopedBag.Default;

public class ScopedBag : IScopedBag
{
    private readonly Dictionary<object, object?> _dictionary = [];
    
    public object? GetValue(object key)
    {
        _dictionary.TryGetValue(key, out var result);
        
        return result;
    }

    public void Delete(object key)
    {
        _dictionary.Remove(key);
    }

    public void SetValue(object key, object value)
    {
        _dictionary[key] = value;
    }
}
