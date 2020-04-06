using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Arslan.Net.Extensions.Builder {
    public class Builder<T> where T : class {
        private readonly int _type;
        private readonly T _value;
        private readonly Dictionary<string, Builder<object>> _values = new Dictionary<string, Builder<object>>();

        internal Builder(T value, int type) {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _type = type;
            _value = value.Clone();
        }

        public Builder<T> Set(string key, object value, bool autoCast = false, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var keys = key.Split('.');
            var property = keys[0];
            if (keys.Length > 1) {
                if(!_values.TryGetValue(property, out var builder)) {
                    var (m, i) = BuilderHelper.GetMemberInfo(_value, property, bindingFlags);
                    builder = new Builder<object>(i, m is PropertyInfo ? 1 : 2);
                    _values.Add(property, builder);
                }

                builder.Set(key.Substring(property.Length + 1), value, autoCast, bindingFlags);
                return this;
            }


            _values.Remove(key);
            var (memberInfo, instance) = BuilderHelper.GetMemberInfo(_value, key, bindingFlags);
            memberInfo.SetValue(instance, value, autoCast);
            return this;
        }

        public Builder<T> Set<V>(Expression<Func<T, V>> key, V value, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var property = key.Body.ToString();
            var keys = property.Split('.');
            property = property.Substring(keys[0].Length + 1);
            return Set(property, value, false, bindingFlags);
        }

        public Builder<T> Patch<I>(I patch, bool autoCast = false, BindingFlags bindingFlags= BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where I : class {
            if (patch == null)
                throw new ArgumentNullException(nameof(patch));

            var properties = patch.GetType().GetProperties(bindingFlags);
            foreach (var p in properties) {
                try {
                    Set(p.Name, p.GetValue(patch), autoCast, bindingFlags);
                } catch (Exception ex) { }
            }

            var fields = patch.GetType().GetFields(bindingFlags);
            foreach (var f in fields) {
                try {
                    Set(f.Name, f.GetValue(patch), autoCast, bindingFlags);
                } catch (Exception ex) { }
            }

            return this;
        }

        public T Build() {
            foreach (var kv in _values) {
                if(kv.Value._type == 1)
                    Set(kv.Key, kv.Value.Build());
            }

            foreach (var kv in _values) {
                if (kv.Value._type == 2)
                    Set(kv.Key, kv.Value.Build());
            }

            return _value;
        }

        public static implicit operator T(Builder<T> builder) => builder.Build();
    }
}
