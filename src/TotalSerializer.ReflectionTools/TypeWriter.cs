using System;
using System.Globalization;

namespace TotalSerializer.ReflectionTools
{
    public class TypeWriter
    {
        public static string ToString<TInput>(TInput obj)
        {
            return ToStringInternal(typeof(TInput), obj);
        }

        public static string ToString(Type inputType, object obj)
        {
            return ToStringInternal(inputType, obj);
        }

        private static string ToStringInternal(Type inputType, object input)
        {
            if (input == null)
            {
                return "null";
            }

            if (inputType == typeof(string))
            {
                return StringToString(input);
            }

            if (inputType == typeof(long))
            {
                return LongToString(input);
            }
            if (inputType == typeof(int))
            {
                return IntToString(input);
            }
            if (inputType == typeof(short))
            {
                return ShortToString(input);
            }
            if (inputType == typeof(byte))
            {
                return ByteToString(input);
            }

            if (inputType == typeof(ulong))
            {
                return ULongToString(input);
            }
            if (inputType == typeof(uint))
            {
                return UIntToString(input);
            }
            if (inputType == typeof(ushort))
            {
                return UShortToString(input);
            }


            if (inputType == typeof(float))
            {
                return FloatToString(input);
            }
            if (inputType == typeof(double))
            {
                return DoubleToString(input);
            }
            if (inputType == typeof(decimal))
            {
                return DecimalToString(input);
            }

            if (inputType == typeof(char))
            {
                return CharToString(input);
            }

            if (inputType == typeof(bool))
            {
                return BoolToString(input);
            }
            
            if (inputType == typeof(Guid))
            {
                return GuidToString(input);
            }

            if (inputType == typeof(TimeSpan))
            {
                return TimeSpanToString(input);
            }

            if (inputType == typeof(DateTime))
            {
                return DateTimeToString(input);
            }

            if (inputType == typeof(DateTimeOffset))
            {
                return DateTimeOffsetToString(input);
            }

            if (inputType.Name.StartsWith("Nullable") && inputType.GenericTypeArguments.Length == 1)
            {
                Type innerType = inputType.GenericTypeArguments[0];
                string str = ToString(innerType, input);
                return str;
            }

            if (inputType.IsEnum)
            {
                return EnumToString(inputType, input);
            }

            if(typeof(Type).IsAssignableFrom(inputType))
            {
                return TypeToString(input);
            }

            throw new NotImplementedException(string.Format("Writing of '{0}' of input type '{1}' is not implemented.", input, inputType));
        }

        private static string StringToString(object input)
        {
            string inputStr = (string) input;
            inputStr = inputStr.Replace("\\", "\\\\");
            return string.Concat("\"", inputStr.Replace("\"", "\\\""), "\""); //escape
        }

        private static string LongToString(object input)
        {
            return ((long)input).ToString(CultureInfo.InvariantCulture);
        }

        private static string IntToString(object input)
        {
            return ((int)input).ToString(CultureInfo.InvariantCulture);
        }

        private static string ShortToString(object input)
        {
            return ((short)input).ToString(CultureInfo.InvariantCulture);
        }

        private static string ByteToString(object input)
        {
            return ((byte)input).ToString(CultureInfo.InvariantCulture);
        }

        private static string ULongToString(object input)
        {
            return ((ulong)input).ToString(CultureInfo.InvariantCulture);
        }

        private static string UIntToString(object input)
        {
            return ((uint)input).ToString(CultureInfo.InvariantCulture);
        }

        private static string UShortToString(object input)
        {
            return ((ushort)input).ToString(CultureInfo.InvariantCulture);
        }

        private static string FloatToString(object input)
        {
            // About format "G9", read the article about formatting float/single precision values: 
            // https://docs.microsoft.com/en-us/dotnet/api/system.single.tostring?view=netframework-4.7.2
            return ((float)input).ToString("G9", CultureInfo.InvariantCulture);
        }

        private static string DoubleToString(object input)
        {
            // About format "G17", read the article about formating double precision values: 
            // https://docs.microsoft.com/en-us/dotnet/api/system.double.tostring?view=netframework-4.7.2
            return ((double)input).ToString("G17", CultureInfo.InvariantCulture);
        }

        private static string DecimalToString(object input)
        {
            return ((decimal)input).ToString(CultureInfo.InvariantCulture);
        }

        private static string CharToString(object input)
        {
            return ((char)input).ToString(CultureInfo.InvariantCulture);
        }

        private static string BoolToString(object input)
        {
            return ((bool)input).ToString(CultureInfo.InvariantCulture).ToLower();
        }
        
        private static string TimeSpanToString(object input)
        {
            return ((TimeSpan)input).Ticks.ToString(CultureInfo.InvariantCulture);
        }

        private static string DateTimeToString(object input)
        {
            return ((DateTime)input).ToBinary().ToString(CultureInfo.InvariantCulture);
        }

        private static string DateTimeOffsetToString(object input)
        {
            return DateTimeToString(((DateTimeOffset)input).UtcDateTime);
        }

        private static string TypeToString(object input)
        {
            return StringToString(((Type)input).AssemblyQualifiedName);
        }

        private static string GuidToString(object input)
        {
            return "\"" + ((Guid)input) + "\"";
        }
        
        private static string EnumToString(Type enumType, object input)
        {
            Type underlyingEnumValueType = Enum.GetUnderlyingType(enumType);
            return ToStringInternal(underlyingEnumValueType, input);
        }
    }
}
