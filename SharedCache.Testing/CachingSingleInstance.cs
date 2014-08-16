using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SCCACHE = SharedCache.WinServiceCommon.Provider.Cache.IndexusDistributionCache;

namespace SharedCache.Testing
{
	/// <summary>
	/// Summary description for CachingSingleInstance
	/// </summary>
	[TestFixture]
	public class CachingSingleInstance: BaseTest
	{

		#region Additional test attributes
		/// <summary>
		/// Use ClassInitialize to run code before running the first test in the class
		/// </summary>
		/// <param name="testContext"></param>
		[SetUp]
		public static void CachingSingleInstanceInitialize() 
		{
			TestServerHelper.LoadServerAsConsole("singleInstance");
			// let the server be loaded
			System.Threading.Thread.Sleep(1500);

		}
		/// <summary>
		/// Use ClassCleanup to run code after all tests in a class have run
		/// </summary>
		[TearDown]
		public static void CachingSingleInstanceCleanup() 
		{
			TestServerHelper.UnLoadServerAsConsole();
		}
		#endregion

		[Test]
		public void TestAddWithOverloads()
		{
			// create some data to add to cache.

			List<HelperObjects.SerializeAttribute.Person> data = new List<HelperObjects.SerializeAttribute.Person>()
      {
        new HelperObjects.SerializeAttribute.Person(){Salutation = "MR", FirstName = "Abcd", LastName = "Lmno", Age = 20},
        new HelperObjects.SerializeAttribute.Person(){Salutation = "MR", FirstName = "Efgh", LastName = "Pqrs", Age = 21},
        new HelperObjects.SerializeAttribute.Person(){Salutation = "MR", FirstName = "Ijkl", LastName = "tuvw", Age = 22}
      };
      // iterate over items and add to each person object an address
			foreach (var item in data)
      {
        item.Address = new List<HelperObjects.SerializeAttribute.Address>()
        {
          new HelperObjects.SerializeAttribute.Address(){ Country = "Switzerland", CountryCode = "CH", ZipCode = "8000", StreetNo = "223", Street = "Bahnhofstrasse" },
          new HelperObjects.SerializeAttribute.Address(){ Country = "United States of America", CountryCode = "US", ZipCode = "917", StreetNo = "10025", Street = "947 Amsterdam Ave" },
          new HelperObjects.SerializeAttribute.Address(){ Country = "Germany", CountryCode = "DE", ZipCode = "50000", StreetNo = "223", Street = "Gartenstrasse" }
        };
      }
			SCCACHE.SharedCache.Add("data", data);
			Expect(SCCACHE.SharedCache.GetAllKeys().Count, Is.EqualTo(1));
		    var returnedData = (List<HelperObjects.SerializeAttribute.Person>)SCCACHE.SharedCache.Get("data");
			Expect(returnedData, Is.Not.Null);
		    Expect(returnedData.ToString(), Is.EquivalentTo(data.ToString()));
		}
	}
}
