using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace KitCashProtocol
{
    public static class EnumHelper
    {
        public static string GetTypeDescription(Enum type)
        {
            FieldInfo fi = type.GetType().GetField(type.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return type.ToString();
            }
        }

        public static List<string> GetValues(Type type)
        {
            return Enum.GetValues(type).Cast<object>().Select<object, string>(x => x.ToString()).ToList();
        }

        public static object GetValueByName(Type type, string name)
        {
            IEnumerable<object> objects = Enum.GetValues(type).Cast<object>();
            foreach (object obj in objects)
            {
                if (obj.ToString() == name)
                {
                    return obj;
                }
            }

            throw new ArgumentException("EnumHelper.GetValueByName(...)." + name);
        }

        public static Dictionary<string, string> GetNamesAndDescriptions(Type type)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (FieldInfo field in type.GetFields())
            {
                if (field.Name == "value__") continue;
                DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                string description = (attributes.Length > 0 ? attributes[0].Description : field.Name);
                result.Add(field.Name, description);
            }

            return result;
        }
    }
}