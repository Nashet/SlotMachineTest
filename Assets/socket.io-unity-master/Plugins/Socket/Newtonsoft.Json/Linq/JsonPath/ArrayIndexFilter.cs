using Socket.Newtonsoft.Json.Utilities;
using System.Collections.Generic;
using System.Globalization;

namespace Socket.Newtonsoft.Json.Linq.JsonPath
{
	internal class ArrayIndexFilter : PathFilter
	{
		public int? Index { get; set; }

		public override IEnumerable<JToken> ExecuteFilter(
		  JToken root,
		  IEnumerable<JToken> current,
		  bool errorWhenNoMatch)
		{
			foreach (JToken jtoken1 in current)
			{
				JToken t = jtoken1;
				if (this.Index.HasValue)
				{
					JToken tokenIndex = PathFilter.GetTokenIndex(t, errorWhenNoMatch, this.Index.GetValueOrDefault());
					if (tokenIndex != null)
						yield return tokenIndex;
				}
				else if (t is JArray || t is JConstructor)
				{
					foreach (JToken jtoken2 in t)
						yield return jtoken2;
				}
				else if (errorWhenNoMatch)
					throw new JsonException("Index * not valid on {0}.".FormatWith(CultureInfo.InvariantCulture,
					   t.GetType().Name));

				t = null;
			}
		}
	}
}