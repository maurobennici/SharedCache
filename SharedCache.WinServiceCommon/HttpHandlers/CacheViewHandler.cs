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
// Name:      CacheViewHandler.cs
// 
// Modified:  28-01-2010 SharedCache.com, chrisme  : clean up code
// ************************************************************************* 

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web;
using System.Collections;

namespace SharedCache.WinServiceCommon.HttpHandlers
{
	/// <summary>
	/// This class is an HttpHandler and reports on the ASP.NET Cache
	/// </summary>
	public class CacheViewHandler : IHttpHandler
	{

		private HttpContext _context;
		private HttpRequest _request;
		private HttpResponse _response;
		private HtmlTextWriter _writer;

		private HttpHandlersConfig _httpHandlerSettings;

        private const string pageStyle = @"<style> {background-color:white; color:black;	font: 10pt verdana, arial;}
				table {font: 10pt verdana, arial; cellspacing:0; 	cellpadding:0; 	margin-bottom:25}
				tr.subhead { background-color:cccccc;}
				th { padding:0,3,0,3 }
				th.alt { background-color:black; color:white; padding:3,3,2,3; }
				td { padding:0,3,0,3 }
				tr.alt { background-color:eeeeee }
				td.duplicate {background-color:red }
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


		#region IHttpHandler Members

		/// <summary>
		/// Standard HttpHandler Entry point. Coordinate the displaying of the Cache View
		/// </summary>
		/// <param name="context">The current HttpContext</param>
		public void ProcessRequest(HttpContext context)
		{

			if (Common.RequestIsLocal(context.Request) == false)
			{
				context.AddError(new ApplicationException("CacheView can only be accessed locally i.e. localhost"));
				return;
			}

			_httpHandlerSettings = (HttpHandlersConfig)context.GetSection("sharedCache/httpHandlers");

			if (_httpHandlerSettings == null ||
					(_httpHandlerSettings.CacheView == null) ||
					(_httpHandlerSettings.CacheView.Enabled == false))
			{
				//Cache View is not enabled fire exception
				context.AddError(new ApplicationException(@"CacheView is not enabled. See the sharedCache/httpHandlers section in your configuration file", null));
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

			if (context.Request.QueryString["Item"] == null)
			{
				//An item specific requets is NOT being made. Display the lot
				EnumerateAndDisplayCache();
			}
			else
			{
				//A cache item specific request is being made. Try and display it
				DisplayCacheItem();
			}

			_writer.Write("\r\n</body>\r\n</html>\r\n");
		}

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		#endregion

		private void DisplayCacheItem()
		{
		    //Get requested item from cache
			object cacheItem = _context.Cache[_request.QueryString["Item"]];

			Table sessionItemTable = TableHelper.CreateTable();

			//Table Header
			var mainHeadingRow = new TableRow();
			sessionItemTable.Rows.Add(mainHeadingRow);

		    TableCell mainHeading = TableHelper.AddHeaderCell(mainHeadingRow, "<h3><b>Cache Item</b></h3>");
			mainHeading.CssClass = "alt";
			mainHeading.ColumnSpan = 4;

			//Sub heading
			var secondaryHeadingRow = new TableRow();
			secondaryHeadingRow.CssClass = "subhead";
			sessionItemTable.Rows.Add(secondaryHeadingRow);

            //TableCell headingRowCol1 = TableHelper.AddHeaderCell(secondaryHeadingRow, "Cache Item Key: " + m_request.QueryString["Item"]); -- chrisme: <- not used!

			//Explanation heading
			var explanationHeadingRow = new TableRow();
			sessionItemTable.Rows.Add(explanationHeadingRow);

			TableHelper.AddCell(explanationHeadingRow, "The outer xml tags are a result of the Xml serialization used to render the item<BR><BR>");

		    var dataRow = new TableRow();

			if (cacheItem != null)
			{

				StringWriter swTest = Common.XmlSerialize(cacheItem);

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
		private void EnumerateAndDisplayCache()
		{

			Table cacheTable = TableHelper.CreateTable();

			//Table Header
			var mainHeadingRow = new TableRow();
			cacheTable.Rows.Add(mainHeadingRow);

		    TableCell mainHeading = TableHelper.AddHeaderCell(mainHeadingRow, "<h3><b>Cache Details</b></h3>");
			mainHeading.ColumnSpan = 3;
			mainHeading.CssClass = "alt";

			var secondaryHeadingRow = new TableRow();
			secondaryHeadingRow.CssClass = "subhead";
			cacheTable.Rows.Add(secondaryHeadingRow);

            //TableCell headingRowCol1 = TableHelper.AddHeaderCell(secondaryHeadingRow, "Cache Key"); -- chrisme: <- not used!

            //TableCell headingRowCol2 = TableHelper.AddHeaderCell(secondaryHeadingRow, "Cached Object Type"); -- chrisme: <- not used!

            //TableCell headingRowCol3 = TableHelper.AddHeaderCell(secondaryHeadingRow, "View Data"); -- chrisme: <- not used!

			bool alternatingRowToggle = false;
            //var enumeratedItems = new ArrayList(); -- chrisme: <- not used!
			TableRow dataRow;

			foreach (DictionaryEntry cacheDictionaryItem in _context.Cache)
			{
				dataRow = new TableRow();

				//Key column
				TableHelper.AddCell(dataRow, cacheDictionaryItem.Key.ToString());

				if (_context.Cache[cacheDictionaryItem.Key.ToString()] != null)
				{

					//Get out of cache
					Object cacheObjectItem = _context.Cache[cacheDictionaryItem.Key.ToString()];

                    //System.Type type = cacheObjectItem.GetType(); -- chrisme: <- not used!

					//Type column
					TableHelper.AddCell(dataRow, cacheObjectItem.GetType().FullName);

				}
				else
				{
					//Type column
					TableHelper.AddCell(dataRow, "NULL");
				}

				//Data link column
				if (_httpHandlerSettings.CacheView.ShowViewDataLink)
				{
					StringWriter swTest = Common.XmlSerialize(cacheDictionaryItem.Value);

					if (swTest == null)
					{
						//Could not serialize the data in a human readable form using Xml
						TableHelper.AddCell(dataRow, "Data could not be serialized to Xml");
					}
					else
					{
						TableHelper.AddCell(dataRow, "Click to view data", "CacheView.axd?Item=" + cacheDictionaryItem.Key);
					}
				}
				else
				{
					TableHelper.AddCell(dataRow, "N/A");
				}

				if (alternatingRowToggle)
				{
					dataRow.CssClass = "alt";
				}

				alternatingRowToggle = alternatingRowToggle == Convert.ToBoolean(0);
				cacheTable.Rows.Add(dataRow);
			}

			cacheTable.RenderControl(_writer);
		}
	}
}
