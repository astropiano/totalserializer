using System;

namespace Astropiano.TotalSerializer.Debug
{
    public class ObjectLevel : IProcessingLevel
    {
        private readonly Type _type;

        public ObjectLevel(Type type)
        {
            _type = type;
        }

        public string GetDebugInfo()
        {
            return "Type=" + _type.Name;
        }

        public Type TryGetObjectType()
        {
            return _type;
        }

        public override string ToString()
        {
            return GetDebugInfo();
        }
    }
}