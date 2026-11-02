namespace TotalSerializer.ReflectionTools.Extensions
{
    public static class ReflectionsExtensions
    {
        private static FieldsReadWriteCache _fieldsReadWriteCache = new FieldsReadWriteCache();
        
        /// <summary>
        /// Get a value of an instance. Either with reflection or emitted IL, whatever is possible.
        /// Builds up a cache in the background to increase writing speed. Mind of this additional memory consumption.
        /// Can throw invalid cast exception.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetValue<T>(this object obj, string propertyName)
        {
            return (T) _fieldsReadWriteCache.Get(ref obj, propertyName);
        }

        /// <summary>
        /// Set value of an instance. Either with reflection or emitted IL, whatever is possible.
        /// Builds up a cache in the background to increase writing speed. Mind of this additional memory consumption.
        /// /// Can throw invalid cast exception.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetValue<T>(this object obj, string propertyName, T value)
        {
            _fieldsReadWriteCache.Set(ref obj, propertyName, value);
        }
    }
}
