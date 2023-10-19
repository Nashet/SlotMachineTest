using Socket.Newtonsoft.Json.Utilities;
using System;
using System.Globalization;

namespace Socket.Newtonsoft.Json.Linq
{
	public class JTokenWriter : JsonWriter
	{
		private JContainer _token;
		private JContainer _parent;
		private JValue _value;
		private JToken _current;

		public JToken CurrentToken
		{
			get { return this._current; }
		}

		public JToken Token
		{
			get
			{
				if (this._token != null)
					return _token;
				return _value;
			}
		}

		public JTokenWriter(JContainer container)
		{
			ValidationUtils.ArgumentNotNull(container, nameof(container));
			this._token = container;
			this._parent = container;
		}

		public JTokenWriter()
		{
		}

		public override void Flush()
		{
		}

		public override void Close()
		{
			base.Close();
		}

		public override void WriteStartObject()
		{
			base.WriteStartObject();
			this.AddParent(new JObject());
		}

		private void AddParent(JContainer container)
		{
			if (this._parent == null)
				this._token = container;
			else
				this._parent.AddAndSkipParentCheck(container);
			this._parent = container;
			this._current = container;
		}

		private void RemoveParent()
		{
			this._current = _parent;
			this._parent = this._parent.Parent;
			if (this._parent == null || this._parent.Type != JTokenType.Property)
				return;
			this._parent = this._parent.Parent;
		}

		public override void WriteStartArray()
		{
			base.WriteStartArray();
			this.AddParent(new JArray());
		}

		public override void WriteStartConstructor(string name)
		{
			base.WriteStartConstructor(name);
			this.AddParent(new JConstructor(name));
		}

		protected override void WriteEnd(JsonToken token)
		{
			this.RemoveParent();
		}

		public override void WritePropertyName(string name)
		{
			(this._parent as JObject)?.Remove(name);
			this.AddParent(new JProperty(name));
			base.WritePropertyName(name);
		}

		private void AddValue(object value, JsonToken token)
		{
			this.AddValue(new JValue(value), token);
		}

		internal void AddValue(JValue value, JsonToken token)
		{
			if (this._parent != null)
			{
				this._parent.Add(value);
				this._current = this._parent.Last;
				if (this._parent.Type != JTokenType.Property)
					return;
				this._parent = this._parent.Parent;
			}
			else
			{
				this._value = value ?? JValue.CreateNull();
				this._current = _value;
			}
		}

		public override void WriteValue(object value)
		{
			base.WriteValue(value);
		}

		public override void WriteNull()
		{
			base.WriteNull();
			this.AddValue(null, JsonToken.Null);
		}

		public override void WriteUndefined()
		{
			base.WriteUndefined();
			this.AddValue(null, JsonToken.Undefined);
		}

		public override void WriteRaw(string json)
		{
			base.WriteRaw(json);
			this.AddValue(new JRaw(json), JsonToken.Raw);
		}

		public override void WriteComment(string text)
		{
			base.WriteComment(text);
			this.AddValue(JValue.CreateComment(text), JsonToken.Comment);
		}

		public override void WriteValue(string value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.String);
		}

		public override void WriteValue(int value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public override void WriteValue(uint value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		public override void WriteValue(long value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public override void WriteValue(ulong value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		public override void WriteValue(float value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Float);
		}

		public override void WriteValue(double value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Float);
		}

		public override void WriteValue(bool value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Boolean);
		}

		public override void WriteValue(short value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public override void WriteValue(ushort value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		public override void WriteValue(char value)
		{
			base.WriteValue(value);
			this.AddValue(value.ToString(CultureInfo.InvariantCulture), JsonToken.String);
		}

		public override void WriteValue(byte value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public override void WriteValue(sbyte value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Integer);
		}

		public override void WriteValue(Decimal value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Float);
		}

		public override void WriteValue(DateTime value)
		{
			base.WriteValue(value);
			value = DateTimeUtils.EnsureDateTime(value, this.DateTimeZoneHandling);
			this.AddValue(value, JsonToken.Date);
		}

		public override void WriteValue(byte[] value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.Bytes);
		}

		public override void WriteValue(TimeSpan value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.String);
		}

		public override void WriteValue(Guid value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.String);
		}

		public override void WriteValue(Uri value)
		{
			base.WriteValue(value);
			this.AddValue(value, JsonToken.String);
		}

		internal override void WriteToken(
		  JsonReader reader,
		  bool writeChildren,
		  bool writeDateConstructorAsDate,
		  bool writeComments)
		{
			JTokenReader jtokenReader = reader as JTokenReader;
			if (jtokenReader != null & writeChildren & writeDateConstructorAsDate & writeComments)
			{
				if (jtokenReader.TokenType == JsonToken.None && !jtokenReader.Read())
					return;
				JToken jtoken = jtokenReader.CurrentToken.CloneToken();
				if (this._parent != null)
				{
					this._parent.Add(jtoken);
					this._current = this._parent.Last;
					if (this._parent.Type == JTokenType.Property)
					{
						this._parent = this._parent.Parent;
						this.InternalWriteValue(JsonToken.Null);
					}
				}
				else
				{
					this._current = jtoken;
					if (this._token == null && this._value == null)
					{
						this._token = jtoken as JContainer;
						this._value = jtoken as JValue;
					}
				}

				jtokenReader.Skip();
			}
			else
				base.WriteToken(reader, writeChildren, writeDateConstructorAsDate, writeComments);
		}
	}
}