using Socket.Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Socket.Newtonsoft.Json.Utilities
{
	internal class LateBoundReflectionDelegateFactory : ReflectionDelegateFactory
	{
		private static readonly LateBoundReflectionDelegateFactory _instance = new LateBoundReflectionDelegateFactory();

		internal static ReflectionDelegateFactory Instance
		{
			get { return _instance; }
		}

		public override ObjectConstructor<object> CreateParameterizedConstructor(
		  MethodBase method)
		{
			ValidationUtils.ArgumentNotNull(method, nameof(method));
			ConstructorInfo c = method as ConstructorInfo;
			if (c != null)
				return a => c.Invoke(a);
			return a => method.Invoke(null, a);
		}

		public override MethodCall<T, object> CreateMethodCall<T>(MethodBase method)
		{
			ValidationUtils.ArgumentNotNull(method, nameof(method));
			ConstructorInfo c = method as ConstructorInfo;
			if (c != null)
				return (o, a) => c.Invoke(a);
			return (o, a) => method.Invoke(o, a);
		}

		public override Func<T> CreateDefaultConstructor<T>(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, nameof(type));
			if (type.IsValueType())
				return () => (T)Activator.CreateInstance(type);
			ConstructorInfo constructorInfo = ReflectionUtils.GetDefaultConstructor(type, true);
			return () => (T)constructorInfo.Invoke(null);
		}

		public override Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
		{
			ValidationUtils.ArgumentNotNull(propertyInfo, nameof(propertyInfo));
			return o => propertyInfo.GetValue(o, null);
		}

		public override Func<T, object> CreateGet<T>(FieldInfo fieldInfo)
		{
			ValidationUtils.ArgumentNotNull(fieldInfo, nameof(fieldInfo));
			return o => fieldInfo.GetValue(o);
		}

		public override Action<T, object> CreateSet<T>(
		  FieldInfo fieldInfo)
		{
			ValidationUtils.ArgumentNotNull(fieldInfo, nameof(fieldInfo));
			return (o, v) => fieldInfo.SetValue(o, v);
		}

		public override Action<T, object> CreateSet<T>(
		  PropertyInfo propertyInfo)
		{
			ValidationUtils.ArgumentNotNull(propertyInfo, nameof(propertyInfo));
			return (o, v) =>
			  propertyInfo.SetValue(o, v, null);
		}
	}
}