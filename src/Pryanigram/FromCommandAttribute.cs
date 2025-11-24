namespace Pryanigram;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class FromCommandAttribute(string message) : Attribute
{
    public string Message { get; } = message;
}
