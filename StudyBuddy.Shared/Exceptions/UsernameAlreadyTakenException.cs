namespace StudyBuddy.Shared.Exceptions;

public class UsernameAlreadyTakenException : Exception
{
    public UsernameAlreadyTakenException(string message) : base(message) {}

    public UsernameAlreadyTakenException(string message, Exception innerException) : base(message, innerException) {}
}
