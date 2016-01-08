using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using RavenJson = Raven.Imports.Newtonsoft.Json;
using NewtonsoftJson = Newtonsoft.Json;


namespace RavenSerializationIssue
{
	[Serializable]
	public class TestObject
	{

		private List<NestedObject> _nestedObjects = new List<NestedObject>();


		public TestObject()
		{
		}


		public string Id { get; set; }


		[NewtonsoftJson.JsonPropertyAttribute(TypeNameHandling = NewtonsoftJson.TypeNameHandling.All, ObjectCreationHandling = NewtonsoftJson.ObjectCreationHandling.Replace)]
		[RavenJson.JsonPropertyAttribute(TypeNameHandling = RavenJson.TypeNameHandling.All, ObjectCreationHandling = RavenJson.ObjectCreationHandling.Replace)]
		public List<NestedObject> NestedObjects
		{
			get { return _nestedObjects; }
			set { _nestedObjects = value; }
		}


		public void Init()
		{
			var nestedObject = new NestedObject();

			List<string> keys = new List<string>() {"48001", "48003", "48005", "48007", "48009", "48011", "48013", "48015", "48017", "48019", "48021", "48023"};
			nestedObject.SetValidationValue("decimalPoints", 0);
			nestedObject.SetValidationValue("keys", keys);
			NestedObjects.Add(nestedObject);
		}
	}


	[Serializable]
	public class NestedObject : ISerializable
	{

		[NewtonsoftJson.JsonPropertyAttribute(TypeNameHandling = NewtonsoftJson.TypeNameHandling.All, ObjectCreationHandling = NewtonsoftJson.ObjectCreationHandling.Replace)]
		[RavenJson.JsonPropertyAttribute(TypeNameHandling = RavenJson.TypeNameHandling.All, ObjectCreationHandling = RavenJson.ObjectCreationHandling.Replace)]
		private Dictionary<string, object> _validationValues;


		public NestedObject()
		{
			_validationValues = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
		}


		[NewtonsoftJson.JsonPropertyAttribute(TypeNameHandling = NewtonsoftJson.TypeNameHandling.All, ObjectCreationHandling = NewtonsoftJson.ObjectCreationHandling.Replace)]
		[RavenJson.JsonPropertyAttribute(TypeNameHandling = RavenJson.TypeNameHandling.All, ObjectCreationHandling = RavenJson.ObjectCreationHandling.Replace)]
		public Dictionary<string, object> ValidationValues
		{
			get { return _validationValues; }
			protected set { _validationValues = value; }
		}


		public void SetValidationValue( string propertyName, object value )
		{
			_validationValues.AddItem(propertyName,value);;
		}


		public object GetValidationValue( string propertyName )
		{
			return _validationValues[propertyName];
		}


		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}
	}

	
}