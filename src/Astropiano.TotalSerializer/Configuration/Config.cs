using System;
using System.Collections.Generic;
using Astropiano.TotalSerializer.Factories;
using Astropiano.TotalSerializer.Interfaces;
using Astropiano.TotalSerializer.Logger;
using Astropiano.TotalSerializer.SpecialSerialization;

namespace Astropiano.TotalSerializer.Configuration
{
    /// <summary>
    /// Static configuration of TotalSerializer. 
    /// </summary>
    public class Config
    {
        public Config()
        {
            SetDefaults();
        }

        static Config()
        {
            SetStaticDefaults();
        }

        public static void SetStaticDefaults()
        {
            LoggerProvider = new TotalSerializerLoggerProvider();
        }

        public void SetDefaults()
        {
            Factory = new DefaultFactory();
            _ignoredTypes = new HashSet<Type>();
            // ReSharper disable once InconsistentlySynchronizedField (lock cannot be null)
            _specialSerializers = new Dictionary<Type, ITotalSpecialSerializer>();
            // ReSharper disable once InconsistentlySynchronizedField (lock cannot be null)
            _genericSpecialSerializers = new Dictionary<Type, ITotalSpecialSerializer>();
            _specialDeserializers  = new Dictionary<Type, ITotalSpecialSerializer>();
            ErrorStrategy = ErrorStrategy.Ignore;
            //register our internal special cases
            foreach (var internalTotalSpecialSerializer in InternalSpecialSerializerRegistration.GetInternalTotalSpecialSerializers())
            {
                RegisterSpecialSerializerInternal(internalTotalSpecialSerializer);
            }
            _recreateTypeFactories = new Dictionary<Type, IFactory>();
            _noRecreateTypeFactories = new HashSet<Type>();
        }
        
        #region public static field

        public static TotalSerializerLoggerProvider LoggerProvider;

        private static ITotalSerializerLogger _logger = LoggerFactory.Create<Config>();

        #endregion
        
        
        #region public fields
        
        
        public ErrorStrategy ErrorStrategy;
        
        #endregion
        
        #region private fields
        
        private HashSet<Type> _ignoredTypes;
        private IFactory _factory;
        private Dictionary<Type, ITotalSpecialSerializer> _specialSerializers;
        private Dictionary<Type, ITotalSpecialSerializer> _genericSpecialSerializers;
        private Dictionary<Type, ITotalSpecialSerializer> _specialDeserializers;
        private Dictionary<Type, IFactory> _recreateTypeFactories;
        private HashSet<Type> _noRecreateTypeFactories;
        
        #endregion

        /// <summary>
        /// If the objects that have to be deserialized do not have an empty constructor, 
        /// a custom factory is needed an can be set here. 
        /// </summary>
        public IFactory Factory
        {
            get { return _factory; }
            set
            {
                if (null == value)
                {
                    throw new InvalidOperationException("Factory was set to null!");
                }

                _factory = value;
            }
        }

        public bool IsIgnoredType(Type type)
        {
            return _ignoredTypes.Contains(type);
        }
        
        public void IgnoreType(Type type) 
        {
            _ignoredTypes.Add(type);
        }
        
        public void IgnoreType<T>()
        {
            IgnoreType(typeof(T));
        }
        
        #region specialSerialization
 
        public void RegisterSpecialSerializer<T>(TotalSpecialSerializer<T> specialSerializer) where T : TotalSpecialSerializerReplacement
        {
            RegisterSpecialSerializerInternal(specialSerializer);
        }

        internal void RegisterSpecialSerializerInternal(ITotalSpecialSerializer specialSerializer)
        {
            lock (_specialSerializers)
            {
                try
                {
                    IList<Type> handlesTypes = specialSerializer.GetHandledTypes();
                    if (handlesTypes != null)
                    {
                        foreach (Type handledType in handlesTypes)
                        {
                            _specialSerializers[handledType] = specialSerializer;
                        }    
                    }
                    
                }
                catch (Exception e)
                {
                    _logger.Error($"Exception while trying to execute {nameof(ITotalSpecialSerializer.GetHandledTypes)}",e);
                }

                try
                {
                    IList<Type> handlesTypes = specialSerializer.GetHandledGenericBaseTypes();
                    if (handlesTypes != null)
                    {
                        foreach (Type handledType in handlesTypes)
                        {
                            _genericSpecialSerializers[handledType] = specialSerializer;
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"Exception while trying to execute {nameof(ITotalSpecialSerializer.GetHandledGenericBaseTypes)}",e);
                }

                try
                {
                    _specialDeserializers[specialSerializer.GetReplacementType()] = specialSerializer;
                }
                catch (Exception e)
                {
                    _logger.Error($"Exception while trying to execute {nameof(ITotalSpecialSerializer.GetReplacementType)}",e);
                }
                
            }
        }

        internal ITotalSpecialSerializer TryGetSpecialSerializer(Type type)
        {
            lock (_specialSerializers)
            {
                ITotalSpecialSerializer specialSerializer;
                //look for direct handling
                if (_specialSerializers.TryGetValue(type, out specialSerializer))
                {
                    return specialSerializer;
                }
                //look for generic handling
                if (type.IsGenericType)
                {
                    Type genericBase = type.GetGenericTypeDefinition();
                    if (_genericSpecialSerializers.TryGetValue(genericBase, out specialSerializer))
                    {
                        return specialSerializer;
                    }    
                }

          
                // This is a fallback for the case where not the exact type is given in the special-Serializer
                foreach (ITotalSpecialSerializer specialSerializersValue in _specialSerializers.Values)
                {
                    foreach (Type handledType in specialSerializersValue.GetHandledTypes())
                    {
                        if (handledType.IsAssignableFrom(type))
                        {
                            // add direct link for speed improvement
                            _specialSerializers[type] = specialSerializersValue;
                            return specialSerializersValue;
                        }
                    }
                }
            }
            return null;
        }
        
        internal ITotalSpecialSerializer TryGetSpecialDeserializer(Type type)
        {
            lock (_specialDeserializers)
            {
                ITotalSpecialSerializer specialDeserializer;
                //look for serializer of replacement type
                if (_specialDeserializers.TryGetValue(type, out specialDeserializer))
                {
                    return specialDeserializer;
                }
            }
            return null;
        }
        
        #endregion specialSerialization

        #region specialFactory
        
        /// <summary>
        /// Register types that should be recreate with a given factory. Use this for cases like loggers, etc.
        /// </summary>
        private void RegisterRecreateTypeFactory(Type type, IFactory factory) 
        {
            lock (_specialDeserializers)
            {
                _recreateTypeFactories.Add(type, factory);
            }
        }
        
        /// <summary>
        /// Register types that should be recreate with a given factory. Use this for cases like loggers, etc.
        /// </summary>
        public void RegisterRecreateTypeFactory<T>(IFactory factory) 
        {
            RegisterRecreateTypeFactory(typeof(T), factory);
        }
        
        internal IFactory TryGetRecreateTypeFactory(Type type)
        {
            lock (_specialDeserializers)
            {
                if (_noRecreateTypeFactories.Contains(type))
                {
                    return null;
                }
                
                IFactory specialFactory;
                //look for serializer of replacement type
                if (_recreateTypeFactories.TryGetValue(type, out specialFactory))
                {
                    return specialFactory;
                }
                // Look for base type
                if (type.BaseType != null)
                {
                    specialFactory = TryGetRecreateTypeFactory(type.BaseType);
                }

                if (specialFactory == null)
                {
                    //look for interface markers
                    var interfaces = type.GetInterfaces();

                    foreach (Type markerInterface in interfaces)
                    {
                        specialFactory = TryGetRecreateTypeFactory(@markerInterface);
                        if (specialFactory != null)
                        {
                            break;
                        }
                    }
                }

                if (specialFactory == null)
                {
                    _noRecreateTypeFactories.Add(type); // do not repeat expensive inheritance/interface lookup
                }
                else
                {
                    return specialFactory;
                }
            }
            return null;
        }

        
        #endregion specialFactory
        
    }
}
