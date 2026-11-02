using System;

namespace TotalSerializer.ReflectionTools.Test.TestCases
{
    public class FieldExample2 : IFieldExample2
    {
        FieldExample2()
        {
            modificationDate = null;
            lifeTime = TimeSpan.FromMilliseconds(2);
        }

        public DateTime? modificationDate;

        public TimeSpan lifeTime;
    }
}
