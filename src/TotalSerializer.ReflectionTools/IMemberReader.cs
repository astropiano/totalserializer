namespace TotalSerializer.ReflectionTools
{
    public interface IMemberReader
    {
        object GetUntypedValue(object obj);
    }
    
    public interface IMemberReader<in TInstance, out TField> : IMemberReader 
    {
        TField GetValue(TInstance obj);
    }
}
