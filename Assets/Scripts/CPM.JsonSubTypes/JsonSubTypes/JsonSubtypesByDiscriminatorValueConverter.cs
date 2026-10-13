using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSubTypes
{
	internal class JsonSubtypesByDiscriminatorValueConverter : JsonSubtypesConverter
	{
		[ThreadStatic]
		private static bool _isInsideWrite;

		[ThreadStatic]
		private static bool _allowNextWrite;

		private readonly bool _addDiscriminatorFirst;

		private readonly bool _serializeDiscriminatorProperty;

		private readonly Dictionary<Type, object> _supportedTypes = new Dictionary<Type, object>();

		private readonly NullableDictionary<object, Type> _subTypeMapping;

		public override bool CanWrite
		{
			get
			{
				if (!_serializeDiscriminatorProperty)
				{
					return false;
				}
				if (!_isInsideWrite)
				{
					return true;
				}
				if (_allowNextWrite)
				{
					_allowNextWrite = false;
					return true;
				}
				_allowNextWrite = true;
				return false;
			}
		}

		internal JsonSubtypesByDiscriminatorValueConverter(Type baseType, string discriminatorProperty, NullableDictionary<object, Type> subTypeMapping, bool serializeDiscriminatorProperty, bool addDiscriminatorFirst, Type fallbackType)
			: base(baseType, discriminatorProperty, fallbackType)
		{
			_serializeDiscriminatorProperty = serializeDiscriminatorProperty;
			_subTypeMapping = subTypeMapping;
			_addDiscriminatorFirst = addDiscriminatorFirst;
			foreach (KeyValuePair<object, Type> item in _subTypeMapping.Entries())
			{
				if (_supportedTypes.ContainsKey(item.Value))
				{
					if (_serializeDiscriminatorProperty)
					{
						throw new InvalidOperationException("Multiple discriminators on single type are not supported when discriminator serialization is enabled");
					}
				}
				else
				{
					_supportedTypes.Add(item.Value, item.Key);
				}
			}
		}

		internal override NullableDictionary<object, Type> GetSubTypeMapping(Type type)
		{
			return _subTypeMapping;
		}

		public override bool CanConvert(Type objectType)
		{
			if (!base.CanConvert(objectType))
			{
				return _supportedTypes.ContainsKey(objectType);
			}
			return true;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			_isInsideWrite = true;
			_allowNextWrite = false;
			JObject jObject;
			try
			{
				jObject = JObject.FromObject(value, serializer);
			}
			finally
			{
				_isInsideWrite = false;
			}
			if (!_supportedTypes.TryGetValue(value.GetType(), out var value2))
			{
				throw new JsonSerializationException("Impossible to serialize type: " + value.GetType().FullName + " because there is no registered mapping for the discriminator property");
			}
			JToken jToken = JToken.FromObject(value2, serializer);
			if (_addDiscriminatorFirst)
			{
				jObject.AddFirst(new JProperty(JsonDiscriminatorPropertyName, jToken));
			}
			else
			{
				jObject.Add(JsonDiscriminatorPropertyName, jToken);
			}
			jObject.WriteTo(writer);
		}
	}
}
