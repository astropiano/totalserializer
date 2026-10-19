using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    // TODO: add unit-test
    public class CompareInfoSerializer :  TotalSpecialSerializer<CompareInfoSerializer.CompareInfoData>
    {
        public class CompareInfoData  :  TotalSpecialSerializerReplacement
        {
            public CompareInfoData(string name)
            {
                Name = name;
            }
            
            public string Name { get; private set; }
        }

        public override IList<Type> GetHandledTypes()
        {
            return new List<Type>()
            {
                typeof(CompareInfo)
            };
        }

        public override CompareInfoData SerializeInstance(Type type, object value)
        {
            CompareInfo compareInfo = value as CompareInfo;
            if(compareInfo != null)
            {
                CompareInfoData compareInfoData;
                try
                {
                    compareInfoData = new CompareInfoData(compareInfo.Name);
                }
                catch (NullReferenceException)
                {
                    compareInfoData = new CompareInfoData(null);
                }
                
                return compareInfoData;
            }

            throw new Exception("Unexpected type");
        }

        public override object DeserializeInstance(CompareInfoData replacementInstance, IFactory factory)
        {
            CompareInfo compareInfo;
            if (replacementInstance.Name != null)
            {
                //need to call internal constructor
                compareInfo = factory.Create<CompareInfo>(replacementInstance.Name);
            }
            else
            {
                compareInfo =  (CompareInfo) FormatterServices.GetUninitializedObject(typeof(CompareInfo));   
            }
            
             
            return compareInfo;
        }
    }
}