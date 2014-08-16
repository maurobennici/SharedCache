using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Configuration;

using CACHE = MergeSystem.Indexus.WinServiceCommon.Provider.Cache.IndexusDistributionCache;

namespace MergeSystem.Indexus.WinServiceCommon.Attributes
{
	public class WebServices
	{
		public WebServices()
		{
			string rootFolder = System.Web.HttpContext.Current.Request.ApplicationPath + (System.Web.HttpContext.Current.Request.ApplicationPath.EndsWith("/") ? "" : "/");
			string n = "roni";
			Type t = GetServiceType(n);
		}

		public void Manage()
		{
			string rootFolder = System.Web.HttpContext.Current.Request.ApplicationPath + (System.Web.HttpContext.Current.Request.ApplicationPath.EndsWith("/") ? "" : "/");
			string n = "roni";
			Type t = GetServiceType(n);
		}

		/// <summary>
		/// Searches all Services and tries to find a class with the specified name
		/// </summary>
		private Type GetServiceType(string TypeName)
		{
			// Todo: Caching mechanism for assembly checks
			foreach (Assembly loadedAssembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type ClassType = loadedAssembly.GetType(TypeName);
				if (ClassType != null)
					return ClassType;
			}
			return null;
		}

		public static int FromMethodInfo(MethodInfo method)
		{
			int result = 0;

			SharedCacheAttribute[] classns = (SharedCacheAttribute[])method.ReflectedType.GetCustomAttributes(typeof(SharedCacheAttribute), true);
			SharedCacheAttribute[] methodns = (SharedCacheAttribute[])method.GetCustomAttributes(typeof(SharedCacheAttribute), true);

			if (classns.Length > 0)
				result = classns[0].SecondToCache;

			return result;
		}

		public static object Invoke<T>(string methodName, object values)
		{
			T cached = CACHE.SharedCache.Get<T>(methodName);
			if (cached == null)
			{
				Type t = typeof(T);

				MethodInfo method = t.GetMethod(methodName);
				SharedCacheAttribute cacheAtt = (SharedCacheAttribute)method.GetCustomAttributes(typeof(SharedCacheAttribute), false)[0];

				object o = Activator.CreateInstance(t);
				object result = null;

				if (values != null)
				{
					result = method.Invoke(o, new object[] { values });
				}
				CACHE.SharedCache.Add(methodName, result, DateTime.Now.AddSeconds(cacheAtt.SecondToCache));	
			}
			return (T)cached;
		}
	}
}
