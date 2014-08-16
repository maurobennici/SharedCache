#region Copyright (c) 2005 - 2007 MergeSystem GmbH, Switzerland. All Rights Reserved
// * --------------------------------------------------------------------- *
// *                            Merge System GmbH                          *
// *              Copyright (c) 2007 All Rights reserved                   *
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
// Name:      UdpExceptions.cs
// 
// Created:   13-07-2007 Merge System GmbH, rschuetz
// Modified:  13-07-2007 Merge System GmbH, rschuetz : Creation
// ************************************************************************* 

using System;
using System.Collections.Generic;
using System.Text;

namespace MergeSystem.Indexus.WinServiceCommon
{
	/// <summary>
	/// Specific UDP Exceptions
	/// </summary>
	public class UdpExceptions : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:UdpExceptions"/> class.
		/// </summary>
		/// <param name="message">The message. A <see cref="T:System.String"/> Object.</param>
		public UdpExceptions(string message) : base(message)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:UdpExceptions"/> class.
		/// </summary>
		/// <param name="message">The message. A <see cref="T:System.String"/> Object.</param>
		/// <param name="innerException">The inner exception. A <see cref="T:System.Exception"/> Object.</param>
		public UdpExceptions(string message, Exception innerException): base(message, innerException)
		{}		
	}
}
