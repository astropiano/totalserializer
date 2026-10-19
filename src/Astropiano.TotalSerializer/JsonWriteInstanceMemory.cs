using System;
using System.Runtime.Serialization;

namespace Astropiano.TotalSerializer
{
    public class JsonWriteInstanceMemory
    {
        /// <summary>
        /// We use this to remember whether we have already seen an object or not.
        /// It seems this is the official way to do this.
        /// </summary>
        /// <returns>true if we have already seen this object before</returns>
        readonly ObjectIDGenerator _objectIdGenerator = new ObjectIDGenerator();
        
        public bool CheckIfFirstAndAddInstance(object instance, out long objId)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            objId = _objectIdGenerator.GetId(instance, out bool firstTime);
            return firstTime;
        }
    }
}