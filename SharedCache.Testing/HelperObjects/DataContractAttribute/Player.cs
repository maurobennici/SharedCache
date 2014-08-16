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
	public class Player : PlayerBase
	{
		[DataMember]
		public List<int> AtTable = new List<int>();
		
		public Player()
		{
			base.KeyPrefix = "player";
		}

		[DataMember]
		public string PlayerNickName { get; set; }

		/// <summary>
		/// Name of lobby server player is connected to
		/// </summary>
		[DataMember]
		public string LoggedInLobbyServer { get; set; }

		public override string Key
		{
			get { return KeyPrefix + PlayerNickName; }

		}
	}
}
