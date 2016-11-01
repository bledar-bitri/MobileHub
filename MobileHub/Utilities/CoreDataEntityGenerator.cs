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
                    string proName = propInfo.Name.Substring(0, 1).ToLower() + propInfo.Name.Substring(1);
                    
                    sb.Append("<attribute name=\"");
                    sb.Append(proName);
                    sb.Append("\" optional=\"YES\" attributeType=\"");
                    sb.Append(GetObjectiveCType(propInfo.PropertyType.Name, proName));
                    sb.Append("\" syncable=\"YES\"/>");
                    sb.Append(Environment.NewLine);
                }
            }
            sb.Append("</entity>");
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        public static string GetCoreDataEntityPosition(Type type, int positionX, int positionY)
        {
            return
                $"<element name=\"{type.Name}\" positionX=\"{positionX}\" positionY=\"{positionY}\" width=\"150\" height=\"100\"/>{Environment.NewLine}";
        }

        public static string GetObjectiveCType(string csharpType, string propName)
        {


            if (propName.Equals("createdAt", StringComparison.InvariantCultureIgnoreCase) ||
                propName.Equals("updatedAt", StringComparison.InvariantCultureIgnoreCase))
            {
                return "Date";
            }

            csharpType = csharpType.ToLower();

            if (csharpType.StartsWith("int"))
            {
                return "Integer 64";
            }
            if (csharpType == "decimal" || csharpType == "double")
            {
                return "Double";
            }
            if (csharpType == "boolean")
            {
                return "Boolean";
            }
            if (csharpType == "datetime" || csharpType == "datetimeoffset")
            {
                return "Date";
            }
            return "String";
        }

    }
}
