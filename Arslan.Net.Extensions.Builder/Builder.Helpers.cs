using System;
using System.Reflection;

namespace Arslan.Net.Extensions.Builder
{
    public static class BuilderHelper
    {
        public static FieldInfo GetFieldInfo(Type type, string key, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) {
            while (type != null)
            {
                var fieldInfo = type.GetField(key, bindingFlags);
                if (fieldInfo == null)
                {
                    type = type.BaseType;
                    continue;
                }

                return fieldInfo;
            }

            return default;
        }

        public static void SetFieldValue(object instance, FieldInfo fieldInfo, object value, bool autoCast) {
            if (autoCast)
                fieldInfo.SetValue(instance, Convert.ChangeType(value, fieldInfo.FieldType));
            else
                fieldInfo.SetValue(instance, value);
        }

        public static PropertyInfo GetPropertyInfo(Type type, string key, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) {
            while (type != null)
            {
                var propertyInfo = type.GetProperty(key, bindingFlags);
                if (propertyInfo == null)
                {
                    type = type.BaseType;
                    continue;
                }

                return propertyInfo;
            }

            return default;
        }

        public static void SetPropertyValue(object instance, PropertyInfo propertyInfo, object value, bool autoCast) {
            if (propertyInfo.CanWrite)
            {
                if (autoCast)
                    propertyInfo.SetValue(instance, Convert.ChangeType(value, propertyInfo.PropertyType));
                else
                    propertyInfo.SetValue(instance, value);

                return;
            }

            var field = propertyInfo.DeclaringType.GetField($"<{propertyInfo.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            SetFieldValue(instance, field, value, autoCast);
        }

       
    }
}
