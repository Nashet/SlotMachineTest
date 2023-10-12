using Socket.Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;

namespace Socket.Newtonsoft.Json.Serialization
{
	public class ReflectionValueProvider : IValueProvider
	{
		private readonly MemberInfo _memberInfo;

		public ReflectionValueProvider(MemberInfo memberInfo)
		{
			ValidationUtils.ArgumentNotNull(memberInfo, nameof(memberInfo));
			this._memberInfo = memberInfo;
		}

		public void SetValue(object target, object value)
		{
			try
			{
				ReflectionUtils.SetMemberValue(this._memberInfo, target, value);
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException(
				  "Error setting value to '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture,
					 _memberInfo.Name, target.GetType()), ex);
			}
		}

		public object GetValue(object target)
		{
			try
			{
				return ReflectionUtils.GetMemberValue(this._memberInfo, target);
			}
			catch (Exception ex)
			{
				throw new JsonSerializationException(
				  "Error getting value from '{0}' on '{1}'.".FormatWith(CultureInfo.InvariantCulture,
					 _memberInfo.Name, target.GetType()), ex);
			}
		}
	}
}