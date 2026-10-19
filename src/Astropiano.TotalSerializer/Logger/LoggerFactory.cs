using System;

namespace Astropiano.TotalSerializer.Logger
{
    public class LoggerFactory
    {
        public static ITotalSerializerLogger Create(Type type)
        {
            return new TotalSerializerLogger(type);
        }
        public static ITotalSerializerLogger Create<TInstance>()
        {
            return new TotalSerializerLogger(typeof(TInstance));
        }
    }
}