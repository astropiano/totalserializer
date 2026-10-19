using System;
using System.Collections.Concurrent;

namespace Astropiano.TotalSerializer.Logger
{
    public class TotalSerializerLoggerProvider
    {
        ConcurrentDictionary<Type, ITotalSerializerLogger> _loggers = new ConcurrentDictionary<Type, ITotalSerializerLogger>();

        public ITotalSerializerLogger CreateLogger<T>()
        {
            return CreateLogger(typeof(T));
        }

        public virtual ITotalSerializerLogger CreateLogger(Type type)
        {
            return new ConsoleLogger(type);
        }

        internal ITotalSerializerLogger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        internal ITotalSerializerLogger GetLogger(Type type)
        {
            if (!_loggers.TryGetValue(type, out var logger))
            {
                logger = CreateLogger(type);
                _loggers.TryAdd(type, logger);
            }

            return logger;
        } 
        
    }
}