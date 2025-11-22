namespace Pryanigram.Exceptions.Command;

public class EmptyCommandException() : Exception(DisplayMessage)
{
    private const string DisplayMessage = "The provided command is empty.";
}