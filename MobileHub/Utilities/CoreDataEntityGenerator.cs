using System;
using System.Reflection;
using System.Text;

namespace Utilities
{
    public class CoreDataEntityGenerator
    {
        public static String GetCoreDataEntity(Type type)
        {
            var sb = new StringBuilder("<entity name=\"");
            sb.Append(type.Name);
            sb.Append("\" syncable=\"YES\">");
            sb.Append(Environment.NewLine);
            var fieldInf = type.GetProperties();
            foreach (var propInfo in fieldInf)
            {
                if (propInfo.MemberType == MemberTypes.Property && propInfo.Name != "TableName")
                {
                    string varName = propInfo.Name.Substring(0, 1).ToLower() + propInfo.Name.Substring(1);
                    
                    sb.Append("<attribute name=\"");
                    sb.Append(varName);
                    sb.Append("\" optional=\"YES\" attributeType=\"");
                    sb.Append(GetObjectiveCType(propInfo.PropertyType.Name));
                    sb.Append("\" syncable=\"YES\"/>");
                    sb.Append(Environment.NewLine);
                }
            }
            sb.Append("</entity>");
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public static string GetObjectiveCType(string csharpType)
        {
            csharpType = csharpType.ToLower();
            if (csharpType.StartsWith("int"))
            {
                return "Integer 64";
            }
            else if (csharpType == "decimal" || csharpType == "double")
            {
                return "Double";
            }
            else if (csharpType == "boolean")
            {
                return "Boolean";
            }
            else if (csharpType == "datetime")
            {
                return "NSDate";
            }
            return "String";
        }

    }
}
