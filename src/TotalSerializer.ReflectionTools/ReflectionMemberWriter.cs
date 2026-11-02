using System.Reflection;

namespace TotalSerializer.ReflectionTools
{
    public class ReflectionMemberWriter : IMemberWriter
    {
        private FieldInfo _fieldInfo;

        public ReflectionMemberWriter(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public void SetUntypedValue(ref object obj, object value)
        {
            _fieldInfo.SetValue(obj, TypeParser.Parse(_fieldInfo.FieldType, value));
        }
    }
}
