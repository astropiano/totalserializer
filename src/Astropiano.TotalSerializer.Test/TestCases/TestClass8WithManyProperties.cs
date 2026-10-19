using System;

namespace Astropiano.TotalSerializer.Test.TestCases
{
    internal class TestClass8WithManyProperties
    {
        public TimeSpan? Duration { get; set; }
        public DateTime? ModificationDate { get; set; }
        public Guid? Oid { get; set; }
        public Guid? OidWithNullValue { get; set; }
        public DateTimeOffset FixedDate { get; set; }
        public decimal? Amount { get; set; }
        public byte? Byte { get; set; }
        public bool? Is { get; set; }
    }
}
