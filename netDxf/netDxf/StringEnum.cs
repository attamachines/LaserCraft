namespace netDxf
{
    using System;
    using System.Collections;
    using System.Reflection;

    public class StringEnum
    {
        private readonly Type enumType;
        private static readonly Hashtable stringValues = new Hashtable();

        public StringEnum(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"Supplied type must be an Enum.  Type was {enumType}");
            }
            this.enumType = enumType;
        }

        public IList GetListValues()
        {
            Type underlyingType = Enum.GetUnderlyingType(this.enumType);
            ArrayList list = new ArrayList();
            foreach (FieldInfo info in this.enumType.GetFields())
            {
                StringValueAttribute[] customAttributes = info.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if ((customAttributes > null) && (customAttributes.Length > 0))
                {
                    object key = Convert.ChangeType(Enum.Parse(this.enumType, info.Name), underlyingType);
                    if (key == null)
                    {
                        throw new Exception();
                    }
                    list.Add(new DictionaryEntry(key, customAttributes[0].Value));
                }
            }
            return list;
        }

        public static string GetStringValue(Enum value)
        {
            if (value == 0)
            {
                throw new ArgumentNullException("value");
            }
            string str = null;
            Type type = value.GetType();
            if (stringValues.ContainsKey(value))
            {
                return ((StringValueAttribute) stringValues[value]).Value;
            }
            StringValueAttribute[] customAttributes = type.GetField(value.ToString()).GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            if ((customAttributes > null) && (customAttributes.Length > 0))
            {
                stringValues.Add(value, customAttributes[0]);
                str = customAttributes[0].Value;
            }
            return str;
        }

        public string GetStringValue(string valueName)
        {
            string stringValue;
            try
            {
                Enum enum2 = (Enum) Enum.Parse(this.enumType, valueName);
                stringValue = GetStringValue(enum2);
            }
            catch
            {
                return null;
            }
            return stringValue;
        }

        public Array GetStringValues()
        {
            ArrayList list = new ArrayList();
            foreach (FieldInfo info in this.enumType.GetFields())
            {
                StringValueAttribute[] customAttributes = info.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if ((customAttributes > null) && (customAttributes.Length > 0))
                {
                    list.Add(customAttributes[0].Value);
                }
            }
            return list.ToArray();
        }

        public bool IsStringDefined(string value) => 
            (Parse(this.enumType, value) > null);

        public bool IsStringDefined(string value, StringComparison comparisonType) => 
            (Parse(this.enumType, value, comparisonType) > null);

        public static bool IsStringDefined(Type enumType, string value) => 
            (Parse(enumType, value) > null);

        public static bool IsStringDefined(Type enumType, string value, StringComparison comparisonType) => 
            (Parse(enumType, value, comparisonType) > null);

        public static object Parse(Type type, string value) => 
            Parse(type, value, StringComparison.Ordinal);

        public static object Parse(Type type, string value, StringComparison comparisonType)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            object obj2 = null;
            string strA = null;
            if (!type.IsEnum)
            {
                throw new ArgumentException($"Supplied type must be an Enum.  Type was {type}");
            }
            foreach (FieldInfo info in type.GetFields())
            {
                StringValueAttribute[] customAttributes = info.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
                if ((customAttributes > null) && (customAttributes.Length > 0))
                {
                    strA = customAttributes[0].Value;
                }
                if (string.Compare(strA, value, comparisonType) == 0)
                {
                    if (Enum.IsDefined(type, info.Name))
                    {
                        obj2 = Enum.Parse(type, info.Name);
                    }
                    return obj2;
                }
            }
            return obj2;
        }

        public Type EnumType =>
            this.enumType;
    }
}

