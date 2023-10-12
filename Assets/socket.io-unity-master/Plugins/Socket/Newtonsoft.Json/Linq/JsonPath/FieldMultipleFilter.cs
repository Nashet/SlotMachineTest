using Socket.Newtonsoft.Json.Utilities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Socket.Newtonsoft.Json.Linq.JsonPath
{
	internal class FieldMultipleFilter : PathFilter
	{
		public List<string> Names { get; set; }

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
					foreach (string name1 in this.Names)
					{
						string name = name1;
						JToken jtoken2 = o[name];
						if (jtoken2 != null)
							yield return jtoken2;
						if (errorWhenNoMatch)
							throw new JsonException(
							  "Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture,
								 name));
						name = null;
					}
				}
				else if (errorWhenNoMatch)
					throw new JsonException("Properties {0} not valid on {1}.".FormatWith(
					   CultureInfo.InvariantCulture,
					   string.Join(", ",
						this.Names.Select<string, string>(n => "'" + n + "'").ToArray<string>()),
					   t.GetType().Name));

				o = null;
				t = null;
			}
		}
	}
}