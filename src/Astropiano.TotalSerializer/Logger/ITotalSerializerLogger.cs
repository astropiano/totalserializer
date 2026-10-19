using System;

namespace Astropiano.TotalSerializer.Logger
{
    public interface ITotalSerializerLogger
    {
        bool IsDebugEnabled { get; }
        void Debug(object message);
        void Debug(object message,Exception exception);
        bool IsInfoEnabled { get; }
        void Info(object message);
        void Info(object message,Exception exception);
        bool IsWarnEnabled { get; }
        void Warn(object message);
        void Warn(object message,Exception exception);
        bool IsErrorEnabled { get; }
        void Error(object message);
        void Error(object message,Exception exception);
        
    }
}