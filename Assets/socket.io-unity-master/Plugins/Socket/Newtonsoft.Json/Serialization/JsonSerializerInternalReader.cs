using Socket.Newtonsoft.Json.Linq;
using Socket.Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Socket.Newtonsoft.Json.Serialization
{
	internal class JsonSerializerInternalReader : JsonSerializerInternalBase
	{
		public JsonSerializerInternalReader(JsonSerializer serializer)
		  : base(serializer)
		{
		}

		public void Populate(JsonReader reader, object target)
		{
			ValidationUtils.ArgumentNotNull(target, nameof(target));
			Type type = target.GetType();
			JsonContract jsonContract = this.Serializer._contractResolver.ResolveContract(type);
			if (!reader.MoveToContent())
				throw JsonSerializationException.Create(reader, "No JSON content found.");
			if (reader.TokenType == JsonToken.StartArray)
			{
				if (jsonContract.ContractType != JsonContractType.Array)
					throw JsonSerializationException.Create(reader,
					  "Cannot populate JSON array onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture,
						 type));
				JsonArrayContract contract = (JsonArrayContract)jsonContract;
				this.PopulateList(contract.ShouldCreateWrapper ? contract.CreateWrapper(target) : (IList)target,
				  reader, contract, null, null);
			}
			else
			{
				if (reader.TokenType != JsonToken.StartObject)
					throw JsonSerializationException.Create(reader,
					  "Unexpected initial token '{0}' when populating object. Expected JSON object or array.".FormatWith(
						 CultureInfo.InvariantCulture, reader.TokenType));
				reader.ReadAndAssert();
				string id = null;
				if (this.Serializer.MetadataPropertyHandling != MetadataPropertyHandling.Ignore &&
					reader.TokenType == JsonToken.PropertyName &&
					string.Equals(reader.Value.ToString(), "$id", StringComparison.Ordinal))
				{
					reader.ReadAndAssert();
					id = reader.Value?.ToString();
					reader.ReadAndAssert();
				}

				if (jsonContract.ContractType == JsonContractType.Dictionary)
				{
					JsonDictionaryContract contract = (JsonDictionaryContract)jsonContract;
					this.PopulateDictionary(
					  contract.ShouldCreateWrapper ? contract.CreateWrapper(target) : (IDictionary)target, reader,
					  contract, null, id);
				}
				else
				{
					if (jsonContract.ContractType != JsonContractType.Object)
						throw JsonSerializationException.Create(reader,
						  "Cannot populate JSON object onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture,
							 type));
					this.PopulateObject(target, reader, (JsonObjectContract)jsonContract, null, id);
				}
			}
		}

		private JsonContract GetContractSafe(Type type)
		{
			if (type == null)
				return null;
			return this.Serializer._contractResolver.ResolveContract(type);
		}

		public object Deserialize(JsonReader reader, Type objectType, bool checkAdditionalContent)
		{
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));
			JsonContract contractSafe = this.GetContractSafe(objectType);
			try
			{
				JsonConverter converter = this.GetConverter(contractSafe, null, null,
				   null);
				if (reader.TokenType == JsonToken.None && !this.ReadForType(reader, contractSafe, converter != null))
				{
					if (contractSafe != null && !contractSafe.IsNullable)
						throw JsonSerializationException.Create(reader,
						  "No JSON content found and type '{0}' is not nullable.".FormatWith(
							 CultureInfo.InvariantCulture, contractSafe.UnderlyingType));
					return null;
				}

				object obj = converter == null || !converter.CanRead
				  ? this.CreateValueInternal(reader, objectType, contractSafe, null,
					 null, null, null)
				  : this.DeserializeConvertable(converter, reader, objectType, null);
				if (checkAdditionalContent)
				{
					while (reader.Read())
					{
						if (reader.TokenType != JsonToken.Comment)
							throw JsonSerializationException.Create(reader,
							  "Additional text found in JSON string after finishing deserializing object.");
					}
				}

				return obj;
			}
			catch (Exception ex)
			{
				if (this.IsErrorHandled(null, contractSafe, null, reader as IJsonLineInfo, reader.Path, ex))
				{
					this.HandleError(reader, false, 0);
					return null;
				}

				this.ClearErrorContext();
				throw;
			}
		}

		private JsonSerializerProxy GetInternalSerializer()
		{
			if (this.InternalSerializer == null)
				this.InternalSerializer = new JsonSerializerProxy(this);
			return this.InternalSerializer;
		}

		private JToken CreateJToken(JsonReader reader, JsonContract contract)
		{
			ValidationUtils.ArgumentNotNull(reader, nameof(reader));
			if (contract != null)
			{
				if (contract.UnderlyingType == typeof(JRaw))
					return JRaw.Create(reader);
				if (reader.TokenType == JsonToken.Null && contract.UnderlyingType != typeof(JValue) &&
					contract.UnderlyingType != typeof(JToken))
					return null;
			}

			using (JTokenWriter jtokenWriter = new JTokenWriter())
			{
				jtokenWriter.WriteToken(reader);
				return jtokenWriter.Token;
			}
		}

		private JToken CreateJObject(JsonReader reader)
		{
			ValidationUtils.ArgumentNotNull(reader, nameof(reader));
			using (JTokenWriter jtokenWriter = new JTokenWriter())
			{
				jtokenWriter.WriteStartObject();
				do
				{
					if (reader.TokenType == JsonToken.PropertyName)
					{
						string str = (string)reader.Value;
						if (reader.ReadAndMoveToContent())
						{
							if (!this.CheckPropertyName(reader, str))
							{
								jtokenWriter.WritePropertyName(str);
								jtokenWriter.WriteToken(reader, true, true, false);
							}
						}
						else
							break;
					}
					else if (reader.TokenType != JsonToken.Comment)
					{
						jtokenWriter.WriteEndObject();
						return jtokenWriter.Token;
					}
				} while (reader.Read());

				throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
			}
		}

		private object CreateValueInternal(
		  JsonReader reader,
		  Type objectType,
		  JsonContract contract,
		  JsonProperty member,
		  JsonContainerContract containerContract,
		  JsonProperty containerMember,
		  object existingValue)
		{
			if (contract != null && contract.ContractType == JsonContractType.Linq)
				return this.CreateJToken(reader, contract);
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.StartObject:
						return this.CreateObject(reader, objectType, contract, member, containerContract, containerMember,
						  existingValue);
					case JsonToken.StartArray:
						return this.CreateList(reader, objectType, contract, member, existingValue, null);
					case JsonToken.StartConstructor:
						string str = reader.Value.ToString();
						return this.EnsureType(reader, str, CultureInfo.InvariantCulture, contract, objectType);
					case JsonToken.Comment:
						continue;
					case JsonToken.Raw:
						return new JRaw((string)reader.Value);
					case JsonToken.Integer:
					case JsonToken.Float:
					case JsonToken.Boolean:
					case JsonToken.Date:
					case JsonToken.Bytes:
						return this.EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
					case JsonToken.String:
						string s = (string)reader.Value;
						if (JsonSerializerInternalReader.CoerceEmptyStringToNull(objectType, contract, s))
							return null;
						if (objectType == typeof(byte[]))
							return Convert.FromBase64String(s);
						return this.EnsureType(reader, s, CultureInfo.InvariantCulture, contract, objectType);
					case JsonToken.Null:
					case JsonToken.Undefined:
						if (objectType == typeof(DBNull))
							return DBNull.Value;
						return this.EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
					default:
						throw JsonSerializationException.Create(reader,
						  "Unexpected token while deserializing object: " + reader.TokenType);
				}
			} while (reader.Read());

			throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
		}

		private static bool CoerceEmptyStringToNull(Type objectType, JsonContract contract, string s)
		{
			if (string.IsNullOrEmpty(s) && objectType != null &&
				(objectType != typeof(string) && objectType != typeof(object)) && contract != null)
				return contract.IsNullable;
			return false;
		}

		internal string GetExpectedDescription(JsonContract contract)
		{
			switch (contract.ContractType)
			{
				case JsonContractType.Object:
				case JsonContractType.Dictionary:
				case JsonContractType.Serializable:
					return "JSON object (e.g. {\"name\":\"value\"})";
				case JsonContractType.Array:
					return "JSON array (e.g. [1,2,3])";
				case JsonContractType.Primitive:
					return "JSON primitive value (e.g. string, number, boolean, null)";
				case JsonContractType.String:
					return "JSON string value";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private JsonConverter GetConverter(
		  JsonContract contract,
		  JsonConverter memberConverter,
		  JsonContainerContract containerContract,
		  JsonProperty containerProperty)
		{
			JsonConverter jsonConverter = null;
			if (memberConverter != null)
				jsonConverter = memberConverter;
			else if (containerProperty?.ItemConverter != null)
				jsonConverter = containerProperty.ItemConverter;
			else if (containerContract?.ItemConverter != null)
				jsonConverter = containerContract.ItemConverter;
			else if (contract != null)
			{
				if (contract.Converter != null)
				{
					jsonConverter = contract.Converter;
				}
				else
				{
					JsonConverter matchingConverter;
					if ((matchingConverter = this.Serializer.GetMatchingConverter(contract.UnderlyingType)) != null)
						jsonConverter = matchingConverter;
					else if (contract.InternalConverter != null)
						jsonConverter = contract.InternalConverter;
				}
			}

			return jsonConverter;
		}

		private object CreateObject(
		  JsonReader reader,
		  Type objectType,
		  JsonContract contract,
		  JsonProperty member,
		  JsonContainerContract containerContract,
		  JsonProperty containerMember,
		  object existingValue)
		{
			Type objectType1 = objectType;
			string id;
			if (this.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.Ignore)
			{
				reader.ReadAndAssert();
				id = null;
			}
			else if (this.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.ReadAhead)
			{
				JTokenReader reader1 = reader as JTokenReader;
				if (reader1 == null)
				{
					reader1 = (JTokenReader)JToken.ReadFrom(reader).CreateReader();
					reader1.Culture = reader.Culture;
					reader1.DateFormatString = reader.DateFormatString;
					reader1.DateParseHandling = reader.DateParseHandling;
					reader1.DateTimeZoneHandling = reader.DateTimeZoneHandling;
					reader1.FloatParseHandling = reader.FloatParseHandling;
					reader1.SupportMultipleContent = reader.SupportMultipleContent;
					reader1.ReadAndAssert();
					reader = reader1;
				}

				object newValue;
				if (this.ReadMetadataPropertiesToken(reader1, ref objectType1, ref contract, member, containerContract,
				  containerMember, existingValue, out newValue, out id))
					return newValue;
			}
			else
			{
				reader.ReadAndAssert();
				object newValue;
				if (this.ReadMetadataProperties(reader, ref objectType1, ref contract, member, containerContract,
				  containerMember, existingValue, out newValue, out id))
					return newValue;
			}

			if (this.HasNoDefinedType(contract))
				return this.CreateJObject(reader);
			switch (contract.ContractType)
			{
				case JsonContractType.Object:
					bool createdFromNonDefaultCreator1 = false;
					JsonObjectContract jsonObjectContract = (JsonObjectContract)contract;
					object newObject =
					  existingValue == null || objectType1 != objectType && !objectType1.IsAssignableFrom(existingValue.GetType())
						? this.CreateNewObject(reader, jsonObjectContract, member, containerMember, id,
						  out createdFromNonDefaultCreator1)
						: existingValue;
					if (createdFromNonDefaultCreator1)
						return newObject;
					return this.PopulateObject(newObject, reader, jsonObjectContract, member, id);
				case JsonContractType.Primitive:
					JsonPrimitiveContract primitiveContract = (JsonPrimitiveContract)contract;
					if (this.Serializer.MetadataPropertyHandling != MetadataPropertyHandling.Ignore &&
						reader.TokenType == JsonToken.PropertyName &&
						string.Equals(reader.Value.ToString(), "$value", StringComparison.Ordinal))
					{
						reader.ReadAndAssert();
						if (reader.TokenType == JsonToken.StartObject)
							throw JsonSerializationException.Create(reader,
							  "Unexpected token when deserializing primitive value: " + reader.TokenType);
						object valueInternal = this.CreateValueInternal(reader, objectType1, primitiveContract,
						  member, null, null, existingValue);
						reader.ReadAndAssert();
						return valueInternal;
					}

					break;
				case JsonContractType.Dictionary:
					JsonDictionaryContract contract1 = (JsonDictionaryContract)contract;
					object obj;
					if (existingValue == null)
					{
						bool createdFromNonDefaultCreator2;
						IDictionary newDictionary = this.CreateNewDictionary(reader, contract1, out createdFromNonDefaultCreator2);
						if (createdFromNonDefaultCreator2)
						{
							if (id != null)
								throw JsonSerializationException.Create(reader,
								  "Cannot preserve reference to readonly dictionary, or dictionary created from a non-default constructor: {0}."
									.FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
							if (contract.OnSerializingCallbacks.Count > 0)
								throw JsonSerializationException.Create(reader,
								  "Cannot call OnSerializing on readonly dictionary, or dictionary created from a non-default constructor: {0}."
									.FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
							if (contract.OnErrorCallbacks.Count > 0)
								throw JsonSerializationException.Create(reader,
								  "Cannot call OnError on readonly list, or dictionary created from a non-default constructor: {0}."
									.FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
							if (!contract1.HasParameterizedCreatorInternal)
								throw JsonSerializationException.Create(reader,
								  "Cannot deserialize readonly or fixed size dictionary: {0}.".FormatWith(
									 CultureInfo.InvariantCulture, contract.UnderlyingType));
						}

						this.PopulateDictionary(newDictionary, reader, contract1, member, id);
						if (createdFromNonDefaultCreator2)
							return (contract1.OverrideCreator ?? contract1.ParameterizedCreator)(new object[1] {
				 newDictionary
			  });
						if (newDictionary is IWrappedDictionary)
							return ((IWrappedDictionary)newDictionary).UnderlyingDictionary;
						obj = newDictionary;
					}
					else
						obj = this.PopulateDictionary(
						  contract1.ShouldCreateWrapper
							? contract1.CreateWrapper(existingValue)
							: (IDictionary)existingValue, reader, contract1, member, id);

					return obj;
				case JsonContractType.Serializable:
					JsonISerializableContract contract2 = (JsonISerializableContract)contract;
					return this.CreateISerializable(reader, contract2, member, id);
			}

			string message =
			  ("Cannot deserialize the current JSON object (e.g. {{\"name\":\"value\"}}) into type '{0}' because the type requires a {1} to deserialize correctly." +
			   Environment.NewLine +
			   "To fix this error either change the JSON to a {1} or change the deserialized type so that it is a normal .NET type (e.g. not a primitive type like integer, not a collection type like an array or List<T>) that can be deserialized from a JSON object. JsonObjectAttribute can also be added to the type to force it to deserialize from a JSON object." +
			   Environment.NewLine).FormatWith(CultureInfo.InvariantCulture, objectType1,
				 this.GetExpectedDescription(contract));
			throw JsonSerializationException.Create(reader, message);
		}

		private bool ReadMetadataPropertiesToken(
		  JTokenReader reader,
		  ref Type objectType,
		  ref JsonContract contract,
		  JsonProperty member,
		  JsonContainerContract containerContract,
		  JsonProperty containerMember,
		  object existingValue,
		  out object newValue,
		  out string id)
		{
			id = null;
			newValue = null;
			if (reader.TokenType == JsonToken.StartObject)
			{
				JObject currentToken = (JObject)reader.CurrentToken;
				JToken jtoken1 = currentToken["$ref"];
				if (jtoken1 != null)
				{
					if (jtoken1.Type != JTokenType.String && jtoken1.Type != JTokenType.Null)
						throw JsonSerializationException.Create(jtoken1, jtoken1.Path,
						  "JSON reference {0} property must have a string or null value.".FormatWith(
							 CultureInfo.InvariantCulture, "$ref"), null);
					JToken parent = jtoken1.Parent;
					JToken jtoken2 = null;
					if (parent.Next != null)
						jtoken2 = parent.Next;
					else if (parent.Previous != null)
						jtoken2 = parent.Previous;
					string reference = (string)jtoken1;
					if (reference != null)
					{
						if (jtoken2 != null)
							throw JsonSerializationException.Create(jtoken2, jtoken2.Path,
							  "Additional content found in JSON reference object. A JSON reference object should only have a {0} property."
								.FormatWith(CultureInfo.InvariantCulture, "$ref"), null);
						newValue = this.Serializer.GetReferenceResolver().ResolveReference(this, reference);
						if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
							this.TraceWriter.Trace_(TraceLevel.Info,
							  JsonPosition.FormatMessage(reader, reader.Path,
								"Resolved object reference '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture,
								   reference, newValue.GetType())), null);
						reader.Skip();
						return true;
					}
				}

				JToken jtoken3 = currentToken["$type"];
				if (jtoken3 != null)
				{
					string qualifiedTypeName = (string)jtoken3;
					JsonReader reader1 = jtoken3.CreateReader();
					reader1.ReadAndAssert();
					this.ResolveTypeName(reader1, ref objectType, ref contract, member, containerContract, containerMember,
					  qualifiedTypeName);
					if (currentToken["$value"] != null)
					{
						while (true)
						{
							reader.ReadAndAssert();
							if (reader.TokenType != JsonToken.PropertyName || !((string)reader.Value == "$value"))
							{
								reader.ReadAndAssert();
								reader.Skip();
							}
							else
								break;
						}

						return false;
					}
				}

				JToken jtoken4 = currentToken["$id"];
				if (jtoken4 != null)
					id = (string)jtoken4;
				JToken jtoken5 = currentToken["$values"];
				if (jtoken5 != null)
				{
					JsonReader reader1 = jtoken5.CreateReader();
					reader1.ReadAndAssert();
					newValue = this.CreateList(reader1, objectType, contract, member, existingValue, id);
					reader.Skip();
					return true;
				}
			}

			reader.ReadAndAssert();
			return false;
		}

		private bool ReadMetadataProperties(
		  JsonReader reader,
		  ref Type objectType,
		  ref JsonContract contract,
		  JsonProperty member,
		  JsonContainerContract containerContract,
		  JsonProperty containerMember,
		  object existingValue,
		  out object newValue,
		  out string id)
		{
			id = null;
			newValue = null;
			if (reader.TokenType == JsonToken.PropertyName)
			{
				string str = reader.Value.ToString();
				if (str.Length > 0 && str[0] == '$')
				{
					bool flag;
					do
					{
						string a = reader.Value.ToString();
						if (string.Equals(a, "$ref", StringComparison.Ordinal))
						{
							reader.ReadAndAssert();
							if (reader.TokenType != JsonToken.String && reader.TokenType != JsonToken.Null)
								throw JsonSerializationException.Create(reader,
								  "JSON reference {0} property must have a string or null value.".FormatWith(
									 CultureInfo.InvariantCulture, "$ref"));
							string reference = reader.Value?.ToString();
							reader.ReadAndAssert();
							if (reference != null)
							{
								if (reader.TokenType == JsonToken.PropertyName)
									throw JsonSerializationException.Create(reader,
									  "Additional content found in JSON reference object. A JSON reference object should only have a {0} property."
										.FormatWith(CultureInfo.InvariantCulture, "$ref"));
								newValue = this.Serializer.GetReferenceResolver().ResolveReference(this, reference);
								if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
									this.TraceWriter.Trace_(TraceLevel.Info,
									  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
										"Resolved object reference '{0}' to {1}.".FormatWith(
										   CultureInfo.InvariantCulture, reference,
										   newValue.GetType())), null);
								return true;
							}

							flag = true;
						}
						else if (string.Equals(a, "$type", StringComparison.Ordinal))
						{
							reader.ReadAndAssert();
							string qualifiedTypeName = reader.Value.ToString();
							this.ResolveTypeName(reader, ref objectType, ref contract, member, containerContract, containerMember,
							  qualifiedTypeName);
							reader.ReadAndAssert();
							flag = true;
						}
						else if (string.Equals(a, "$id", StringComparison.Ordinal))
						{
							reader.ReadAndAssert();
							id = reader.Value?.ToString();
							reader.ReadAndAssert();
							flag = true;
						}
						else
						{
							if (string.Equals(a, "$values", StringComparison.Ordinal))
							{
								reader.ReadAndAssert();
								object list = this.CreateList(reader, objectType, contract, member, existingValue, id);
								reader.ReadAndAssert();
								newValue = list;
								return true;
							}

							flag = false;
						}
					} while (flag && reader.TokenType == JsonToken.PropertyName);
				}
			}

			return false;
		}

		private void ResolveTypeName(
		  JsonReader reader,
		  ref Type objectType,
		  ref JsonContract contract,
		  JsonProperty member,
		  JsonContainerContract containerContract,
		  JsonProperty containerMember,
		  string qualifiedTypeName)
		{
			TypeNameHandling? typeNameHandling1 = member?.TypeNameHandling;
			int num;
			if (!typeNameHandling1.HasValue)
			{
				TypeNameHandling? typeNameHandling2 = containerContract?.ItemTypeNameHandling;
				if (!typeNameHandling2.HasValue)
				{
					TypeNameHandling? typeNameHandling3 = containerMember?.ItemTypeNameHandling;
					num = typeNameHandling3.HasValue
					  ? (int)typeNameHandling3.GetValueOrDefault()
					  : (int)this.Serializer._typeNameHandling;
				}
				else
					num = (int)typeNameHandling2.GetValueOrDefault();
			}
			else
				num = (int)typeNameHandling1.GetValueOrDefault();

			if (num == 0)
				return;
			TypeNameKey typeNameKey = ReflectionUtils.SplitFullyQualifiedTypeName(qualifiedTypeName);
			Type type;
			try
			{
				type = this.Serializer._serializationBinder.BindToType(typeNameKey.AssemblyName, typeNameKey.TypeName);
			}
			catch (Exception ex)
			{
				throw JsonSerializationException.Create(reader,
				  "Error resolving type specified in JSON '{0}'.".FormatWith(CultureInfo.InvariantCulture,
					 qualifiedTypeName), ex);
			}

			if (type == null)
				throw JsonSerializationException.Create(reader,
				  "Type specified in JSON '{0}' was not resolved.".FormatWith(CultureInfo.InvariantCulture,
					 qualifiedTypeName));
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
				this.TraceWriter.Trace_(TraceLevel.Verbose,
				  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
					"Resolved type '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture,
					   qualifiedTypeName, type)), null);
			if (objectType != null && !objectType.IsAssignableFrom(type))
				throw JsonSerializationException.Create(reader,
				  "Type specified in JSON '{0}' is not compatible with '{1}'.".FormatWith(
					 CultureInfo.InvariantCulture, type.AssemblyQualifiedName,
					 objectType.AssemblyQualifiedName));
			objectType = type;
			contract = this.GetContractSafe(type);
		}

		private JsonArrayContract EnsureArrayContract(
		  JsonReader reader,
		  Type objectType,
		  JsonContract contract)
		{
			if (contract == null)
				throw JsonSerializationException.Create(reader,
				  "Could not resolve type '{0}' to a JsonContract.".FormatWith(CultureInfo.InvariantCulture,
					 objectType));
			JsonArrayContract jsonArrayContract = contract as JsonArrayContract;
			if (jsonArrayContract != null)
				return jsonArrayContract;
			string message =
			  ("Cannot deserialize the current JSON array (e.g. [1,2,3]) into type '{0}' because the type requires a {1} to deserialize correctly." +
			   Environment.NewLine +
			   "To fix this error either change the JSON to a {1} or change the deserialized type to an array or a type that implements a collection interface (e.g. ICollection, IList) like List<T> that can be deserialized from a JSON array. JsonArrayAttribute can also be added to the type to force it to deserialize from a JSON array." +
			   Environment.NewLine).FormatWith(CultureInfo.InvariantCulture, objectType,
				 this.GetExpectedDescription(contract));
			throw JsonSerializationException.Create(reader, message);
		}

		private object CreateList(
		  JsonReader reader,
		  Type objectType,
		  JsonContract contract,
		  JsonProperty member,
		  object existingValue,
		  string id)
		{
			if (this.HasNoDefinedType(contract))
				return this.CreateJToken(reader, contract);
			JsonArrayContract contract1 = this.EnsureArrayContract(reader, objectType, contract);
			object obj;
			if (existingValue == null)
			{
				bool createdFromNonDefaultCreator;
				IList list = this.CreateNewList(reader, contract1, out createdFromNonDefaultCreator);
				if (createdFromNonDefaultCreator)
				{
					if (id != null)
						throw JsonSerializationException.Create(reader,
						  "Cannot preserve reference to array or readonly list, or list created from a non-default constructor: {0}."
							.FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
					if (contract.OnSerializingCallbacks.Count > 0)
						throw JsonSerializationException.Create(reader,
						  "Cannot call OnSerializing on an array or readonly list, or list created from a non-default constructor: {0}."
							.FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
					if (contract.OnErrorCallbacks.Count > 0)
						throw JsonSerializationException.Create(reader,
						  "Cannot call OnError on an array or readonly list, or list created from a non-default constructor: {0}."
							.FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
					if (!contract1.HasParameterizedCreatorInternal && !contract1.IsArray)
						throw JsonSerializationException.Create(reader,
						  "Cannot deserialize readonly or fixed size list: {0}.".FormatWith(
							 CultureInfo.InvariantCulture, contract.UnderlyingType));
				}

				if (!contract1.IsMultidimensionalArray)
					this.PopulateList(list, reader, contract1, member, id);
				else
					this.PopulateMultidimensionalArray(list, reader, contract1, member, id);
				if (createdFromNonDefaultCreator)
				{
					if (contract1.IsMultidimensionalArray)
						list = CollectionUtils.ToMultidimensionalArray(list, contract1.CollectionItemType,
						  contract.CreatedType.GetArrayRank());
					else if (contract1.IsArray)
					{
						Array instance = Array.CreateInstance(contract1.CollectionItemType, list.Count);
						list.CopyTo(instance, 0);
						list = instance;
					}
					else
						return (contract1.OverrideCreator ?? contract1.ParameterizedCreator)(new object[1] {
			   list
			});
				}
				else if (list is IWrappedCollection)
					return ((IWrappedCollection)list).UnderlyingCollection;

				obj = list;
			}
			else
			{
				if (!contract1.CanDeserialize)
					throw JsonSerializationException.Create(reader,
					  "Cannot populate list type {0}.".FormatWith(CultureInfo.InvariantCulture,
						 contract.CreatedType));
				obj = this.PopulateList(
				  contract1.ShouldCreateWrapper ? contract1.CreateWrapper(existingValue) : (IList)existingValue,
				  reader, contract1, member, id);
			}

			return obj;
		}

		private bool HasNoDefinedType(JsonContract contract)
		{
			if (contract != null && contract.UnderlyingType != typeof(object))
				return contract.ContractType == JsonContractType.Linq;
			return true;
		}

		private object EnsureType(
		  JsonReader reader,
		  object value,
		  CultureInfo culture,
		  JsonContract contract,
		  Type targetType)
		{
			if (targetType == null || ReflectionUtils.GetObjectType(value) == targetType)
				return value;
			if (value == null && contract.IsNullable)
				return null;
			try
			{
				if (!contract.IsConvertable)
					return ConvertUtils.ConvertOrCast(value, culture, contract.NonNullableUnderlyingType);
				JsonPrimitiveContract primitiveContract = (JsonPrimitiveContract)contract;
				if (contract.IsEnum)
				{
					if (value is string)
						return Enum.Parse(contract.NonNullableUnderlyingType, value.ToString(), true);
					if (ConvertUtils.IsInteger(primitiveContract.TypeCode))
						return Enum.ToObject(contract.NonNullableUnderlyingType, value);
				}

				return Convert.ChangeType(value, contract.NonNullableUnderlyingType, culture);
			}
			catch (Exception ex)
			{
				throw JsonSerializationException.Create(reader,
				  "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture,
					 MiscellaneousUtils.FormatValueForPrint(value), targetType), ex);
			}
		}

		private bool SetPropertyValue(
		  JsonProperty property,
		  JsonConverter propertyConverter,
		  JsonContainerContract containerContract,
		  JsonProperty containerProperty,
		  JsonReader reader,
		  object target)
		{
			bool useExistingValue;
			object currentValue;
			JsonContract propertyContract;
			bool gottenCurrentValue;
			if (this.CalculatePropertyDetails(property, ref propertyConverter, containerContract, containerProperty, reader,
			  target, out useExistingValue, out currentValue, out propertyContract, out gottenCurrentValue))
				return false;
			object obj;
			if (propertyConverter != null && propertyConverter.CanRead)
			{
				if (!gottenCurrentValue && target != null && property.Readable)
					currentValue = property.ValueProvider.GetValue(target);
				obj = this.DeserializeConvertable(propertyConverter, reader, property.PropertyType, currentValue);
			}
			else
				obj = this.CreateValueInternal(reader, property.PropertyType, propertyContract, property, containerContract,
				  containerProperty, useExistingValue ? currentValue : null);

			if (useExistingValue && obj == currentValue || !this.ShouldSetPropertyValue(property, obj))
				return useExistingValue;
			property.ValueProvider.SetValue(target, obj);
			if (property.SetIsSpecified != null)
			{
				if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
					this.TraceWriter.Trace_(TraceLevel.Verbose,
					  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
						"IsSpecified for property '{0}' on {1} set to true.".FormatWith(
						   CultureInfo.InvariantCulture, property.PropertyName,
						   property.DeclaringType)), null);
				property.SetIsSpecified(target, true);
			}

			return true;
		}

		private bool CalculatePropertyDetails(
		  JsonProperty property,
		  ref JsonConverter propertyConverter,
		  JsonContainerContract containerContract,
		  JsonProperty containerProperty,
		  JsonReader reader,
		  object target,
		  out bool useExistingValue,
		  out object currentValue,
		  out JsonContract propertyContract,
		  out bool gottenCurrentValue)
		{
			currentValue = null;
			useExistingValue = false;
			propertyContract = null;
			gottenCurrentValue = false;
			if (property.Ignored)
				return true;
			JsonToken tokenType = reader.TokenType;
			if (property.PropertyContract == null)
				property.PropertyContract = this.GetContractSafe(property.PropertyType);
			if (property.ObjectCreationHandling.GetValueOrDefault(this.Serializer._objectCreationHandling) !=
				ObjectCreationHandling.Replace && (tokenType == JsonToken.StartArray || tokenType == JsonToken.StartObject) &&
				property.Readable)
			{
				currentValue = property.ValueProvider.GetValue(target);
				gottenCurrentValue = true;
				if (currentValue != null)
				{
					propertyContract = this.GetContractSafe(currentValue.GetType());
					useExistingValue = !propertyContract.IsReadOnlyOrFixedSize && !propertyContract.UnderlyingType.IsValueType();
				}
			}

			if (!property.Writable && !useExistingValue ||
				property.NullValueHandling.GetValueOrDefault(this.Serializer._nullValueHandling) ==
				NullValueHandling.Ignore && tokenType == JsonToken.Null ||
				this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling),
				  DefaultValueHandling.Ignore) &&
				(!this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling),
				   DefaultValueHandling.Populate) && JsonTokenUtils.IsPrimitiveToken(tokenType) &&
				 MiscellaneousUtils.ValueEquals(reader.Value, property.GetResolvedDefaultValue())))
				return true;
			if (currentValue == null)
			{
				propertyContract = property.PropertyContract;
			}
			else
			{
				propertyContract = this.GetContractSafe(currentValue.GetType());
				if (propertyContract != property.PropertyContract)
					propertyConverter = this.GetConverter(propertyContract, property.MemberConverter, containerContract,
					  containerProperty);
			}

			return false;
		}

		private void AddReference(JsonReader reader, string id, object value)
		{
			try
			{
				if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
					this.TraceWriter.Trace_(TraceLevel.Verbose,
					  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
						"Read object reference Id '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture,
						   id, value.GetType())), null);
				this.Serializer.GetReferenceResolver().AddReference(this, id, value);
			}
			catch (Exception ex)
			{
				throw JsonSerializationException.Create(reader,
				  "Error reading object reference '{0}'.".FormatWith(CultureInfo.InvariantCulture,
					 id), ex);
			}
		}

		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool ShouldSetPropertyValue(JsonProperty property, object value)
		{
			return (property.NullValueHandling.GetValueOrDefault(this.Serializer._nullValueHandling) !=
					NullValueHandling.Ignore || value != null) &&
				   (!this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling),
					  DefaultValueHandling.Ignore) ||
					(this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling),
					   DefaultValueHandling.Populate) ||
					 !MiscellaneousUtils.ValueEquals(value, property.GetResolvedDefaultValue()))) && property.Writable;
		}

		private IList CreateNewList(
		  JsonReader reader,
		  JsonArrayContract contract,
		  out bool createdFromNonDefaultCreator)
		{
			if (!contract.CanDeserialize)
				throw JsonSerializationException.Create(reader,
				  "Cannot create and populate list type {0}.".FormatWith(CultureInfo.InvariantCulture,
					 contract.CreatedType));
			if (contract.OverrideCreator != null)
			{
				if (contract.HasParameterizedCreator)
				{
					createdFromNonDefaultCreator = true;
					return contract.CreateTemporaryCollection();
				}

				object list = contract.OverrideCreator(new object[0]);
				if (contract.ShouldCreateWrapper)
					list = contract.CreateWrapper(list);
				createdFromNonDefaultCreator = false;
				return (IList)list;
			}

			if (contract.IsReadOnlyOrFixedSize)
			{
				createdFromNonDefaultCreator = true;
				IList list = contract.CreateTemporaryCollection();
				if (contract.ShouldCreateWrapper)
					list = contract.CreateWrapper(list);
				return list;
			}

			if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic ||
													this.Serializer._constructorHandling ==
													ConstructorHandling.AllowNonPublicDefaultConstructor))
			{
				object list = contract.DefaultCreator();
				if (contract.ShouldCreateWrapper)
					list = contract.CreateWrapper(list);
				createdFromNonDefaultCreator = false;
				return (IList)list;
			}

			if (contract.HasParameterizedCreatorInternal)
			{
				createdFromNonDefaultCreator = true;
				return contract.CreateTemporaryCollection();
			}

			if (!contract.IsInstantiable)
				throw JsonSerializationException.Create(reader,
				  "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated."
					.FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			throw JsonSerializationException.Create(reader,
			  "Unable to find a constructor to use for type {0}.".FormatWith(CultureInfo.InvariantCulture,
				 contract.UnderlyingType));
		}

		private IDictionary CreateNewDictionary(
		  JsonReader reader,
		  JsonDictionaryContract contract,
		  out bool createdFromNonDefaultCreator)
		{
			if (contract.OverrideCreator != null)
			{
				if (contract.HasParameterizedCreator)
				{
					createdFromNonDefaultCreator = true;
					return contract.CreateTemporaryDictionary();
				}

				createdFromNonDefaultCreator = false;
				return (IDictionary)contract.OverrideCreator(new object[0]);
			}

			if (contract.IsReadOnlyOrFixedSize)
			{
				createdFromNonDefaultCreator = true;
				return contract.CreateTemporaryDictionary();
			}

			if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic ||
													this.Serializer._constructorHandling ==
													ConstructorHandling.AllowNonPublicDefaultConstructor))
			{
				object dictionary = contract.DefaultCreator();
				if (contract.ShouldCreateWrapper)
					dictionary = contract.CreateWrapper(dictionary);
				createdFromNonDefaultCreator = false;
				return (IDictionary)dictionary;
			}

			if (contract.HasParameterizedCreatorInternal)
			{
				createdFromNonDefaultCreator = true;
				return contract.CreateTemporaryDictionary();
			}

			if (!contract.IsInstantiable)
				throw JsonSerializationException.Create(reader,
				  "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated."
					.FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			throw JsonSerializationException.Create(reader,
			  "Unable to find a default constructor to use for type {0}.".FormatWith(
				 CultureInfo.InvariantCulture, contract.UnderlyingType));
		}

		private void OnDeserializing(JsonReader reader, JsonContract contract, object value)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
				this.TraceWriter.Trace_(TraceLevel.Info,
				  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
					"Started deserializing {0}".FormatWith(CultureInfo.InvariantCulture,
					   contract.UnderlyingType)), null);
			contract.InvokeOnDeserializing(value, this.Serializer._context);
		}

		private void OnDeserialized(JsonReader reader, JsonContract contract, object value)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
				this.TraceWriter.Trace_(TraceLevel.Info,
				  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
					"Finished deserializing {0}".FormatWith(CultureInfo.InvariantCulture,
					   contract.UnderlyingType)), null);
			contract.InvokeOnDeserialized(value, this.Serializer._context);
		}

		private object PopulateDictionary(
		  IDictionary dictionary,
		  JsonReader reader,
		  JsonDictionaryContract contract,
		  JsonProperty containerProperty,
		  string id)
		{
			IWrappedDictionary wrappedDictionary = dictionary as IWrappedDictionary;
			object currentObject = wrappedDictionary != null ? wrappedDictionary.UnderlyingDictionary : dictionary;
			if (id != null)
				this.AddReference(reader, id, currentObject);
			this.OnDeserializing(reader, contract, currentObject);
			int depth = reader.Depth;
			if (contract.KeyContract == null)
				contract.KeyContract = this.GetContractSafe(contract.DictionaryKeyType);
			if (contract.ItemContract == null)
				contract.ItemContract = this.GetContractSafe(contract.DictionaryValueType);
			JsonConverter converter = contract.ItemConverter ?? this.GetConverter(contract.ItemContract, null,
										 contract, containerProperty);
			JsonPrimitiveContract keyContract = contract.KeyContract as JsonPrimitiveContract;
			PrimitiveTypeCode primitiveTypeCode = keyContract != null ? keyContract.TypeCode : PrimitiveTypeCode.Empty;
			bool flag = false;
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.PropertyName:
						object keyValue = reader.Value;
						if (!this.CheckPropertyName(reader, keyValue.ToString()))
						{
							try
							{
								try
								{
									DateTime dt;
									keyValue =
									  primitiveTypeCode == PrimitiveTypeCode.DateTime ||
									  primitiveTypeCode == PrimitiveTypeCode.DateTimeNullable
										? (!DateTimeUtils.TryParseDateTime(keyValue.ToString(), reader.DateTimeZoneHandling,
										  reader.DateFormatString, reader.Culture, out dt)
										  ? this.EnsureType(reader, keyValue, CultureInfo.InvariantCulture, contract.KeyContract,
											contract.DictionaryKeyType)
										  : dt)
										: this.EnsureType(reader, keyValue, CultureInfo.InvariantCulture, contract.KeyContract,
										  contract.DictionaryKeyType);
								}
								catch (Exception ex)
								{
									throw JsonSerializationException.Create(reader,
									  "Could not convert string '{0}' to dictionary key type '{1}'. Create a TypeConverter to convert from the string to the key type object."
										.FormatWith(CultureInfo.InvariantCulture, reader.Value,
										   contract.DictionaryKeyType), ex);
								}

								if (!this.ReadForType(reader, contract.ItemContract, converter != null))
									throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
								object obj = converter == null || !converter.CanRead
								  ? this.CreateValueInternal(reader, contract.DictionaryValueType, contract.ItemContract,
									 null, contract, containerProperty, null)
								  : this.DeserializeConvertable(converter, reader, contract.DictionaryValueType, null);
								dictionary[keyValue] = obj;
								goto case JsonToken.Comment;
							}
							catch (Exception ex)
							{
								if (this.IsErrorHandled(currentObject, contract, keyValue, reader as IJsonLineInfo,
								  reader.Path, ex))
								{
									this.HandleError(reader, true, depth);
									goto case JsonToken.Comment;
								}
								else
									throw;
							}
						}
						else
							goto case JsonToken.Comment;
					case JsonToken.Comment:
						continue;
					case JsonToken.EndObject:
						flag = true;
						goto case JsonToken.Comment;
					default:
						throw JsonSerializationException.Create(reader,
						  "Unexpected token when deserializing object: " + reader.TokenType);
				}
			} while (!flag && reader.Read());

			if (!flag)
				this.ThrowUnexpectedEndException(reader, contract, currentObject,
				  "Unexpected end when deserializing object.");
			this.OnDeserialized(reader, contract, currentObject);
			return currentObject;
		}

		private object PopulateMultidimensionalArray(
		  IList list,
		  JsonReader reader,
		  JsonArrayContract contract,
		  JsonProperty containerProperty,
		  string id)
		{
			int arrayRank = contract.UnderlyingType.GetArrayRank();
			if (id != null)
				this.AddReference(reader, id, list);
			this.OnDeserializing(reader, contract, list);
			JsonContract contractSafe = this.GetContractSafe(contract.CollectionItemType);
			JsonConverter converter = this.GetConverter(contractSafe, null, contract,
			  containerProperty);
			int? nullable1 = new int?();
			Stack<IList> listStack = new Stack<IList>();
			listStack.Push(list);
			IList list1 = list;
			bool flag = false;
			do
			{
				int depth = reader.Depth;
				if (listStack.Count == arrayRank)
				{
					try
					{
						if (this.ReadForType(reader, contractSafe, converter != null))
						{
							if (reader.TokenType == JsonToken.EndArray)
							{
								listStack.Pop();
								list1 = listStack.Peek();
								nullable1 = new int?();
							}
							else
							{
								object obj = converter == null || !converter.CanRead
								  ? this.CreateValueInternal(reader, contract.CollectionItemType, contractSafe, null,
									 contract, containerProperty, null)
								  : this.DeserializeConvertable(converter, reader, contract.CollectionItemType, null);
								list1.Add(obj);
							}
						}
						else
							break;
					}
					catch (Exception ex)
					{
						JsonPosition position1 = reader.GetPosition(depth);
						if (this.IsErrorHandled(list, contract, position1.Position,
						  reader as IJsonLineInfo, reader.Path, ex))
						{
							this.HandleError(reader, true, depth);
							if (nullable1.HasValue)
							{
								int? nullable2 = nullable1;
								int position2 = position1.Position;
								if ((nullable2.GetValueOrDefault() == position2 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
									throw JsonSerializationException.Create(reader, "Infinite loop detected from error handling.", ex);
							}

							nullable1 = new int?(position1.Position);
						}
						else
							throw;
					}
				}
				else if (reader.Read())
				{
					switch (reader.TokenType)
					{
						case JsonToken.StartArray:
							IList list2 = new List<object>();
							list1.Add(list2);
							listStack.Push(list2);
							list1 = list2;
							break;
						case JsonToken.Comment:
							break;
						case JsonToken.EndArray:
							listStack.Pop();
							if (listStack.Count > 0)
							{
								list1 = listStack.Peek();
								break;
							}

							flag = true;
							break;
						default:
							throw JsonSerializationException.Create(reader,
							  "Unexpected token when deserializing multidimensional array: " + reader.TokenType);
					}
				}
				else
					break;
			} while (!flag);

			if (!flag)
				this.ThrowUnexpectedEndException(reader, contract, list,
				  "Unexpected end when deserializing array.");
			this.OnDeserialized(reader, contract, list);
			return list;
		}

		private void ThrowUnexpectedEndException(
		  JsonReader reader,
		  JsonContract contract,
		  object currentObject,
		  string message)
		{
			try
			{
				throw JsonSerializationException.Create(reader, message);
			}
			catch (Exception ex)
			{
				if (this.IsErrorHandled(currentObject, contract, null, reader as IJsonLineInfo, reader.Path, ex))
					this.HandleError(reader, false, 0);
				else
					throw;
			}
		}

		private object PopulateList(
		  IList list,
		  JsonReader reader,
		  JsonArrayContract contract,
		  JsonProperty containerProperty,
		  string id)
		{
			IWrappedCollection wrappedCollection = list as IWrappedCollection;
			object currentObject = wrappedCollection != null ? wrappedCollection.UnderlyingCollection : list;
			if (id != null)
				this.AddReference(reader, id, currentObject);
			if (list.IsFixedSize)
			{
				reader.Skip();
				return currentObject;
			}

			this.OnDeserializing(reader, contract, currentObject);
			int depth = reader.Depth;
			if (contract.ItemContract == null)
				contract.ItemContract = this.GetContractSafe(contract.CollectionItemType);
			JsonConverter converter = this.GetConverter(contract.ItemContract, null,
			   contract, containerProperty);
			int? nullable1 = new int?();
			bool flag = false;
			do
			{
				try
				{
					if (this.ReadForType(reader, contract.ItemContract, converter != null))
					{
						if (reader.TokenType == JsonToken.EndArray)
						{
							flag = true;
						}
						else
						{
							object obj = converter == null || !converter.CanRead
							  ? this.CreateValueInternal(reader, contract.CollectionItemType, contract.ItemContract,
								 null, contract, containerProperty, null)
							  : this.DeserializeConvertable(converter, reader, contract.CollectionItemType, null);
							list.Add(obj);
						}
					}
					else
						break;
				}
				catch (Exception ex)
				{
					JsonPosition position1 = reader.GetPosition(depth);
					if (this.IsErrorHandled(currentObject, contract, position1.Position,
					  reader as IJsonLineInfo, reader.Path, ex))
					{
						this.HandleError(reader, true, depth);
						if (nullable1.HasValue)
						{
							int? nullable2 = nullable1;
							int position2 = position1.Position;
							if ((nullable2.GetValueOrDefault() == position2 ? (nullable2.HasValue ? 1 : 0) : 0) != 0)
								throw JsonSerializationException.Create(reader, "Infinite loop detected from error handling.", ex);
						}

						nullable1 = new int?(position1.Position);
					}
					else
						throw;
				}
			} while (!flag);

			if (!flag)
				this.ThrowUnexpectedEndException(reader, contract, currentObject,
				  "Unexpected end when deserializing array.");
			this.OnDeserialized(reader, contract, currentObject);
			return currentObject;
		}

		private object CreateISerializable(
		  JsonReader reader,
		  JsonISerializableContract contract,
		  JsonProperty member,
		  string id)
		{
			Type underlyingType = contract.UnderlyingType;
			if (!JsonTypeReflector.FullyTrusted)
			{
				string message =
				  ("Type '{0}' implements ISerializable but cannot be deserialized using the ISerializable interface because the current application is not fully trusted and ISerializable can expose secure data." +
				   Environment.NewLine +
				   "To fix this error either change the environment to be fully trusted, change the application to not deserialize the type, add JsonObjectAttribute to the type or change the JsonSerializer setting ContractResolver to use a new DefaultContractResolver with IgnoreSerializableInterface set to true." +
				   Environment.NewLine).FormatWith(CultureInfo.InvariantCulture, underlyingType);
				throw JsonSerializationException.Create(reader, message);
			}

			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
				this.TraceWriter.Trace_(TraceLevel.Info,
				  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
					"Deserializing {0} using ISerializable constructor.".FormatWith(
					   CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
			SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType,
			   new JsonFormatterConverter(this, contract, member));
			bool flag = false;
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.PropertyName:
						string name = reader.Value.ToString();
						if (!reader.Read())
							throw JsonSerializationException.Create(reader,
							  "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture,
								 name));
						serializationInfo.AddValue(name, JToken.ReadFrom(reader));
						goto case JsonToken.Comment;
					case JsonToken.Comment:
						continue;
					case JsonToken.EndObject:
						flag = true;
						goto case JsonToken.Comment;
					default:
						throw JsonSerializationException.Create(reader,
						  "Unexpected token when deserializing object: " + reader.TokenType);
				}
			} while (!flag && reader.Read());

			if (!flag)
				this.ThrowUnexpectedEndException(reader, contract, serializationInfo,
				  "Unexpected end when deserializing object.");
			if (contract.ISerializableCreator == null)
				throw JsonSerializationException.Create(reader,
				  "ISerializable type '{0}' does not have a valid constructor. To correctly implement ISerializable a constructor that takes SerializationInfo and StreamingContext parameters should be present."
					.FormatWith(CultureInfo.InvariantCulture, underlyingType));
			object obj = contract.ISerializableCreator(new object[2] {
		 serializationInfo,
		 Serializer._context
	  });
			if (id != null)
				this.AddReference(reader, id, obj);
			this.OnDeserializing(reader, contract, obj);
			this.OnDeserialized(reader, contract, obj);
			return obj;
		}

		internal object CreateISerializableItem(
		  JToken token,
		  Type type,
		  JsonISerializableContract contract,
		  JsonProperty member)
		{
			JsonContract contractSafe = this.GetContractSafe(type);
			JsonConverter converter =
			  this.GetConverter(contractSafe, null, contract, member);
			JsonReader reader = token.CreateReader();
			reader.ReadAndAssert();
			return converter == null || !converter.CanRead
			  ? this.CreateValueInternal(reader, type, contractSafe, null, contract,
				member, null)
			  : this.DeserializeConvertable(converter, reader, type, null);
		}

		private object CreateObjectUsingCreatorWithParameters(
		  JsonReader reader,
		  JsonObjectContract contract,
		  JsonProperty containerProperty,
		  ObjectConstructor<object> creator,
		  string id)
		{
			ValidationUtils.ArgumentNotNull(creator, nameof(creator));
			bool flag = contract.HasRequiredOrDefaultValueProperties ||
						this.HasFlag(this.Serializer._defaultValueHandling, DefaultValueHandling.Populate);
			Type underlyingType = contract.UnderlyingType;
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				string str = string.Join(", ",
				  contract.CreatorParameters.Select<JsonProperty, string>(p => p.PropertyName)
					.ToArray<string>());
				this.TraceWriter.Trace_(TraceLevel.Info,
				  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
					"Deserializing {0} using creator with parameters: {1}.".FormatWith(
					   CultureInfo.InvariantCulture, contract.UnderlyingType, str)),
				   null);
			}

			List<JsonSerializerInternalReader.CreatorPropertyContext> source =
			  this.ResolvePropertyAndCreatorValues(contract, containerProperty, reader, underlyingType);
			if (flag)
			{
				foreach (JsonProperty property1 in (Collection<JsonProperty>)contract.Properties)
				{
					JsonProperty property = property1;
					if (source.All<JsonSerializerInternalReader.CreatorPropertyContext>(
					   p => p.Property != property))
						source.Add(new JsonSerializerInternalReader.CreatorPropertyContext()
						{
							Property = property,
							Name = property.PropertyName,
							Presence = new JsonSerializerInternalReader.PropertyPresence?(JsonSerializerInternalReader
							.PropertyPresence.None)
						});
				}
			}

			object[] objArray = new object[contract.CreatorParameters.Count];
			foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext in source)
			{
				if (flag && creatorPropertyContext.Property != null && !creatorPropertyContext.Presence.HasValue)
				{
					object obj = creatorPropertyContext.Value;
					JsonSerializerInternalReader.PropertyPresence propertyPresence = obj != null
					  ? (!(obj is string)
						? JsonSerializerInternalReader.PropertyPresence.Value
						: (JsonSerializerInternalReader.CoerceEmptyStringToNull(creatorPropertyContext.Property.PropertyType,
						  creatorPropertyContext.Property.PropertyContract, (string)obj)
						  ? JsonSerializerInternalReader.PropertyPresence.Null
						  : JsonSerializerInternalReader.PropertyPresence.Value))
					  : JsonSerializerInternalReader.PropertyPresence.Null;
					creatorPropertyContext.Presence = new JsonSerializerInternalReader.PropertyPresence?(propertyPresence);
				}

				JsonProperty constructorProperty = creatorPropertyContext.ConstructorProperty;
				if (constructorProperty == null && creatorPropertyContext.Property != null)
					constructorProperty = contract.CreatorParameters.ForgivingCaseSensitiveFind<JsonProperty>(
					   p => p.PropertyName, creatorPropertyContext.Property.UnderlyingName);
				if (constructorProperty != null && !constructorProperty.Ignored)
				{
					if (flag)
					{
						JsonSerializerInternalReader.PropertyPresence? presence = creatorPropertyContext.Presence;
						JsonSerializerInternalReader.PropertyPresence propertyPresence1 =
						  JsonSerializerInternalReader.PropertyPresence.None;
						if ((presence.GetValueOrDefault() == propertyPresence1 ? (presence.HasValue ? 1 : 0) : 0) == 0)
						{
							presence = creatorPropertyContext.Presence;
							JsonSerializerInternalReader.PropertyPresence propertyPresence2 =
							  JsonSerializerInternalReader.PropertyPresence.Null;
							if ((presence.GetValueOrDefault() == propertyPresence2 ? (presence.HasValue ? 1 : 0) : 0) == 0)
								goto label_25;
						}

						if (constructorProperty.PropertyContract == null)
							constructorProperty.PropertyContract = this.GetContractSafe(constructorProperty.PropertyType);
						if (this.HasFlag(
						  constructorProperty.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling),
						  DefaultValueHandling.Populate))
							creatorPropertyContext.Value = this.EnsureType(reader, constructorProperty.GetResolvedDefaultValue(),
							  CultureInfo.InvariantCulture, constructorProperty.PropertyContract, constructorProperty.PropertyType);
					}

					label_25:
					int index = contract.CreatorParameters.IndexOf(constructorProperty);
					objArray[index] = creatorPropertyContext.Value;
					creatorPropertyContext.Used = true;
				}
			}

			object obj1 = creator(objArray);
			if (id != null)
				this.AddReference(reader, id, obj1);
			this.OnDeserializing(reader, contract, obj1);
			foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext in source)
			{
				if (!creatorPropertyContext.Used && creatorPropertyContext.Property != null &&
					!creatorPropertyContext.Property.Ignored)
				{
					JsonSerializerInternalReader.PropertyPresence? presence = creatorPropertyContext.Presence;
					JsonSerializerInternalReader.PropertyPresence propertyPresence =
					  JsonSerializerInternalReader.PropertyPresence.None;
					if ((presence.GetValueOrDefault() == propertyPresence ? (presence.HasValue ? 1 : 0) : 0) == 0)
					{
						JsonProperty property = creatorPropertyContext.Property;
						object obj2 = creatorPropertyContext.Value;
						if (this.ShouldSetPropertyValue(property, obj2))
						{
							property.ValueProvider.SetValue(obj1, obj2);
							creatorPropertyContext.Used = true;
						}
						else if (!property.Writable && obj2 != null)
						{
							JsonContract jsonContract = this.Serializer._contractResolver.ResolveContract(property.PropertyType);
							if (jsonContract.ContractType == JsonContractType.Array)
							{
								JsonArrayContract jsonArrayContract = (JsonArrayContract)jsonContract;
								if (jsonArrayContract.CanDeserialize)
								{
									object list1 = property.ValueProvider.GetValue(obj1);
									if (list1 != null)
									{
										IList list2 = jsonArrayContract.ShouldCreateWrapper
										  ? jsonArrayContract.CreateWrapper(list1)
										  : (IList)list1;
										foreach (object obj3 in jsonArrayContract.ShouldCreateWrapper
										  ? jsonArrayContract.CreateWrapper(obj2)
										  : (IEnumerable)obj2)
											list2.Add(obj3);
									}
								}
							}
							else if (jsonContract.ContractType == JsonContractType.Dictionary)
							{
								JsonDictionaryContract dictionaryContract = (JsonDictionaryContract)jsonContract;
								if (!dictionaryContract.IsReadOnlyOrFixedSize)
								{
									object dictionary1 = property.ValueProvider.GetValue(obj1);
									if (dictionary1 != null)
									{
										IDictionary dictionary2 = dictionaryContract.ShouldCreateWrapper
										  ? dictionaryContract.CreateWrapper(dictionary1)
										  : (IDictionary)dictionary1;
										IDictionaryEnumerator enumerator =
										  (dictionaryContract.ShouldCreateWrapper
											? dictionaryContract.CreateWrapper(obj2)
											: (IDictionary)obj2).GetEnumerator();
										try
										{
											while (enumerator.MoveNext())
											{
												DictionaryEntry entry = enumerator.Entry;
												dictionary2[entry.Key] = entry.Value;
											}
										}
										finally
										{
											(enumerator as IDisposable)?.Dispose();
										}
									}
								}
							}

							creatorPropertyContext.Used = true;
						}
					}
				}
			}

			if (contract.ExtensionDataSetter != null)
			{
				foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext in source)
				{
					if (!creatorPropertyContext.Used)
						contract.ExtensionDataSetter(obj1, creatorPropertyContext.Name, creatorPropertyContext.Value);
				}
			}

			if (flag)
			{
				foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext in source)
				{
					if (creatorPropertyContext.Property != null)
						this.EndProcessProperty(obj1, reader, contract, reader.Depth, creatorPropertyContext.Property,
						  creatorPropertyContext.Presence.GetValueOrDefault(), !creatorPropertyContext.Used);
				}
			}

			this.OnDeserialized(reader, contract, obj1);
			return obj1;
		}

		private object DeserializeConvertable(
		  JsonConverter converter,
		  JsonReader reader,
		  Type objectType,
		  object existingValue)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
				this.TraceWriter.Trace_(TraceLevel.Info,
				  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
					"Started deserializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture,
					   objectType, converter.GetType())), null);
			object obj = converter.ReadJson(reader, objectType, existingValue, this.GetInternalSerializer());
			if (this.TraceWriter == null || this.TraceWriter.LevelFilter < TraceLevel.Info)
				return obj;
			this.TraceWriter.Trace_(TraceLevel.Info,
			  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
				"Finished deserializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture,
				   objectType, converter.GetType())), null);
			return obj;
		}

		private List<JsonSerializerInternalReader.CreatorPropertyContext> ResolvePropertyAndCreatorValues(
		  JsonObjectContract contract,
		  JsonProperty containerProperty,
		  JsonReader reader,
		  Type objectType)
		{
			List<JsonSerializerInternalReader.CreatorPropertyContext> creatorPropertyContextList =
			  new List<JsonSerializerInternalReader.CreatorPropertyContext>();
			bool flag = false;
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.PropertyName:
						string propertyName = reader.Value.ToString();
						JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext =
						  new JsonSerializerInternalReader.CreatorPropertyContext()
						  {
							  Name = reader.Value.ToString(),
							  ConstructorProperty = contract.CreatorParameters.GetClosestMatchProperty(propertyName),
							  Property = contract.Properties.GetClosestMatchProperty(propertyName)
						  };
						creatorPropertyContextList.Add(creatorPropertyContext);
						JsonProperty member = creatorPropertyContext.ConstructorProperty ?? creatorPropertyContext.Property;
						if (member != null && !member.Ignored)
						{
							if (member.PropertyContract == null)
								member.PropertyContract = this.GetContractSafe(member.PropertyType);
							JsonConverter converter = this.GetConverter(member.PropertyContract, member.MemberConverter,
							   contract, containerProperty);
							if (!this.ReadForType(reader, member.PropertyContract, converter != null))
								throw JsonSerializationException.Create(reader,
								  "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture,
									 propertyName));
							creatorPropertyContext.Value = converter == null || !converter.CanRead
							  ? this.CreateValueInternal(reader, member.PropertyType, member.PropertyContract, member,
								 contract, containerProperty, null)
							  : this.DeserializeConvertable(converter, reader, member.PropertyType, null);
							goto case JsonToken.Comment;
						}
						else
						{
							if (!reader.Read())
								throw JsonSerializationException.Create(reader,
								  "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture,
									 propertyName));
							if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
								this.TraceWriter.Trace_(TraceLevel.Verbose,
								  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
									"Could not find member '{0}' on {1}.".FormatWith(CultureInfo.InvariantCulture,
									   propertyName, contract.UnderlyingType)), null);
							if (this.Serializer._missingMemberHandling == MissingMemberHandling.Error)
								throw JsonSerializationException.Create(reader,
								  "Could not find member '{0}' on object of type '{1}'".FormatWith(
									 CultureInfo.InvariantCulture, propertyName, objectType.Name));
							if (contract.ExtensionDataSetter != null)
							{
								creatorPropertyContext.Value = this.ReadExtensionDataValue(contract, containerProperty, reader);
								goto case JsonToken.Comment;
							}
							else
							{
								reader.Skip();
								goto case JsonToken.Comment;
							}
						}
					case JsonToken.Comment:
						continue;
					case JsonToken.EndObject:
						flag = true;
						goto case JsonToken.Comment;
					default:
						throw JsonSerializationException.Create(reader,
						  "Unexpected token when deserializing object: " + reader.TokenType);
				}
			} while (!flag && reader.Read());

			if (!flag)
				this.ThrowUnexpectedEndException(reader, contract, null,
				  "Unexpected end when deserializing object.");
			return creatorPropertyContextList;
		}

		private bool ReadForType(JsonReader reader, JsonContract contract, bool hasConverter)
		{
			if (hasConverter)
				return reader.Read();
			switch (contract != null ? (int)contract.InternalReadType : 0)
			{
				case 0:
					return reader.ReadAndMoveToContent();
				case 1:
					reader.ReadAsInt32();
					break;
				case 2:
					reader.ReadAsBytes();
					break;
				case 3:
					reader.ReadAsString();
					break;
				case 4:
					reader.ReadAsDecimal();
					break;
				case 5:
					reader.ReadAsDateTime();
					break;
				case 6:
					reader.ReadAsDouble();
					break;
				case 7:
					reader.ReadAsBoolean();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return (uint)reader.TokenType > 0U;
		}

		public object CreateNewObject(
		  JsonReader reader,
		  JsonObjectContract objectContract,
		  JsonProperty containerMember,
		  JsonProperty containerProperty,
		  string id,
		  out bool createdFromNonDefaultCreator)
		{
			object obj = null;
			if (objectContract.OverrideCreator != null)
			{
				if (objectContract.CreatorParameters.Count > 0)
				{
					createdFromNonDefaultCreator = true;
					return this.CreateObjectUsingCreatorWithParameters(reader, objectContract, containerMember,
					  objectContract.OverrideCreator, id);
				}

				obj = objectContract.OverrideCreator(CollectionUtils.ArrayEmpty<object>());
			}
			else if (objectContract.DefaultCreator != null && (!objectContract.DefaultCreatorNonPublic ||
																 this.Serializer._constructorHandling == ConstructorHandling
																   .AllowNonPublicDefaultConstructor ||
																 objectContract.ParameterizedCreator == null))
				obj = objectContract.DefaultCreator();
			else if (objectContract.ParameterizedCreator != null)
			{
				createdFromNonDefaultCreator = true;
				return this.CreateObjectUsingCreatorWithParameters(reader, objectContract, containerMember,
				  objectContract.ParameterizedCreator, id);
			}

			if (obj == null)
			{
				if (!objectContract.IsInstantiable)
					throw JsonSerializationException.Create(reader,
					  "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated."
						.FormatWith(CultureInfo.InvariantCulture, objectContract.UnderlyingType));
				throw JsonSerializationException.Create(reader,
				  "Unable to find a constructor to use for type {0}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute."
					.FormatWith(CultureInfo.InvariantCulture, objectContract.UnderlyingType));
			}

			createdFromNonDefaultCreator = false;
			return obj;
		}

		private object PopulateObject(
		  object newObject,
		  JsonReader reader,
		  JsonObjectContract contract,
		  JsonProperty member,
		  string id)
		{
			this.OnDeserializing(reader, contract, newObject);
			Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> requiredProperties =
			  contract.HasRequiredOrDefaultValueProperties ||
			  this.HasFlag(this.Serializer._defaultValueHandling, DefaultValueHandling.Populate)
				? contract.Properties.ToDictionary<JsonProperty, JsonProperty, JsonSerializerInternalReader.PropertyPresence>(
				   m => m,
				   m =>
					JsonSerializerInternalReader.PropertyPresence.None)
				: null;
			if (id != null)
				this.AddReference(reader, id, newObject);
			int depth = reader.Depth;
			bool flag = false;
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.PropertyName:
						string str = reader.Value.ToString();
						if (!this.CheckPropertyName(reader, str))
						{
							try
							{
								JsonProperty closestMatchProperty = contract.Properties.GetClosestMatchProperty(str);
								if (closestMatchProperty == null)
								{
									if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
										this.TraceWriter.Trace_(TraceLevel.Verbose,
										  JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path,
											"Could not find member '{0}' on {1}".FormatWith(CultureInfo.InvariantCulture,
											   str, contract.UnderlyingType)), null);
									if (this.Serializer._missingMemberHandling == MissingMemberHandling.Error)
										throw JsonSerializationException.Create(reader,
										  "Could not find member '{0}' on object of type '{1}'".FormatWith(
											 CultureInfo.InvariantCulture, str,
											 contract.UnderlyingType.Name));
									if (reader.Read())
									{
										this.SetExtensionData(contract, member, reader, str, newObject);
										goto case JsonToken.Comment;
									}
									else
										goto case JsonToken.Comment;
								}
								else if (closestMatchProperty.Ignored ||
										   !this.ShouldDeserialize(reader, closestMatchProperty, newObject))
								{
									if (reader.Read())
									{
										this.SetPropertyPresence(reader, closestMatchProperty, requiredProperties);
										this.SetExtensionData(contract, member, reader, str, newObject);
										goto case JsonToken.Comment;
									}
									else
										goto case JsonToken.Comment;
								}
								else
								{
									if (closestMatchProperty.PropertyContract == null)
										closestMatchProperty.PropertyContract = this.GetContractSafe(closestMatchProperty.PropertyType);
									JsonConverter converter = this.GetConverter(closestMatchProperty.PropertyContract,
									  closestMatchProperty.MemberConverter, contract, member);
									if (!this.ReadForType(reader, closestMatchProperty.PropertyContract, converter != null))
										throw JsonSerializationException.Create(reader,
										  "Unexpected end when setting {0}'s value.".FormatWith(
											 CultureInfo.InvariantCulture, str));
									this.SetPropertyPresence(reader, closestMatchProperty, requiredProperties);
									if (!this.SetPropertyValue(closestMatchProperty, converter, contract, member,
									  reader, newObject))
									{
										this.SetExtensionData(contract, member, reader, str, newObject);
										goto case JsonToken.Comment;
									}
									else
										goto case JsonToken.Comment;
								}
							}
							catch (Exception ex)
							{
								if (this.IsErrorHandled(newObject, contract, str, reader as IJsonLineInfo,
								  reader.Path, ex))
								{
									this.HandleError(reader, true, depth - 1);
									goto case JsonToken.Comment;
								}
								else
									throw;
							}
						}
						else
							goto case JsonToken.Comment;
					case JsonToken.Comment:
						continue;
					case JsonToken.EndObject:
						flag = true;
						goto case JsonToken.Comment;
					default:
						throw JsonSerializationException.Create(reader,
						  "Unexpected token when deserializing object: " + reader.TokenType);
				}
			} while (!flag && reader.Read());

			if (!flag)
				this.ThrowUnexpectedEndException(reader, contract, newObject,
				  "Unexpected end when deserializing object.");
			if (requiredProperties != null)
			{
				foreach (KeyValuePair<JsonProperty, JsonSerializerInternalReader.PropertyPresence> keyValuePair in
				  requiredProperties)
				{
					JsonProperty key = keyValuePair.Key;
					JsonSerializerInternalReader.PropertyPresence presence = keyValuePair.Value;
					this.EndProcessProperty(newObject, reader, contract, depth, key, presence, true);
				}
			}

			this.OnDeserialized(reader, contract, newObject);
			return newObject;
		}

		private bool ShouldDeserialize(JsonReader reader, JsonProperty property, object target)
		{
			if (property.ShouldDeserialize == null)
				return true;
			bool flag = property.ShouldDeserialize(target);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
				this.TraceWriter.Trace_(TraceLevel.Verbose,
				  JsonPosition.FormatMessage(null, reader.Path,
					"ShouldDeserialize result for property '{0}' on {1}: {2}".FormatWith(
					   CultureInfo.InvariantCulture, property.PropertyName,
					   property.DeclaringType, flag)), null);
			return flag;
		}

		private bool CheckPropertyName(JsonReader reader, string memberName)
		{
			if (this.Serializer.MetadataPropertyHandling != MetadataPropertyHandling.ReadAhead || !(memberName == "$id") &&
				!(memberName == "$ref") && (!(memberName == "$type") && !(memberName == "$values")))
				return false;
			reader.Skip();
			return true;
		}

		private void SetExtensionData(
		  JsonObjectContract contract,
		  JsonProperty member,
		  JsonReader reader,
		  string memberName,
		  object o)
		{
			if (contract.ExtensionDataSetter != null)
			{
				try
				{
					object obj = this.ReadExtensionDataValue(contract, member, reader);
					contract.ExtensionDataSetter(o, memberName, obj);
				}
				catch (Exception ex)
				{
					throw JsonSerializationException.Create(reader,
					  "Error setting value in extension data for type '{0}'.".FormatWith(
						 CultureInfo.InvariantCulture, contract.UnderlyingType), ex);
				}
			}
			else
				reader.Skip();
		}

		private object ReadExtensionDataValue(
		  JsonObjectContract contract,
		  JsonProperty member,
		  JsonReader reader)
		{
			return !contract.ExtensionDataIsJToken
			  ? this.CreateValueInternal(reader, null, null, null,
				 contract, member, null)
			  : JToken.ReadFrom(reader);
		}

		private void EndProcessProperty(
		  object newObject,
		  JsonReader reader,
		  JsonObjectContract contract,
		  int initialDepth,
		  JsonProperty property,
		  JsonSerializerInternalReader.PropertyPresence presence,
		  bool setDefaultValue)
		{
			if (presence != JsonSerializerInternalReader.PropertyPresence.None &&
				presence != JsonSerializerInternalReader.PropertyPresence.Null)
				return;
			try
			{
				Required? required1 = property._required;
				int num;
				if (!required1.HasValue)
				{
					Required? itemRequired = contract.ItemRequired;
					num = itemRequired.HasValue ? (int)itemRequired.GetValueOrDefault() : 0;
				}
				else
					num = (int)required1.GetValueOrDefault();

				Required required2 = (Required)num;
				switch (presence)
				{
					case JsonSerializerInternalReader.PropertyPresence.None:
						if (required2 == Required.AllowNull || required2 == Required.Always)
							throw JsonSerializationException.Create(reader,
							  "Required property '{0}' not found in JSON.".FormatWith(CultureInfo.InvariantCulture,
								 property.PropertyName));
						if (!setDefaultValue || property.Ignored)
							break;
						if (property.PropertyContract == null)
							property.PropertyContract = this.GetContractSafe(property.PropertyType);
						if (!this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling),
							  DefaultValueHandling.Populate) || !property.Writable)
							break;
						property.ValueProvider.SetValue(newObject,
						  this.EnsureType(reader, property.GetResolvedDefaultValue(), CultureInfo.InvariantCulture,
							property.PropertyContract, property.PropertyType));
						break;
					case JsonSerializerInternalReader.PropertyPresence.Null:
						if (required2 == Required.Always)
							throw JsonSerializationException.Create(reader,
							  "Required property '{0}' expects a value but got null.".FormatWith(
								 CultureInfo.InvariantCulture, property.PropertyName));
						if (required2 != Required.DisallowNull)
							break;
						throw JsonSerializationException.Create(reader,
						  "Required property '{0}' expects a non-null value.".FormatWith(
							 CultureInfo.InvariantCulture, property.PropertyName));
				}
			}
			catch (Exception ex)
			{
				if (this.IsErrorHandled(newObject, contract, property.PropertyName,
				  reader as IJsonLineInfo, reader.Path, ex))
					this.HandleError(reader, true, initialDepth);
				else
					throw;
			}
		}

		private void SetPropertyPresence(
		  JsonReader reader,
		  JsonProperty property,
		  Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> requiredProperties)
		{
			if (property == null || requiredProperties == null)
				return;
			JsonSerializerInternalReader.PropertyPresence propertyPresence;
			switch (reader.TokenType)
			{
				case JsonToken.String:
					propertyPresence =
					  JsonSerializerInternalReader.CoerceEmptyStringToNull(property.PropertyType, property.PropertyContract,
						(string)reader.Value)
						? JsonSerializerInternalReader.PropertyPresence.Null
						: JsonSerializerInternalReader.PropertyPresence.Value;
					break;
				case JsonToken.Null:
				case JsonToken.Undefined:
					propertyPresence = JsonSerializerInternalReader.PropertyPresence.Null;
					break;
				default:
					propertyPresence = JsonSerializerInternalReader.PropertyPresence.Value;
					break;
			}

			requiredProperties[property] = propertyPresence;
		}

		private void HandleError(JsonReader reader, bool readPastError, int initialDepth)
		{
			this.ClearErrorContext();
			if (!readPastError)
				return;
			reader.Skip();
			do
				;
			while (reader.Depth > initialDepth + 1 && reader.Read());
		}

		internal enum PropertyPresence
		{
			None,
			Null,
			Value,
		}

		internal class CreatorPropertyContext
		{
			public string Name;
			public JsonProperty Property;
			public JsonProperty ConstructorProperty;
			public JsonSerializerInternalReader.PropertyPresence? Presence;
			public object Value;
			public bool Used;
		}
	}
}