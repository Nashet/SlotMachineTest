using System.Collections.Generic;

namespace Socket.Newtonsoft.Json.Linq.JsonPath
{
	internal class ScanMultipleFilter : PathFilter
	{
		public List<string> Names { get; set; }

		public override IEnumerable<JToken> ExecuteFilter(
		  JToken root,
		  IEnumerable<JToken> current,
		  bool errorWhenNoMatch)
		{
			foreach (JToken jtoken1 in current)
			{
				JToken c = jtoken1;
				JToken value = c;
				JToken jtoken2 = c;
				while (true)
				{
					if (jtoken2 != null && jtoken2.HasValues)
					{
						value = jtoken2.First;
					}
					else
					{
						while (value != null && value != c && value == value.Parent.Last)
							value = value.Parent;
						if (value != null && value != c)
							value = value.Next;
						else
							break;
					}

					JProperty e = value as JProperty;
					if (e != null)
					{
						foreach (string name in this.Names)
						{
							if (e.Name == name)
								yield return e.Value;
						}
					}

					jtoken2 = value as JContainer;
					e = null;
				}

				value = null;
				c = null;
			}
		}
	}
}
