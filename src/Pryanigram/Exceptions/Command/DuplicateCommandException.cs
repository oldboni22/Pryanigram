namespace Pryanigram.Exceptions.Command;

public class DuplicateCommandException(string command) : Exception(GenerateMessage(command))
{
    private static string GenerateMessage(string command) => $"The command {command} has already been registered.";
}