using System;
using System.Collections.Generic;

namespace TotalSerializer.ReflectionTools
{
    /// <summary>
    /// Using a custom key type seems to be faster than using a Tuple as key.
    /// </summary>
    public class MemberKey: IEqualityComparer<MemberKey>
    {
        
        private Type _type;
        private string _memberName;

        public MemberKey(Type type, string memberName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (memberName == null)
            {
                throw new ArgumentNullException(nameof(memberName));
            }

            if (string.IsNullOrEmpty(memberName))
            {
                throw new ArgumentException($"argument {nameof(memberName)} must not be empty");
            }
            _type = type; 
            _memberName = memberName;
        }

        #region IEqualityComparer<Key> Members

        public override bool Equals(object obj)
        {
            return MemberKeyEquals(this, obj as MemberKey);
        }

        public override int GetHashCode()
        {
            return MemberKeyHashCode(this);
        }

        bool IEqualityComparer<MemberKey>.Equals(MemberKey x, MemberKey y)
        {
            return MemberKeyEquals(x, y);
        }

        private bool MemberKeyEquals(MemberKey x, MemberKey y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            else
                return x._type.Equals(y._type) && x._memberName.Equals(y._memberName);
        }

        int IEqualityComparer<MemberKey>.GetHashCode(MemberKey obj)
        {
            return MemberKeyHashCode(obj);
        }

        private int MemberKeyHashCode(MemberKey obj)
        {
            return obj._type.GetHashCode() ^ obj._memberName.GetHashCode();
        }

        #endregion
    }
}
