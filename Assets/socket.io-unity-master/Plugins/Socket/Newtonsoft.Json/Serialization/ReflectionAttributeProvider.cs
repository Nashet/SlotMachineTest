using Socket.Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

namespace Socket.Newtonsoft.Json.Serialization
{
	public class ReflectionAttributeProvider : IAttributeProvider
	{
		private readonly object _attributeProvider;

		public ReflectionAttributeProvider(object attributeProvider)
		{
			ValidationUtils.ArgumentNotNull(attributeProvider, nameof(attributeProvider));
			this._attributeProvider = attributeProvider;
		}

		public IList<Attribute> GetAttributes(bool inherit)
		{
			return ReflectionUtils.GetAttributes(this._attributeProvider, null, inherit);
		}

		public IList<Attribute> GetAttributes(Type attributeType, bool inherit)
		{
			return ReflectionUtils.GetAttributes(this._attributeProvider, attributeType, inherit);
		}
	}
}