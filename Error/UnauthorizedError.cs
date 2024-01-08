using System;

namespace Subscriptions.Example.Chat;

public class UnauthorizedError : Exception
{
    public UnauthorizedError(string message)
        : base(message)
    {
    }

    public static UnauthorizedError Create() =>
        new("You are not authorized to perform this action.");
}
