using System;
using System.Security.Cryptography;
using System.Text;

namespace EVEMon.Common.Helpers
{
    /// <summary>
    /// Provides encryption/decryption for sensitive credential data using Windows DPAPI.
    /// Data encrypted with this class can only be decrypted by the same Windows user
    /// on the same machine.
    /// </summary>
    public static class CredentialProtection
    {
        // Marker prefix to identify encrypted data
        private const string EncryptedMarker = "DPAPI:";

        // Optional entropy for additional protection (application-specific)
        private static readonly byte[] s_entropy = Encoding.UTF8.GetBytes("EVEMon.Credentials.v1");

        /// <summary>
        /// Encrypts a string using Windows DPAPI (CurrentUser scope).
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <returns>Base64-encoded encrypted string with marker prefix, or null if input is null/empty.</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            try
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = ProtectedData.Protect(
                    plainBytes,
                    s_entropy,
                    DataProtectionScope.CurrentUser);

                // Clear plain bytes from memory
                Array.Clear(plainBytes, 0, plainBytes.Length);

                return EncryptedMarker + Convert.ToBase64String(encryptedBytes);
            }
            catch (CryptographicException ex)
            {
                EveMonClient.Trace($"CredentialProtection.Encrypt failed: {ex.Message}");
                // Return null to indicate encryption failure - caller should handle
                return null;
            }
        }

        /// <summary>
        /// Decrypts a DPAPI-encrypted string.
        /// </summary>
        /// <param name="encryptedText">The encrypted text (with DPAPI: prefix).</param>
        /// <returns>Decrypted string, or the original string if not encrypted, or null on failure.</returns>
        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return encryptedText;

            // If not encrypted (legacy data), return as-is for migration
            if (!IsEncrypted(encryptedText))
                return encryptedText;

            try
            {
                string base64Data = encryptedText.Substring(EncryptedMarker.Length);
                byte[] encryptedBytes = Convert.FromBase64String(base64Data);
                byte[] decryptedBytes = ProtectedData.Unprotect(
                    encryptedBytes,
                    s_entropy,
                    DataProtectionScope.CurrentUser);

                string result = Encoding.UTF8.GetString(decryptedBytes);

                // Clear decrypted bytes from memory
                Array.Clear(decryptedBytes, 0, decryptedBytes.Length);

                return result;
            }
            catch (CryptographicException ex)
            {
                // This happens when trying to decrypt on a different machine/user
                EveMonClient.Trace($"CredentialProtection.Decrypt failed (expected on new machine): {ex.Message}");
                return null;
            }
            catch (FormatException ex)
            {
                // Invalid base64
                EveMonClient.Trace($"CredentialProtection.Decrypt failed (invalid format): {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if a string is DPAPI-encrypted (has the marker prefix).
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if the text appears to be encrypted.</returns>
        public static bool IsEncrypted(string text)
        {
            return !string.IsNullOrEmpty(text) && text.StartsWith(EncryptedMarker, StringComparison.Ordinal);
        }

        /// <summary>
        /// Attempts to decrypt, returning a flag indicating if re-authentication is needed.
        /// </summary>
        /// <param name="encryptedText">The encrypted text.</param>
        /// <param name="decryptedText">The decrypted text, or null if decryption failed.</param>
        /// <returns>True if decryption succeeded or data was unencrypted, false if re-auth needed.</returns>
        public static bool TryDecrypt(string encryptedText, out string decryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                decryptedText = encryptedText;
                return true;
            }

            // Unencrypted legacy data - return as-is (will be encrypted on next save)
            if (!IsEncrypted(encryptedText))
            {
                decryptedText = encryptedText;
                return true;
            }

            // Try to decrypt
            decryptedText = Decrypt(encryptedText);
            return decryptedText != null;
        }
    }
}
