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
// Name:      SharedCacheSoapExtension.cs
// 
// Modified:  28-01-2010 SharedCache.com, chrisme  : clean up code
// ************************************************************************* 

using System;
using System.IO;
using System.Web.Services.Protocols;

namespace SharedCache.WinServiceCommon.Attributes
{
	/// <summary>
	/// Define a SOAP Extension that traces the SOAP request and SOAP response for 
	/// the XML Web service method the SOAP extension is applied to.
	/// </summary>
	public class SharedCacheSoapExtension : SoapExtension
	{
		// Fields
	    const string filename = "C:\\ronitest.txt";
		private Stream newStream;
		private Stream oldStream;
        private int keep = 59999;
        private string cacheKey = string.Empty;

		/// <summary>
		/// The SOAP extension was configured to run using a configuration file instead 
		/// of an attribute applied to a specific XML Web service method.
		/// </summary>
		/// <param name="serviceType">A <see cref="Type"/> obejct.</param>
		/// <returns>An <see cref="object"/> obejct.</returns>
		public override object GetInitializer(Type serviceType)
		{
			return typeof(SharedCacheSoapExtension);
		}

		/// <summary>
		/// When the SOAP extension is accessed for the first time, the XML Web service method it is applied to 
		/// is accessed to store the file name passed in, using the corresponding SoapExtensionAttribute.
		/// </summary>
        /// <param name="methodInfo">A <see cref="LogicalMethodInfo"/> methodInfo.</param>
		/// <param name="attribute">A <see cref="SoapExtensionAttribute"/> object.</param>
		/// <returns>An <see cref="object"/> obejct.</returns>
		public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
		{
			// keep = ((SharedCacheSoapExtensionAttribute)attribute).CacheInSecond;
			return attribute;
		}
		
		/// <summary>
		/// Receive the file name stored by GetInitializer and store it in 
		/// a member variable for this specific instance.
		/// </summary>
		/// <param name="initializer">An <see cref="object"/> obejct.</param>
		public override void Initialize(object initializer)
		{
			//You'd usually get the attribute here and pull whatever you need off it.
			var attr = initializer as SharedCacheSoapExtensionAttribute;
			if (attr != null)
			{
				keep = attr.CacheInSecond;
			}
		}

	    /// <summary>
	    /// When overridden in a derived class, allows a SOAP extension access to the memory buffer containing the SOAP request or response.
	    /// </summary>
	    /// <returns>
	    /// A <see cref="T:System.IO.Stream"/> representing a new memory buffer that this SOAP extension can modify.
	    /// </returns>
	    /// <param name="stream">A memory buffer containing the SOAP request or response. 
	    ///                 </param>
	    public override Stream ChainStream(Stream stream)
		{
			this.oldStream = stream;
			this.newStream = new MemoryStream();
			return this.newStream;
			// return base.ChainStream(stream);
		}

        /// <summary>
        /// Yanks it.
        /// </summary>
        /// <param name="streamToPrefix">The stream to prefix.</param>
        /// <returns></returns>
		public MemoryStream YankIt(Stream streamToPrefix)
		{
			var outStream = new MemoryStream();
			//debug
			outStream.Seek(0, SeekOrigin.Begin);
			//outStream.Position = 0L;
			var reader2 = new StreamReader(outStream);
			string s = reader2.ReadToEnd();
			System.Diagnostics.Debug.WriteLine(s);

			outStream.Position = 0L;
			outStream.Seek(0, SeekOrigin.Begin);
			return outStream;
		}

		private void GetReady()
		{
			Copy(this.oldStream, this.newStream);
			this.newStream.Position = 0L;
		}

		// Methods
		private void StripWhitespace()
		{
			this.newStream.Position = 0L;
			this.newStream = this.YankIt(this.newStream);
			Copy(this.newStream, this.oldStream);
		}

		private static void Copy(Stream from, Stream to)
		{
			TextReader reader = new StreamReader(from);
			TextWriter writer = new StreamWriter(to);
			writer.WriteLine(reader.ReadToEnd());
			writer.Flush();
		}

        /// <summary>
        /// When overridden in a derived class, allows a SOAP extension to receive a <see cref="T:System.Web.Services.Protocols.SoapMessage"/> to process at each <see cref="T:System.Web.Services.Protocols.SoapMessageStage"/>.
        /// </summary>
        /// <param name="message">The <see cref="T:System.Web.Services.Protocols.SoapMessage"/> to process.</param>
		public override void ProcessMessage(SoapMessage message)
		{
			switch (message.Stage)
			{
				case SoapMessageStage.BeforeSerialize:
					break;
				case SoapMessageStage.AfterSerialize:
					WriteOutput((SoapServerMessage)message);
					break;
				case SoapMessageStage.BeforeDeserialize:
					WriteInput((SoapServerMessage)message);
					break;
				case SoapMessageStage.AfterDeserialize:
					break;
				default:
					throw new Exception("invalid stage");
			}
		}

		///<summary>
        /// Write the contents of the incoming SOAP message to the log file.
		///</summary>
		///<param name="message"></param>
		public void WriteInput(SoapServerMessage message)
		{
			// Utility method to copy the contents of one stream to another. 
			Copy(oldStream, newStream);
			var myFileStream = new FileStream(filename, FileMode.Append, FileAccess.Write);
			var myStreamWriter = new StreamWriter(myFileStream);
			myStreamWriter.WriteLine("================================== Request at "
				 + DateTime.Now);
			myStreamWriter.WriteLine("The method that has been invoked is : ");
			myStreamWriter.WriteLine("\t" + message.MethodInfo.Name);
			myStreamWriter.WriteLine("The contents of the SOAP envelope are : ");
			myStreamWriter.Flush();
			newStream.Position = 0;
			Copy(newStream, myFileStream);
			myFileStream.Close();
			newStream.Position = 0;
		}

		///<summary>
        /// Write the contents of the outgoing SOAP message to the log file.
		///</summary>
		///<param name="message"></param>
		public void WriteOutput(SoapServerMessage message)
		{
			newStream.Position = 0;
			var myFileStream = new FileStream(filename, FileMode.Append, FileAccess.Write);
			var myStreamWriter = new StreamWriter(myFileStream);
			myStreamWriter.WriteLine("---------------------------------- Response at "
																					+ DateTime.Now);
			myStreamWriter.Flush();
			// Utility method to copy the contents of one stream to another. 
			Copy(newStream, myFileStream);
			myFileStream.Close();
			newStream.Position = 0;
			Copy(newStream, oldStream);
		}



	}
}
