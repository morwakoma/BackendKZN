using SITA.Cryptography.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SITA.Cryptography
{
    public class Cryptograph : ICryptography
    {
        #region "--Declarations--"
        string sKey = "!#$a54?3";
        byte[] key = { };
        byte[] IV = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        #endregion

        #region "--Encrypt Text--"
        /// <summary>
        /// Encrypt Text
        /// </summary>
        /// <param name="sTextToEncrypt">String Text to Ecrypt</param>
        /// <returns></returns>
        /// <remarks><para><b>Created By:</b>Ntshengedzeni Badamarema - 27/03/2018</para></remarks>
        public string EncryptText(string value)
        {
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(sKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(value);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion

        #region "--Encrypt Text--"
        /// <summary>
        /// Decrypt Text
        /// </summary>
        /// <param name="sTextToDecrypt"> String Text to be Decrypted</param>
        /// <returns></returns>
        /// <remarks><para><b>Created By:</b>Ntshengedzeni Badamarema - 27/03/2018</para></remarks>
        public string DecryptText(string value)
        {
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(sKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Convert.FromBase64String(value);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion
    }
}
