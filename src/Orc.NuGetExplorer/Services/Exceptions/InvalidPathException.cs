namespace Orc.NuGetExplorer
{
    using System;
    using System.Runtime.Serialization;

    public class InvalidPathException : Exception
    {
        public InvalidPathException()
        {
        }

        public InvalidPathException(string? message) 
            : base(message)
        {
        }

        public InvalidPathException(string? message, Exception? innerException) 
            : base(message, innerException)
        {
        }

        protected InvalidPathException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
