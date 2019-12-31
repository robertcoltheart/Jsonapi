using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jsonapi.Serialization
{
    internal class ReflectionJsonPropertyInfo<TClass, TProperty> : JsonPropertyInfo<TClass>
    {
        public ReflectionJsonPropertyInfo(PropertyInfo property, JsonConverter converter, JsonSerializerOptions options)
            : base(property, options)
        {
            Get = CreateGetter(property);
            Set = CreateSetter(property);

            HasGetter = Get != null;
            HasSetter = Set != null;

            Converter = converter as JsonConverter<TProperty>;
        }

        public Func<object, TProperty> Get { get; }

        public Action<object, TProperty> Set { get; }

        public override bool HasGetter { get; }

        public override bool HasSetter { get; }

        public JsonConverter<TProperty> Converter { get; }

        public override object GetValueAsObject(object resource)
        {
            return Get(resource);
        }

        public override void SetValueAsObject(object resource, object value)
        {
            var typedValue = (TProperty) value;

            if (typedValue != null)
            {
                Set(resource, (TProperty) value);
            }
        }

        public override void Read(TClass resource, ref Utf8JsonReader reader)
        {
            var value = Converter.Read(ref reader, PropertyType, Options);

            Set(resource, value);

            reader.Read();
        }

        private Func<object, TProperty> CreateGetter(PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod();

            if (getMethod == null)
            {
                return null;
            }

            var getter = CreateDelegate<GetProperty>(getMethod);

            return item => getter((TClass) item);
        }
        
        private Action<object, TProperty> CreateSetter(PropertyInfo property)
        {
            var setMethod = property.GetSetMethod();

            if (setMethod == null)
            {
                return null;
            }

            var setter = CreateDelegate<SetProperty>(setMethod);

            return (resource, value) => setter((TClass) resource, value);
        }

        private TDelegate CreateDelegate<TDelegate>(MethodInfo methodInfo)
            where TDelegate : Delegate
        {
            return (TDelegate) Delegate.CreateDelegate(typeof(TDelegate), methodInfo);
        }

        private delegate TProperty GetProperty(TClass obj);

        private delegate void SetProperty(TClass obj, TProperty value);
    }
}
