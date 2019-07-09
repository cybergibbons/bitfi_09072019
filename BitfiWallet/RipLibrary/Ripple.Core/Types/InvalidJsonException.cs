using System;

namespace Ripple.Core.Types
{
    /// \\
    /// Thrown when JSON is not valid.
    /// \\
    public class InvalidJsonException : Exception
    {
        public InvalidJsonException() 
        {
        }

        public InvalidJsonException(string message) : base(message)
        {
        }

        public InvalidJsonException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}