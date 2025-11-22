# Inline flow

Inline flow allows you to add anonymous *(not defined by a class)* pipeline steps. 
You can think of them as lambdas. 

## Usage
You can add an inline flow by using 
`FlowBuilder.Use(Func<FlowContext,FlowDelegate,Task>)`.
>**FlowContext** is the context of current flow. <br>
>**FlowDelegate** is the next entry of the flow.

### Example

```csharp
builder
    .FlowBuilder
    .Use( async (context, next) =>
    {
        Console.WriteLine("Inline flow");

        await next(context);
    });
```

### Tips

>You can access the di container by using the `FlowContext.ServiceProvider` property.

### Pros:

>- Flexibility: Allows adding quick, custom behavior without formal class creation.
>- Conciseness: Enables defining short logic inline, improving readability for simple cases.
>- Rapid prototyping: Great for testing or small modifications without adding named types.

### Cons:

>- Discoverability: Without named classes it is harder to maintain or find in large codebases. 
>- Complexity: For complex logic, inline lambdas can get verbose and reduce readability.
>- Testing: Inline flows are harder to isolate and unit test compared to named implementations.