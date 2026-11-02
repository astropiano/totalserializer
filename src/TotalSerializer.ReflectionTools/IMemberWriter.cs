namespace TotalSerializer.ReflectionTools
{
    public interface IMemberWriter
    {
        void SetUntypedValue(ref object obj, object value);
    }
    
    public interface IMemberWriter<TInstance, in TField> : IMemberWriter
    {
        void SetValue(ref TInstance obj, TField value);
    }
}
