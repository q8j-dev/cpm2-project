using System;

namespace JsonSubTypes
{
	internal class JsonSubtypesConverter : JsonSubtypes
	{
		private readonly Type _baseType;

		private readonly Type _fallbackType;

		public JsonSubtypesConverter(Type baseType, Type fallbackType)
		{
			_baseType = baseType;
			_fallbackType = fallbackType;
		}

		public JsonSubtypesConverter(Type baseType, string jsonDiscriminatorPropertyName, Type fallbackType)
			: base(jsonDiscriminatorPropertyName)
		{
			_baseType = baseType;
			_fallbackType = fallbackType;
		}

		internal override Type GetFallbackSubType(Type type)
		{
			return _fallbackType;
		}

		public override bool CanConvert(Type objectType)
		{
			if (!(objectType == _baseType))
			{
				return JsonSubtypes.ToTypeInfo(_baseType).IsAssignableFrom(JsonSubtypes.ToTypeInfo(objectType));
			}
			return true;
		}
	}
}
