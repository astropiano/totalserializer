using System;
using System.Text;

namespace TotalSerializer.ReflectionTools
{
    public static class TypeNameWriter
    {
        private static void AppendTypeName(StringBuilder sb, Type type)
        {
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                AppendTypeName(sb, elementType);
                AppendGenericIfRequired(sb, elementType);
                sb.Append('[');
                sb.Append(']');
                return;
            }

            if (type.DeclaringType == null)
            {
                sb.Append(type.Name);
                return;
            }

            AppendTypeName(sb, type.DeclaringType);
            sb.Append('+');
            sb.Append(type.Name);
        }

        private static void AppendGenericIfRequired(StringBuilder sb, Type type)
        {
            if (type.IsGenericType)
            {
                sb.Append('[');
                for (int i = 0; i < type.GenericTypeArguments.Length; i++)
                {
                    sb.Append('[');
                    AppendTypeNameForJson(sb, type.GenericTypeArguments[i]);
                    sb.Append(']');
                    if (i < type.GenericTypeArguments.Length - 1)
                    {
                        sb.Append(',');
                    }
                }

                sb.Append(']');
            }
        }

        public static void AppendTypeNameForJson(StringBuilder sb, Type type)
        {
            //string typeName = type.AssemblyQualifiedName;
            sb.Append(type.AssemblyQualifiedName);
            //sb.Append(RemoveAssemblyDetails(type.AssemblyQualifiedName)); //TODO: does not work yet
            /*
            sb.Append(type.Namespace);
            sb.Append('.');
            AppendTypeName(sb, type);
            AppendGenericIfRequired(sb, type);
            sb.Append(',');
            sb.Append(' ');
            sb.Append(type.Assembly.GetName().Name);
            */
        }
        
        
        //TODO: improve this
        private static string RemoveAssemblyDetails(string fullyQualifiedTypeName)
        {
            StringBuilder builder = new StringBuilder();

            // loop through the type name and filter out qualified assembly details from nested type names
            bool writingAssemblyName = false;
            bool skippingAssemblyDetails = false;
            for (int i = 0; i < fullyQualifiedTypeName.Length; i++)
            {
                char current = fullyQualifiedTypeName[i];
                switch (current)
                {
                    case '[':
                    case ']':
                        writingAssemblyName = false;
                        skippingAssemblyDetails = false;
                        builder.Append(current);
                        break;
                    case ',':
                        if (!writingAssemblyName)
                        {
                            writingAssemblyName = true;
                            builder.Append(current);
                        }
                        else
                        {
                            skippingAssemblyDetails = true;
                        }
                        break;
                    default:
                        if (!skippingAssemblyDetails)
                        {
                            builder.Append(current);
                        }
                        break;
                }
            }

            return builder.ToString();
        }

        public static string GetTypeNameForJson(Type type)
        {
            StringBuilder sb = new StringBuilder();
            AppendTypeNameForJson(sb, type);
            return sb.ToString();
        }
    }
}
