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
// Name:      IndexusServerSettingElement.cs
// 
// Created:   24-02-2008 SharedCache.com, rschuetz
// Modified:  24-02-2008 SharedCache.com, rschuetz : Creation
// Modified:  28-01-2010 SharedCache.com, chrisme  : clean up code
// ************************************************************************* 

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SharedCache.WinServiceCommon.Provider.Encryption
{
	internal class EncryptionProvider : IEncryptionProvider
	{
		// private static string hashSalt = "A sweet secret password";
		private static readonly string HashSalt = Provider.Cache.IndexusDistributionCache.ProviderSection.ClientSetting.EncryptionProviderHashSalt;

		public byte[] Encrypt(byte[] plainText)
		{

			if (string.IsNullOrEmpty(HashSalt))
				return plainText;

			var rijndaelCipher = new RijndaelManaged();
			byte[] salt = Encoding.ASCII.GetBytes(HashSalt.Length.ToString());

			var secretKey = new PasswordDeriveBytes(HashSalt, salt);
			ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
			var memoryStream = new MemoryStream();
			var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

			// Start the encryption process.
			cryptoStream.Write(plainText, 0, plainText.Length);

			// Finish encrypting.
			cryptoStream.FlushFinalBlock();

			// Convert our encrypted data from a memoryStream into a byte array.
			byte[] cipherBytes = memoryStream.ToArray();

			// Close both streams.
			memoryStream.Close();
			cryptoStream.Close();

			return cipherBytes;
		}

		public byte[] Decrypt(byte[] encryptedData)
		{
			if (string.IsNullOrEmpty(HashSalt))
				return encryptedData;

			var rijndaelCipher = new RijndaelManaged();

			byte[] salt = Encoding.ASCII.GetBytes(HashSalt.Length.ToString());

			var secretKey = new PasswordDeriveBytes(HashSalt, salt);

			// Create a decryptor from the existing SecretKey bytes.
			ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
			var memoryStream = new MemoryStream(encryptedData);

			// Create a CryptoStream. (always use Read mode for decryption).
			var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

			// Since at this point we don't know what the size of decrypted data
			// will be, allocate the buffer long enough to hold EncryptedData;
			// DecryptedData is never longer than EncryptedData.
			var plainText = new byte[encryptedData.Length];

			// Start decrypting.
			int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);

			memoryStream.Close();
			cryptoStream.Close();

			if (decryptedCount == 0) return null;

			//get rid of extra
			var returnBytes = new List<byte>(plainText);
			returnBytes.RemoveRange(decryptedCount, returnBytes.Count - decryptedCount);

			// Return decrypted data 
			return returnBytes.ToArray();
		}

	}
}
