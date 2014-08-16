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
// Name:      FamilyServerScanner.cs
// 
// Created:   17-07-2007 Merge System GmbH, rschuetz
// Modified:  17-07-2007 Merge System GmbH, rschuetz : Creation
// ************************************************************************* 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Net;
using System.Net.Sockets;

using COM = MergeSystem.Indexus.WinServiceCommon;
using COMO = MergeSystem.Indexus.WinServiceCommon.Udp.Command;

namespace MergeSystem.Indexus.WinServiceCommon.Handler
{
	/// <summary>
	/// Searching additional Servers within network
	/// for exchaning data.
	/// </summary>
	public class FamilyServerScanner
	{
		public event FoundScannerDelegate FoundScannerDelegateSubscriber;

		#region Properties
		#region Property: Listener
		private COM.Udp.BroadcastListener listener;

		/// <summary>
		/// Gets/sets the Listener
		/// </summary>
		public COM.Udp.BroadcastListener Listener
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return this.listener; }

			[System.Diagnostics.DebuggerStepThrough]
			set { this.listener = value; }
		}
		#endregion

		#region Property: Servers
		private List<string> servers = new List<string>();

		/// <summary>
		/// Gets/sets the Servers. Servers are searched over UDP within the network
		/// and will be registerd in this List.
		/// </summary>
		public List<string> Servers
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return this.servers; }

			[System.Diagnostics.DebuggerStepThrough]
			set { this.servers = value; }
		}
		#endregion

		#region Property: SetServerListenerPort
		private bool setServerListenerPort = false;

		/// <summary>
		/// Gets/sets the SetServerListenerPort
		/// </summary>
		public bool SetServerListenerPort
		{
			[System.Diagnostics.DebuggerStepThrough]
			get { return this.setServerListenerPort; }

			[System.Diagnostics.DebuggerStepThrough]
			set { this.setServerListenerPort = value; }
		}
		#endregion

		/// <summary>
		/// Defines the founded port, initialization happens over an 
		/// offset, and will be assigned by the founded port.
		/// </summary>
		private int foundedFreePort = 1;

		#endregion Properties

		#region CTOR's
		/// <summary>
		/// Initializes a new instance of the <see cref="T:FamilyServerScanner"/> class.
		/// </summary>
		public FamilyServerScanner()
		{
			#region Logging
			if (1 == COM.Handler.Config.GetIntValueFromConfigByKey(COM.Constants.ConfigLoggingEnable))
			{
				string msg = "Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;";
				Handler.LogHandler.Tracking(msg);
				Console.WriteLine(msg);
			}
			#endregion Logging
			this.InitListener();
		}
		#endregion CTOR's

		#region Public Methods

		public void Shutdown()
		{
			#region Logging
			if (1 == COM.Handler.Config.GetIntValueFromConfigByKey(COM.Constants.ConfigLoggingEnable))
			{
				string msg = "Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;";
				Handler.LogHandler.Tracking(msg);
			}
			Handler.LogHandler.Info(string.Format(@"Shutdown: {0}", this.listener.IpAddressAndPort));

			// this.listener.Dispose();

			#endregion Logging
		}

		/// <summary>
		/// Search over UDP for additional installations over UDP on the Network.
		/// All founded Servers are populated in the Servers Property.
		/// </summary>
		public void Scanner()
		{
			#region Logging
			if (1 == COM.Handler.Config.GetIntValueFromConfigByKey(COM.Constants.ConfigLoggingEnable))
			{
				string msg = "Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;";
				Handler.LogHandler.Tracking(msg);
				Console.WriteLine(msg);
			}
			#endregion Logging

			#region verify listener
			if (this.Listener == null)
			{
				string msg = "Listener were not initialized! Scanner cannot be started without a listener.";
				Handler.LogHandler.Error(msg);
				Console.WriteLine(msg);
				return;
			}
			#endregion verify listener

			#region Logging Listener Information
			if (1 == COM.Handler.Config.GetIntValueFromConfigByKey(COM.Constants.ConfigLoggingEnable))
			{
				Handler.LogHandler.Info(this.listener.IpAddressAndPort);
				Console.WriteLine(this.listener.IpAddressAndPort);
			}
			#endregion Logging Listener Information

			IPHostEntry localMachineInfo = Dns.GetHostEntry(Dns.GetHostName());

			// starting broadcaster
			COM.Udp.Broadcast.BroadcastCommand(
				new COM.Udp.Command.AppCommand(
					COM.Udp.Command.SystemCommandOption.PingClients,
					localMachineInfo.AddressList[0].ToString(),
					this.foundedFreePort
				)
			);
		}
		#endregion Public Methods

		#region Private Methods
		private void InitListener()
		{
			#region Logging
			if (1 == COM.Handler.Config.GetIntValueFromConfigByKey(COM.Constants.ConfigLoggingEnable))
			{
				string msg = "Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;";
				Handler.LogHandler.Tracking(msg);
			}
			#endregion Logging

			bool listenerCreated = false;
			int portCounter = 1;

			do
			{
				// starting listener
				try
				{
					this.listener = new COM.Udp.BroadcastListener(new COM.Udp.CommandReceived(ReceivedCommand), COM.Ports.ClientPortUdp(portCounter));
					if (this.listener != null)
					{
						foundedFreePort = COM.Ports.ClientPortUdp(portCounter);
						listenerCreated = true;
					}
				}
				catch (Exception ex)
				{
					#region Logging
					string msg = ex.Message + Environment.NewLine + ex.StackTrace;
					Handler.LogHandler.Info(string.Format(@"Port: {0} has been taken. Search next port.", COM.Ports.ClientPortUdp(portCounter)) + Environment.NewLine + msg, ex);
					Console.WriteLine(string.Format(@"Port: {0} is in use. Search next port.", COM.Ports.ClientPortUdp(portCounter)));
					#endregion Logging
					// increase counter for next port
					portCounter++;
				}

				System.Threading.Thread.Sleep(10);
			} while (!listenerCreated || portCounter >= 10);

			if (portCounter >= 10)
			{
				throw new COM.UdpExceptions("Could not found a free Port to listen to it!!");
			}
		}

		/// <summary>
		/// Receiveds UDP Commands from founded Servers and Acknowledge it; 
		/// </summary>
		/// <param name="cmd">The CMD. A <see cref="T:MergeSystem.Indexus.WinServiceCommon.Udp.Command.AppCommand"/> Object.</param>
		private void ReceivedCommand(COM.Udp.Command.AppCommand cmd)
		{
			#region Logging
			if (1 == COM.Handler.Config.GetIntValueFromConfigByKey(COM.Constants.ConfigLoggingEnable))
			{
				string msg = "Access Method: " + this.GetType().ToString() + "->" + ((object)MethodBase.GetCurrentMethod()).ToString() + " ;";
				Handler.LogHandler.Tracking(msg);
			}
			#endregion Logging

			IPHostEntry localMachineInfo = Dns.GetHostEntry(Dns.GetHostName());

			if (cmd.CommandType == COMO.SystemCommandOption.PongClients)
			{
				if (cmd.Args.Length == 3 && cmd.Args[2] != null)
				{
					if (!this.servers.Contains(cmd.Args[2].ToString()))
					{
						this.servers.Add(cmd.Args[2].ToString());
					}

					#region Logging
					if (1 == COM.Handler.Config.GetIntValueFromConfigByKey(COM.Constants.ConfigLoggingEnable))
					{
						string msg = "Founded Server [IP:" + cmd.Args[2].ToString() + "]";
						Handler.LogHandler.Info(msg);
						Handler.LogHandler.Traffic(msg);
						Console.WriteLine(msg);
					}
					#endregion Logging


					// Server events are acknowledged via UDP so that the server does not
					// call into us time after time. 
					//COM.Udp.Broadcast.BroadcastCommand(
					//  new COM.Udp.Command.AppCommand(
					//    COMO.SystemCommandOption.PongClientAcknowledge,
					//    localMachineInfo.AddressList[0].ToString(),
					//    this.foundedFreePort
					//  )
					//);

					COM.Udp.Broadcast.BroadcastCommand(
						new COM.Udp.Command.AppCommand(
							COMO.SystemCommandOption.PongClientAcknowledge,
							localMachineInfo.AddressList[0].ToString(),
							cmd.Args[1].ToString()/*this.foundedFreePort*/
						),
						cmd.Args[2].ToString()
					);

					if (FoundScannerDelegateSubscriber != null)
					{
						Console.WriteLine("Notify Event Subscribers");
						FoundScannerDelegateSubscriber(new ServerInfo(cmd.Args[2].ToString()));
					}
					
				}
			}
		}
		#endregion Private Methods
	}
}
