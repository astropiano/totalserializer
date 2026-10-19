using System;

namespace Astropiano.TotalSerializer.CharArray
{
    public class CharArrayConverter
    {
        /// <summary>
        /// Create a substring out of the given <paramref name="charArray"/> going from 
        /// <paramref name="start"/> to <paramref name="end"/>.
        /// </summary>
        public string GetPartialString(Span<char> charArray, int start, int end)
        {
            if (end > charArray.Length)
            {
                end = charArray.Length;
            }
            return GetPartial(charArray, start, end).ToString();
        }

        public Span<char> GetPartial(Span<char> span, int start, int end)
        {
            return span.Slice(start, end - start);
        }
    }
}