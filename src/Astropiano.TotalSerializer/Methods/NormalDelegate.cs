using System;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.Methods
{
    internal class NormalDelegate : PostponedFieldHolder, IRequireReferenceStorageBackLink
    {
        public NormalDelegate(Type declaringType)
        {
            DeclaringType = declaringType;
        }
        

        public NormalDelegate(string methodName, object target, Type declaringType, string[] signature, string methodType)
        {
            MethodName = methodName;
            DeclaringType = declaringType;
            Target = target;
            Signature = signature;
            MethodType = methodType;
        }

        internal object Target { get; private set; }
        internal string MethodName { get; private set; }
        internal string[] Signature{ get; private set; }
        
        internal string MethodType  { get; private set; }
        public Type DeclaringType { get; private set; }
    }
}