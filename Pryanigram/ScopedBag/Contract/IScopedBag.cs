namespace Pryanigram.ScopedBag.Contract;

public interface IScopedBag
{
    object? GetValue(object key);
    
    void Delete(object key);
    
    void SetValue(object key, object value);
}
