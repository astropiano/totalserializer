using System;

namespace Astropiano.TotalSerializer.Test.TestCases.PseudoPrimitives
{
    public class TestClass12WithDateTime
    {
        private DateTime _dateTime;

        public DateTime DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

    }
}