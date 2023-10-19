using Socket.Newtonsoft.Json.Serialization;
using System;
using System.Data;

namespace Socket.Newtonsoft.Json.Converters
{
	public class DataSetConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			DataSet dataSet = (DataSet)value;
			DefaultContractResolver contractResolver = serializer.ContractResolver as DefaultContractResolver;
			DataTableConverter dataTableConverter = new DataTableConverter();
			writer.WriteStartObject();
			foreach (DataTable table in (InternalDataCollectionBase)dataSet.Tables)
			{
				writer.WritePropertyName(contractResolver != null
				  ? contractResolver.GetResolvedPropertyName(table.TableName)
				  : table.TableName);
				dataTableConverter.WriteJson(writer, table, serializer);
			}

			writer.WriteEndObject();
		}

		public override object ReadJson(
		  JsonReader reader,
		  Type objectType,
		  object existingValue,
		  JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;
			DataSet dataSet = objectType == typeof(DataSet) ? new DataSet() : (DataSet)Activator.CreateInstance(objectType);
			DataTableConverter dataTableConverter = new DataTableConverter();
			reader.ReadAndAssert();
			while (reader.TokenType == JsonToken.PropertyName)
			{
				DataTable table1 = dataSet.Tables[(string)reader.Value];
				int num = table1 != null ? 1 : 0;
				DataTable table2 =
				  (DataTable)dataTableConverter.ReadJson(reader, typeof(DataTable), table1, serializer);
				if (num == 0)
					dataSet.Tables.Add(table2);
				reader.ReadAndAssert();
			}

			return dataSet;
		}

		public override bool CanConvert(Type valueType)
		{
			return typeof(DataSet).IsAssignableFrom(valueType);
		}
	}
}