using System;
using System.Reflection;

namespace Astropiano.TotalSerializer.Debug
{
    public class FieldLevel : IProcessingLevel
    {
        private FieldInfo _fieldInfo;

        public FieldLevel(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public string GetDebugInfo()
        {
            return "Field=" +  _fieldInfo.Name;
        }

        public Type TryGetObjectType()
        {
            return null;
        }
        
        public override string ToString()
        {
            return GetDebugInfo();
        }
    }
}