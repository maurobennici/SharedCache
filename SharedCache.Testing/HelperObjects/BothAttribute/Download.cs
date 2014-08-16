using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SharedCache.Testing.HelperObjects.BothAttribute
{
	/// <summary>
	/// This class were created to create Test to ensure 
	/// that shared cache works also if both attributes (DataContract & Serializable)
	/// are assigned to a class.
	/// </summary>
	[System.Runtime.Serialization.DataContract]
	[Serializable]
	public class Download
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public long Size { get; set; }
		[DataMember]
		public long Count { get; set; }

	}
}
