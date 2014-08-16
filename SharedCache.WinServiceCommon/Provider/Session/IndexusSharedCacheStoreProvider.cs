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
// Name:      IndexusSharedCacheStoreProvider.cs
// 
// Created:   24-04-2009 SharedCache.com, drapp
// Modified:  01-05-2009 SharedCache.com, drapp : Exposed Cache provider and AppID for unit tests
// Modified:  01-05-2009 SharedCache.com, drapp : Removed usage of CacheUtil
using System;
using System.Web;
using System.Web.SessionState;
using SharedCache.WinServiceCommon.Provider.Cache;

namespace SharedCache.WinServiceCommon.Provider.Session
{
    /// <summary>
    /// Custom implementation of the <see cref="SessionStateStoreProviderBase"/>
    /// that uses the shared cache to store and fetch session state information.
    /// Session locking is implemented by adding a flag into the cached object after
    /// reading the object.  This is inefficient and not 100% safe and should be 
    /// changed if and when locking is written into the core SharedCache library.
    /// </summary>
    public class IndexusSharedCacheStoreProvider : IndexusProviderBase
    {
        public event SessionStateItemExpireCallback SessionExpired;
        
        private static int _timeout = 60;
        private Cache.IndexusProviderBase _cache = IndexusDistributionCache.SharedCache;
        private string _appID = HttpRuntime.AppDomainAppId;

        /// <summary>
        /// Exposed internal Cache property to allow injection of new provider
        /// </summary>
        public Cache.IndexusProviderBase Cache
        {
            get { return _cache; }
            set { _cache = value; }
        }

        /// <summary>
        /// Gets and sets the app id used to distinguish this web application. 
        /// Defaults to <see cref="HttpRuntime.AppDomainAppId"/>
        /// </summary>
        public string AppID
        {
            get { return _appID; }
            set { _appID = value; }
        }

        private string GetCacheKey(string id)
        {
            return String.Format("Session({0}, {1})", id, AppID);
        }

        private SessionStateStoreData FetchCachedStoreData(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions, bool exclusive)
        {
            // 1-2 remote calls
            locked = false;
            lockAge = TimeSpan.Zero;
            lockId = null;
            actions = SessionStateActions.None;

            // check for existing session
            byte[] bytes = Cache.Get<byte[]>(GetCacheKey(id));
            if (bytes == null)
            {
                return null;
            }

            SessionStoreDataContext data = SessionStoreDataContext.Deserialize(bytes, context);

            locked = data.IsLocked;
            if (locked)
            {
                lockAge = DateTime.Now.Subtract(data.LockDate.Value);
                lockId = data.LockDate;
                // specs require null to be returned
                return null;
            }
            
            //todo: rewrite with real locking when available
            // as of a few microseconds ago, this session was unlocked, so proceed
            if (exclusive)
            {
                data.LockDate = DateTime.Now;
                // not perfectly thread safe as there can be race conditions between 
                // clustered servers, but this is the best we've got
                Cache.Add(GetCacheKey(id), (object) SessionStoreDataContext.Serialize(data), DateTime.Now.AddMinutes(data.Data.Timeout));
            }

            return data.Data;
        }

        #region SessionStateStoreProviderBase methods

        public override void InitializeRequest(HttpContext context)
        {
            // 0 remote calls
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            // 0 remote calls
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log

            SessionExpired += expireCallback;
            return true;
        }

        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            // 0 remote calls
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log

            return FetchCachedStoreData(context, id, out locked, out lockAge, out lockId, out actions, false);
        }

        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            // 0 remote calls
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log

            return FetchCachedStoreData(context, id, out locked, out lockAge, out lockId, out actions, true);
        }

        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            // 0 remote calls
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log

            _timeout = timeout;
            HttpStaticObjectsCollection staticObjects = (context != null) ? SessionStateUtility.GetSessionStaticObjects(context) : new HttpStaticObjectsCollection();
            return new SessionStateStoreData(new SessionStateItemCollection(), staticObjects, timeout);
        }

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            // 1 remote call
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log

            _timeout = timeout;
            SessionStoreDataContext data = new SessionStoreDataContext(CreateNewStoreData(context, timeout));
            Cache.Add(GetCacheKey(id), (object) SessionStoreDataContext.Serialize(data), DateTime.Now.AddMinutes(data.Data.Timeout));
        }

        public override void ResetItemTimeout(HttpContext context, string id)
        {
            // 1 remote call
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log

            Cache.ExtendTtl(GetCacheKey(id), DateTime.Now.AddMinutes(_timeout));
        }

        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            // 1-2 remote calls
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log

            //todo: replace with single unlock call when available
            byte[] bytes = Cache.Get<byte[]>(GetCacheKey(id));
            if (bytes != null)
            {
                SessionStoreDataContext data = SessionStoreDataContext.Deserialize(bytes, context);
                data.ClearLock();
                Cache.Add(GetCacheKey(id), (object) SessionStoreDataContext.Serialize(data), DateTime.Now.AddMinutes(data.Data.Timeout));
            }
        }

        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            // 1 remote call
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log

            // creating a new StoreDataInfo will initialize LockDate to null (unlocked)
            SessionStoreDataContext data = new SessionStoreDataContext(item);
            Cache.Add(GetCacheKey(id), (object) SessionStoreDataContext.Serialize(data), DateTime.Now.AddMinutes(item.Timeout));
        }

        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            // 1 remote call
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log

            // removing item also clears any lock
            Cache.Remove(GetCacheKey(id));
            if (SessionExpired != null)
            {
                SessionExpired(id, item);
            }
        }

        public override void EndRequest(HttpContext context)
        {
            // 0 remote calls
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log
        }

        /// <summary>
        /// Releases all resources used by the <see cref="T:System.Web.SessionState.SessionStateStoreProviderBase"></see> implementation.
        /// </summary>
        public override void Dispose()
        {
            // 0 remote calls
            #region Access Log
#if TRACE			
			{
				Handler.LogHandler.Tracking("Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;");
			}
#endif
            #endregion Access Log
        }

        #endregion
    }
}
