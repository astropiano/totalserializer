using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Timers;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    internal class SystemTimersTimerSerializer: TotalSpecialSerializer<SystemTimersTimerSerializer.TimerData>
    {
        internal class TimerData : TotalSpecialSerializerReplacement
        {
            public bool Enabled;
            public bool AutoReset;
            public double Interval;
            public ISite Site;
            public ISynchronizeInvoke SynchronizingObject;
            public Dictionary<string, object> Delegates = new Dictionary<string, object>();
        }

        public override IList<Type> GetHandledTypes()
        {
            return new List<Type>(){typeof(Timer)};
        }

        public override TimerData SerializeInstance(Type type, object value)
        {
            Timer timer = (Timer) value;
            TimerData timerData = new TimerData();
            timerData.Enabled = timer.Enabled;
            timerData.AutoReset = timer.AutoReset;
            timerData.Interval = timer.Interval;
            timerData.Site = timer.Site;
            timerData.SynchronizingObject = timer.SynchronizingObject;


            //TODO: provide infrastructure in base class
            FieldInfo[] fieldInfos = FieldInfoCache.GetFieldInfos(type);
            foreach (var fieldInfo in fieldInfos)
            {
                if (MethodChecker.IsDelegate(fieldInfo.FieldType))
                {
                    timerData.Delegates.Add(fieldInfo.Name, fieldInfo.GetValue(timer));
                }
            }



            return timerData;
        }

        public override object DeserializeInstance(TimerData timerData, IFactory factory)
        {
            Timer timer = new Timer();
            
            timer.AutoReset = timerData.AutoReset;
            timer.Interval = timerData.Interval;
            timer.Site = timerData.Site;
            timer.SynchronizingObject = timerData.SynchronizingObject;
            timer.Enabled = timerData.Enabled; //call this at last

            foreach (var dataDelegate in timerData.Delegates)
            {
                FieldInfo fieldInfo = FieldInfoCache.GetFieldInfo(timer.GetType(), dataDelegate.Key);
                fieldInfo.SetValue(timer,dataDelegate.Value);
            }
            
            if (timer.Enabled)
            {
                timer.Start();
            }

            
                    
            return timer;
        }
    }
}