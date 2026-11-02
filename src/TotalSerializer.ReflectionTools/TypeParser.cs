using System;
using System.Globalization;

namespace TotalSerializer.ReflectionTools
{
    /// <summary>
    /// Class to parse the string input of the JSON into the correct type.
    /// </summary>
    public class TypeParser
    {
        /// <summary>
        /// Transform a given input string (e.g. "3.14159") into a given 
        /// target type (e.g. 3.14159).
        /// </summary>
        public static TTarget Parse<TTarget>(object input)
        {
            if (null == input)
            {
                return default(TTarget);
            }

            if (input is string)
            {
                return (TTarget) ParseString<TTarget>((string) input);
            }
            else
            {
                return (TTarget) input;
            }
        }
        
        public static object GetDefault(Type type)
        {
            if(type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
        
        public static object Parse(Type target, object input)
        {
            if (null == input)
            {
                return GetDefault(target);
            }

            if (input is string)
            {
                return ParseString(target, (string) input);
            }
            else
            {
                return input;
            }
        }
        

        private static object ParseString<TTarget>(string input)
        {
            Type targetType = typeof(TTarget);
            return ParseString(targetType, input);
        }

        public static object ParseString(Type targetType, string input)
        {
            if (input == "null")
            {
                return null;
            }
            
            if (targetType == typeof(object))
            {
                return input;
            }

            if (targetType == typeof(string))
            {
                return ParseStringToString(input);
            }

            if (targetType == typeof(long))
            {
                return ParseStringToLong(input);
            }

            if (targetType == typeof(int))
            {
                return ParseStringToInt(input);
            }

            if (targetType == typeof(short))
            {
                return ParseStringToShort(input);
            }

            if (targetType == typeof(byte))
            {
                return ParseStringToByte(input);
            }

            if (targetType == typeof(ulong))
            {
                return ParseStringToULong(input);
            }

            if (targetType == typeof(uint))
            {
                return ParseStringToUInt(input);
            }

            if (targetType == typeof(ushort))
            {
                return ParseStringToUShort(input);
            }


            if (targetType == typeof(float))
            {
                return ParseStringToFloat(input);
            }

            if (targetType == typeof(double))
            {
                return ParseStringToDouble(input);
            }

            if (targetType == typeof(decimal))
            {
                return ParseStringToDecimal(input);
            }

            if (targetType == typeof(char))
            {
                return ParseStringToChar(input);
            }

            if (targetType == typeof(bool))
            {
                return ParseStringToBool(input);
            }

            if (targetType.Name.StartsWith("Nullable") && targetType.GenericTypeArguments.Length == 1)
            {
                if (null == input)
                {
                    return null;
                }

                Type innerType = targetType.GenericTypeArguments[0];
                object value = ParseString(innerType, input);
                return value;
            }

            if (targetType == typeof(Guid))
            {
                return ParseStringToGuid(input);
            }

            if (targetType == typeof(TimeSpan))
            {
                return ParseStringToTimeSpan(input);
            }

            if (targetType == typeof(DateTime))
            {
                return ParseStringToDateTime(input);
            }

            if (targetType == typeof(DateTimeOffset))
            {
                return ParseStringToDateTimeOffset(input);
            }

            if (targetType.IsEnum)
            {
                return ParseStringToEnum(targetType, input);
            }

            if(typeof(Type).IsAssignableFrom(targetType))
            {
                return ParseStringToSystemType(input);
            }


            throw new NotImplementedException(string.Format("Parsing of '{0}' to targe type '{1}' is not implemented.",
                input, targetType));
        }

        private static string ParseStringToString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            string output;
            if (input.Length >= 2 &&
                '\"' == input[0] &&
                '\"' == input[input.Length - 1])
            {
                output = input.Substring(1, input.Length - 2);
            }
            else
            {
                output = input;
            }

            output = output.Replace("\\\\", "\\");

            return output.Replace("\\\"", "\""); //unescape
        }

        private static char ParseStringToChar(string input)
        {
            char c;
            if (char.TryParse(input, out c))
            {
                return c;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to char.", input));
        }

        private static long ParseStringToLong(string input)
        {
            long l;
            if (long.TryParse(input, out l))
            {
                return l;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to long.", input));
        }

        private static int ParseStringToInt(string input)
        {
            int i;
            if (int.TryParse(input, out i))
            {
                return i;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to int.", input));
        }

        private static short ParseStringToShort(string input)
        {
            short s;
            if (short.TryParse(input, out s))
            {
                return s;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to short.", input));
        }

        private static byte ParseStringToByte(string input)
        {
            byte b;
            if (byte.TryParse(input, out b))
            {
                return b;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to byte.", input));
        }

        private static ulong ParseStringToULong(string input)
        {
            ulong ul;
            if (ulong.TryParse(input, out ul))
            {
                return ul;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to ulong.", input));
        }

        private static uint ParseStringToUInt(string input)
        {
            uint ui;
            if (uint.TryParse(input, out ui))
            {
                return ui;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to uint.", input));
        }

        private static ushort ParseStringToUShort(string input)
        {
            ushort us;
            if (ushort.TryParse(input, out us))
            {
                return us;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to ushort.", input));
        }

        private static float ParseStringToFloat(string input)
        {
            float f;
            if (float.TryParse(input, NumberStyles.Float | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out f))
            {
                return f;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to float.", input));
        }

        private static double ParseStringToDouble(string input)
        {
            double d;
            if (double.TryParse(input, NumberStyles.Float | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out d))
            {
                return d;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to double.", input));
        }

        private static decimal ParseStringToDecimal(string input)
        {
            decimal m;
            if (decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out m))
            {
                return m;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to decimal.", input));
        }

        private static bool ParseStringToBool(string input)
        {
            bool b;
            if (bool.TryParse(input, out b))
            {
                return b;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to bool.", input));
        }

        private static TimeSpan ParseStringToTimeSpan(string input)
        {
            long ticks = ParseStringToLong(input);
            return new TimeSpan(ticks);
        }

        private static DateTime ParseStringToDateTime(string input)
        {
            long binary = ParseStringToLong(input);
            return DateTime.FromBinary(binary);
        }

        private static DateTimeOffset ParseStringToDateTimeOffset(string input)
        {
            DateTime dateTimeUtc = ParseStringToDateTime(input);
            return new DateTimeOffset(dateTimeUtc);
        }

        private static Type ParseStringToSystemType(string input)
        {
            return Type.GetType(ParseStringToString(input));
        }

        private static Guid ParseStringToGuid(string input)
        {
            input = input.Trim('\"');
            Guid g;
            if (Guid.TryParse(input, out g))
            {
                return g;
            }

            throw new InvalidCastException(string.Format("Cannot parse '{0}' to Guid.", input));
        }
        
        private static object ParseStringToEnum(Type targetEnumType, string input)
        {
            Type underlyingEnumValueType = Enum.GetUnderlyingType(targetEnumType);
            return Enum.ToObject(targetEnumType, ParseString(underlyingEnumValueType, input));
        }

    }
}
