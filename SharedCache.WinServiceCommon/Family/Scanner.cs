#region Copyright (c) 2005 - 2006 MergeSystem GmbH, Switzerland. All Rights Reserved
// * --------------------------------------------------------------------- *
// *                            Merge System GmbH                          *
// *              Copyright (c) 2006 All Rights reserved                   *
// *                                                                       *
// *                                                                       *
// * This file and its contents are protected by Swiss and International   *
// * copyright laws. Unauthorized reproduction and/or distribution of all  *
// * or any portion of the code contained herein is strictly prohibited    *
// * and will result in severe civil and criminal penalties. Any           *
// * violations of this copyright will be prosecuted to the fullest        *
// * extent possible under law.                                            *
// *                                                                       *
// * THE SOURCE CODE CONTAINED HEREIN AND IN RELATED FILES IS PROVIDED     *
// * TO AUTHORIZED CUSTOMERS FOR THE PURPOSES OF EDUCATION AND             *
// * TROUBLESHOOTING. UNDER NO CIRCUMSTANCES MAY ANY PORTION OF THE SOURCE *
// * CODE BE DISTRIBUTED, DISCLOSED OR OTHERWISE MADE AVAILABLE TO ANY     *
// * THIRD PARTY WITHOUT THE EXPRESS WRITTEN CONSENT OF Merge System GMBH. *
// *                                                                       *
// * UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *
// * PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *
// * SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY Merge System GMBH        *
// * PRODUCT.                                                              *
// *                                                                       *
// * THE AUTHORIZED CUSTOMER ACKNOWLEDGES THAT THIS SOURCE CODE            *
// * CONTAINS VALUABLE AND PROPRIETARY TRADE SECRETS OF Merge System GMBH, *
// * THE AUTHORIZED CUSTOMER AGREES TO EXPEND EVERY EFFORT TO INSURE       *
// * ITS CONFIDENTIALITY.                                                  *
// *                                                                       *
// * THE LICENSE AGREEMENT ACCOMPANYING THE PRODUCT DOES NOT PROVIDE ANY   *
// * RIGHTS REGARDING THE SOURCE CODE CONTAINED HEREIN.                    *
// *                                                                       *
// * THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *
// * --------------------------------------------------------------------- *
#endregion 

// *************************************************************************
//
// Name:      Scanner.cs
// 
// Created:   17-06-2007 Merge System GmbH, rschuetz
// Modified:  17-06-2007 Merge System GmbH, rschuetz : Creation
// ************************************************************************* 
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;

using COM = MergeSystem.Indexus.WinServiceCommon;


namespace MergeSystem.Indexus.WinServiceCommon.Family
{
	/// <summary>
	/// Handling the network scan's
	/// </summary>
	public class Scanner
	{	
		/// <summary>
		/// Contains all Shared Cache Familiy List
		/// </summary>
		public static List<string> Familiy = new List<string>();
		
		/// <summary>
		/// Starts the scan over network segment.
		/// </summary>
		public void StartFamilyScan()
		{
			COM.Udp.UDPBroadcaster broadcaster = new COM.Udp.UDPBroadcaster();
			Familiy = broadcaster.Broadcast();
		}
	}
}
