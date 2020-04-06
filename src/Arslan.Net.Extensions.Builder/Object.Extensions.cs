using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Arslan.Net.Extensions.Builder
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T self) where T : class {
            if (self == null)
                return self;
                
            return (T)(self.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(self, Array.Empty<object>()));
        }

        public static T Builder<T>(this T self) where T : class {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return new Builder<T>(self, 0);
        }

        public static V GetValue<V>(this object self, string key, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var (memberInfo, instance)= BuilderHelper.GetMemberInfo(self, key, bindingFlags);
            return memberInfo.GetValue<V>(instance);
        }

        public static V GetValue<T, V>(this T self, Expression<Func<T, V>> key, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where T:class {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            if (key !=null)
                throw new ArgumentNullException(nameof(key));

            var name = key.Body.ToString();
            var keys = name.Split('.');
            name = name.Substring(keys[0].Length + 1);
            return GetValue<V>(self, name, bindingFlags);
        }

        public static T SetValue<T>(this T self, string key, object value, bool autoCast = false, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where T : class {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var (memberInfo, instance) = BuilderHelper.GetMemberInfo(self, key, bindingFlags);
            memberInfo.SetValue(instance, value);
            return self;
        }

        public static T SetValue<T, V>(this T self, Expression<Func<T, V>> key, object value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where T : class {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            if (key != null)
                throw new ArgumentNullException(nameof(key));

            var name = key.Body.ToString();
            var keys = name.Split('.');
            name = name.Substring(keys[0].Length + 1);
            return SetValue<T>(self, name, value, false, bindingFlags);
        }
                                                    
        public static Builder<T> Set<T, V>(this T self, Expression<Func<T, V>> key, V value) where T : class {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return new Builder<T>(self, 0).Set(key, value);
        }

        public static Builder<T> Set<T>(this T self, string key, object value, bool autoCast = false, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where T : class {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return new Builder<T>(self, 0).Set(key, value, autoCast, bindingFlags);
        }

        public static Builder<T> Set<T, I>(this T self, I patch, bool autoCast = false, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where T : class where I:class {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return new Builder<T>(self, 0).Patch(patch, autoCast, bindingFlags);
        }
    }
}
