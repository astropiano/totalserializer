using System;
using Astropiano.TotalSerializer.Attributes;

namespace Astropiano.TotalSerializer.SpecialSerialization
{
    public abstract class TotalSpecialSerializerReplacement
    {
        [TotalSerializerIgnoreField]
        internal long RefId;

        internal Type OriginalType;
    }
}