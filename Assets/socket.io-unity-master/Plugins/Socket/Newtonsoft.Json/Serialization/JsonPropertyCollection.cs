using Socket.Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Socket.Newtonsoft.Json.Serialization
{
	public class JsonPropertyCollection : KeyedCollection<string, JsonProperty>
	{
		private readonly Type _type;
		private readonly List<JsonProperty> _list;

		public JsonPropertyCollection(Type type)
		  : base(StringComparer.Ordinal)
		{
			ValidationUtils.ArgumentNotNull(type, nameof(type));
			this._type = type;
			this._list = (List<JsonProperty>)this.Items;
		}

		protected override string GetKeyForItem(JsonProperty item)
		{
			return item.PropertyName;
		}

		public void AddProperty(JsonProperty property)
		{
			if (this.Contains(property.PropertyName))
			{
				if (property.Ignored)
					return;
				JsonProperty jsonProperty = this[property.PropertyName];
				bool flag = true;
				if (jsonProperty.Ignored)
				{
					this.Remove(jsonProperty);
					flag = false;
				}
				else if (property.DeclaringType != null && jsonProperty.DeclaringType != null)
				{
					if (property.DeclaringType.IsSubclassOf(jsonProperty.DeclaringType) ||
						jsonProperty.DeclaringType.IsInterface() &&
						property.DeclaringType.ImplementInterface(jsonProperty.DeclaringType))
					{
						this.Remove(jsonProperty);
						flag = false;
					}

					if (jsonProperty.DeclaringType.IsSubclassOf(property.DeclaringType) || property.DeclaringType.IsInterface() &&
						jsonProperty.DeclaringType.ImplementInterface(property.DeclaringType))
						return;
				}

				if (flag)
					throw new JsonSerializationException(
					  "A member with the name '{0}' already exists on '{1}'. Use the JsonPropertyAttribute to specify another name."
						.FormatWith(CultureInfo.InvariantCulture, property.PropertyName,
						   _type));
			}

			this.Add(property);
		}

		public JsonProperty GetClosestMatchProperty(string propertyName)
		{
			return this.GetProperty(propertyName, StringComparison.Ordinal) ??
				   this.GetProperty(propertyName, StringComparison.OrdinalIgnoreCase);
		}

		private bool TryGetValue(string key, out JsonProperty item)
		{
			if (this.Dictionary != null)
				return this.Dictionary.TryGetValue(key, out item);
			item = null;
			return false;
		}

		public JsonProperty GetProperty(
		  string propertyName,
		  StringComparison comparisonType)
		{
			if (comparisonType == StringComparison.Ordinal)
			{
				JsonProperty jsonProperty;
				if (this.TryGetValue(propertyName, out jsonProperty))
					return jsonProperty;
				return null;
			}

			for (int index = 0; index < this._list.Count; ++index)
			{
				JsonProperty jsonProperty = this._list[index];
				if (string.Equals(propertyName, jsonProperty.PropertyName, comparisonType))
					return jsonProperty;
			}

			return null;
		}
	}
}