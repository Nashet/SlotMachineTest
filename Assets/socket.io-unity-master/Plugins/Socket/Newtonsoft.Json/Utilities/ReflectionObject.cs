using Socket.Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Socket.Newtonsoft.Json.Utilities
{
	internal class ReflectionObject
	{
		public ObjectConstructor<object> Creator { get; }

		public IDictionary<string, ReflectionMember> Members { get; }

		private ReflectionObject(ObjectConstructor<object> creator)
		{
			this.Members = new Dictionary<string, ReflectionMember>();
			this.Creator = creator;
		}

		public object GetValue(object target, string member)
		{
			return this.Members[member].Getter(target);
		}

		public void SetValue(object target, string member, object value)
		{
			this.Members[member].Setter(target, value);
		}

		public Type GetType(string member)
		{
			return this.Members[member].MemberType;
		}

		public static ReflectionObject Create(Type t, params string[] memberNames)
		{
			return ReflectionObject.Create(t, null, memberNames);
		}

		public static ReflectionObject Create(
		  Type t,
		  MethodBase creator,
		  params string[] memberNames)
		{
			ReflectionDelegateFactory reflectionDelegateFactory = JsonTypeReflector.ReflectionDelegateFactory;
			ObjectConstructor<object> creator1 = null;
			if (creator != null)
				creator1 = reflectionDelegateFactory.CreateParameterizedConstructor(creator);
			else if (ReflectionUtils.HasDefaultConstructor(t, false))
			{
				Func<object> ctor = reflectionDelegateFactory.CreateDefaultConstructor<object>(t);
				creator1 = args => ctor();
			}

			ReflectionObject reflectionObject = new ReflectionObject(creator1);
			foreach (string memberName in memberNames)
			{
				MemberInfo[] member = t.GetMember(memberName, BindingFlags.Instance | BindingFlags.Public);
				if (member.Length != 1)
					throw new ArgumentException(
					  "Expected a single member with the name '{0}'.".FormatWith(CultureInfo.InvariantCulture,
						 memberName));
				MemberInfo memberInfo = member.Single<MemberInfo>();
				ReflectionMember reflectionMember = new ReflectionMember();
				switch (memberInfo.MemberType())
				{
					case MemberTypes.Field:
					case MemberTypes.Property:
						if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
							reflectionMember.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
						if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
						{
							reflectionMember.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
							break;
						}

						break;
					case MemberTypes.Method:
						MethodInfo methodInfo = (MethodInfo)memberInfo;
						if (methodInfo.IsPublic)
						{
							ParameterInfo[] parameters = methodInfo.GetParameters();
							if (parameters.Length == 0 && methodInfo.ReturnType != typeof(void))
							{
								MethodCall<object, object> call =
								  reflectionDelegateFactory.CreateMethodCall<object>(methodInfo);
								reflectionMember.Getter = target => call(target, new object[0]);
								break;
							}

							if (parameters.Length == 1 && methodInfo.ReturnType == typeof(void))
							{
								MethodCall<object, object> call =
								  reflectionDelegateFactory.CreateMethodCall<object>(methodInfo);
								object obj;
								reflectionMember.Setter = (target, arg) => obj =
								  call(target, new object[1] {
					arg
								  });
								break;
							}

							break;
						}

						break;
					default:
						throw new ArgumentException("Unexpected member type '{0}' for member '{1}'.".FormatWith(
						   CultureInfo.InvariantCulture, memberInfo.MemberType(),
						   memberInfo.Name));
				}

				if (ReflectionUtils.CanReadMemberValue(memberInfo, false))
					reflectionMember.Getter = reflectionDelegateFactory.CreateGet<object>(memberInfo);
				if (ReflectionUtils.CanSetMemberValue(memberInfo, false, false))
					reflectionMember.Setter = reflectionDelegateFactory.CreateSet<object>(memberInfo);
				reflectionMember.MemberType = ReflectionUtils.GetMemberUnderlyingType(memberInfo);
				reflectionObject.Members[memberName] = reflectionMember;
			}

			return reflectionObject;
		}
	}
}