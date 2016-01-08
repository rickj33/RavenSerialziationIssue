using System;
using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using Raven.Client;
using Raven.Tests.Helpers;

using RavenJson = Raven.Imports.Newtonsoft.Json;
using NewtonsoftJson = Newtonsoft.Json;


namespace RavenSerializationIssue
{
	[TestFixture]
	public class SerializationTests : RavenTestBase
	{

		private IDocumentStore _localStore;

	//	private RavenJson.JsonSerializerSettings _ravenSettings;

		private NewtonsoftJson.JsonSerializerSettings _settings;


		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			_localStore = CreateLocalDocumentStore();
		}


		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			_localStore.Dispose();
		}


		[SetUp]
		public void Setup()
		{
			_settings = Helper.GetJsonNetSerializerSettings();
		}


		[TearDown]
		public void TearDown()
		{
		}


		[Test]
		public void SerialzationIssue()
		{
			var testObject = new TestObject();
			testObject.Init();

			//serialize to string  using newtonsoft to memic webapi request 
			var jsonString = NewtonsoftJson.JsonConvert.SerializeObject(testObject);

			//deserialize with Json.net to mimick webapi
			TestObject objectToStore = NewtonsoftJson.JsonConvert.DeserializeObject<TestObject>(jsonString, _settings);

			var count = objectToStore.NestedObjects[0].ValidationValues.Values.Count;
			testObject.ShouldBeEquivalentTo(objectToStore);

			//save the test object to the db
			var objectId = SaveToDb(objectToStore);

			TestObject loadedObject;

			//load the saved object from the db.
			using (var session1 = _localStore.OpenSession())
			{
				loadedObject = session1.Load<TestObject>(objectId);
			}

			//the expectedKeyValues should look like this {["48001","48003","48005","48007","48009","48011","48013","48015","48017","48019","48021","48023"]}
			var expectedKeyValues = objectToStore.NestedObjects[0].GetValidationValue("keys");

			var keyValues = loadedObject.NestedObjects[0].GetValidationValue("keys");

			var dog = 2;
		}


		[Test]
		public void SerialzationDictonary()
		{
			

			Dictionary<string, object> validationValues = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
			List<string> keys = new List<string>() { "48001", "48003", "48005", "48007", "48009", "48011", "48013", "48015", "48017", "48019", "48021", "48023" };
			validationValues.Add("decimalPoints", 0);
			validationValues.Add("keys", keys);



			//serialize to string  using newtonsoft to memic webapi request 
			var jsonString = NewtonsoftJson.JsonConvert.SerializeObject(validationValues);

			//deserialize with Json.net to mimick webapi
			Dictionary<string, object> objectToStore = NewtonsoftJson.JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString, _settings);

			var values = objectToStore.Values;

			validationValues.ShouldBeEquivalentTo(objectToStore);
		}




		private string SaveToDb( TestObject objectToSave )
		{
			using (var session = _localStore.OpenSession())
			{
				session.Store(objectToSave);
				session.SaveChanges();
				return objectToSave.Id;
			}
		}


		public IDocumentStore CreateLocalDocumentStore()
		{
			IDocumentStore localStore = NewDocumentStore();
			localStore.Conventions.CustomizeJsonSerializer = x =>
			{
				x.Converters.Add(new RavenJson.Converters.StringEnumConverter());
				x.ObjectCreationHandling = RavenJson.ObjectCreationHandling.Replace;
				x.TypeNameHandling = RavenJson.TypeNameHandling.None;
				x.Formatting = RavenJson.Formatting.None;
				x.ContractResolver = new RavenJson.Serialization.CamelCasePropertyNamesContractResolver();
			};
			localStore.Initialize();
			return localStore;
		}

	}
}