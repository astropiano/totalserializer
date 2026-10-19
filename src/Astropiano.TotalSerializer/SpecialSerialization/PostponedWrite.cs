using System;
using System.Collections.Generic;
using Astropiano.ReflectionTools;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    /// <summary>
    /// This class is used to store postponed actions. This could be writing to a field ar an array element
    /// </summary>
    internal class PostponedWrite
    {
        private class PostponedWriteParameters
        {
            internal object Instance;
            internal string FieldName;
            internal object FieldValue;

            public PostponedWriteParameters(object instance, string fieldName, object fieldValue)
            {
                Instance = instance;
                FieldName = fieldName;
                FieldValue = fieldValue;
            }
        }
        
        private class PostponedArrayWriteParameters
        {
            internal Array ArrayInstance;
            internal object Instance;
            //TODO: do we need to create a copy here? see ProcessArrayItems() in JsonReader where we check for PostponedFieldHolder type
            internal int[] DimensionsIterator;

            public PostponedArrayWriteParameters(Array arrayInstance, object instance, int[] dimensionsIterator)
            {
                ArrayInstance = arrayInstance;
                Instance = instance;
                DimensionsIterator = dimensionsIterator;
            }
        }
        
        private long _refId;
        
        private List<PostponedWriteParameters> _writes = new List<PostponedWriteParameters>();
        private List<PostponedArrayWriteParameters> _arrayWrites = new List<PostponedArrayWriteParameters>();

         

        internal long RefId
        {
            get { return _refId; }
        }
        
        internal PostponedWrite(long refId)
        {
            _refId = refId;
        }

        internal void AddWrite(object instance, string fieldname, object value)
        {
            _writes.Add(new PostponedWriteParameters(instance, fieldname, value));
            PostponedFieldHolder postponedFieldHolder = instance as PostponedFieldHolder;
            if (postponedFieldHolder != null)
            {
                postponedFieldHolder.AddPostponedWrite(this);
            }
        }

        internal void Execute(FieldsReadWriteCache readWriteCache)
        {
            foreach (var write in _writes)
            {
                readWriteCache.Set(ref write.Instance, write.FieldName, ref write.FieldValue);
                PostponedFieldHolder postponedFieldHolder = write.Instance as PostponedFieldHolder;
                if (postponedFieldHolder != null)
                {
                    postponedFieldHolder.RemovePostponedWrite(this);
                    postponedFieldHolder.TryExecutePostponedActions();
                }
            }
            _writes.Clear();
            foreach (var arrayWrite in _arrayWrites)
            {
                arrayWrite.ArrayInstance.SetValue(arrayWrite.Instance, arrayWrite.DimensionsIterator);   
            }
            _arrayWrites.Clear();
        }

        public void AddArrayWrite(Array arrayInstance, object instance, int[] dimensionsIterator)
        {
            _arrayWrites.Add(new PostponedArrayWriteParameters(arrayInstance, instance, dimensionsIterator));
        }
    }
}