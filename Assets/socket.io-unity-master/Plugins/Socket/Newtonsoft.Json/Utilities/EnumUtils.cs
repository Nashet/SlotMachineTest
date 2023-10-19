using Socket.Newtonsoft.Json.Utilities.LinqBridge;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Socket.Newtonsoft.Json.Utilities
{
	internal static class EnumUtils
	{
		private static readonly ThreadSafeStore<Type, BidirectionalDictionary<string, string>> EnumMemberNamesPerType =
		  new ThreadSafeStore<Type, BidirectionalDictionary<string, string>>(
			new Func<Type, BidirectionalDictionary<string, string>>(EnumUtils.InitializeEnumType));

		private static BidirectionalDictionary<string, string> InitializeEnumType(
		  Type type)
		{
			BidirectionalDictionary<string, string> bidirectionalDictionary =
			  new BidirectionalDictionary<string, string>(StringComparer.Ordinal,
				 StringComparer.Ordinal);
			foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				string name1 = field.Name;
				string name2 = field.Name;
				string first;
				if (bidirectionalDictionary.TryGetBySecond(name2, out first))
					throw new InvalidOperationException(
					  "Enum name '{0}' already exists on enum '{1}'.".FormatWith(CultureInfo.InvariantCulture,
						 name2, type.Name));
				bidirectionalDictionary.Set(name1, name2);
			}

			return bidirectionalDictionary;
		}

		public static IList<T> GetFlagsValues<T>(T value) where T : struct
		{
			Type type = typeof(T);
			if (!type.IsDefined(typeof(FlagsAttribute), false))
				throw new ArgumentException(
				  "Enum type {0} is not a set of flags.".FormatWith(CultureInfo.InvariantCulture,
					 type));
			Type underlyingType = Enum.GetUnderlyingType(value.GetType());
			ulong uint64 = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
			IList<EnumValue<ulong>> namesAndValues = EnumUtils.GetNamesAndValues<T>();
			IList<T> objList = new List<T>();
			foreach (EnumValue<ulong> enumValue in (IEnumerable<EnumValue<ulong>>)namesAndValues)
			{
				if (((long)uint64 & (long)enumValue.Value) == (long)enumValue.Value && enumValue.Value != 0UL)
					objList.Add((T)Convert.ChangeType(enumValue.Value, underlyingType,
					   CultureInfo.CurrentCulture));
			}

			if (objList.Count == 0 &&
				namesAndValues.SingleOrDefault<EnumValue<ulong>>(v => v.Value == 0UL) !=
				null)
				objList.Add(default);
			return objList;
		}

		public static IList<EnumValue<ulong>> GetNamesAndValues<T>() where T : struct
		{
			return EnumUtils.GetNamesAndValues<ulong>(typeof(T));
		}

		public static IList<EnumValue<TUnderlyingType>> GetNamesAndValues<TUnderlyingType>(
		  Type enumType)
		  where TUnderlyingType : struct
		{
			if (enumType == null)
				throw new ArgumentNullException(nameof(enumType));
			if (!enumType.IsEnum())
				throw new ArgumentException(
				  "Type {0} is not an enum.".FormatWith(CultureInfo.InvariantCulture, enumType.Name),
				  nameof(enumType));
			IList<object> values = EnumUtils.GetValues(enumType);
			IList<string> names = EnumUtils.GetNames(enumType);
			IList<EnumValue<TUnderlyingType>> enumValueList =
			   new List<EnumValue<TUnderlyingType>>();
			for (int index = 0; index < values.Count; ++index)
			{
				try
				{
					enumValueList.Add(new EnumValue<TUnderlyingType>(names[index],
					  (TUnderlyingType)Convert.ChangeType(values[index], typeof(TUnderlyingType),
						 CultureInfo.CurrentCulture)));
				}
				catch (OverflowException ex)
				{
					throw new InvalidOperationException(
					  "Value from enum with the underlying type of {0} cannot be added to dictionary with a value type of {1}. Value was too large: {2}"
						.FormatWith(CultureInfo.InvariantCulture, Enum.GetUnderlyingType(enumType),
						   typeof(TUnderlyingType),
						   Convert.ToUInt64(values[index], CultureInfo.InvariantCulture)),
					   ex);
				}
			}

			return enumValueList;
		}

		public static IList<object> GetValues(Type enumType)
		{
			if (!enumType.IsEnum())
				throw new ArgumentException(
				  "Type {0} is not an enum.".FormatWith(CultureInfo.InvariantCulture, enumType.Name),
				  nameof(enumType));
			List<object> objectList = new List<object>();
			foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				object obj = field.GetValue(enumType);
				objectList.Add(obj);
			}

			return objectList;
		}

		public static IList<string> GetNames(Type enumType)
		{
			if (!enumType.IsEnum())
				throw new ArgumentException(
				  "Type {0} is not an enum.".FormatWith(CultureInfo.InvariantCulture, enumType.Name),
				  nameof(enumType));
			List<string> stringList = new List<string>();
			foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.Public))
				stringList.Add(field.Name);
			return stringList;
		}

		public static object ParseEnumName(
		  string enumText,
		  bool isNullable,
		  bool disallowValue,
		  Type t)
		{
			if (enumText == string.Empty & isNullable)
				return null;
			BidirectionalDictionary<string, string> map = EnumUtils.EnumMemberNamesPerType.Get(t);
			string resolvedEnumName;
			string str;
			if (EnumUtils.TryResolvedEnumName(map, enumText, out resolvedEnumName))
				str = resolvedEnumName;
			else if (enumText.IndexOf(',') != -1)
			{
				string[] strArray = enumText.Split(',');
				for (int index = 0; index < strArray.Length; ++index)
				{
					string enumText1 = strArray[index].Trim();
					strArray[index] = EnumUtils.TryResolvedEnumName(map, enumText1, out resolvedEnumName)
					  ? resolvedEnumName
					  : enumText1;
				}

				str = string.Join(", ", strArray);
			}
			else
			{
				str = enumText;
				if (disallowValue)
				{
					bool flag = true;
					for (int index = 0; index < str.Length; ++index)
					{
						if (!char.IsNumber(str[index]))
						{
							flag = false;
							break;
						}
					}

					if (flag)
						throw new FormatException(
						  "Integer string '{0}' is not allowed.".FormatWith(CultureInfo.InvariantCulture,
							 enumText));
				}
			}

			return Enum.Parse(t, str, true);
		}

		public static string ToEnumName(Type enumType, string enumText, bool camelCaseText)
		{
			BidirectionalDictionary<string, string> bidirectionalDictionary = EnumUtils.EnumMemberNamesPerType.Get(enumType);
			string[] strArray = enumText.Split(',');
			for (int index = 0; index < strArray.Length; ++index)
			{
				string first = strArray[index].Trim();
				string second;
				bidirectionalDictionary.TryGetByFirst(first, out second);
				second = second ?? first;
				if (camelCaseText)
					second = StringUtils.ToCamelCase(second);
				strArray[index] = second;
			}

			return string.Join(", ", strArray);
		}

		private static bool TryResolvedEnumName(
		  BidirectionalDictionary<string, string> map,
		  string enumText,
		  out string resolvedEnumName)
		{
			if (map.TryGetBySecond(enumText, out resolvedEnumName))
				return true;
			resolvedEnumName = null;
			return false;
		}
	}
}