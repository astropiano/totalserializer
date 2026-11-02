using System;

namespace TotalSerializer.ReflectionTools
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PostWriteActionAttribute : Attribute
    {
        public string Action { get; private set; }
        
        public PostWriteActionAttribute(string action)
        {
            Action = action;
        }
    }
}
