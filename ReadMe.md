# Pryanigram 🍪 <br>

> Please, note that the current implementation is a preview version,
> provided for demonstation purposes and intended to collect feedback.
 
A modern, high-performance, and modular framework for building Telegram bots on .NET.

Pryanigram brings the best architectural practices of ASP.NET Core
to the world of bot development. Forget about endless switch
statements and spaghetti code. Build bots using familiar Middleware Pipelines, 
Dependency Injection, and strictly typed handlers.

```csharp
class Program
{
    static void Main(string[] args)
    {
        var builder = new BotBuilder();

        builder.WithDefaultClient("Your api token");
        
        builder.UseExceptionHandler((client, exception, _) =>
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);

            return Task.CompletedTask;
        });
        
        builder.Services.AddScopedBag();
        builder.Services.AddScoped<ISessionProvider, ExampleSessionProvider>();

        builder.FlowBuilder
            .Use<ParseMessageFlow>()
            .UseConverters(typeof(Program).Assembly)
            .UseHandlers(typeof(Program).Assembly);

        var bot = builder.Build();

        bot.Start();
        Console.ReadLine();
    }
}
```

> Underlying Client Architecture
>
> While Pryanigram uses the
> <a href = "https://github.com/TelegramBots/Telegram.Bot">Telegram.Bot</a>
> library as its default driver,
> it is architected around the <code>ITelegramBotClient</code> abstraction.
> Dependencies are limited to this interface and the <code>Update</code>
> model, making the system highly testable and
> allowing for potential substitution of the underlying telegram api client.