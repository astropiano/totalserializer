using System;

namespace Astropiano.TotalSerializer.Exceptions
{
    public class UnsupportedTypeException : Exception
    {
        public UnsupportedTypeException(string message) : base(message)
        {

        }

        public UnsupportedTypeException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}