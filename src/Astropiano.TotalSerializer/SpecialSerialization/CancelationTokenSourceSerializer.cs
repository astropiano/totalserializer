using System;
using System.Threading;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    //TODO: we cannot handle LinkedTokenSource.... and those that are time-based.... Think about what to do
    
    public class CancellationTokenSourceSerializer :  TypedTotalSpecialSerializer<CancellationTokenSource, CancellationTokenSourceSerializer.CancellationTokenSourceData>
    {
        public class CancellationTokenSourceData  :  TotalSpecialSerializerReplacement
        {
            public CancellationTokenSourceData(bool isCancellationRequested)
            {
                IsCancellationRequested = isCancellationRequested;
            }

            public bool IsCancellationRequested { get; private set; }
        }


        public override CancellationTokenSourceData SerializeInstanceTyped(Type type, CancellationTokenSource value)
        {
            CancellationTokenSourceData cancellationTokenSourceData = new CancellationTokenSourceData(value.IsCancellationRequested);

            return cancellationTokenSourceData;
        }

        public override CancellationTokenSource DeserializeInstanceTyped(CancellationTokenSourceData replacementInstance, IFactory factory)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            if (replacementInstance.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel();
            }

            return cancellationTokenSource;
        }
    }
}