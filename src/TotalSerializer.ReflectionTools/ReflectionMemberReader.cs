using System.Reflection;

namespace TotalSerializer.ReflectionTools
{
    // TODO: is there a case we can use this class? Do we need it?
    public class ReflectionMemberReader: IMemberReader
    {
        private FieldInfo _fieldInfo;

        public ReflectionMemberReader(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public object GetUntypedValue(object obj)
        {
            return _fieldInfo.GetValue(obj);
        }
    }
}
