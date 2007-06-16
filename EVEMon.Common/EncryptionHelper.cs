using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EVEMon.Common
{
    
    public static class EncryptionHelper
    {
        private static string m_key = "e8Now%n(7Or;[+ow"; //keep it secret, keep it safe
        private static UTF8Encoding m_encoding = new UTF8Encoding();
        private  static bool _FIPSWarningShown = false;

        public static string Decrypt(string key, string value)
        {
            //set up the encryption
            RijndaelManaged RMCrypto = null;
            try
            {
                RMCrypto = new RijndaelManaged();
            }
            catch (InvalidOperationException)
            {
                if (!_FIPSWarningShown)
                {
                    _FIPSWarningShown = true;
                    System.Windows.Forms.MessageBox.Show("You are running EVEMon on a PC that enforces FIPS compliant encryption.\n This means that EVEMon cannot decrypt your password from your settings file.\nYou will need to go to the character sheet, right-click the thobber and select \"Change Login Information\" for each of your characters.", "Cannot decode password", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                }
                return String.Empty;
                
            }
            string longName = key;
            while (longName.Length < 16)
            {
                longName += longName;
            }

            using (MemoryStream m_plainStream = new MemoryStream())
            using (CryptoStream m_cryptStream = new CryptoStream(m_plainStream,
                                                                 RMCrypto.CreateDecryptor(m_encoding.GetBytes(m_key),
                                                                                          m_encoding.GetBytes(
                                                                                              longName.Substring(0, 16))),
                                                                 CryptoStreamMode.Write))
            {
                byte[] pBytes = Convert.FromBase64String(value);
                m_cryptStream.Write(pBytes, 0, pBytes.Length);
                m_cryptStream.FlushFinalBlock();
                string decrypted = m_encoding.GetString(m_plainStream.ToArray());
                return decrypted;
            }
        }

        public static string Encrypt(string key, string password)
        {
            //set up the encryption
            RijndaelManaged RMCrypto = null;
            try
            {
                RMCrypto = new RijndaelManaged();
            }
            catch (InvalidOperationException)
            {
                // They've got FIPS compliance set! Let the caller decide what to do
                return password;
            }
            string longName = key;
            while (longName.Length < 16)
            {
                longName += longName;
            }

            using (MemoryStream m_plainStream = new MemoryStream())
            using (CryptoStream m_cryptStream = new CryptoStream(m_plainStream,
                                                                 RMCrypto.CreateEncryptor(m_encoding.GetBytes(m_key),
                                                                                          m_encoding.GetBytes(
                                                                                              longName.Substring(0, 16))),
                                                                 CryptoStreamMode.Write))
            {
                byte[] pBytes = m_encoding.GetBytes(password);
                m_cryptStream.Write(pBytes, 0, pBytes.Length);
                m_cryptStream.FlushFinalBlock();
                string encrypted = Convert.ToBase64String(m_plainStream.ToArray());
                return encrypted;
            }
        }
    }
}