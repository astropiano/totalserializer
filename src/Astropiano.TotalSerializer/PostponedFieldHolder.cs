using System;
using System.Collections.Generic;
using Astropiano.TotalSerializer.Attributes;
using Astropiano.TotalSerializer.SpecialSerialization;

namespace Astropiano.TotalSerializer
{
    /// <summary>
    /// Base class for cases where we want to store some action will not all processing has been finished.
    /// This class holds fields of which the write could be postponed, so that the actual value will be written
    /// only at a later point in time. So further processing of instances of this class might need to get postponed
    /// until all fields got written.
    ///
    /// See also: PostponedWrite.
    /// </summary>
    public abstract class PostponedFieldHolder
    {
        [TotalSerializerIgnoreField]         
        private List<Action> _actions;

#if DEBUG
        /// <summary>
        /// This is here so it can get inspected for debugging purpose. Only available for DEBUG builds.
        /// </summary>
        [TotalSerializerIgnoreField] 
        private HashSet<PostponedWrite> _postponedWrites;
        
#else
        [TotalSerializerIgnoreField] 
        private int _postponedFieldWrites;
#endif
        
        
        
        
        /// <summary>
        /// Number of fields that still need to be written until the instance of this class is complete.
        /// (Only after that future processing might be possible)
        /// </summary>
        internal virtual int PostponedFieldWrites
        {
            get
            {
#if DEBUG
                if (_postponedWrites == null)
                {
                    return 0;
                }
                return _postponedWrites.Count;
#else
                return _postponedFieldWrites;
#endif
            }
        }

        internal void AddPostponedWrite(PostponedWrite postponedWrite)
        {
#if DEBUG
            if (_postponedWrites == null)
            {
                _postponedWrites = new HashSet<PostponedWrite>();
            }
            _postponedWrites.Add(postponedWrite);
#else
            _postponedFieldWrites++;
#endif
        }
        
        internal void RemovePostponedWrite(PostponedWrite postponedWrite)
        {
#if DEBUG

            if (_postponedWrites == null)
            {
                throw new Exception("_postponedWrites was null.");
            }
            _postponedWrites.Remove(postponedWrite);
#else
            _postponedFieldWrites--;
#endif
        }
        
        internal void AddAction(Action action)
        {
            if (_actions == null)
            {
                _actions = new List<Action>();
            }
            _actions.Add(action);
        }

        /// <summary>
        /// Try to execute postponed action. Will only happen if a fields have been written.
        /// (no postponed field writes are left).
        /// </summary>
        internal void TryExecutePostponedActions()
        {
            if (PostponedFieldWrites <= 0 && _actions != null)
            {
                foreach (var action in _actions)
                {
                    action();
                }
                _actions.Clear();
            }
            
        }
    }
}