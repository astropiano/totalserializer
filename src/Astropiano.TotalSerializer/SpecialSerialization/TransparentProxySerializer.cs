#if !(NETCOREAPP)

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Proxies;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    public class TransparentProxySerializer : TotalSpecialSerializer<TransparentProxySerializer.RealProxyData>
    {
        public class RealProxyData : TotalSpecialSerializerReplacement
        {
            public RealProxy RealProxy;

            public RealProxyData(RealProxy realProxy)
            {
                RealProxy = realProxy;
            }
        }
        
        
        public override IList<Type> GetHandledTypes()
        {
            return new List<Type>();
        }

        public override RealProxyData SerializeInstance(Type type, object value)
        {
            RealProxy realProxy = System.Runtime.Remoting.RemotingServices.GetRealProxy(value);
            return new RealProxyData(realProxy);
        }

        public override object DeserializeInstance(RealProxyData replacementInstance, IFactory factory)
        {
            return replacementInstance.RealProxy.GetTransparentProxy();
        }
    }
}

#endif