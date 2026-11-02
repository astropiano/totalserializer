using System;

namespace TotalSerializer.SpecialSerialization
{
    public class RecreateType
    {
        public RecreateType(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; private set; }
    }
}