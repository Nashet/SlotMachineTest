using Socket.Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Socket.Newtonsoft.Json.Linq
{
	public static class Extensions
	{
		public static IJEnumerable<JToken> Ancestors<T>(this IEnumerable<T> source) where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, nameof(source));
			return source.SelectMany<T, JToken>(j => j.Ancestors()).AsJEnumerable();
		}

		public static IJEnumerable<JToken> AncestorsAndSelf<T>(
		  this IEnumerable<T> source)
		  where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, nameof(source));
			return source.SelectMany<T, JToken>(j => j.AncestorsAndSelf()).AsJEnumerable();
		}

		public static IJEnumerable<JToken> Descendants<T>(this IEnumerable<T> source) where T : JContainer
		{
			ValidationUtils.ArgumentNotNull(source, nameof(source));
			return source.SelectMany<T, JToken>(j => j.Descendants()).AsJEnumerable();
		}

		public static IJEnumerable<JToken> DescendantsAndSelf<T>(
		  this IEnumerable<T> source)
		  where T : JContainer
		{
			ValidationUtils.ArgumentNotNull(source, nameof(source));
			return source.SelectMany<T, JToken>(j => j.DescendantsAndSelf()).AsJEnumerable();
		}

		public static IJEnumerable<JProperty> Properties(
		  this IEnumerable<JObject> source)
		{
			ValidationUtils.ArgumentNotNull(source, nameof(source));
			return source.SelectMany<JObject, JProperty>(d => d.Properties()).AsJEnumerable<JProperty>();
		}

		public static IJEnumerable<JToken> Values(
		  this IEnumerable<JToken> source,
		  object key)
		{
			return source.Values<JToken, JToken>(key).AsJEnumerable();
		}

		public static IJEnumerable<JToken> Values(this IEnumerable<JToken> source)
		{
			return source.Values(null);
		}

		public static IEnumerable<U> Values<U>(this IEnumerable<JToken> source, object key)
		{
			return source.Values<JToken, U>(key);
		}

		public static IEnumerable<U> Values<U>(this IEnumerable<JToken> source)
		{
			return source.Values<JToken, U>(null);
		}

		public static U Value<U>(this IEnumerable<JToken> value)
		{
			return value.Value<JToken, U>();
		}

		public static U Value<T, U>(this IEnumerable<T> value) where T : JToken
		{
			ValidationUtils.ArgumentNotNull(value, nameof(value));
			JToken token = value as JToken;
			if (token == null)
				throw new ArgumentException("Source value must be a JToken.");
			return token.Convert<JToken, U>();
		}

		internal static IEnumerable<U> Values<T, U>(this IEnumerable<T> source, object key) where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, nameof(source));
			if (key == null)
			{
				foreach (T obj in source)
				{
					T token = obj;
					JValue token1 = (object)token as JValue;
					if (token1 != null)
					{
						yield return token1.Convert<JValue, U>();
					}
					else
					{
						foreach (JToken child in token.Children())
							yield return child.Convert<JToken, U>();
					}
					token = default;
				}
			}
			else
			{
				foreach (T obj in source)
				{
					JToken token = obj[key];
					if (token != null)
						yield return token.Convert<JToken, U>();
				}
			}
		}

		public static IJEnumerable<JToken> Children<T>(this IEnumerable<T> source) where T : JToken
		{
			return source.Children<T, JToken>().AsJEnumerable();
		}

		public static IEnumerable<U> Children<T, U>(this IEnumerable<T> source) where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, nameof(source));
			return source.SelectMany<T, JToken>(c => c.Children()).Convert<JToken, U>();
		}

		internal static IEnumerable<U> Convert<T, U>(this IEnumerable<T> source) where T : JToken
		{
			ValidationUtils.ArgumentNotNull(source, nameof(source));
			foreach (T obj in source)
				yield return obj.Convert<JToken, U>();
		}

		internal static U Convert<T, U>(this T token) where T : JToken
		{
			if (token == null)
				return default;
			if ((object)token is U && typeof(U) != typeof(IComparable) && typeof(U) != typeof(IFormattable))
				return (U)(object)token;
			JValue jvalue = (object)token as JValue;
			if (jvalue == null)
				throw new InvalidCastException("Cannot cast {0} to {1}.".FormatWith(CultureInfo.InvariantCulture, token.GetType(), typeof(T)));
			if (jvalue.Value is U)
				return (U)jvalue.Value;
			Type type = typeof(U);
			if (ReflectionUtils.IsNullableType(type))
			{
				if (jvalue.Value == null)
					return default;
				type = Nullable.GetUnderlyingType(type);
			}
			return (U)System.Convert.ChangeType(jvalue.Value, type, CultureInfo.InvariantCulture);
		}

		public static IJEnumerable<JToken> AsJEnumerable(
		  this IEnumerable<JToken> source)
		{
			return source.AsJEnumerable<JToken>();
		}

		public static IJEnumerable<T> AsJEnumerable<T>(this IEnumerable<T> source) where T : JToken
		{
			if (source == null)
				return null;
			if (source is IJEnumerable<T>)
				return (IJEnumerable<T>)source;
			return new JEnumerable<T>(source);
		}
	}
}
