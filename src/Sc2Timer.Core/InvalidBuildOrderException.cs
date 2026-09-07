namespace Sc2Timer.Core;

public sealed class InvalidBuildOrderException : Exception
{
    public InvalidBuildOrderException(string message) : base(message)
    {
    }
}
