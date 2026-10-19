using System;

namespace Astropiano.TotalSerializer.Exceptions
{
    public class JsonFormatException : Exception
    {
        public JsonFormatException(string message) : base(message)
        {
            
        }
        
        public JsonFormatException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}