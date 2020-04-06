using System;
using System.Reflection;

namespace Arslan.Net.Extensions.Builder
{
    internal static class MemberInfoExtensions
    {
        public static object GetValue(this MemberInfo self, object instance) {
            if (self is PropertyInfo p)
                return p.GetValue(instance);

            if (self is FieldInfo f)
                return f.GetValue(instance);

            throw new InvalidOperationException();
        }
        public static V GetValue<V>(this MemberInfo self, object instance) => (V)Convert.ChangeType(GetValue(self, instance), typeof(V));

        public static void SetValue(this MemberInfo self, object instance, object value, bool autoCast = false) {
            if (self is PropertyInfo p)
            {
                if (autoCast)
                    value = Convert.ChangeType(value, p.PropertyType);

                if (p.CanWrite)
                {
                        p.SetValue(instance, value);
                    return;
                }

                var field = p.DeclaringType.GetField($"<{p.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(instance, value);
                    return;
                }
            }

            if (self is FieldInfo f)
            {
                if (autoCast)
                    value = Convert.ChangeType(value, f.FieldType);
                f.SetValue(instance, value);
                return;
            }

            throw new InvalidOperationException();
        }

    }
}
