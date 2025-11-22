namespace Pryanigram.Exceptions.Flow;

public class DuplicateFlowException(Type type) : Exception(GenerateMessage(type))
{
    private static string GenerateMessage(Type type) => $"The type {type} has already been registered.";
}
