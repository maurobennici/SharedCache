using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace MergeSystem.Indexus.WinServiceCommon.Handler
{
	/// <summary>
	/// <b>RAW calculation</b>
	/// </summary>
	public class Raw
	{
		/// <summary>
		/// defines the max. size
		/// </summary>
		private int m_nMaxDataSize = 4 * 1024 * 1024;

		/// <summary>
		/// Sends the binary data.
		/// </summary>
		/// <param name="socket">The socket.</param>
		/// <param name="data">The data.</param>
		/// <returns>a <see cref="bool"/> object.</returns>
		public bool SendBinaryData(Socket socket, byte[] data)
		{
			Byte[] result = new Byte[data.Length + 4];
			result[0] = (Byte)(1 + (data.Length / 16777216) * 16);
			result[1] = (Byte)(data.Length % 256);
			result[2] = (Byte)((data.Length % 65536) / 256);
			result[3] = (Byte)((data.Length / 65536) % 256);
			data.CopyTo(result, 4);
			return SendRawData(socket, result);
		}

		/// <summary>
		/// Sends the raw data.
		/// </summary>
		/// <param name="socket">The socket.</param>
		/// <param name="data">The data.</param>
		/// <returns>a <see cref="bool"/> object.</returns>
		private bool SendRawData(Socket socket, byte[] data)
		{
			try
			{
				return socket.Send(data) == data.Length;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Handler.LogHandler.Error(ex.Message + Environment.NewLine + ex.StackTrace);
				return false;
			}
			finally
			{
			}
		}

		/// <summary>
		/// Receives the data.
		/// </summary>
		/// <param name="socket">The socket.</param>
		/// <returns>a array of <see cref="object"/> objects.</returns>
		public object[] ReceiveData(Socket socket)
		{
			try
			{
				socket.Blocking = false;
				int read = 0;
				int total = 4;
				Byte[] data = new Byte[4];
				Byte[] header = null;
				while (true)
				{
					try
					{
						if (socket.Available > 0)
						{
							read += socket.Receive(data, read, total - read, SocketFlags.None);
							if ((data[0] & 0x0000000F) == 2)
							{
								read = 0;
							}
						}
					}
					catch (Exception)
					{

					}
					if (header == null && read == 4)
					{
						total = data[1] + data[2] * 256 + data[3] * 65536 + (data[0] / 16) * 16777216;
						if ((data[0] & 0x0000000F) > 1)
						{
							throw new Exception("Invalid input data type byte!");
						}
						if (total > m_nMaxDataSize)
						{
							throw new Exception("Data size too large");
						}

						header = data;
						read = 0;
						data = new Byte[total];
					}
					if (header != null && read == total)
					{
						break;
					}
					#region commented
					//if (((DateTime.Now.Ticks - nStart) / 10000) > m_nReadTimeout * 1000)
					//{
					//  throw new Exception("Timeout while receiving incoming data");
					//}
					#endregion commented
				}
				if ((header[0] & 0x0000000F) == 1)
				{
					return new Object[2] { data, null };
				}
				else
				{
					if (data.Length % 2 != 0)
					{
						throw new Exception("Invalid string data size");
					}
					return new Object[2] { null, BinaryToString(data) };
				}
			}
			catch (Exception ex)
			{
				Handler.LogHandler.Error(ex.Message);
				return null;
			}
			finally
			{
				socket.Blocking = true;
			}
		}

		/// <summary>
		/// Binaries to string helper method
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>a <see cref="string"/> object.</returns>
		public static string BinaryToString(byte[] data)
		{
			if ((data.Length % 2) != 0)
			{
				throw new Exception("Invalid string data size");
			}
			
			char[] ch = new char[data.Length / 2];
			for (int i = 0; i < ch.Length; i++)
			{
				ch[i] = (char)(data[2 * i] + data[2 * i + 1] * 256);
			}
			return new string(ch);
		}
	}



}
