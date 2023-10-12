using Socket.Newtonsoft.Json.Utilities;
using System.Collections.Generic;
using System.Globalization;

namespace Socket.Newtonsoft.Json.Linq.JsonPath
{
	internal class FieldFilter : PathFilter
	{
		public string Name { get; set; }

		public override IEnumerable<JToken> ExecuteFilter(
		  JToken root,
		  IEnumerable<JToken> current,
		  bool errorWhenNoMatch)
		{
			foreach (JToken jtoken1 in current)
			{
				JToken t = jtoken1;
				JObject o = t as JObject;
				if (o != null)
				{
					if (this.Name != null)
					{
						JToken jtoken2 = o[this.Name];
						if (jtoken2 != null)
							yield return jtoken2;
						else if (errorWhenNoMatch)
							throw new JsonException(
							  "Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture,
								 Name));
					}
					else
					{
						foreach (KeyValuePair<string, JToken> keyValuePair in o)
							yield return keyValuePair.Value;
					}
				}
				else if (errorWhenNoMatch)
					throw new JsonException("Property '{0}' not valid on {1}.".FormatWith(
					   CultureInfo.InvariantCulture, this.Name ?? "*", t.GetType().Name));

				o = null;
				t = null;
			}
		}
	}
}