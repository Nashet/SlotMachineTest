using Socket.Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Socket.Newtonsoft.Json.Linq
{
	public struct JEnumerable<T> : IJEnumerable<T>, IEnumerable<T>, IEnumerable, IEquatable<JEnumerable<T>>
	where T : JToken
	{
		public static readonly JEnumerable<T> Empty = new JEnumerable<T>(Enumerable.Empty<T>());
		private readonly IEnumerable<T> _enumerable;

		public JEnumerable(IEnumerable<T> enumerable)
		{
			ValidationUtils.ArgumentNotNull(enumerable, nameof(enumerable));
			this._enumerable = enumerable;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)((object)this._enumerable ?? Empty)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IJEnumerable<JToken> this[object key]
		{
			get
			{
				if (this._enumerable == null)
					return JEnumerable<JToken>.Empty;
				return new JEnumerable<JToken>(this._enumerable.Values<T, JToken>(key));
			}
		}

		public bool Equals(JEnumerable<T> other)
		{
			return object.Equals(_enumerable, other._enumerable);
		}

		public override bool Equals(object obj)
		{
			if (obj is JEnumerable<T>)
				return this.Equals((JEnumerable<T>)obj);
			return false;
		}

		public override int GetHashCode()
		{
			if (this._enumerable == null)
				return 0;
			return this._enumerable.GetHashCode();
		}
	}
}
