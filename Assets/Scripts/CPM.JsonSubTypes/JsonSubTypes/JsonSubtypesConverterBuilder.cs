using System;
using Newtonsoft.Json;

namespace JsonSubTypes
{
	public class JsonSubtypesConverterBuilder
	{
		private Type _baseType;

		private string _discriminatorProperty;

		private readonly NullableDictionary<object, Type> _subTypeMapping = new NullableDictionary<object, Type>();

		private bool _serializeDiscriminatorProperty;

		private bool _addDiscriminatorFirst = true;

		private Type _fallbackSubtype;

		public static JsonSubtypesConverterBuilder Of(Type baseType, string discriminatorProperty)
		{
			return new JsonSubtypesConverterBuilder
			{
				_baseType = baseType,
				_discriminatorProperty = discriminatorProperty
			};
		}

		public static JsonSubtypesConverterBuilder Of<T>(string discriminatorProperty)
		{
			return Of(typeof(T), discriminatorProperty);
		}

		public JsonSubtypesConverterBuilder SerializeDiscriminatorProperty()
		{
			return SerializeDiscriminatorProperty(addDiscriminatorFirst: true);
		}

		public JsonSubtypesConverterBuilder SerializeDiscriminatorProperty(bool addDiscriminatorFirst)
		{
			_serializeDiscriminatorProperty = true;
			_addDiscriminatorFirst = addDiscriminatorFirst;
			return this;
		}

		public JsonSubtypesConverterBuilder RegisterSubtype(Type subtype, object value)
		{
			_subTypeMapping.Add(value, subtype);
			return this;
		}

		public JsonSubtypesConverterBuilder RegisterSubtype<T>(object value)
		{
			return RegisterSubtype(typeof(T), value);
		}

		public JsonSubtypesConverterBuilder SetFallbackSubtype(Type fallbackSubtype)
		{
			_fallbackSubtype = fallbackSubtype;
			return this;
		}

		public JsonSubtypesConverterBuilder SetFallbackSubtype<T>()
		{
			return SetFallbackSubtype(typeof(T));
		}

		public JsonConverter Build()
		{
			return new JsonSubtypesByDiscriminatorValueConverter(_baseType, _discriminatorProperty, _subTypeMapping, _serializeDiscriminatorProperty, _addDiscriminatorFirst, _fallbackSubtype);
		}
	}
}
