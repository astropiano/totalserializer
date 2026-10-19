using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    public class CultureInfoSerializer: TotalSpecialSerializer<CultureInfoSerializer.CultureInfoData>
    {
        
        public override CultureInfoData SerializeInstance(Type type, object value)
        {
            CultureInfo cultureInfo = value as CultureInfo;
            if (cultureInfo != null)
            {
                CultureInfoData cultureInfoData;
                try
                {
                    cultureInfoData = new CultureInfoData(cultureInfo.Name);
                }
                catch (NullReferenceException)
                {
                    cultureInfoData = new CultureInfoData(null);
                }
                
                return cultureInfoData;
            }

            throw new Exception("Unexpected type");
        }

        public override object DeserializeInstance(CultureInfoData replacementInstance, IFactory factory)
        {
            CultureInfo cultureInfo;
            if (replacementInstance.Name != null)
            {
                cultureInfo = new CultureInfo(replacementInstance.Name);
            }
            else
            {
                cultureInfo =  (CultureInfo) FormatterServices.GetUninitializedObject(typeof(CultureInfo));   
            }
            
             
            return cultureInfo;
        }
        
        public class CultureInfoData  : TotalSpecialSerializerReplacement
        {
            public CultureInfoData(string name)
            {
                Name = name;
            }
            
            public string Name { get; private set; }
        }

        public override IList<Type> GetHandledTypes()
        {
            return new List<Type>()
            {
                typeof(CultureInfo)
            };
        }

       
    }
}