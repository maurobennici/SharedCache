#region Copyright (c) Roni Schuetz - All Rights Reserved
// * --------------------------------------------------------------------- *
// *                              Roni Schuetz                             *
// *              Copyright (c) 2010 All Rights reserved                   *
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
// Name:      SessionViewHandler.cs
// 
// Modified:  28-01-2010 SharedCache.com, chrisme  : clean up code
// ************************************************************************* 

using System;
using System.IO;
using System.Web.UI.WebControls;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace SharedCache.WinServiceCommon.HttpHandlers
{
	/// <summary>
	/// This class is an HttpHandler and reports on ASP.NET Session State
	/// </summary>
	public class SessionViewHandler : IHttpHandler, IRequiresSessionState
	{
		private HttpContext _context;
		private HttpRequest _request;
		private HttpResponse _response;
		private HtmlTextWriter _writer;

		private readonly Type[] _altSerializationType;

		private HttpHandlersConfig _HttpHandlerSettings;

		private const string pageStyle = @"<style> {background-color:white; color:black;	font: 10pt verdana, arial;}
				table {font: 10pt verdana, arial; cellspacing:0; 	cellpadding:0; 	margin-bottom:25}
				tr.subhead { background-color:cccccc;}
				th { padding:0,3,0,3 }
				th.alt { background-color:black; color:white; padding:3,3,2,3; }
				td { padding:0,3,0,3 }
				tr.alt { background-color:eeeeee }
				h1 { font: 24pt verdana, arial; margin:0,0,0,0}
				h2 { font: 18pt verdana, arial; margin:0,0,0,0}
				h3 { font: 12pt verdana, arial; margin:0,0,0,0}
				th a { color:darkblue; font: 8pt verdana, arial; }
				a { color:darkblue;text-decoration:none }
				a:hover { color:darkblue;text-decoration:underline; }
				div.outer { width:90%; margin:15,15,15,15}
				table.viewmenu td { background-color:006699; color:white; padding:0,5,0,5; }
				table.viewmenu td.end { padding:0,0,0,0; }
				table.viewmenu a {color:white; font: 8pt verdana, arial; }
				table.viewmenu a:hover {color:white; font: 8pt verdana, arial; }
				a.tinylink {color:darkblue; font: 8pt verdana, arial;text-decoration:underline;}
				a.link {color:darkblue; text-decoration:underline;}
				div.buffer {padding-top:7; padding-bottom:17;}
				.small { font: 8pt verdana, arial }
				table td { padding-right:20 } 
				table td.nopad { padding-right:5 }
			</style>";

		///<summary>
		/// Public Constructor
		///</summary>
		public SessionViewHandler()
		{
			_altSerializationType = new Type[19];

			_altSerializationType[0] = typeof(String);
			_altSerializationType[1] = typeof(Int32);
			_altSerializationType[2] = typeof(Boolean);
			_altSerializationType[3] = typeof(DateTime);
			_altSerializationType[4] = typeof(Decimal);
			_altSerializationType[5] = typeof(Byte);
			_altSerializationType[6] = typeof(Char);
			_altSerializationType[7] = typeof(Single);
			_altSerializationType[8] = typeof(Double);
			_altSerializationType[9] = typeof(Int16);
			_altSerializationType[10] = typeof(Int64);
			_altSerializationType[11] = typeof(UInt16);
			_altSerializationType[12] = typeof(UInt32);
			_altSerializationType[13] = typeof(UInt64);
			_altSerializationType[14] = typeof(SByte);
			_altSerializationType[15] = typeof(TimeSpan);
			_altSerializationType[16] = typeof(Guid);
			_altSerializationType[17] = typeof(IntPtr);
			_altSerializationType[18] = typeof(UIntPtr);

		}

		private bool TypeIsInAlternativeSerializationList(Type type)
		{

			for (int i = 0; i <= (_altSerializationType.Length - 1); i++)
			{
				if (type == _altSerializationType[i])
				{
					return true;
				}
			}

			//Only would have got to here is the type was not found in the list of types
			//that go though the "alternative" serialization
			return false;
		}

		#region IHttpHandler Members

		/// <summary>
		/// Standard HttpHandler Entry point. Coordinate the displaying of the Session View
		/// </summary>
		/// <param name="context">The current HttpContext</param>
		public void ProcessRequest(HttpContext context)
		{

			//Only allow session viewing on requests that have originated locally
			if (Common.RequestIsLocal(context.Request) == false)
			{
				context.AddError(new ApplicationException("SessionView can only be accessed locally i.e. localhost"));
				return;
			}

			_HttpHandlerSettings = (HttpHandlersConfig)context.GetSection("sharedCache/httpHandlers");

			if (_HttpHandlerSettings == null ||
					(_HttpHandlerSettings.SessionView == null) ||
					(_HttpHandlerSettings.SessionView.Enabled == false))
			{
				//Session View is not enabled fire exception
				context.AddError(new ApplicationException(@"SessionView is not enabled. See the sharedCache\httpHandlers\ section in your configuration file", null));
				return;
			}

			_context = context;
			_request = context.Request;
			_response = context.Response;

			_writer = new HtmlTextWriter(_response.Output);

			_writer.Write("<html>\r\n");

			//Write out Html head and style tags
			_writer.Write("<head>\r\n");
			_writer.Write(pageStyle);
			_writer.Write("</head>\r\n");

			if (_context.Session.Mode != SessionStateMode.Off)
			{
				if (context.Request.QueryString["Item"] == null)
				{
					//An item specific requets is NOT being made. Display the lot
					EnumerateAndDisplaySessionState();
				}
				else
				{
					//A session item specific request is being made. Try and display it
					DisplaySessionStateItem();
				}
			}
			else
			{
				//Session state is off
				DisplaySessionStateOff();
			}

			_writer.Write("\r\n</body>\r\n</html>\r\n");

		}

		/// <summary>
		/// Gets the bool saying whether this HttpHandler is Reusable or not
		/// </summary>
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		#endregion

		private void DisplaySessionStateOff()
		{
			Table sessionTable = TableHelper.CreateTable();

			//Table Header
			var mainHeadingRow = new TableRow();
			sessionTable.Rows.Add(mainHeadingRow);

			TableHelper.AddHeaderCell(mainHeadingRow, "Session State is Off");

			sessionTable.RenderControl(_writer);
		}

		private void EnumerateAndDisplaySessionState()
		{

			bool displayingObjectSize;

			Table sessionTable = TableHelper.CreateTable();

			//Table Header
			var mainHeadingRow = new TableRow();
			sessionTable.Rows.Add(mainHeadingRow);

			TableCell mainHeading;

			if (_context.Session.Mode == SessionStateMode.SQLServer ||
				_context.Session.Mode == SessionStateMode.StateServer)
			{
				displayingObjectSize = true;
				mainHeading = TableHelper.AddHeaderCell(mainHeadingRow, "<h3><b>Session State Details including approximate item size</b></h3>");
				mainHeading.CssClass = "alt";
			}
			else
			{
				displayingObjectSize = false;
				mainHeading = TableHelper.AddHeaderCell(mainHeadingRow, "<h3><b>Session State Details</b></h3>");
			}

			mainHeading.ColumnSpan = 4;
			mainHeading.CssClass = "alt";

			TableRow secondaryHeadingRow = new TableRow();
			secondaryHeadingRow.CssClass = "subhead";
			sessionTable.Rows.Add(secondaryHeadingRow);

            //TableCell headingRowCol1 = TableHelper.AddHeaderCell(secondaryHeadingRow, "Session Key"); -- chrisme: not used!

			TableCell headingRowCol2 = TableHelper.AddHeaderCell(secondaryHeadingRow, "Object Type");

			if (!displayingObjectSize)
            {
				//Make second column span 2
				headingRowCol2.ColumnSpan = 2;
			}

            //TableCell headingRowCol4 = TableHelper.AddHeaderCell(secondaryHeadingRow, "View Data"); -- chrisme: not used!

			bool alternatingRowToggle = false;
			TableRow dataRow;

			//Loop through all the session item keys 
			foreach (string sessionKey in _context.Session.Keys)
			{

				dataRow = new TableRow();

				//Key column
				TableHelper.AddCell(dataRow, sessionKey);

				if (_context.Session[sessionKey] != null)
				{
					//Get out of session
					Object sessionItem = _context.Session[sessionKey];

					//System.Type type = sessionItem.GetType();  -- chrisme: not used!

					//Type column
					TableHelper.AddCell(dataRow, sessionItem.GetType().FullName);

					if (displayingObjectSize)
					{

						//Size column

						Stream alternativeSerializationStreamSessionKey = BinaryWrite(sessionKey);

						if (TypeIsInAlternativeSerializationList(sessionItem.GetType()))
						{
							Stream alternativeSerializationStream = BinaryWrite(sessionItem);

							//Alt Serialization Size column
							TableHelper.AddCell(dataRow,
								Convert.ToDouble(alternativeSerializationStream.Length + alternativeSerializationStreamSessionKey.Length)
													/ Convert.ToDouble(1000) + " KB");

						}
						else
						{
							MemoryStream m;
							m = Common.BinarySerialize(sessionItem);

							TableHelper.AddCell(dataRow,
												Convert.ToDouble(m.Length + alternativeSerializationStreamSessionKey.Length)
																	/ Convert.ToDouble(1000) + " KB");
						}
					}
					else
					{
						// Make second column span 2 to make up for unused third col
						dataRow.Cells[1].ColumnSpan = 2;
					}

					//Data link column
					if (_HttpHandlerSettings.SessionView.ShowViewDataLink)
					{
						StringWriter swTest = Common.XmlSerialize(sessionItem);

						if (swTest == null)
						{
							//Could not serialize the data in a human readable form using Xml
							TableHelper.AddCell(dataRow, "Data could not be serialized to Xml");
						}
						else
						{
							TableHelper.AddCell(dataRow, "Click to view data", "SessionView.axd?Item=" + sessionKey);
						}
					}
					else
					{
						TableHelper.AddCell(dataRow, "N/A");
					}
				}
				else
				{
					TableHelper.AddCell(dataRow, "N/A");

					if (displayingObjectSize)
					{
						TableHelper.AddCell(dataRow, "N/A");
					}
					else
					{
						dataRow.Cells[1].ColumnSpan = 3;
					}
				}

				if (alternatingRowToggle)
				{
					dataRow.CssClass = "alt";
				}

				alternatingRowToggle = alternatingRowToggle == Convert.ToBoolean(0);
				sessionTable.Rows.Add(dataRow);
			}

			sessionTable.RenderControl(_writer);
		}

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="valueToWrite"></param>
	    /// <returns></returns>
	    private Stream BinaryWrite(object valueToWrite)
		{

			var writer = new BinaryWriter(new MemoryStream());

			Type valueType = valueToWrite.GetType();

            //if (valueToWrite == null)
            //{
            //    writer.Write(16);
            //    return writer.BaseStream;
            //}

			if (valueType == _altSerializationType[0])
			{
				writer.Write(1);
				writer.Write((String)valueToWrite);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[1])
			{
				writer.Write(2);
				writer.Write((Int32)valueToWrite);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[2])
			{
				writer.Write(3);
				writer.Write((Boolean)valueToWrite);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[3])
			{
				writer.Write(4);
			    var tempdateTime = (DateTime)valueToWrite;
				writer.Write(tempdateTime.Ticks);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[4])
			{
				writer.Write(5);
			    int[] decimalBits = Decimal.GetBits((Decimal)valueToWrite);
				int i = 0;
				while (i < 4)
				{
					writer.Write(decimalBits[i]);
					i++;
				}
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[5])
			{
				writer.Write(6);
				writer.Write((Byte)valueToWrite);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[6])
			{
				writer.Write(6);
				writer.Write((Char)valueToWrite);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[7])
			{
				writer.Write(8);
				writer.Write((Single)valueToWrite);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[8])
			{
				writer.Write(9);
				writer.Write((Double)valueToWrite);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[9])
			{
				writer.Write(10);
				writer.Write((Int16)valueToWrite);
				return writer.BaseStream;
			}
			if (valueToWrite == _altSerializationType[10])
			{
				writer.Write(11);
				writer.Write((Int64)valueToWrite);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[11])
			{
				writer.Write(12);
				writer.Write((UInt16)valueToWrite);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[12])
			{
				writer.Write(13);
				writer.Write((UInt32)valueToWrite);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[13])
			{
				writer.Write(14);
				writer.Write((UInt64)valueToWrite);
				return writer.BaseStream;
			}

			if (valueType == _altSerializationType[14])
			{
				writer.Write(10);
				writer.Write((SByte)valueToWrite);
				return writer.BaseStream;
			}

			if (valueType == _altSerializationType[15])
			{
				writer.Write(16);
				var timespanToWrite = (TimeSpan)valueToWrite;
				writer.Write(timespanToWrite.Ticks);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[16])
			{
				writer.Write(17);
				var guidToWrite = (Guid)valueToWrite;
				byte[] guidAsByteArray = guidToWrite.ToByteArray();
				writer.Write(guidAsByteArray);
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[17])
			{
				writer.Write(18);
				var intptrToWrite = (IntPtr)valueToWrite;
				if (IntPtr.Size == 4)
				{
					writer.Write(intptrToWrite.ToInt32());
					return writer.BaseStream;
				}
				writer.Write(intptrToWrite.ToInt64());
				return writer.BaseStream;
			}
			if (valueType == _altSerializationType[18])
			{
				writer.Write(19);
				var uintptrToWrite = (UIntPtr)valueToWrite;
				if (UIntPtr.Size == 4)
				{
					writer.Write(uintptrToWrite.ToUInt32());
					return writer.BaseStream;
				}
				writer.Write(uintptrToWrite.ToUInt64());
				return writer.BaseStream;
			}

			return writer.BaseStream;

		}

		private void DisplaySessionStateItem()
		{
		    //Get requested item from session state
			object sessionItem = _context.Session[_request.QueryString["Item"]];

			Table sessionItemTable = TableHelper.CreateTable();

			//Table Header
			var mainHeadingRow = new TableRow();
			sessionItemTable.Rows.Add(mainHeadingRow);

		    TableCell mainHeading = TableHelper.AddHeaderCell(mainHeadingRow, "<h3><b>Session State Item</b></h3>");
			mainHeading.CssClass = "alt";
			mainHeading.ColumnSpan = 4;

			//Sub heading
			var secondaryHeadingRow = new TableRow();
			secondaryHeadingRow.CssClass = "subhead";
			sessionItemTable.Rows.Add(secondaryHeadingRow);

            //TableCell headingRowCol1 = TableHelper.AddHeaderCell(secondaryHeadingRow, "Session Item Key: " + _request.QueryString["Item"]); -- chrisme: not used!

			//Explanation heading
			var explanationHeadingRow = new TableRow();
			sessionItemTable.Rows.Add(explanationHeadingRow);

			TableHelper.AddCell(explanationHeadingRow, "The outer xml tags are a result of the Xml serialization used to render the item");

		    var dataRow = new TableRow();

			if (sessionItem != null)
			{

				StringWriter swTest = Common.XmlSerialize(sessionItem);

				if (swTest == null)
				{
					//Could not serialize the data in a human readable form using Xml
					TableHelper.AddCell(dataRow, "Item was not viewable");
				}
				else
				{
					TableHelper.AddCell(dataRow, _context.Server.HtmlEncode(swTest.ToString()));
				}
			}
			else
			{
				TableHelper.AddCell(dataRow, "Item is NULL in value");
			}

			sessionItemTable.Rows.Add(dataRow);

			sessionItemTable.RenderControl(_writer);
		}
	}
}
