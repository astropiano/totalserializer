using System;
using Astropiano.TotalSerializer.Configuration;

namespace Astropiano.TotalSerializer.Logger
{
    internal class TotalSerializerLogger : ITotalSerializerLogger
    {
        private ITotalSerializerLogger _logger;
        private TotalSerializerLoggerProvider _lastLoggerProvider;
        private Type _type;

        
        
        private ITotalSerializerLogger Logger
        {
            get
            {
                if (_lastLoggerProvider != Config.LoggerProvider)
                {
                    _lastLoggerProvider = Config.LoggerProvider;
                    _logger = null;
                }
                if (_logger == null)
                {
                    _logger = _lastLoggerProvider.CreateLogger(_type);
                }
                return _logger;
            }
        }

        internal TotalSerializerLogger(Type type)
        {
            _type = type;
            _lastLoggerProvider = Config.LoggerProvider;
        }

        public bool IsDebugEnabled => Logger.IsDebugEnabled;

        public void Debug(object message)
        {
            Logger.Debug(message);
        }

        public void Debug(object message, Exception e)
        {
            Logger.Debug(message, e);
        }

        public bool IsInfoEnabled => Logger.IsInfoEnabled;
        
        public void Info(object message)
        {
            Logger.Info(message);
        }

        public void Info(object message, Exception e)
        {
            Logger.Info(message, e);
        }

        public bool IsWarnEnabled => Logger.IsWarnEnabled;
        
        public void Warn(object message)
        {
            Logger.Warn(message);
        }

        public void Warn(object message, Exception e)
        {
            Logger.Warn(message, e);
        }

        public bool IsErrorEnabled => Logger.IsErrorEnabled;
        
        public void Error(object message)
        {
            Logger.Error(message);
        }

        public void Error(object message, Exception e)
        {
            Logger.Error(message, e);
        }
    }
}