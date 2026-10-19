using System;

namespace Astropiano.TotalSerializer.Logger
{
    public class ConsoleLogger : ITotalSerializerLogger
    {
        private Type _type;
        public ConsoleLogger(Type type)
        {
            _type = type;

            IsDebugEnabled = false;
            IsInfoEnabled = true;
            IsWarnEnabled = true;
            IsErrorEnabled = true;
        }
        
        public bool IsDebugEnabled { get; set; }
        public void Debug(object message)
        {
            if (IsDebugEnabled)
            {
                Write(nameof(Debug), message);    
            }
        }

        public void Debug(object message, Exception e)
        {
            if (IsDebugEnabled)
            {
                Write(nameof(Debug), message, e);    
            }
        }

        public bool IsInfoEnabled { get; set; }
        
        public void Info(object message)
        {
            if (IsInfoEnabled)
            {
                Write(nameof(Info), message);    
            }
        }

        public void Info(object message, Exception e)
        {
            if (IsInfoEnabled)
            {
                Write(nameof(Info), message, e);    
            }
        }
        
        public bool IsWarnEnabled { get; set; }
        
        public void Warn(object message)
        {
            if (IsWarnEnabled)
            {
                Write(nameof(Warn), message);    
            }
        }

        public void Warn(object message, Exception e)
        {
            if (IsWarnEnabled)
            {
                Write(nameof(Warn), message, e);    
            }
        }

        public bool IsErrorEnabled { get; set; }
        
        public void Error(object message)
        {
            if (IsErrorEnabled)
            {
                Write(nameof(Error), message);    
            }
        }

        public void Error(object message, Exception e)
        {
            if (IsErrorEnabled)
            {
                Write(nameof(Error), message, e);    
            }
        }
        
        private void Write(string prefix, object message, Exception e = null)
        {
            prefix =  $"[{prefix}] {_type.Name} : ";
            Console.WriteLine(prefix + message + e);
        }
    }
}