
namespace SharedCache.WinServiceCommon.Provider.Encryption
{
    /// <summary>
    /// IEncryptionProvider
    /// </summary>
	public interface IEncryptionProvider
	{
		///<summary>
		///</summary>
		///<param name="plainText"></param>
		///<returns></returns>
		byte[] Encrypt(byte[] plainText);

		///<summary>
		///</summary>
		///<param name="encryptedData"></param>
		///<returns></returns>
		byte[] Decrypt(byte[] encryptedData);
	}
}
