# Getting Started with Pryanigram 🍪

Pryanigram is a flexible and extensible C# framework designed for building Telegram bots with a clean,
modular architecture based on the pipeline pattern.
It provides a structured way to build message processing flows by composing discrete,
reusable processing steps known as "flow entries".

## What Pryanigram offers

>- Pipeline-based Message Processing:
   Pryanigram implements a middleware-like pipeline where each flow step (FlowEntry) processes incoming updates sequentially. 
   This approach separates concerns and allows you to flexibly add, remove, 
   or reorder processing steps such as command parsing, argument conversion, and message handling.
>- Dependency Injection Integration:
   The framework leverages **Microsoft.Extensions.DependencyInjection** 
   for managing dependencies within each flow step, allowing easy access to services and resources.
>- Flexible Command Handling: Commands are implemented as handlers, 
   discovered automatically via reflection with the ability to register custom handlers associated with command attributes.
>- Argument Conversion System: Supports converting string arguments from user messages into typed objects using pluggable converters, facilitating strong typing and validation.
>- Inline Flows: You can insert anonymous, lambda-style pipeline steps for quick, localized behavior without needing to create full classes.
>- Asynchronous Processing: Pryanigram fully supports async flows, allowing smooth integration with asynchronous operations like API calls or database access.
>- Extensibility: New flows, handlers, and converters can be plugged in without changing the core framework code, making it easy to grow and maintain your bot. 


With Pryanigram, you get a powerful and modular foundation for building scalable, maintainable Telegram bots. Its pipeline design encourages clean separation of concerns, allowing you to customize or extend bot behavior at any stage of message processing. Whether you're building a simple command handler or complex conversational flows, Pryanigram provides the flexibility and tools needed to bring your ideas to life efficiently.

### Welcome to Pryanigram — streamline your Telegram bot development with a modern pipeline approach!