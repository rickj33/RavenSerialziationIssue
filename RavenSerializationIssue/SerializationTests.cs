using System;
using System.Collections.Generic;
using System.Dynamic;

using FluentAssertions;

using NUnit.Framework;

using Raven.Client;
using Raven.Client.Document;
using Raven.Tests.Helpers;

using RavenJson = Raven.Imports.Newtonsoft.Json;
using NewtonsoftJson = Newtonsoft.Json;


namespace RavenSerializationIssue
{
	[TestFixture]
	public class SerializationTests : RavenTestBase
	{

		[SetUp]
		public void Setup()
		{
			_settings = Helper.GetJsonNetSerializerSettings();
		}


		[TearDown]
		public void TearDown()
		{
		}


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




		private string SaveToDb(Dog objectToSave)
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
			//IDocumentStore localStore = NewDocumentStore();
			IDocumentStore localStore = new DocumentStore() { Url = "http://localhost:8082", DefaultDatabase = "SettingsKeys", };
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


		[Test]
		public void DictionarySerialzationIssue()
		{
			var testObject = new TestObject();
			testObject.Init();

			//serialize to string  using newtonsoft to memic webapi request 
			var jsonString = NewtonsoftJson.JsonConvert.SerializeObject(testObject);

			//deserialize with Json.net to mimick webapi
			TestObject objectToStore = NewtonsoftJson.JsonConvert.DeserializeObject<TestObject>(jsonString, _settings);


			testObject.ShouldBeEquivalentTo(objectToStore);


			var dog = 2;
		}



		public class Dog
		{

			private IDictionary<string, object> _wrapper;

			public Dog()
			{
				ValidationVaues = new ExpandoObject();
				_wrapper = ValidationVaues as IDictionary<string, object>;
			}
			public string Id { get; set; }
			public string Before { get; set; }

			public dynamic ValidationVaues { get; set; }

			public string After { get; set; }

			public void SetValidationValue(string propertyName, object value)
			{
				_wrapper[propertyName] = value;
				//_breakValidationValues[propertyName] = value;
			}


		}

		[Test]
		public void SerialzationDictonary()
		{
			List<Dog> dogs = new List<Dog>();

			Dog testExpando = new Dog
			{
				Before = "Before!",
				After = "After!"
			};

		
			List<string> keys = new List<string>() {"48001", "48003", "48005", "48007", "48009", "48011", "48013", "48015", "48017", "48019", "48021", "48023"};

			testExpando.SetValidationValue("keys", keys);
			testExpando.SetValidationValue("DecimalPoints", 2);

			//serialize to string  using newtonsoft to memic webapi request 
			var jsonString = NewtonsoftJson.JsonConvert.SerializeObject(testExpando);

			//deserialize with Json.net to mimick webapi
			Dog objectToStore = NewtonsoftJson.JsonConvert.DeserializeObject<Dog>(jsonString, _settings);

			var objectId = SaveToDb(objectToStore);

			Dog loadedObject;

			//load the saved object from the db.
			using (var session1 = _localStore.OpenSession())
			{
				loadedObject = session1.Load<Dog>(objectId);
			}

			var loadedObjectJsonString = RavenJson.JsonConvert.SerializeObject(testExpando);
			var dog = 2;
		}


	/*	[Test]
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

			
		}*/

	}
}