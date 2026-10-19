using System;
using System.Collections.Generic;
using System.Reflection;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    internal class WeakReferenceSerializer : TotalSpecialSerializer<WeakReferenceSerializer.WeakReferenceData>
    {
        internal class WeakReferenceData : TotalSpecialSerializerReplacement
        {
            public string Type;
            public object Target;
        }
        
        public override IList<Type> GetHandledTypes()
        {
            return new List<Type>() {typeof(WeakReference)};
        }
        
        public override IList<Type> GetHandledGenericBaseTypes()
        {
            return new List<Type>() {typeof(WeakReference<>)};
        }

        public override WeakReferenceData SerializeInstance(Type type, object value)
        {
            WeakReferenceData data = new WeakReferenceData();
            if (value is WeakReference)
            {
                //case untyped Weak
                WeakReference weakReference = (WeakReference)value;
                if (weakReference.IsAlive)
                {
                    data.Target = weakReference.Target;    
                }else
                {
                    data.Target = null;
                }
            }
            else
            {
                //we are in the generic case
                Type targetType = type.GenericTypeArguments[0];
                var targetGetterMethod = GetType().GetMethod(nameof(GetTypedWeakReferenceData), BindingFlags.NonPublic | BindingFlags.Instance);
                if (targetGetterMethod == null)
                {
                    throw new Exception($"Fatal internal error. Cannot call generic method {nameof(GetTypedWeakReferenceData)}");
                }
                targetGetterMethod = targetGetterMethod.MakeGenericMethod(targetType);
                data.Target = targetGetterMethod.Invoke(this, new []{value});
                
            }
            data.Type = TypeNameWriter.GetTypeNameForJson(type);

            return data;
        }

        public override object DeserializeInstance(WeakReferenceData weakReferenceData, IFactory factory)
        {
            Type exactWeakReferenceType = TypeGenerator.GetType(weakReferenceData.Type);
            object weakReference = factory.Activate(exactWeakReferenceType, weakReferenceData.Target);
            return weakReference;
        }
        
        private object GetTypedWeakReferenceData<T>(WeakReference<T> weakReference) where T: class
        {
            T target;
            if (weakReference.TryGetTarget(out target))
            {
                return target;
            }
            return null; //Target does not exist any longer
        }
    }
}