# Pryanigram 🍪 <br>
A modern, high-performance, and modular framework for building Telegram bots on .NET.

Pryanigram brings the best architectural practices of ASP.NET Core
to the world of bot development. Forget about endless switch
statements and spaghetti code. Build bots using familiar Middleware Pipelines, 
Dependency Injection, and strictly typed handlers.



> Underlying Client Architecture
>
> While Pryanigram uses the
> <a href = "https://github.com/TelegramBots/Telegram.Bot">Telegram.Bot</a>
> library as its default driver,
> it is architected around the <code>ITelegramBotClient</code> abstraction.
> Dependencies are limited to this interface and the <code>Update</code>
> model, making the system highly testable and
> allowing for potential substitution of the underlying telegram api client.