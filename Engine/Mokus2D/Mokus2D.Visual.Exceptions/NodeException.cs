using System;

namespace Mokus2D.Visual.Exceptions;

public class NodeException : Exception
{
    public NodeException()
    {
    }

    public NodeException(string message)
        : base(message)
    {
    }
}
