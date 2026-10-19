﻿using System;
 using Astropiano.TotalSerializer.Exceptions;

namespace Astropiano.TotalSerializer.CharArray
{
    public static class CharArrayComparer
    {
        /// <summary>
        /// Whether or not the big array contains the small array at the given position.
        /// </summary>
        /// <param name="bigArray">In this array we perform the search for <paramref name="smallArray"/>.</param>
        /// <param name="bigArrayStartIndex">Index to start the search in <paramref name="bigArray"/>. </param>
        /// <param name="smallArray">That's what we are searching for in <paramref name="bigArray"/>.</param>
        /// <returns>True if the search was successful.</returns>
        public static bool ContainsAt(Span<char> bigArray, int bigArrayStartIndex, Span<char> smallArray)
        {
            if (bigArrayStartIndex + smallArray.Length > bigArray.Length)
            {
                return false;
            }
            for (int i = 0; i < smallArray.Length; i++)
            {
                if (bigArray[bigArrayStartIndex] != smallArray[i])
                {
                    return false;
                }
                bigArrayStartIndex++;
            }
            return true;
        }

        public static void SeekNextStringScopedOccurence(Span<char> array, char toSeek, ref int iterator, out int index)
        {
            bool inString = false;
            bool escaped = false;
            while (iterator < array.Length)
            {
                if (array[iterator] == '"' && !escaped)
                {
                    inString = !inString;
                }
                else if (array[iterator] == '\\')
                {
                    escaped = !escaped;
                }
                else
                {
                    escaped = false;
                }

                if (array[iterator] == toSeek && !inString && !escaped)
                {
                    index = iterator;
                    return;
                }

                iterator++;
            }

            index = -1;
        }

        public static void SeekNextJsonScopedOccurence(Span<char> array, char toSeek, ref int iterator, out int index)
        {
            bool inString = false;
            int level = 0;
            while (iterator < array.Length)
            {
                if (array[iterator] == '"')
                {
                    if (iterator > 0)
                    {
                        if (array[iterator - 1] != '\\')
                        {
                            inString = !inString;
                        }
                    }
                    else
                    {
                        inString = !inString;
                    }
                }
                if(!inString)
                {
                    if (array[iterator] == '{' || array[iterator] == '[')
                    {
                        level++;
                    }
                    if (array[iterator] == '}' || array[iterator] == ']')
                    {
                        level--;
                    }
                    if (array[iterator] == toSeek && level <= 0)
                    {
                        index = iterator;
                        return;
                    }
                }
                iterator++;
            }

            index = -1;
        }
        
        public static void SeekNextJsonScopedOccurence(Span<char> array, char[] toSeek, ref int iterator, out int index)
        {
            bool inString = false;
            int level = 0;
            while (iterator < array.Length)
            {
                if (array[iterator] == '"')
                {
                    if (iterator > 0)
                    {
                        if (array[iterator - 1] != '\\')
                        {
                            inString = !inString;
                        }
                    }
                    else
                    {
                        inString = !inString;
                    }
                }
                if(!inString)
                {
                    if (array[iterator] == '{' || array[iterator] == '[')
                    {
                        level++;
                    }
                    if (array[iterator] == '}' || array[iterator] == ']')
                    {
                        level--;
                    }
                    if (level <= 0)
                    {
                        foreach (char c in toSeek)
                        {
                            if (c == array[iterator])
                            {
                                index = iterator;
                                return;
                            }
                        }
                    }
                }
                iterator++;
            }

            index = -1;
        }

        
        public static void SeekNextOccurenceWithOrLink(Span<char> array, ReadOnlySpan<char> toSeek, ref int iterator, ref int index)
        {
            int originalIterator = iterator;
            foreach (char charToSeek in toSeek)
            {
                iterator = originalIterator;
                SeekNextOccurence(array, charToSeek, ref iterator, out index);
                if (index >= 0)
                {
                    return;
                }
            }
        }

        public static void SeekNextOccurence(Span<char> array, Span<char> toSeek, ref int iterator, out int index)
        {
            while (iterator < array.Length)
            {
                foreach (char c in toSeek)
                {
                    if (array[iterator] == c)
                    {
                        index = iterator;
                        return;
                    }
                }
                iterator++;
            }
            index = -1;
        }

        /// <summary>
        /// Look for a character (<paramref name="toSeek"/>) inside character array (<paramref name="array"/>) by using the 
        /// <paramref name="iterator"/> and update <paramref name="index"/>.
        /// </summary>
        public static void SeekNextOccurence(Span<char> array, char toSeek, ref int iterator, out int index)
        {
            if (iterator >= array.Length)
            {
                index = -1;
                return;
            }

            Span<char> slice = array.Slice(iterator);
            int sliceIndex = slice.IndexOf(toSeek);

            if (sliceIndex < 0)
            {
                iterator = array.Length;
                index = sliceIndex;
            }
            else
            {
                // The index in the slice has to be adjusted to give to index in the outer span:
                iterator += sliceIndex;
                index = iterator;
            }
        }       
        
        public static void SeekJsonScopeEnd(Span<char> array, ref int iterator, out int index)
        {
            index = 0; //use index as level (saving variables, sorry :-P)
            bool inString = false;
            while (iterator < array.Length)
            {
                if (array[iterator] == '"')
                {
                    if (iterator > 0)
                    {
                        if (array[iterator - 1] != '\\')
                        {
                            inString = ! inString;        
                        }
                    }
                    else
                    {
                        inString = ! inString;
                    }
                }
                if(!inString)
                {
                    if (array[iterator] == '{' || array[iterator] == '[')
                    {
                        index++;
                    }
                    if (array[iterator] == '}' || array[iterator] == ']')
                    {
                        index--;
                    }
                    if ((array[iterator] == ',' && index == 0) || index == -1)
                    {
                        index = iterator;
                        return;
                    }
                }
                iterator++;
            }

            index = -1;
        }


        /// <summary>
        /// Make sure that the char at index <paramref name="index"/> in 
        /// <paramref name="array"/> is equal the char <paramref name="toSeek"/>. 
        /// 
        /// Throw exception otherwise.
        /// </summary>
        /// <param name="array">Array of chars.</param>
        /// <param name="toSeek">The char we want to have at position <paramref name="index"/>.</param>
        /// <param name="index">The index in <paramref name="array"/> which is compared.</param>
        /// <param name="allowFormattingChar">If true [default] it will ignore formatting characters like
        /// whitespace and newline ('\n' and '\r').
        /// </param>
        public static void EnforceNextOccurence(Span<char> array, char toSeek, ref int index, bool allowFormattingChar = true)
        {
            if (allowFormattingChar)
            {
                while (char.IsWhiteSpace(array[index]) || array[index] == '\n' || array[index] == '\r')
                {
                    index++;
                }    
            }
            if (array[index] != toSeek)
            {
                throw new JsonFormatException($"Unexpected character '{array[index]}', expected '{toSeek}'.");
            }
        }

        public static void EnforceNextOccurence(Span<char> array, char[] toSeek, ref int index,
            bool allowFormattingChar = true)
        {
            if (allowFormattingChar)
            {
                while (char.IsWhiteSpace(array[index]) || array[index] == '\n' || array[index] == '\r')
                {
                    index++;
                }
            }

            foreach (char seekChar in toSeek)
            {
                if (array[index] != seekChar)
                {
                    return;
                }
            }

            throw new JsonFormatException($"Unexpected character '{array[index]}', expected '{string.Join(",",toSeek)}'.");
        }

        public static void CountOccurence(Span<char> array, char toSeek, ref int iterator, out int count)
        {
            count = 0;
            while (iterator < array.Length)
            {
                if (array[iterator] == toSeek)
                {
                    count++;
                }
                iterator++;
            }
        }
        
        public static void CountJsonScopedOccurence(Span<char> array, char toSeek, ref int iterator, out int count)
        {
            bool inString = false;
            int level = 0;
            count = 0;
            while (iterator < array.Length)
            {
                if (array[iterator] == '"')
                {
                    if (iterator > 0)
                    {
                        if (array[iterator - 1] != '\\')
                        {
                            inString = ! inString;        
                        }
                    }
                    else
                    {
                        inString = ! inString;
                    }
                }
                if (array[iterator] == '{' || array[iterator] == '[')
                {
                    level++;
                }
                if (array[iterator] == '}' || array[iterator] == ']')
                {
                    level--;
                }
                if (level < 0)
                {
                    return;
                }
                if (array[iterator] == toSeek && !inString && level == 0)
                {
                    count++;
                }

                iterator++;
            }
        }

        public static void CountStringScopedOccurence(Span<char> array, char toSeek, ref int iterator, out int count)
        {
            bool inString = false;
            count = 0;
            while (iterator < array.Length)
            {
                if (array[iterator] == '"')
                {
                    if (iterator > 0)
                    {
                        if (array[iterator - 1] != '\\')
                        {
                            inString = ! inString;        
                        }
                    }
                    else
                    {
                        inString = ! inString;
                    }
                }
                if (array[iterator] == toSeek && !inString)
                {
                    count++;
                }
                
                iterator++;
            }
        }
        
        public static bool IsNext(Span<char> array, char toSeek, int index, bool allowFormattingChar = true)
        {
            if (allowFormattingChar)
            {
                while (char.IsWhiteSpace(array[index]) || array[index] == '\n' || array[index] == '\r')
                {
                    index++;
                }    
            }

            return array[index] == toSeek;
        }
    }
}
