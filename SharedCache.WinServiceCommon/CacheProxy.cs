//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Xml;
//using System.Xml.Serialization;

//namespace MergeSystem.Indexus.WinServiceCommon
//{
//  [Serializable]
//  public class CacheProxy
//  {
//    public object data;
//    public string key;
//    public Enums.Action action;
//    public DateTime expires;
//    public int length;
		
//    public CacheProxy()
//    {
//      //this.action = Enums.Action.Empty;
//      this.data = null;
//      this.expires = DateTime.MaxValue;
//      this.key = string.Empty;
//    }
		
//    public CacheProxy(Enums.Action act, string key, object data, DateTime exipres)
//    {
//      this.key = key;
//      this.action = act;
//      this.data = data;
//      this.expires = exipres;
//    }		
//  }
//  [Serializable]
//  public class StatPayload
//  {
//    public long Amount = 0;
//    public long Size = 0;
//  }
	
//}
