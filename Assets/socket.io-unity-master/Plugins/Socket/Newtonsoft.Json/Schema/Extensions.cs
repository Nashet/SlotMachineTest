using Socket.Newtonsoft.Json.Linq;
using Socket.Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;

namespace Socket.Newtonsoft.Json.Schema
{
	[Obsolete(
	"JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
	public static class Extensions
	{
		[Obsolete(
		  "JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
		public static bool IsValid(this JToken source, JsonSchema schema)
		{
			bool valid = true;
			source.Validate(schema, (sender, args) => valid = false);
			return valid;
		}

		[Obsolete(
		  "JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
		public static bool IsValid(
		  this JToken source,
		  JsonSchema schema,
		  out IList<string> errorMessages)
		{
			IList<string> errors = new List<string>();
			source.Validate(schema, (sender, args) => errors.Add(args.Message));
			errorMessages = errors;
			return errorMessages.Count == 0;
		}

		[Obsolete(
		  "JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
		public static void Validate(this JToken source, JsonSchema schema)
		{
			source.Validate(schema, null);
		}

		[Obsolete(
		  "JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.")]
		public static void Validate(
		  this JToken source,
		  JsonSchema schema,
		  ValidationEventHandler validationEventHandler)
		{
			ValidationUtils.ArgumentNotNull(source, nameof(source));
			ValidationUtils.ArgumentNotNull(schema, nameof(schema));
			using (JsonValidatingReader validatingReader = new JsonValidatingReader(source.CreateReader()))
			{
				validatingReader.Schema = schema;
				if (validationEventHandler != null)
					validatingReader.ValidationEventHandler += validationEventHandler;
				do
					;
				while (validatingReader.Read());
			}
		}
	}
}