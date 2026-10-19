using System.Collections.Generic;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    internal static class InternalSpecialSerializerRegistration
    {
        internal static IList<ITotalSpecialSerializer> GetInternalTotalSpecialSerializers()
        {
            IList<ITotalSpecialSerializer> specialSerializers = new List<ITotalSpecialSerializer>()
            {
                new WeakReferenceSerializer(),
                new PropertyInfoSerializer(),
                new FieldInfoSerializer(),
                new SystemTimersTimerSerializer(),
                new CultureInfoSerializer(),
                new CompareInfoSerializer(),
                new CancellationTokenSourceSerializer(),
#if !(NETCOREAPP)
                new TransparentProxySerializer(),
#endif
                new DictionarySerializer()
            };
            return specialSerializers;
        }
    }
}