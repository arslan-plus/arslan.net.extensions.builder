using System;
using System.Reflection;

namespace Arslan.Net.Extensions.Builder
{
    internal static class BuilderHelper
    {
        private static MemberInfo GetMemberInfo(Type type, string key, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) {
            var t = type;
            while (t != null)
            {
                var propertyInfo = t.GetProperty(key, bindingFlags);
                if (propertyInfo == null)
                {
                    t = t.BaseType;
                    continue;
                }

                return propertyInfo;
            }

            t = type;
            while (t != null)
            {
                var fieldInfo = t.GetField(key, bindingFlags);
                if (fieldInfo == null)
                {
                    t = t.BaseType;
                    continue;
                }

                return fieldInfo;
            }

            return default;
        }

        public static (MemberInfo memberInfo, object instance) GetMemberInfo(object instance, string key, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var keys = key.Split('.');

            var memberInfo = GetMemberInfo(instance.GetType(), keys[0], bindingFlags);
            if (keys.Length == 1)
                return (memberInfo, instance);

            instance = memberInfo.GetValue(instance);
            key = key.Substring(keys[0].Length + 1);
            return GetMemberInfo(instance, key, bindingFlags);

        }
    }
}
