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
// Name:      HttpHandlersConfig.cs
// 
// Modified:  28-01-2010 SharedCache.com, chrisme  : clean up code
// ************************************************************************* 

using System.Xml;

namespace SharedCache.WinServiceCommon.HttpHandlers
{
	/// <summary>
	/// Summary description for ConfigSettings.
	/// </summary>
	public class HttpHandlersConfig
	{

		CacheViewSettings CacheViewSettings;
		SessionViewSettings sessionViewSettings;

		internal HttpHandlersConfig(HttpHandlersConfig parent)
		{
			if (parent != null)
			{
				CacheViewSettings = parent.CacheView;
				sessionViewSettings = parent.SessionView;
			}

		}

		internal void LoadValuesFromConfigurationXml(XmlNode node)
		{

			//Retreive the sessionView and CacheView config nodes
			//and create new config classes with the config xmlnode passed into their
			//ctor
			XmlNode sessionViewNode = node.SelectNodes("sessionView")[0];
			XmlNode cacheViewNode = node.SelectNodes("cacheView")[0];

			sessionViewSettings = new SessionViewSettings(sessionViewNode);

			CacheViewSettings = new CacheViewSettings(cacheViewNode);
		}

		/// <summary>
		/// Gets the setting for is the restriction on pages being bookmarkable On or Off by default
		/// </summary>
		public SessionViewSettings SessionView
		{
			get
			{
				return sessionViewSettings;
			}
		}

		/// <summary>
		/// Gets the setting for a Namespace to try and load Pages from
		/// </summary>
		public CacheViewSettings CacheView
		{
			get
			{
				return CacheViewSettings;
			}
		}
	}

    /// <summary>
    /// SessionViewSettings
    /// </summary>
	public class SessionViewSettings : StateViewerSettings
	{
		internal SessionViewSettings(XmlNode sessionViewConfigNode)
			: base(sessionViewConfigNode)
		{
		}
	}

    /// <summary>
    /// CacheViewSettings
    /// </summary>
	public class CacheViewSettings : StateViewerSettings
	{
		internal CacheViewSettings(XmlNode cacheViewConfigNode)
			: base(cacheViewConfigNode)
		{
		}
	}

    /// <summary>
    /// StateViewerSettings
    /// </summary>
	public abstract class StateViewerSettings
	{

        /// <summary>
        /// 
        /// </summary>
		protected bool enabled;
        /// <summary>
        /// 
        /// </summary>
		protected bool showViewDataLink;

		internal StateViewerSettings(XmlNode configNode)
		{
			if (configNode != null)
			{
				XmlAttributeCollection attribCol = configNode.Attributes;

				enabled = attribCol["enabled"].Value.ToLower() == "true";

				showViewDataLink = attribCol["showViewDataLink"].Value.ToLower() == "true";

			}
			else
			{
				enabled = false;
				showViewDataLink = false;
			}
		}

        /// <summary>
        /// Gets a value indicating whether this <see cref="StateViewerSettings"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public virtual bool Enabled
		{
			get
			{
				return enabled;
			}
		}

        /// <summary>
        /// Gets a value indicating whether [show view data link].
        /// </summary>
        /// <value><c>true</c> if [show view data link]; otherwise, <c>false</c>.</value>
		public virtual bool ShowViewDataLink
		{
			get
			{
				return showViewDataLink;
			}
		}
	}
}
