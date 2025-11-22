using System.Text;

namespace Pryanigram.Exceptions;

public sealed class ConversionException(string arguments, Exception? innerException = null) 
    : Exception(CreateMessage(arguments), innerException)
{
    private const string DisplayMessage = "Failed to convert arguments.";

    private static string CreateMessage(string arguments)
    {
        var sb = new StringBuilder(DisplayMessage);
        sb.AppendLine($"The argument string was {arguments}.");
        
        return sb.ToString();
    }
}
