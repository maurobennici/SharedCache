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
// Name:      SessionStoreDataContext.cs
// 
// Created:   24-04-2009 SharedCache.com, drapp
// Modified:  01-05-2009 SharedCache.com, drapp : Renamed object
// Modified:  01-05-2009 SharedCache.com, drapp : Added comments
using System;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace SharedCache.WinServiceCommon.Provider.Session
{
    /// <summary>
    /// Encapsulates a SessionStateStoreData with a lock object.  Static methods
    /// define how to serialize and deserialize the store data.
    /// </summary>
    internal class SessionStoreDataContext
    {
        private readonly SessionStateStoreData _data;
        private DateTime? _lockDate;

        internal SessionStoreDataContext(SessionStateStoreData data)
        {
            _data = data;
        }

        /// <summary>
        /// Gets the read-only session state data
        /// </summary>
        internal SessionStateStoreData Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Gets or sets the lock date
        /// </summary>
        internal DateTime? LockDate
        {
            get { return _lockDate; }
            set { _lockDate = value; }
        }

        /// <summary>
        /// Gets whether this session is currently locked by checking 
        /// whether the lock date has a value
        /// </summary>
        internal bool IsLocked
        {
            get { return _lockDate.HasValue; }
        }

        /// <summary>
        /// Clears the lock date
        /// </summary>
        internal void ClearLock()
        {
            _lockDate = null;
        }

        /// <summary>
        /// Serializes a SessionStoreDataContext into binary data.
        /// </summary>
        /// <param name="info">The object to serialize</param>
        /// <returns>Binary data that can later be deserialized into a fully
        /// hydrated SessionStoreDataContext</returns>
        internal static byte[] Serialize(SessionStoreDataContext info)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    bool hasItems = info.Data.Items != null && info.Data.Items.Count > 0;
                    bool hasStaticObjects = info.Data.StaticObjects != null && !info.Data.StaticObjects.NeverAccessed;
                    bool isLocked = info.IsLocked;
                    
                    //Serialize timeout information
                    writer.Write(info.Data.Timeout);
                    //Serialize whether to bother reading SessionStateItemCollection data
                    writer.Write(hasItems);
                    //Serialize whether to bother reading HttpStaticObjectCollection data
                    writer.Write(hasStaticObjects);
                    //Serialize whether to bother reading a lock date
                    writer.Write(isLocked);
                    if (isLocked)
                    {
                        //Serialize numeric representation of DateTime object
                        writer.Write(info.LockDate.Value.ToBinary());
                    }
                    if (hasItems)
                    {
                        //Defer to the item collection class for serialization of collection
                        ((SessionStateItemCollection)info.Data.Items).Serialize(writer);
                    }
                    if (hasStaticObjects)
                    {
                        //Defer to the static object collection class for serialization of collection
                        info.Data.StaticObjects.Serialize(writer);
                    }
                    //Trailing byte helpful for sanity checks during deserialization
                    writer.Write((byte)0xff);
                }
                return stream.GetBuffer();
            }
        }

        /// <summary>
        /// Deserializes binary data into a fully hydrated SessionStoreDataContext.
        /// </summary>
        /// <param name="bytes">serialized binary data</param>
        /// <returns>The object representation of the session data</returns>
        /// <exception cref="HttpException">Thrown if deserialization encounters a problem</exception>
        internal static SessionStoreDataContext Deserialize(byte[] bytes, HttpContext context)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        //See comments in Serialize method for description of fields
                        int timeout = reader.ReadInt32();
                        bool hasItems = reader.ReadBoolean();
                        bool hasStaticObjects = reader.ReadBoolean();
                        bool isLocked = reader.ReadBoolean();
                        DateTime? lockDate = null;
                        if (isLocked)
                        {
                            lockDate = DateTime.FromBinary(reader.ReadInt64());
                        }

                        SessionStateItemCollection items = hasItems
                            ? SessionStateItemCollection.Deserialize(reader)
                            : new SessionStateItemCollection();
                        HttpStaticObjectsCollection staticObjects = hasStaticObjects
                            ? HttpStaticObjectsCollection.Deserialize(reader)
                            : SessionStateUtility.GetSessionStaticObjects(context);

                        //this is a sanity byte.  If it does not equal 0xFF, there is a problem
                        if (reader.ReadByte() != 0xff)
                        {
                            throw new HttpException("Invalid Session State");
                        }

                        SessionStoreDataContext data = new SessionStoreDataContext(new SessionStateStoreData(items, staticObjects, timeout));
                        data.LockDate = lockDate;
                        return data;
                    }
                }
            }
            catch (EndOfStreamException)
            {
                throw new HttpException("Invalid Session State");
            }
        }
    }
}
