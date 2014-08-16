using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SharedCache.Testing.HelperObjects.DataContractAttribute
{
	/// <summary>
	/// Sample class to test all DataContract classes
	/// </summary>
	[System.Runtime.Serialization.DataContract]
	public abstract class PlayerBase
	{
		private string keyPrefix = "base";

		[DataMember]
		public virtual string Target { get; set; }

		[DataMember]
		public virtual string Key
		{
			get { return KeyPrefix + new Guid(); }
		}
		
		[DataMember]
		public virtual string KeyPrefix
		{
			get { return keyPrefix + "_"; }
			protected set { keyPrefix = value; }
		}
	}
}
