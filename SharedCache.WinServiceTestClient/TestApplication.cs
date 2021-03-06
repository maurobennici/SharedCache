#region Copyright (c) Roni Schuetz - All Rights Reserved
// * --------------------------------------------------------------------- *
// *                              Roni Schuetz                             *
// *              Copyright (c) 2008 All Rights reserved                   *
// *                                                                       *
// * Shared Cache high-performance, distributed caching and    *
// * replicated caching system, generic in nature, but intended to         *
// * speeding up dynamic web and / or win applications by alleviating      *
// * database load.                                                        *
// *                                                                       *
// * This Software is written by Roni Schuetz (schuetz AT gmail DOT com)   *
// *                                                                       *
// * This library is free software; you can redistribute it and/or         *
// * modify it under the terms of the GNU Lesser General Public License    *
// * as published by the Free Software Foundation; either version 2.1      *
// * of the License, or (at your option) any later version.                *
// *                                                                       *
// * This library is distributed in the hope that it will be useful,       *
// * but WITHOUT ANY WARRANTY; without even the implied warranty of        *
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU      *
// * Lesser General Public License for more details.                       *
// *                                                                       *
// * You should have received a copy of the GNU Lesser General Public      *
// * License along with this library; if not, write to the Free            *
// * Software Foundation, Inc., 59 Temple Place, Suite 330,                *
// * Boston, MA 02111-1307 USA                                             *
// *                                                                       *
// *       THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.        *
// * --------------------------------------------------------------------- *
#endregion 

// *************************************************************************
//
// Name:      TestApplication.cs
// 
// Created:   01-01-2008 SharedCache.com, rschuetz
// Modified:  01-01-2008 SharedCache.com, rschuetz : Creation
// Modified:  12-02-2008 SharedCache.com, rschuetz : added test no 410
// Modified:  12-02-2008 SharedCache.com, rschuetz : added test option 800
// Modified:  12-02-2008 SharedCache.com, rschuetz : added test option no 540
// Modified:  29-02-2008 SharedCache.com, rschuetz : extended testing cases
// ************************************************************************* 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using COM = SharedCache.WinServiceCommon;

namespace SharedCache.WinServiceTestClient
{
	/// <summary>
	/// This is a test application to demonstarate how to use Shared Cache.
	/// </summary>
	public class TestApplication
	{

		private string lastSelectMenuOption = string.Empty;

		/// <summary>
		/// defines object size
		/// </summary>
		public enum ObjectSize
		{
			/// <summary>
			/// 1 kb
			/// </summary>
			One,
			/// <summary>
			/// 100 kb
			/// </summary>
			Hundert,
			/// <summary>
			/// 1 mb
			/// </summary>
			Thousend
		}

		public void PrintSettings()
		{
			Console.WriteLine(@"CompressionEnabled: " + COM.Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.CompressionEnabled);
			Console.WriteLine(@"CompressionMinSize: " + COM.Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.CompressionMinSize);
			Console.WriteLine(@"HashingAlgorithm: " + COM.Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.HashingAlgorithm);
			Console.WriteLine(@"LoggingEnable: " + COM.Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.LoggingEnable);
			Console.WriteLine(@"SharedCacheVersionNumber: " + COM.Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.SharedCacheVersionNumber);
			Console.WriteLine(@"SocketPoolMinAvailableSize: " + COM.Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.SocketPoolMinAvailableSize);			
		}

		/// <summary>
		/// Starts the test applicaiton
		/// </summary>
		public void Start()
		{
			Console.WriteLine(@"Welcome - This is Shared Cache test application.");
			
			bool doBreak = true;

			do
			{
				Menu.PrintMenu();
				Console.WriteLine();
				Console.Write("Enter your choice and press enter: ");

				try
				{
					#region switch
					switch (Console.ReadLine())
					{
						case "":
							{
								Console.Clear();
								break;
							}
						case "0":
							{
								lastSelectMenuOption = "0";
								Console.Clear();
								break;
							}
							case "1":
							{
								lastSelectMenuOption = "1";
								UserData();
								break;
							}
						case "5":
							{
								lastSelectMenuOption = "5";
								PrintSettings();
								break;
							}
							
						case "9":
							{
								lastSelectMenuOption = "9";
								doBreak = false;
								break;
							}
						case "100":
							{
								lastSelectMenuOption = "100";
								this.TestCountryMethod(true, true);
								break;
							}
						case "110":
							{
								lastSelectMenuOption = "110";
								this.TestCountryMethod(false, true);
								break;
							}
						case "120":
							{
								lastSelectMenuOption = "120";
								this.TestCountryMethod(false, false);
								break;
							}
						case "130":
							{
								lastSelectMenuOption = "130";
								this.TestCountryMethodRandomize(false);
								break;
							}
						case "200":
							{
								lastSelectMenuOption = "200";
								UserData();
								break;
							}						
						case "300":
							{
								lastSelectMenuOption = "300";
								MultiOperation(ObjectSize.Hundert, 100);								
								break;
							}
						case "310":
							{
								lastSelectMenuOption = "310";
								RegExTestCase();
								break;
							}
						case "400":
							{
								lastSelectMenuOption = "400";
								CompareComressionUsage();
								break;
							}
						case "410":
							{
								ConCurrentClient client = new ConCurrentClient();
								Thread thread = new Thread(client.DoIt);
								thread.Start();
								break;
							}
						case "420":
							{
								CallDataWhichIsNotAvailable();
								break;
							}
						case "500":
							{
								lastSelectMenuOption = "500";
								Console.Clear();
								Console.WriteLine();
								Console.WriteLine();
								// we provide her null while upon testing normally just one server is available
								// TODO: in future it will search within the provider for all servers and send the request over each defined server 
								Console.WriteLine(Common.Util.CacheGetStats());
								Console.WriteLine();
								Console.WriteLine(@"Operation Done");
								Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
								Console.WriteLine();
								Console.WriteLine(@"Press enter to go on.");
								Console.ReadLine();

								Console.Clear();
								break;
							}
						case "510":
							{
								lastSelectMenuOption = "510";
								this.RetrieveRandomizeKey(100);
								break;
							}
						case "520":
							{
								lastSelectMenuOption = "520";
								Console.Clear();
								Console.WriteLine(@"A list with all available Key's");
								Console.WriteLine();
								// we provide her null while upon testing normally just one server is available
								// TODO: in future it will search within the provider for all servers and send the request over each defined server 
								List<string> keys = Common.Util.CacheGetAllKeys();
								keys.Sort();
								foreach (string key in keys)
								{
									Console.WriteLine(@"Key: {0}", key.ToString());
								}
								Console.WriteLine(@"Total amount of items in sharedcache: {0}", keys.Count.ToString());
								Console.WriteLine();
								Console.WriteLine(@"Operation Done");
								Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
								Console.WriteLine();
								Console.WriteLine(@"Press enter to go on.");
								Console.ReadLine();

								Console.Clear();
								break;
							}
						case "530":
							{
								lastSelectMenuOption = "530";
								Console.Clear();
								Console.WriteLine(@"Clear Cache");
								Console.WriteLine();
								// clear cache
								Common.Util.CacheClear();
								Console.WriteLine();
								Console.WriteLine(@"Operation Done");
								Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
								Console.WriteLine();
								Console.WriteLine(@"Press enter to go on.");
								Console.ReadLine();

								Console.Clear();
								break;
							}
						case "540":
							{
								lastSelectMenuOption = "540";
								CheckHitRatio();
								break;
							}
						case "550":
							{
								lastSelectMenuOption = "550";
								CheckKeyWithSpecialCharset();
								break;
							}
						case "560":
							{
								lastSelectMenuOption = "560";
								AddSimpleTypeToCache();
								break;
							}
						case "570":
							{
								lastSelectMenuOption = "570";
								TestStatsAndAllKeys();
								break;
							}
						case "580":
							{
								lastSelectMenuOption = "580";
								CheckAbsolutExpiraitonTime();
								break;
							}
						case "600":
							{
								lastSelectMenuOption = "600";
								CompareObjectWithAndWithoutList(ObjectSize.Hundert, 100);
								break;
							}
						case "610":
							{
								lastSelectMenuOption = "610";
								CompareObjectWithAndWithoutList(ObjectSize.Hundert, 1000);
								break;
							}
						case "700":
							{
								lastSelectMenuOption = "700";
								AddObjectsWithOffset(ObjectSize.Hundert, 1);
								break;
							}
						case "710":
							{
								lastSelectMenuOption = "710";
								ExtendItemTtl();
								break;
							}
						case "800":
							{
								lastSelectMenuOption = "800";
								LongRunTest();
								break;
							}
						case "900":
							{
								lastSelectMenuOption = "900";
								PerformanceTestCompare();
								break;
							}
						case "1000":
							{
								lastSelectMenuOption = "1000";
								DataContractTester();
								break;
							}
						
					}
					#endregion switch
				}
				catch (Exception ex)
				{
					Console.WriteLine(@"An exception appears [feel free to contact us at: sharedcache@sharedcache.com]" + Environment.NewLine + ex.Message);
				}
			} while (doBreak);
		}

		private void DataContractTester()
		{
			Console.Clear();
			System.Diagnostics.Stopwatch sp = new System.Diagnostics.Stopwatch();
			Common.Util.CacheClear();
			sp.Start();
			WcfClient client = new WcfClient { ClientIp = "home ip", ClientName = "client name" };
			try
			{
				Console.WriteLine("This method tests WCF overloads within Client API.");
				Console.WriteLine("Test: public override void WcfAdd(string key, object payload)");

				Common.Util.CacheAddWcf("test", client);
				WcfClient c = Common.Util.CacheGetWcf<WcfClient>("test");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			
			sp.Stop();
			//Console.WriteLine("Adding object with size: {0} bytes and exiry {1}. Needed Time: {2} ms",
			//  obj.byteArray.Length.ToString(),
			//  DateTime.Now.AddMinutes(10).ToString("dd.MMM.yyyy hh:mm"),
			//  sp.ElapsedMilliseconds);

		}

		private void ExtendItemTtl()
		{
			Console.Clear();
			System.Diagnostics.Stopwatch sp = new System.Diagnostics.Stopwatch();
			Common.Util.CacheClear();
			sp.Start();
			TestSizeObject obj = new TestSizeObject(ObjectSize.Thousend);
			Common.Util.CacheAdd("ExtendItemTtl", obj, DateTime.Now.AddMinutes(10));
			sp.Stop();
			Console.WriteLine("Adding object with size: {0} bytes and exiry {1}. Needed Time: {2} ms",
				obj.byteArray.Length.ToString(), 
				DateTime.Now.AddMinutes(10).ToString("dd.MMM.yyyy hh:mm"),
				sp.ElapsedMilliseconds);

			sp.Reset();
			sp.Start();
			Console.WriteLine("Get Object, set new expiry DateTime [{0}] and send back to server", DateTime.Now.AddMinutes(30).ToString("dd.MMM.yyyy hh:mm"));
			TestSizeObject objFromCache = Common.Util.CacheGet<TestSizeObject>("ExtendItemTtl");
			Common.Util.CacheAdd("ExtendItemTtl", objFromCache, DateTime.Now.AddMinutes(30));
			sp.Stop();
			Console.WriteLine("Needed time for add / set: {0}ms", sp.ElapsedMilliseconds);
			sp.Reset();
			sp.Start();
			Common.Util.CacheExtendTtl("ExtendItemTtl", DateTime.Now.AddMinutes(45));
			sp.Stop();
			Console.WriteLine("Needed time for add / set: {0}ms", sp.ElapsedMilliseconds);
		}

		private void RegExTestCase()
		{
			Console.WriteLine("Regular Expression");
			Console.Clear();
			Common.Util.CacheClear();

			List<string> result = new List<string>();
			List<string> keys = new List<string>();
			
			for (int i = 0; i < 100; i++)
			{
				keys.Add(string.Format(@"TestEmp:Mgr:{0}", i.ToString()));
				keys.Add(string.Format(@"TopMgr:Mgr:{0}", i.ToString()));
			}
			
			Dictionary<string,TestSizeObject> data = new Dictionary<string,TestSizeObject>();
			foreach(string n in keys)
			{
				data.Add(n, new TestSizeObject(ObjectSize.Hundert));
			}

			System.Diagnostics.Stopwatch sp = new System.Diagnostics.Stopwatch();
			int t = 0;
			Console.WriteLine(@"Adding {0} items to cache.", keys.Count);
			sp.Start();			
			foreach(KeyValuePair<string, TestSizeObject> item in data)
			{
				if((++t % 20) == 0)
					Console.Write(". ");
				Common.Util.CacheAdd(item.Key, item.Value);	
			}
			sp.Stop();
			Console.WriteLine();
			Console.WriteLine(@"Added {0} items to cache in {1} ms", keys.Count, sp.ElapsedMilliseconds);
			Console.WriteLine(@"Current amount of items in Cache: {0}", Common.Util.CacheGetAllKeys().Count);

			sp.Start();
			List<string> getAllKeys = Common.Util.CacheGetAllKeys();
			sp.Stop();

			Console.WriteLine();
			Console.WriteLine("Needed time to make call GetAllKeys {0} ms", sp.ElapsedMilliseconds);
			Console.WriteLine();

			string regularExpressionToExecute = "Top*.*:Mgr:*.";
			sp.Start();			
			Common.Util.CacheRegexRemove(regularExpressionToExecute);
			sp.Stop();
			
			Console.WriteLine(@"Removed {0} items from cache in {1} ms", keys.Count - Common.Util.CacheGetAllKeys().Count, sp.ElapsedMilliseconds);

			regularExpressionToExecute = "Test*.*:Mgr:*.";
			sp.Start();
			IDictionary<string, byte[]> getRes = Common.Util.CacheRegexGet(regularExpressionToExecute);
			sp.Stop();
			Console.WriteLine();
			Console.WriteLine("Received {0} items from cache in {1} ms", getRes.Count, sp.ElapsedMilliseconds);
			Console.WriteLine();

			int tt = 0;

			foreach (KeyValuePair<string, byte[]> item in getRes)
			{
				TestSizeObject tso = COM.Formatters.Serialization.BinaryDeSerialize<TestSizeObject>(item.Value);
				if (tso == null)
				{
					++tt;
				}
			}
			Console.WriteLine();
			Console.WriteLine(@"A total of {0} items were NULL in the result", tt);
			Console.WriteLine();

			Console.WriteLine();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();

			Console.Clear();
		}

		private void MultiOperation(ObjectSize objectSize, int p)
		{
			Console.Clear();

			Console.WriteLine(@"Adding {0} items to cache with apporx. {1} kb. Please ensure you have defined more then 1 server node!!", p, objectSize.ToString());
			List<string> keys = new List<string>();
			string keyPrefix = string.Format("ThreadID-{0}-MultiGet:", System.Threading.Thread.CurrentThread.ManagedThreadId);
			Dictionary<string, byte[]> multiDataToAdd = new Dictionary<string, byte[]>();
			for (int i = 0; i < p; i++)
			{
				if(i%10 == 0)
					Console.Write(". ");
				multiDataToAdd.Add(keyPrefix + i.ToString(), 
					COM.Formatters.Serialization.BinarySerialize(
						new TestSizeObject(objectSize)
						)
					);

				keys.Add(keyPrefix + i.ToString());
			}
			Console.WriteLine();
			System.Diagnostics.Stopwatch sp = new System.Diagnostics.Stopwatch();
			sp.Start();
			Common.Util.CacheMultiAdd(multiDataToAdd);
			sp.Stop();
			Console.WriteLine(@"Multi Add: {0} ms", sp.ElapsedMilliseconds);
			sp.Start();
			IDictionary<string, byte[]> data = Common.Util.CacheMultiGet(keys);
			sp.Stop();

			Console.WriteLine(@"Multi Get: {0} ms - received items: {1}", sp.ElapsedMilliseconds, data != null ? data.Count.ToString() : "0");

			#region Commented
			
			//if (data != null)
			//{
			//  Console.WriteLine("Total amount of received items: {0}", data.Count);
			//  foreach (KeyValuePair<string, byte[]> item in data)
			//  {
			//    Console.WriteLine("Item with key {0} was available", item.Key);
			//  }
			//}
			//else
			//{
			//  Console.WriteLine("no data received !!!");
			//}
			#endregion
			sp.Start();
			Common.Util.CacheMultiDelete(keys);
			sp.Stop();
			Console.WriteLine(@"Multi Del: {0} ms", sp.ElapsedMilliseconds);

			Console.WriteLine();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();

			Console.Clear();
		}

		private void PerformanceTestCompare()
		{
			Console.Clear();
			Common.Reporting report = new Common.Reporting();
			report.ReportingOption = new List<Common.ReportingOption>();
			report.RunDateTime = DateTime.Now;
			report.VersionNumber = COM.Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.SharedCacheVersionNumber;
			int loops = 250;
			ObjectSize size;

			for (int d = 0; d < 25; d++)
			{
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine(@"run {0} of {1}",d+1,25);
				Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
				Console.WriteLine();
				for (int i = 0; i < 3; i++)
				{
					Common.ReportingOption option = new Common.ReportingOption();
					#region Set object size
					switch (i)
					{
						case 1:
							size = ObjectSize.Hundert;
							break;
						case 2:
							size = ObjectSize.Thousend;
							break;
						default:
							size = ObjectSize.One;
							break;
					}
					#endregion

					#region Option details
					option.RunDateTime = report.RunDateTime;
					option.HashingAlgorithm = COM.Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.HashingAlgorithm.ToString();
					option.LoggingEnabled = COM.Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.LoggingEnable == 1 ? true : false;
					option.OneThread = true;
					option.Option = string.Format("{0}-{1}", loops, size.ToString());
					option.ZipEnabled = COM.Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.CompressionEnabled == 1 ? true : false;
					option.CompressionMinSize = COM.Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.CompressionMinSize;
					#endregion Option details

					Console.WriteLine("init {0} objects with a size of {1}", loops, size.ToString());
					#region init
					List<TestSizeObject> testObjectList = new List<TestSizeObject>();
					for (int k = 0; k < loops; ++k)
					{
						testObjectList.Add(new TestSizeObject(size));
					}
					#endregion init

					Console.WriteLine("Add objects");
					#region Adding objects
					DateTime start = DateTime.Now;
					foreach (TestSizeObject o in testObjectList)
					{
						Common.Util.CacheAdd(o.Id, o);
					}
					TimeSpan span = DateTime.Now.Subtract(start);
					option.NeededAddTime = (long)span.TotalMilliseconds;
					
					// free up memory
					testObjectList = null;

					Console.WriteLine(@"needed {0} ms", option.NeededAddTime);
					#endregion Adding objects

					List<string> keyList = Common.Util.CacheGetAllKeys();

					Console.WriteLine("Get objects");
					#region Get objects					
					int success = 0;
					int failed = 0;
					TestSizeObject o2 = null;
					start = DateTime.Now;
					foreach (string key in keyList)
					{
						o2 = Common.Util.CacheGet<TestSizeObject>(key);
						if (o2 != null)
							++success;
						else
							++failed;
						o2 = null;
					}
					span = DateTime.Now.Subtract(start);
					option.NeededGetTime = (long)span.TotalMilliseconds;
					Console.WriteLine(@"needed {0} ms", option.NeededGetTime);
					#endregion Get objects

					Console.WriteLine("Remove objects");
					#region Remove objects
					start = DateTime.Now;
					foreach (string key in keyList)
					{
						Common.Util.CacheRemove(key);
					}
					span = DateTime.Now.Subtract(start);
					option.NeededDelTime = (long)span.TotalMilliseconds;
					Console.WriteLine(@"needed {0} ms", option.NeededDelTime);
					#endregion Remove objects
					
					report.ReportingOption.Add(option);
				}					
			}
			Console.WriteLine("Save run to disc.");
			BLL.BllReporting.Save(report);
			

			Console.WriteLine();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
		}
		
		private void TestStatsAndAllKeys()
		{
			Console.Clear();
			Console.WriteLine();
			Console.WriteLine(@"Clear Cache");
			Console.WriteLine();
			Common.Util.CacheClear();
			Console.WriteLine("Add and Receive some data");
			Console.WriteLine("This test makes only sense if you use more then shared cache server server.");

			Common.Util.CacheAdd("indeXus.NetSharedCache1", new TestSizeObject(ObjectSize.One));
			Common.Util.CacheAdd("indeXus.NetSharedCache2", new TestSizeObject(ObjectSize.One));

			Common.Util.CacheGet<TestSizeObject>("indeXus.NetSharedCache1");
			Common.Util.CacheGet<TestSizeObject>("indeXus.NetSharedCache2");

			COM.IndexusStatistic result1 = Common.Util.CacheGetStats();

			Console.WriteLine(result1.ToString());
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();
			
			Console.Clear();
			COM.IndexusStatistic result2 = Common.Util.CacheGetStats("127.0.0.1");
			Console.WriteLine(result1.ToString());
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();

			Console.Clear();
			List<string> result3 = Common.Util.CacheGetAllKeys();
			result3.ForEach(delegate(string s) { Console.WriteLine(s); });
			// people.ForEach(delegate(Person p) { Console.WriteLine(String.Format("{0} {1}", p.age, p.name)); });
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();

			Console.Clear();
			List<string> result4 = Common.Util.CacheGetAllKeys("127.0.0.1");
			result4.ForEach(delegate(string s) { Console.WriteLine(s); });
			Console.WriteLine();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();

		}

		private void AddSimpleTypeToCache()
		{
			Console.Clear();
			Console.WriteLine();
			Console.WriteLine(@"Clear Cache");
			Console.WriteLine();
			Common.Util.CacheClear();

			Console.WriteLine("This add primitive types to cache like byte / byte[] / int / bool!");
			Console.WriteLine();
			Console.WriteLine();

			string keyInt = @"keyInt";
			string keyByte = @"keyByte";
			string keyShort = @"keyShort";
			string keyDateTime = @"keyDateTime";
			string keyString = @"keyString";
			string keyLong = @"keyLong";
			string keyByteArray = @"keyByteArray";

			int valueInt = 7;
			Console.WriteLine("Handling int value ({0})", valueInt);
			Common.Util.CacheAdd(keyInt, valueInt);
			int? keyIntValue = Common.Util.CacheGet<int>(keyInt);
			if(keyIntValue == null)
				Console.WriteLine("could not read '{0}' from cache!!!!", valueInt);

			Console.WriteLine();
			byte valueByte = new byte[1] { 6 }[0];
			Console.WriteLine("Handling byte value ({0})", valueByte);
			Common.Util.CacheAdd(keyByte, valueByte);
			byte? keyByteValue = Common.Util.CacheGet<byte>(keyByte);
			if (keyByteValue == null)
				Console.WriteLine("could not read '{0}' from cache!!!!", valueByte);

			Console.WriteLine();
			short valueShort = 99;
			Console.WriteLine("Handling short value ({0})", valueShort);
			Common.Util.CacheAdd(keyShort, valueShort);
			short? keyShortValue = Common.Util.CacheGet<short>(keyShort);
			if (keyShortValue == null)
				Console.WriteLine("could not read '{0}' from cache!!!!", valueShort);

			Console.WriteLine();
			DateTime valueDateTime = DateTime.Now.AddYears(15);
			Console.WriteLine("Handling DateTime value ({0})", valueDateTime);
			Common.Util.CacheAdd(keyDateTime, valueDateTime);
			DateTime? keyDateTimeValue = Common.Util.CacheGet<DateTime>(keyDateTime);
			if (keyDateTimeValue == null)
				Console.WriteLine("could not read '{0}' from cache!!!!", valueDateTime);

			Console.WriteLine();
			string valueString = "Shared Cache";
			Console.WriteLine("Handling string value ({0})", valueString);
			Common.Util.CacheAdd(keyString, valueString);
			string keyStringValue = Common.Util.CacheGet<string>(keyString);
			if (keyStringValue == null)
				Console.WriteLine("could not read '{0}' from cache!!!!", valueString);

			Console.WriteLine();
			long valueLong = 9999999999;
			Console.WriteLine("Handling byte value ({0})", valueLong);
			Common.Util.CacheAdd(keyLong, valueLong);
			long? keyLongValue = Common.Util.CacheGet<long>(keyLong);
			if (keyLongValue == null)
				Console.WriteLine("could not read '{0}' from cache!!!!", valueLong);

			Console.WriteLine();
			string dataToSend = "Shared Cache as byte array";
			byte[] valueByteArray = System.Text.Encoding.UTF8.GetBytes(dataToSend);
			Console.WriteLine("Handling byte value ({0})", valueByteArray);
			Common.Util.CacheAdd(keyByteArray, valueByteArray);
			byte[] keyByteArrayValue = Common.Util.CacheGet<byte[]>(keyByteArray);
			if (keyByteArrayValue == null)
			{
				Console.WriteLine("could not read '{0}' from cache!!!!", valueByteArray);
			}
			else
			{
				Console.WriteLine("Sent and Received Data:\nS:'{0}'\nR:'{1}'", dataToSend, System.Text.Encoding.UTF8.GetString(keyByteArrayValue));
			}

			Console.WriteLine();
			Console.WriteLine();
			Common.Util.CacheClear();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();
			Console.Clear();
		}

		private void CheckKeyWithSpecialCharset()
		{
			Console.Clear();
			Console.WriteLine();
			Console.WriteLine(@"Clear Cache");
			Console.WriteLine();
			Common.Util.CacheClear();

			string key1 = "用户数据:001";
			string key2 = "שלום";
			
			Console.WriteLine();
			Console.WriteLine("This test uses special charsets as key's!");
			Console.WriteLine(@"Key 1: {0} (free translation - user data)", key1);
			Console.WriteLine(@"Key 2: {0} (free translation - hello)", key2);
			Console.WriteLine();
			Console.WriteLine();
			Common.Util.CacheAdd(key1, new TestSizeObject(ObjectSize.One));
			Common.Util.CacheAdd(key2, new TestSizeObject(ObjectSize.One));
			Console.WriteLine();
			Console.WriteLine();
			TestSizeObject obj = Common.Util.CacheGet<TestSizeObject>(key1);
			if (obj == null)
			{
				Console.WriteLine(@"Could not receive data for key: {0}", key1);
			}
			else
			{
				Console.WriteLine("Cache Key {0} could received successfully from cache.", key1);
			}
			Console.WriteLine();
			Console.WriteLine();
			obj = Common.Util.CacheGet<TestSizeObject>(key2);
			if (obj == null)
			{
				Console.WriteLine(@"Could not receive data for key: {0}", key2);
			}
			else
			{
				Console.WriteLine("Cache Key {0} could received successfully from cache.", key2);
			}
			Console.WriteLine();
			Console.WriteLine();
			Common.Util.CacheClear();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();
			Console.Clear();
		}


		private void CheckAbsolutExpiraitonTime()
		{
			Console.Clear();
			Console.WriteLine();
			List<TestSizeObject> data = new List<TestSizeObject>();
			Console.WriteLine("Creating 30 objects.");
			for (int i = 0; i < 30; i++)
			{
				data.Add(new TestSizeObject(ObjectSize.One));
			}

			DateTime dt = DateTime.Now.AddMinutes(3);
			List<string> keys = new List<string>();
			for (int i = 0; i < data.Count; i++)
			{
				string key = "CheckAbsolutExpiraitonTime_" + i;
				keys.Add(key);
				Common.Util.CacheAdd(key, data[i], dt);
				dt = dt.AddMinutes(1);
			}

			IDictionary<string, DateTime> res = Common.Util.GetAbsolutExpireDateTime(keys);

			foreach (KeyValuePair<string, DateTime> item in res)
			{
				Console.WriteLine("Key: {0} expires at: {1}", item.Key, item.Value.ToString("hh:mm:ss"));	 
			}
			

			Console.WriteLine();
			Console.WriteLine();
			Common.Util.CacheClear();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();
			Console.Clear();
		}

		private void CheckHitRatio()
		{
			Console.Clear();
			Console.WriteLine(@"Consider this test takes several minutes!!!!");
			Console.WriteLine();
			Console.WriteLine(@"Clear Cache");
			Console.WriteLine();
			Common.Util.CacheClear();

			Console.WriteLine("Fill Cache with some information");			
			int loop = new Random().Next(800, 2000);
			Console.WriteLine(@"Adding {0} Objects - inclusive allocation", loop);

			for (int i = 0; i < loop; ++i)
			{
				Console.Write(". ");
				int r = new Random().Next(1, 4);
				Thread.Sleep(r * 100);
				switch (r)
				{
					case 1:
						Common.Util.CacheAdd(@"1_A" + i.ToString(), new TestSizeObject(ObjectSize.One));
						break;
					case 2:
						Common.Util.CacheAdd(@"100_A" + i.ToString(), new TestSizeObject(ObjectSize.Hundert));
						break;
					case 3:
						Common.Util.CacheAdd(@"1000_A" + i.ToString(), new TestSizeObject(ObjectSize.Thousend));
						break;
					default:
						break;
				}
			}

			List<string> keys = Common.Util.CacheGetAllKeys();
			int removeCount = 0;

			Console.WriteLine();
			Console.WriteLine("Delete randomized keys from cache - contains a sleep time between 0.5 - 0.8 sec.");

			for (int i = 0; i < keys.Count; i++)
			{
				Console.Write(". ");
				int d = new Random().Next(4, 8);
				if (i % d == 0)
				{
					++removeCount;
					Common.Util.CacheRemove(keys[i]);
				}
				int r = new Random().Next(1, 10);
				Thread.Sleep(r);
			}
			Console.WriteLine();
			Console.WriteLine(@"removed {0} keys", removeCount);
			Thread.Sleep(500);
			removeCount = 0;
			Console.WriteLine();
			Console.WriteLine(@"Start to receive key's");
			foreach (string key in keys)
			{
				Console.Write(". ");
				int r = new Random().Next(20, 30);
				Thread.Sleep(r);

				TestSizeObject t = Common.Util.CacheGet<TestSizeObject>(key);
				if (t == null)
				{
					++removeCount;
				}
			}
			Console.WriteLine();
			Console.WriteLine(@"Totally the system could not receive {0} items from cache.", removeCount);
			Console.WriteLine();
			Console.WriteLine(@"Hit Ratio: {0}%", Common.Util.CacheGetStats().GetHitRatio);
			Console.WriteLine();
			Console.WriteLine(@"Operation Done - call option 500 to receive detaild stats");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();
			Console.Clear();
		}

		private void LongRunTest()
		{
			Console.Clear();
			Console.WriteLine(@"Consider this test takes several minutes!!!!");
			Console.WriteLine();
			Console.WriteLine(@"Clear Cache");
			Console.WriteLine();
			Common.Util.CacheClear();
			
			Console.WriteLine("Fill Cache with some information");
			Console.WriteLine(@"Adding 1000 Objects - inclusive allocation");
			for (int i = 0; i < 1000; ++i)
			{
				Console.Write(". ");
				int r = new Random().Next(1,4);
				Thread.Sleep(r * 10);
				switch (r)
				{
					case 1:
						Common.Util.CacheAdd(@"1_A" + "_" + System.Diagnostics.Process.GetCurrentProcess().Id + i.ToString(), new TestSizeObject(ObjectSize.One));
						break;
					case 2:
						Common.Util.CacheAdd(@"100_A" + "_" + System.Diagnostics.Process.GetCurrentProcess().Id + i.ToString(), new TestSizeObject(ObjectSize.Hundert));
						break;
					case 3:
						Common.Util.CacheAdd(@"1000_A" + "_" + System.Diagnostics.Process.GetCurrentProcess().Id + i.ToString(), new TestSizeObject(ObjectSize.Thousend));
						break;
					default:
						break;
				}				
			}

			Console.WriteLine(@"Simulate single user with wait perioud of 0.5 - 1 sec.");

			List<string> keys = Common.Util.CacheGetAllKeys();
			foreach (string key in keys)
			{
				Console.Write(". ");
				int r = new Random().Next(500, 1000);
				Thread.Sleep(r);

				TestSizeObject t = Common.Util.CacheGet<TestSizeObject>(key);
				if (t == null)
				{
					throw new Exception("Object was not available in cache therefore test aborted!!");
				}
			}
			
			Console.WriteLine("Delete some keys from the cache also with sleep times between 0.5 - 0.8 sec.");

			for (int i = 0; i < keys.Count; i++)
			{
				Console.Write(". ");
				int d = new Random().Next(4, 8);
				if (i % d == 0)
				{
					Common.Util.CacheRemove(keys[i]);
				}

				int r = new Random().Next(500, 800);
				Thread.Sleep(r);
			}

			Console.WriteLine("Simulate several clients");
			ConCurrentClient client = new ConCurrentClient();
			Thread thread = new Thread(client.DoIt);
			thread.Start();
			Common.Util.CacheClear();
			
		}

		private void CallDataWhichIsNotAvailable()
		{
			Console.Clear();
			Console.WriteLine(@"Consider to set for this test the server app.config value of: ServiceCacheCleanupThreadJob - 30000 or even less");
			Console.WriteLine();
			Console.WriteLine(@"Clear Cache");
			Console.WriteLine();
			Common.Util.CacheClear();
			System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
			st.Start();
			Console.WriteLine(@"Adding 1000 Objects - inclusive allocation");
			for (int i = 0; i < 1000; ++i)
			{
				Common.Util.CacheAdd(@"A" + i.ToString(), new TestSizeObject(ObjectSize.One));
			}
			st.Stop();
			Console.WriteLine(@"{0}s {1}ms", st.Elapsed.Seconds, st.Elapsed.Milliseconds);
			
			Console.WriteLine(@"Receive 1000 objects which are not available");
			st.Start();
			for (int i = 0; i < 1000; ++i)
			{
				TestSizeObject t = Common.Util.CacheGet<TestSizeObject>(@"G" + i.ToString());
			}
			st.Stop();
			Console.WriteLine(@"{0}s {1}ms", st.Elapsed.Seconds, st.Elapsed.Milliseconds);
			Console.WriteLine(@"Clear Cache");
			Console.WriteLine();
			Common.Util.CacheClear();
			Console.WriteLine(@"Adding 100 Objects - inclusive allocation");
			st.Start();
			for (int i = 0; i < 100; ++i)
			{
				Common.Util.CacheAdd(@"A2" + i.ToString(), new TestSizeObject(ObjectSize.One));
			}
			st.Stop();
			Console.WriteLine(@"{0}s {1}ms", st.Elapsed.Seconds, st.Elapsed.Milliseconds);

			Console.WriteLine(@"Receive 1000 objects which are not available");
			st.Start();
			for (int i = 0; i < 1000; ++i)
			{
				TestSizeObject t = Common.Util.CacheGet<TestSizeObject>(@"G" + i.ToString());
			}
			st.Stop();
			Console.WriteLine(@"{0}s {1}ms", st.Elapsed.Seconds, st.Elapsed.Milliseconds);
		}

		private void AddObjectsWithOffset(ObjectSize size, int p)
		{
			Console.Clear();
			Console.WriteLine(@"Consider to set for this test the server app.config value of: ServiceCacheCleanupThreadJob - 30000 or even less");
			Console.WriteLine();
			Console.WriteLine(@"Clear Cache");
			Console.WriteLine();
			Common.Util.CacheClear();

			Console.WriteLine(@"Adding one object without any expiry date (takes per default DateTime.MaxValue)");

			Common.Util.CacheAdd(@"StaticObject", new TestSizeObject(size));

			for (int i = 0; i < 10; ++i)
			{
				DateTime dt = DateTime.Now.AddMinutes(p);
				Common.Util.CacheAdd(i.ToString(), new TestSizeObject(size), dt);
				Console.Clear();
				Console.WriteLine(@"Added object with an expiry datetime: {0:u}", dt);
				Console.WriteLine(@"Wait 20 seconds.");
				Console.WriteLine(Common.Util.CacheGetStats());
				System.Threading.Thread.Sleep(20000);				
			}
			Console.WriteLine(@"Watch log files or Server Console, the Objects get deleted within the expiry date.");
			for (int i = 0; i < 5; ++i)
			{
				Console.WriteLine(@"Wait 20 seconds.");
				Console.WriteLine(Common.Util.CacheGetStats());
				System.Threading.Thread.Sleep(20000);
			}
			
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();
			Console.Clear();
		}

		private void CompareObjectWithAndWithoutList(ObjectSize size, int loops)
		{
			Console.Clear();
			Console.WriteLine(@"Clear Cache");
			Console.WriteLine();
			Common.Util.CacheClear();

			string result1 = string.Empty;
			string result2 = string.Empty;
			string result3 = string.Empty;

			Console.WriteLine(@"Step 1: The clients loads {0} objects with an appox. size of {1} kb", loops, size.ToString());

			List<string> without = new List<string>();
			List<string> with = new List<string>();

			List<TestSizeObject> testObjectList = new List<TestSizeObject>();
			List<TestSizeObjectWithList> testObjectListWithList = new List<TestSizeObjectWithList>();

			for (int i = 0; i < loops; ++i)
			{
				testObjectList.Add(new TestSizeObject(size));
				testObjectListWithList.Add(new TestSizeObjectWithList(size));
			}
			Console.WriteLine(@"Client is done to load the data");

			Console.WriteLine("Object Comparsion: \nwithout: {0, 7} bytes \nwith: {1, 10} bytes", 
					COM.Formatters.Serialization.BinarySerialize(testObjectList[0]).Length, 
					COM.Formatters.Serialization.BinarySerialize(testObjectListWithList[0]).Length
				);

			Console.WriteLine("Step 2a: Adding all items into cache (without list)");
			DateTime start = DateTime.Now;
			foreach (TestSizeObject o in testObjectList)
			{
				Common.Util.CacheAdd(o.Id, o);
				without.Add(o.Id);
			}
			TimeSpan span = DateTime.Now.Subtract(start);

			Console.WriteLine();
			result1 = string.Format(@"without list taken time: {0}h {1}m {2}s {3}mi", span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
			
			Console.WriteLine("Step 2b: Adding all items into cache (with list)");
			
			start = DateTime.Now;
			foreach (TestSizeObjectWithList o in testObjectListWithList)
			{
				Common.Util.CacheAdd(o.Id, o);
				with.Add(o.Id);
			}
			span = DateTime.Now.Subtract(start);

			Console.WriteLine();
			Console.WriteLine(Common.Util.CacheGetStats());
			Console.WriteLine();
			Console.WriteLine(result1);
			result1 = string.Format(@"with taken time: {0}h {1}m {2}s {3}mi", span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
			Console.WriteLine(result1);
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();

			Console.WriteLine(@"Step 3a: Retrieve each item from cache (without list)");

			int success = 0;
			int failed = 0;
			TestSizeObject o2 = null;
			start = DateTime.Now;
			foreach (string key in without)
			{
				o2 = Common.Util.CacheGet<TestSizeObject>(key);
				if (o2 != null)
					++success;
				else
					++failed;
				o2 = null;
			}
			span = DateTime.Now.Subtract(start);
			result2 = string.Format(@"taken time: {0}h {1}m {2}s {3}mi / success {4} - failed {5}", span.Hours, span.Minutes, span.Seconds, span.Milliseconds, success, failed);
			Console.WriteLine(result2);
			Console.WriteLine(@"Step 3b: Retrieve each item from cache (with list)");
			
			success = 0;
			failed = 0;
			TestSizeObjectWithList o3 = null;
			start = DateTime.Now;
			foreach (string key in with)
			{
				o3 = Common.Util.CacheGet<TestSizeObjectWithList>(key);
				if (o3 != null)
					++success;
				else
					++failed;
				o3 = null;
			}
			span = DateTime.Now.Subtract(start);

			Console.WriteLine();
			Console.WriteLine(Common.Util.CacheGetStats());
			Console.WriteLine();
			result2 = string.Format(@"taken time: {0}h {1}m {2}s {3}mi / success {4} - failed {5}", span.Hours, span.Minutes, span.Seconds, span.Milliseconds, success, failed);
			Console.WriteLine(result2);
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();

			Console.WriteLine();
			Console.WriteLine(@"Step 4a: Remove each item by key without list... NOT ALL AT ONCE!");
			start = DateTime.Now;
			foreach (string key in with)
			{
				Common.Util.CacheRemove(key);
			}
			span = DateTime.Now.Subtract(start);
			Console.WriteLine();
			Console.WriteLine();
			result3 = string.Format(@"taken time: {0}h {1}m {2}s {3}mi", span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
			Console.WriteLine();
			Console.WriteLine(@"Step 4b: Remove each item by key with list... NOT ALL AT ONCE!");
			start = DateTime.Now;
			foreach (string key in without)
			{
				Common.Util.CacheRemove(key);
			}
			span = DateTime.Now.Subtract(start);
			Console.WriteLine();
			Console.WriteLine(Common.Util.CacheGetStats());
			Console.WriteLine();
			Console.WriteLine(result3);
			result3 = string.Format(@"taken time: {0}h {1}m {2}s {3}mi", span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
			Console.WriteLine(result3);
			
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();
			Console.Clear();
		}

		private void CompareComressionUsage()
		{
			Console.Clear();
			Console.WriteLine(@"Clear Cache");
			Console.WriteLine();

			List<TestSizeObject> testObjectList = new List<TestSizeObject>();
			for (int i = 0; i < 1; ++i)
			{
				testObjectList.Add(new TestSizeObject(ObjectSize.Hundert));
			}

			testObjectList.Add(new TestSizeObject(ObjectSize.Thousend));

			byte[] dd = COM.Formatters.Compression.Compress(
					COM.Formatters.Serialization.BinarySerialize(testObjectList[0])
				);

			testObjectList[0] =
				COM.Formatters.Serialization.BinaryDeSerialize<TestSizeObject>(
					COM.Formatters.Compression.Decompress(dd)
				);



			dd = COM.Formatters.Compression.Compress(
					COM.Formatters.Serialization.BinarySerialize(testObjectList[1])
				);

			testObjectList[1] =
				COM.Formatters.Serialization.BinaryDeSerialize<TestSizeObject>(
					COM.Formatters.Compression.Decompress(dd)
				);

			Console.WriteLine();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
		}

		private void UserData()
		{
			Console.Clear();

			ObjectSize size;
			int objectSizeOption,loops = -1;
			//bool allData = true;
			do
			{
				try
				{
					Console.WriteLine("Choose Object Size to fake:");
					Console.WriteLine("1 -    1 Kb");
					Console.WriteLine("2 -  100 Kb");
					Console.WriteLine("3 - 1000 Kb");
					Console.Write(">");
					string result = Console.ReadLine();
					int.TryParse(result, out objectSizeOption);
					if (objectSizeOption == -1 || objectSizeOption > 3)
						throw new Exception("Wrong object size input");
					
					Console.Clear();

					switch (objectSizeOption)
					{
						case 2:
							Console.WriteLine("Your selection:  100 Kb");
							size = ObjectSize.Hundert;
							break;
						case 3:
							Console.WriteLine("Your selection: 1000 Kb");
							size = ObjectSize.Thousend;
							break;
						default:
							Console.WriteLine("Your selection:    1 Kb");
							size = ObjectSize.One;
							break;
					}

					Console.WriteLine("Choose amount of objects to send to server:");
					Console.Write(">");
					result = Console.ReadLine();
					int.TryParse(result, out loops);
					if (loops == -1)
						throw new Exception("Wrong amount input");

					Console.WriteLine("Choose time amount, (ZERO) 0 - same like 100000 seconds.");
					
					Console.Write(">");
					result = Console.ReadLine();
					double addSeconds = -1;
					double.TryParse(result, out addSeconds);
					if (addSeconds == 0)
					{
						Console.WriteLine("Default Time has been Taken - 100000 seconds");
						Console.ReadLine();
						addSeconds = 100000;
					}					

					Console.Clear();
					Console.WriteLine("Using {0} roundtrips with an amount of approx.: {1}", loops, size.ToString());

					CalculateRequestTest(size, loops, addSeconds);
				}
				catch (Exception ex)
				{
					Console.WriteLine(@"Exception: " + ex.Message);
					Console.WriteLine(@"Press enter to go on.");
					Console.ReadLine();
					Console.Clear();
					GC.Collect();
					return;
				}
			} while ( !(loops > -1 && objectSizeOption>-1) );
			Console.Clear();
			GC.Collect();
		}

		private void CalculateRequestTest(ObjectSize size, int loops, double addSeconds)
		{
			Console.WriteLine(@"ClearCache!");
			Common.Util.CacheClear();

			string result1 = string.Empty;
			string result2 = string.Empty;
			string result3 = string.Empty;

			Console.WriteLine(@"Step 1: The clients loads {0} objects with an appox. size of {1} kb", loops, size.ToString());

			List<TestSizeObject> testObjectList = new List<TestSizeObject>();
			for (int i = 0; i < loops; ++i)
			{
				if (i % 10 == 0)
					Console.Write(". ");
				testObjectList.Add(new TestSizeObject(size));
			}
			Console.WriteLine();
			Console.WriteLine(@"Client is done to load the data");

			Console.WriteLine("Step 2: Adding all items into cache");
			
			DateTime start = DateTime.Now;			
			foreach (TestSizeObject o in testObjectList)
			{
				Common.Util.CacheAdd(o.Id, o, DateTime.Now.AddSeconds(addSeconds));
			}
			TimeSpan span = DateTime.Now.Subtract(start);

			Console.WriteLine();
			Console.WriteLine(Common.Util.CacheGetStats());
			Console.WriteLine();
			result1 = string.Format(@"taken time: {0}h {1}m {2}s {3}mi", span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
			Console.WriteLine(result1);
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();

			Console.WriteLine(@"Step 3: Retrieve each item from cache");

			List<string> keyList = Common.Util.CacheGetAllKeys();
			int success = 0;
			int failed = 0;
			TestSizeObject o2 = null ;
			start = DateTime.Now;
			foreach (string key in keyList)
			{
				o2 = Common.Util.CacheGet<TestSizeObject>(key);
				if (o2 != null)
					++success;
				else
					++failed;
				o2 = null;
			}
			span = DateTime.Now.Subtract(start);
			Console.WriteLine();
			Console.WriteLine(Common.Util.CacheGetStats());
			Console.WriteLine();
			result2 = string.Format(@"taken time: {0}h {1}m {2}s {3}mi / success {4} - failed {5}", span.Hours, span.Minutes, span.Seconds, span.Milliseconds, success, failed);
			Console.WriteLine(result2);
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();

			Console.WriteLine();
			Console.WriteLine(@"Step 4: Remove each item by key... NOT ALL AT ONCE!");
			start = DateTime.Now;
			foreach (string key in keyList)
			{
				Common.Util.CacheRemove(key);
			}
			span = DateTime.Now.Subtract(start);
			Console.WriteLine();
			Console.WriteLine(Common.Util.CacheGetStats());
			Console.WriteLine();
			result3 = string.Format(@"taken time: {0}h {1}m {2}s {3}mi", span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
			Console.WriteLine(result3);
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();

			Console.Clear();
			Console.WriteLine();
			Console.WriteLine("Using {0} roundtrips with an amount of approx.: {1}", loops, size.ToString());
			Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - -");
			// Console.WriteLine(@"Overview {0}:", lastSelectMenuOption);
			Console.WriteLine(@"Result 1:" + result1);
			Console.WriteLine(@"Result 2:" + result2);
			Console.WriteLine(@"Result 3:" + result3);
			Console.WriteLine();
			Console.WriteLine(Common.Util.CacheGetStats());
			Console.WriteLine(@"Free Memory");

			for (int i = 0; i < testObjectList.Count; ++i)
			{
				testObjectList[i] = null;
			}
			testObjectList = null;
			Console.WriteLine(@"Press enter to go on.");
			Console.ReadLine();
			Console.Clear();
			GC.Collect();

		}

		/// <summary>
		/// Retrieves randomize keys, the user has the option to enter a number.
		/// use this method only in combination with same objects, it will throw an exception if you have 
		/// other objects in cache....
		/// </summary>
		/// <param name="p">The p.</param>
		private void RetrieveRandomizeKey(int p)
		{
			Console.Clear();
			Console.WriteLine();
			Console.WriteLine();

			int amountOfRequst;
			Console.Write(@"Please enter an amount of calls to the server [wrong input will force 10 calls]: ");
			int.TryParse(Console.ReadLine(), out amountOfRequst);

			List<string> keys = Common.Util.CacheGetAllKeys();
			if (keys == null)
			{
				Console.WriteLine(@"No keys are available on cache!!!");
				return;
			}

			if (amountOfRequst == 0)
				amountOfRequst = 10;

			Console.WriteLine();
			Console.WriteLine();
			if (keys.Count > 0)
			{
				for (int i = 1; i <= amountOfRequst; ++i)
				{
					string key = keys[new Random().Next(0, keys.Count)];
					Console.WriteLine("[Request {0}/{1} for Key:{2}]:", i, amountOfRequst, key);

					object o = Common.Util.CacheGet<object>(key);

					if (o is Common.Country)
					{
						Console.Write("\t");
						(o as Common.Country).PrintToConsole();
					}
					else if (o is Common.Region)
					{
						Console.Write("\t");
						(o as Common.Region).PrintToConsole();
					}
					else if (o is List<Common.Country>)
					{
						foreach (Common.Country c in (o as List<Common.Country>))
						{
							Console.Write("\t");
							c.PrintToConsole();
						}
					}
					else if (o is List<Common.Region>)
					{
						foreach (Common.Region r in (o as List<Common.Region>))
						{
							Console.Write("\t");
							r.PrintToConsole();
						}
					}
					else
					{
						Console.Write("\t");
						Console.WriteLine(o.ToString());
					}

					Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
					Console.WriteLine();
				}
			}
			else
			{
				Console.WriteLine(@"There is no data in cache, please add first some data before you use this option.");
			}
			
			Console.WriteLine();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
		}

		/// <summary>
		/// Tests the country method randomize.
		/// </summary>
		/// <param name="withRegionData">if set to <c>true</c> [with region data].</param>
		private void TestCountryMethodRandomize(bool withRegionData)
		{
			Console.Clear();
			Console.WriteLine();
			Console.WriteLine();

			int id;
			for (int i = 1; i <= 100; ++i)
			{
				id = new Random().Next(1, 274);
				Console.Write("[No:{0} Id:{1}] - ", i.ToString(), id);
				Common.Country country = new BLL.BllCountry().GetById(id, withRegionData);
				if (country != null)
					country.PrintToConsole();
			}

			Console.WriteLine();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
		}

		/// <summary>
		/// Tests the country method.
		/// </summary>
		/// <param name="withPrintout">if set to <c>true</c> [with printout].</param>
		/// <param name="withRegionData">if set to <c>true</c> [with region data].</param>
		public void TestCountryMethod(bool withPrintout, bool withRegionData)
		{
			Console.Clear();
			Console.WriteLine();
			Console.WriteLine();

			if (!withPrintout)
				Console.WriteLine(@"Operation started.");

			BLL.BllCountry country = new BLL.BllCountry();
			DateTime start = DateTime.Now;
			country.GetAll(withPrintout, withRegionData);
			TimeSpan span = DateTime.Now.Subtract(start);

			Console.WriteLine();
			Console.WriteLine(span.TotalMilliseconds + "ms");
			Console.WriteLine();
			Console.WriteLine(@"Operation Done");
			Console.WriteLine(@"- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ");
			Console.WriteLine();
		}


	}

	[Serializable]
	public class TestSizeObjectWithList
	{
		public TestSizeObjectWithList() { }

		public string Id = Guid.NewGuid().ToString();

		public List<byte> data = new List<byte>();
		
		int length = 0;

		public TestSizeObjectWithList(TestApplication.ObjectSize size) 
		{
			switch (size)
			{
				case TestApplication.ObjectSize.One:
					length = 1024;
					break;
				case TestApplication.ObjectSize.Hundert:
					length = 1024 * 128;
					break;
				case TestApplication.ObjectSize.Thousend:
					length = 1024 * 1024;
					break;
			}

			Random r = new Random();
			for (int i = 0; i < length; i++)
			{
				int bb = r.Next(65, 97);
				data.Add(Convert.ToByte(bb));
			}
		}
	}


	/// <summary>
	/// Test Size Object
	/// </summary>
	[Serializable]
	public class TestSizeObject
	{
		/// <summary>
		/// an byte array which contains object payload.
		/// </summary>
		public byte[] byteArray;
		/// <summary>
		/// object id
		/// </summary>
		public string Id = Guid.NewGuid().ToString();

		/// <summary>
		/// Initializes a new instance of the <see cref="TestSizeObject"/> class.
		/// </summary>
		public TestSizeObject()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="TestSizeObject"/> class.
		/// </summary>
		/// <param name="size">The size.</param>
		public TestSizeObject(TestApplication.ObjectSize size)
		{
			switch (size)
			{
				case TestApplication.ObjectSize.One:
					byteArray = new byte[1024];
					break;
				case TestApplication.ObjectSize.Hundert:
					byteArray = new byte[1024 * 128];
					break;
				case TestApplication.ObjectSize.Thousend:
					byteArray = new byte[1024 * 1024];
					break;
			}

			Random r = new Random();
			for (int i = 0; i < byteArray.Length; i++)
			{
				int bb = r.Next(65, 97);
				byteArray[i] = Convert.ToByte(bb);
			}
		}
	}


}
