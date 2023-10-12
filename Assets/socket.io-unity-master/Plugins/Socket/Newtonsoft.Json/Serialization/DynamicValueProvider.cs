using Socket.Newtonsoft.Json.Utilities;
using System;
using System.Globalization;
using System.Reflection;

namespace Socket.Newtonsoft.Json.Serialization
{
	public class DynamicValueProvider : IValueProvider
	{
		private readonly MemberInfo _memberInfo;
		private Func<object, object> _getter;
		private Action<object, object> _setter;

		public DynamicValueProvider(MemberInfo memberInfo)
		{
			ValidationUtils.ArgumentNotNull(memberInfo, nameof(memberInfo));
			this._memberInfo = memberInfo;
		}

		public void SetValue(object target, object value)
		{
			try
			{
				if (this._setter == null)
					this._setter = DynamicReflectionDelegateFactory.Instance.CreateSet<object>(this._memberInfo);
				this._setter(target, value);
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
				if (this._getter == null)
					this._getter = DynamicReflectionDelegateFactory.Instance.CreateGet<object>(this._memberInfo);
				return this._getter(target);
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