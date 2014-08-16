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
// Name:      Common.cs
// 
// Modified:  28-01-2010 SharedCache.com, chrisme  : clean up code
// ************************************************************************* 

using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace SharedCache.WinServiceCommon.HttpHandlers 
{
	/// <summary>
	/// Summary description for Common.
	/// </summary>
	internal class Common
	{
		/// <summary>
		/// Check if curremt request originated locally
		/// </summary>
		/// <param name="request">The current HttpRequest</param>
		/// <returns>True if the request originated locally</returns>
		internal static bool RequestIsLocal(HttpRequest request)
		{
		    if (request.UserHostAddress == "127.0.0.1" ||
				request.UserHostAddress == request.ServerVariables["LOCAL_ADDR"])
			{
				return true;
			}
		    return false;
		}

	    /// <summary>
		/// Serilaize object into a memory stream using Binary Serialization
		/// </summary>
		/// <param name="objectToSerialize">object to serialize</param>
		/// <returns></returns>
		internal static MemoryStream BinarySerialize(object objectToSerialize)
		{
			var m = new MemoryStream();
			var b = new BinaryFormatter();

			b.Serialize(m, objectToSerialize);

			return m;
		}

		/// <summary>
		/// Serialize object into StringWriter using XmlSerialization
		/// </summary>
		/// <param name="objectToSerialize">object to serialize</param>
		/// <returns></returns>
		internal static StringWriter XmlSerialize(object objectToSerialize)
		{


			try
			{
				var serializer = new XmlSerializer(objectToSerialize.GetType());

				var writer = new StringWriter();

				//This is used to remove the standard XMl namespaces from the serialized output
				//so as to make it easier to see in the browser output
				var dummyNamespaceName = new XmlQualifiedName[1];

				dummyNamespaceName[0] = new XmlQualifiedName();

				serializer.Serialize(writer, objectToSerialize, new XmlSerializerNamespaces(dummyNamespaceName));

				return writer;
			}
			//If we cannot serialize then we can't leave it at that
			catch (InvalidOperationException)
			{
				//Ignore This can happen when some objects are just not Serializable using XML serialization
			}
			catch (System.Runtime.Serialization.SerializationException)
			{
				//Ignore. This can happen when storing a set of custom objects in a collection.
				//The XmlSerializer will start to serialize the collection come across the custom objects
				//amd not know what to do. The use of custom serialization attributes will help the serializer.
			}
			catch (Exception)
			{
                //TODO: This exception should be logged!
                //string s = ex.Message;
			}

			//This will only be hit by a failed serialization execution
			return null;
		}
	}
}
