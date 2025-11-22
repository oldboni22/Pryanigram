namespace Pryanigram;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class FromCommandAttribute(string message) : Attribute
{
    public string Message { get; } = message;
}
