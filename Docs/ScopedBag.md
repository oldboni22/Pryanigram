# Scoped bag

Scoped bag serves as a place where you can store any data within an isolated scope.

## Usage

This interface defines the contract of the scoped bag used in pryanigram:

```csharp
public interface IScopedBag
{
    object? GetValue(object key);
    
    void Delete(object key);
    
    void SetValue(object key, object value);
}
```

Pryanigram offers a set of generic extension methods designed to simplify the usage of the scoped bag:

```csharp
public static T? Get<T>(this IScopedBag bag, object key)
public static void Set<T>(this IScopedBag bag, object key, T value)
```

Pryanigram includes a built-in implementation of scoped bag that uses dictionary. 
You can add it to ServiceCollection by using `AddScopedBag()`.
You can also use `AddScopedBag<T>()` to provide your own implementation.

### Tips 

>- Use static readonly objects for keys: `static readonly object ExampleKey = new()`
   this approach ensures that you won't experience random collisions.
>- You can define your custom extension methods to work with concrete members of the bag:
>```csharp
>public static class ScopedBagExtensions 
>{
>   private static object _yourTypeKey = new();
>
>   public static YourType GetYourType(this IScopedBag bag) => bag.Get<YourType>(_yourTypeKey);
>   public static void SetYourType(this IScopedBag bag, YourTpye value) =>
>       bag.Set<YourType>(_yourTypeKey, value);
>}
>  ```