using System;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.Methods
{
    public class EventMulticastDelegate : PostponedFieldHolder, IRequireReferenceStorageBackLink
    {
        public EventMulticastDelegate(Type type)
        {
            Type = type;
        }
        
        public EventMulticastDelegate(Type type, string[] signature, string methodType, params EventMultiCastTarget[] multiCastTargets)
        {
            Type = type;
            Signature = signature;
            MethodType = methodType;
            MultiCastTargets = multiCastTargets;
        }

        internal Type Type { get; private set; }
        
        internal string[] Signature{ get; private set; }
        internal string MethodType  { get; private set; }

        [PostWriteAction(nameof(Test))] 
        private EventMultiCastTarget[] _multiCastTargets;

        internal EventMultiCastTarget[] MultiCastTargets
        {
            get { return _multiCastTargets; }
            private set { _multiCastTargets = value; }
        }

        public void Test()
        {
            
        }

        internal override int PostponedFieldWrites
        {
            get
            {
                int postponedFieldWrites = base.PostponedFieldWrites;
                if (MultiCastTargets != null) //TODO: add unit-test
                {
                    foreach (var multiCastTarget in MultiCastTargets)
                    {
                        postponedFieldWrites += multiCastTarget.PostponedFieldWrites;
                    }    
                }
                return postponedFieldWrites;
            }
        }

        public class EventMultiCastTarget : PostponedFieldHolder
        {
            public EventMultiCastTarget()
            {
                
            }
            
            public EventMultiCastTarget(object target, string methodName, Type declaringType)
            {
                Target = target;
                MethodName = methodName;
                DeclaringType = declaringType;
            }

            internal object Target { get; private set; }


            private string _declaringType;

            internal Type DeclaringType
            {
                get { return TypeGenerator.GetType(_declaringType); }
                set { _declaringType = TypeNameWriter.GetTypeNameForJson(value); }
            }

            internal string MethodName { get; private set; } 
        }
    }
}