using System;
using TotalSerializer.Attributes;

namespace TotalSerializer.SpecialSerialization
{
    public abstract class TotalSpecialSerializerReplacement
    {
        [TotalSerializerIgnoreField]
        internal long RefId;

        internal Type OriginalType;
    }
}