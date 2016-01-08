using System.Collections.Generic;

using Raven.Tests.Helpers;

using RavenJson = Raven.Imports.Newtonsoft.Json;
using NewtonsoftJson = Newtonsoft.Json;


namespace RavenSerializationIssue
{
	public class Helper : RavenTestBase
	{
	
		public static RavenJson.JsonSerializerSettings GetRavenJsonNetSerializerSettings()
		{
			List<RavenJson.JsonConverter> converters = new List<RavenJson.JsonConverter>();

			converters.Add(new RavenJson.Converters.StringEnumConverter());

			return new RavenJson.JsonSerializerSettings
			{
				ObjectCreationHandling = RavenJson.ObjectCreationHandling.Replace,
				TypeNameHandling = RavenJson.TypeNameHandling.None,
				Formatting = RavenJson.Formatting.None,
				ContractResolver = new RavenJson.Serialization.CamelCasePropertyNamesContractResolver(),
				Converters = converters
			};
		}


		public static NewtonsoftJson.JsonSerializerSettings GetJsonNetSerializerSettings()
		{
			List<NewtonsoftJson.JsonConverter> converters = new List<NewtonsoftJson.JsonConverter>();
			converters.Add(new NewtonsoftJson.Converters.StringEnumConverter());
			return new NewtonsoftJson.JsonSerializerSettings
			{
				TypeNameHandling = NewtonsoftJson.TypeNameHandling.None,
				Formatting = NewtonsoftJson.Formatting.None,
				ContractResolver = new NewtonsoftJson.Serialization.CamelCasePropertyNamesContractResolver(),
				Converters = converters
			};
		}

	}
}

