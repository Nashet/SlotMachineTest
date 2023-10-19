using System;

namespace Socket.Newtonsoft.Json.Converters
{
	public abstract class CustomCreationConverter<T> : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException("CustomCreationConverter should only be used while deserializing.");
		}

		public override object ReadJson(
		  JsonReader reader,
		  Type objectType,
		  object existingValue,
		  JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;
			T obj = this.Create(objectType);
			if (obj == null)
				throw new JsonSerializationException("No object created.");
			serializer.Populate(reader, obj);
			return obj;
		}

		public abstract T Create(Type objectType);

		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}

		public override bool CanWrite
		{
			get { return false; }
		}
	}
}