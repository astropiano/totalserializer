using System.Collections.Generic;
using Astropiano.ReflectionTools;
using Astropiano.TotalSerializer.Interfaces;
using Astropiano.TotalSerializer.SpecialSerialization;

namespace Astropiano.TotalSerializer
{
    internal class ReferenceStorage
    {
        private Dictionary<long, object> _dictionary;

        private Dictionary<object, long> _selectiveBackLink;

        private Dictionary<long, PostponedWrite> _postponedWriteMarkers;
        
        

        internal ReferenceStorage()
        {
            _dictionary = new Dictionary<long, object>();
            _postponedWriteMarkers = new Dictionary<long, PostponedWrite>();
            _selectiveBackLink = new Dictionary<object, long>();
        }

        internal bool TryGet(long id, out object referenced)
        {
            return _dictionary.TryGetValue(id, out referenced);

        }
        
        public void Set(long id, object instance, FieldsReadWriteCache fieldsReadWriteCache)
        {
            _dictionary.Add(id, instance);
            // execute postponed writes for this ID
            PostponedWrite postponedWrites;
            if (_postponedWriteMarkers.TryGetValue(id, out postponedWrites))
            {
                postponedWrites.Execute(fieldsReadWriteCache);
            }

            if (instance is IRequireReferenceStorageBackLink)
            {
                _selectiveBackLink.Add(instance, id);
            }
        }

        public void Update(long id, object instance)
        {
            _dictionary[id] = instance;
        }

        public void AddPostponedWrite(PostponedWrite postponedWrite)
        {
            _postponedWriteMarkers.Add(postponedWrite.RefId, postponedWrite);
        }

        public bool TryGetPostponedWrite(long id, out PostponedWrite postponedWrite)
        {
            if (_postponedWriteMarkers.TryGetValue(id, out postponedWrite))
            {
                return true;
            }

            return false;
        }

        public long TryGetKey(object instance)
        {
            long key;
            if (_selectiveBackLink.TryGetValue(instance, out key))
            {
                return key;
            }

            return -1;
        }
    }
}