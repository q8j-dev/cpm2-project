using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

namespace JsonSubTypes
{
	public class JsonSubtypes : JsonConverter
	{
		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
		public class KnownSubTypeAttribute : Attribute
		{
			public Type SubType { get; }

			public object AssociatedValue { get; }

			public KnownSubTypeAttribute(Type subType, object associatedValue)
			{
				SubType = subType;
				AssociatedValue = associatedValue;
			}
		}

		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
		public class FallBackSubTypeAttribute : Attribute
		{
			public Type SubType { get; }

			public FallBackSubTypeAttribute(Type subType)
			{
				SubType = subType;
			}
		}

		[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
		public class KnownSubTypeWithPropertyAttribute : Attribute
		{
			public Type SubType { get; }

			public string PropertyName { get; }

			public bool StopLookupOnMatch { get; set; }

			public KnownSubTypeWithPropertyAttribute(Type subType, string propertyName)
			{
				SubType = subType;
				PropertyName = propertyName;
			}
		}

		protected readonly string JsonDiscriminatorPropertyName;

		[ThreadStatic]
		private static bool _isInsideRead;

		[ThreadStatic]
		private static JsonReader _reader;

		private static readonly ConcurrentDictionary<Type, IEnumerable<object>> _attributesCache = new ConcurrentDictionary<Type, IEnumerable<object>>();

		private static readonly Func<Type, IEnumerable<object>> _getCustomAttributes = (Type ti) => ti.GetCustomAttributes(inherit: false);

		public override bool CanRead
		{
			get
			{
				if (!_isInsideRead)
				{
					return true;
				}
				return !string.IsNullOrEmpty(_reader.Path);
			}
		}

		public override bool CanWrite => false;

		[Preserve]
		public JsonSubtypes()
		{
		}

		[Preserve]
		public JsonSubtypes(string jsonDiscriminatorPropertyName)
		{
			JsonDiscriminatorPropertyName = jsonDiscriminatorPropertyName;
		}

		public override bool CanConvert(Type objectType)
		{
			return false;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return ReadJson(reader, objectType, serializer);
		}

		private object ReadJson(JsonReader reader, Type objectType, JsonSerializer serializer)
		{
			while (reader.TokenType == JsonToken.Comment)
			{
				reader.Read();
			}
			switch (reader.TokenType)
			{
			case JsonToken.Null:
				return null;
			case JsonToken.StartObject:
				return ReadObject(reader, objectType, serializer);
			case JsonToken.StartArray:
			{
				Type elementType = GetElementType(objectType);
				if (elementType == null)
				{
					throw CreateJsonReaderException(reader, "Impossible to read JSON array to fill type: " + objectType.Name);
				}
				return ReadArray(reader, objectType, elementType, serializer);
			}
			default:
				throw CreateJsonReaderException(reader, $"Unrecognized token: {reader.TokenType}");
			}
		}

		private static JsonReaderException CreateJsonReaderException(JsonReader reader, string message)
		{
			int lineNumber = 0;
			int linePosition = 0;
			if (reader is IJsonLineInfo jsonLineInfo && jsonLineInfo.HasLineInfo())
			{
				lineNumber = jsonLineInfo.LineNumber;
				linePosition = jsonLineInfo.LinePosition;
			}
			return new JsonReaderException(message, reader.Path, lineNumber, linePosition, null);
		}

		private IList ReadArray(JsonReader reader, Type targetType, Type elementType, JsonSerializer serializer)
		{
			IList list = CreateCompatibleList(targetType, elementType);
			while (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				list.Add(ReadJson(reader, elementType, serializer));
			}
			if (!targetType.IsArray)
			{
				return list;
			}
			Array array = Array.CreateInstance(elementType, list.Count);
			list.CopyTo(array, 0);
			return array;
		}

		private static IList CreateCompatibleList(Type targetContainerType, Type elementType)
		{
			Type type = ToTypeInfo(targetContainerType);
			if (type.IsArray || type.IsAbstract)
			{
				return (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
			}
			return (IList)Activator.CreateInstance(targetContainerType);
		}

		private static Type GetElementType(Type arrayOrGenericContainer)
		{
			if (arrayOrGenericContainer.IsArray)
			{
				return arrayOrGenericContainer.GetElementType();
			}
			return GetGenericTypeArguments(arrayOrGenericContainer).FirstOrDefault();
		}

		private object ReadObject(JsonReader reader, Type objectType, JsonSerializer serializer)
		{
			JObject jObject = JObject.Load(reader);
			Type targetType = GetType(jObject, objectType, serializer) ?? objectType;
			return ThreadStaticReadObject(reader, serializer, jObject, targetType);
		}

		private static JsonReader CreateAnotherReader(JToken jToken, JsonReader reader)
		{
			JsonReader jsonReader = jToken.CreateReader();
			jsonReader.Culture = reader.Culture;
			jsonReader.CloseInput = reader.CloseInput;
			jsonReader.SupportMultipleContent = reader.SupportMultipleContent;
			jsonReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
			jsonReader.FloatParseHandling = reader.FloatParseHandling;
			jsonReader.DateFormatString = reader.DateFormatString;
			jsonReader.DateParseHandling = reader.DateParseHandling;
			jsonReader.MaxDepth = reader.MaxDepth;
			return jsonReader;
		}

		private Type ResolveType(JObject jObject, Type parentType, JsonSerializer serializer)
		{
			Type type = ((JsonDiscriminatorPropertyName != null) ? GetTypeFromDiscriminatorValue(jObject, parentType, serializer) : GetTypeByPropertyPresence(jObject, parentType));
			return type ?? GetFallbackSubType(parentType);
		}

		private Type GetType(JObject jObject, Type parentType, JsonSerializer serializer)
		{
			Type type = parentType;
			JsonSubtypes jsonSubtypes = null;
			JsonSubtypes currentTypeResolver = this;
			HashSet<Type> hashSet = new HashSet<Type> { type };
			List<JsonSubtypes> list = serializer.Converters.OfType<JsonSubtypes>().ToList();
			while (currentTypeResolver != null && currentTypeResolver != jsonSubtypes)
			{
				type = currentTypeResolver.ResolveType(jObject, type, serializer);
				if (!hashSet.Add(type))
				{
					break;
				}
				jsonSubtypes = currentTypeResolver;
				list = list.Where((JsonSubtypes c) => c != currentTypeResolver).ToList();
				currentTypeResolver = GetTypeResolver(ToTypeInfo(type), list);
			}
			return type;
		}

		private JsonSubtypes GetTypeResolver(Type targetType, IEnumerable<JsonSubtypes> jsonConverterCollection)
		{
			if (targetType == null)
			{
				return null;
			}
			JsonConverterAttribute attribute = GetAttribute<JsonConverterAttribute>(targetType);
			if (attribute != null && ToTypeInfo(typeof(JsonSubtypes)).IsAssignableFrom(ToTypeInfo(attribute.ConverterType)))
			{
				return (JsonSubtypes)Activator.CreateInstance(attribute.ConverterType, attribute.ConverterParameters);
			}
			return jsonConverterCollection.FirstOrDefault((JsonSubtypes c) => c.CanConvert(ToType(targetType)));
		}

		private Type GetTypeByPropertyPresence(JObject jObject, Type parentType)
		{
			List<TypeWithPropertyMatchingAttributes> typesByPropertyPresence = GetTypesByPropertyPresence(parentType);
			HashSet<Type> hashSet = new HashSet<Type>();
			foreach (TypeWithPropertyMatchingAttributes item in typesByPropertyPresence)
			{
				Type type = null;
				if (TryGetValueInJson(jObject, item.JsonPropertyName, out var _))
				{
					type = item.Type;
				}
				else if (jObject.SelectToken(item.JsonPropertyName) != null)
				{
					type = item.Type;
				}
				if (type != null)
				{
					if (item.StopLookupOnMatch)
					{
						return item.Type;
					}
					hashSet.Add(type);
				}
			}
			if (hashSet.Count == 1)
			{
				return hashSet.First();
			}
			if (hashSet.Count > 1)
			{
				throw new JsonSerializationException("Ambiguous type resolution, expected only one type but got: " + string.Join(", ", hashSet.Select((Type t) => t.FullName).ToArray()));
			}
			return null;
		}

		internal virtual List<TypeWithPropertyMatchingAttributes> GetTypesByPropertyPresence(Type parentType)
		{
			return (from a in GetAttributes<KnownSubTypeWithPropertyAttribute>(ToTypeInfo(parentType))
				select new TypeWithPropertyMatchingAttributes(a.SubType, a.PropertyName, a.StopLookupOnMatch)).ToList();
		}

		private Type GetTypeFromDiscriminatorValue(JObject jObject, Type parentType, JsonSerializer serializer)
		{
			if (!TryGetValueInJson(jObject, JsonDiscriminatorPropertyName, out var value))
			{
				value = jObject.SelectToken(JsonDiscriminatorPropertyName);
			}
			if (value == null)
			{
				return null;
			}
			NullableDictionary<object, Type> subTypeMapping = GetSubTypeMapping(parentType);
			if (subTypeMapping.Entries().Any())
			{
				return GetTypeFromMapping(subTypeMapping, value, serializer);
			}
			return GetTypeByName(value.Value<string>(), ToTypeInfo(parentType));
		}

		private static bool TryGetValueInJson(IDictionary<string, JToken> jObject, string propertyName, out JToken value)
		{
			if (jObject.TryGetValue(propertyName, out value))
			{
				return true;
			}
			string text = jObject.Keys.FirstOrDefault((string jsonProperty) => string.Equals(jsonProperty, propertyName, StringComparison.OrdinalIgnoreCase));
			if (text == null)
			{
				return false;
			}
			value = jObject[text];
			return true;
		}

		private static Type GetTypeByName(string typeName, Type parentType)
		{
			if (typeName == null)
			{
				return null;
			}
			Assembly assembly = parentType.Assembly;
			string fullName = parentType.FullName;
			Type type = assembly.GetType(typeName);
			if (fullName != null && type == null)
			{
				string text = fullName.Substring(0, fullName.Length - parentType.Name.Length);
				type = assembly.GetType(text + typeName, throwOnError: false, ignoreCase: true);
			}
			Type type2 = ToTypeInfo(type);
			if (type2 == null)
			{
				return null;
			}
			if (parentType.IsAssignableFrom(type2) || (parentType.IsGenericType && IsSubclassOfRawGeneric(parentType, type2)))
			{
				return type;
			}
			return null;
		}

		private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
		{
			Type type = toCheck;
			Type type2 = ToTypeInfo(typeof(object));
			while (generic != type && toCheck != type2)
			{
				type = (toCheck.IsGenericType ? ToTypeInfo(toCheck.GetGenericTypeDefinition()) : toCheck);
				toCheck = ToTypeInfo(toCheck.BaseType);
			}
			return generic == type;
		}

		private static Type GetTypeFromMapping(NullableDictionary<object, Type> typeMapping, JToken discriminatorToken, JsonSerializer serializer)
		{
			if (discriminatorToken.Type == JTokenType.Null)
			{
				typeMapping.TryGetValue(null, out var value);
				return value;
			}
			object obj = typeMapping.NotNullKeys().FirstOrDefault();
			if (obj != null)
			{
				Type type = obj.GetType();
				object key = discriminatorToken.ToObject(type, serializer);
				if (typeMapping.TryGetValue(key, out var value2))
				{
					return value2;
				}
			}
			return null;
		}

		internal virtual NullableDictionary<object, Type> GetSubTypeMapping(Type type)
		{
			NullableDictionary<object, Type> dictionary = new NullableDictionary<object, Type>();
			GetAttributes<KnownSubTypeAttribute>(ToTypeInfo(type)).ToList().ForEach((KnownSubTypeAttribute x) =>
			{
				dictionary.Add(x.AssociatedValue, x.SubType);
			});
			return dictionary;
		}

		internal virtual Type GetFallbackSubType(Type type)
		{
			return GetAttribute<FallBackSubTypeAttribute>(ToTypeInfo(type))?.SubType;
		}

		private static object ThreadStaticReadObject(JsonReader reader, JsonSerializer serializer, JToken jToken, Type targetType)
		{
			_reader = CreateAnotherReader(jToken, reader);
			_isInsideRead = true;
			try
			{
				return serializer.Deserialize(_reader, targetType);
			}
			finally
			{
				_isInsideRead = false;
			}
		}

		private static IEnumerable<object> GetAttributes(Type typeInfo)
		{
			return _attributesCache.GetOrAdd(typeInfo, _getCustomAttributes);
		}

		private static IEnumerable<T> GetAttributes<T>(Type typeInfo) where T : Attribute
		{
			return GetAttributes(typeInfo).OfType<T>();
		}

		private static T GetAttribute<T>(Type typeInfo) where T : Attribute
		{
			return GetAttributes<T>(typeInfo).FirstOrDefault();
		}

		private static IEnumerable<Type> GetGenericTypeArguments(Type type)
		{
			return type.GenericTypeArguments;
		}

		internal static Type ToTypeInfo(Type type)
		{
			return type;
		}

		internal static Type ToType(Type typeInfo)
		{
			return typeInfo;
		}
	}
}
