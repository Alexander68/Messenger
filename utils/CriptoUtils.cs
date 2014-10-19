using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using MessageException;

namespace Messenger.utils
{
    public sealed class CriptoUtils
    {
        private const string salt = "Kosher";
        private const string hashAlgorithm = "SHA1";
        private const string initialVector = "OFRna73m*aze01xY";
        private const int countChars = 32;

        public const int PASSWORD_ITER = 2;
        public const int KEY_SIZE = 256;


        public static char powMod(int bas, int power, int mod)
        {
            int sum = 1;
            for (int i = power; i > 0; i--)
            {
                sum = (sum * bas) % mod;
            }
            return (char)sum;
        }

        public static int ReadPublicKey(ref char[] buf)
        {
            FileStream outStream = null;
            int symbol, elements = 0;
            try
            {
                outStream = new FileStream("file.txt", FileMode.Open, FileAccess.Read);
                do
                {
                    symbol = outStream.ReadByte();
                    if (symbol != -1)
                    {
                        buf[elements] = (char)symbol;
                        elements++;
                    }
                }
                while (symbol != -1);
                return elements;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
            finally
            {
                closeStream(outStream);
            }

        }

        public static char[] Generic(int len)
        {
            char[] buf = new char[len];
            Random rand = new Random();
            for (int i = 0; i < len; i++)
            {
                buf[i] = (char)rand.Next(2, 255);
            }
            return buf;
        }

        public static string Encrypt(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText)) return "";

            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, PASSWORD_ITER);
            byte[] keyBytes = derivedPassword.GetBytes(KEY_SIZE / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;

            byte[] cipherTextBytes = null;
            MemoryStream memStream = null;
            CryptoStream cryptoStream = null;
            try
            {
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes);
                memStream = new MemoryStream();
                cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                cipherTextBytes = memStream.ToArray();

                return Convert.ToBase64String(cipherTextBytes);
            }
            finally
            {
                closeStream(memStream);
                closeStream(cryptoStream);
                symmetricKey.Clear();
            }
        }

        public static string Decrypt(string cipherText, string password)
        {
            if (string.IsNullOrEmpty(cipherText)) return "";

            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, PASSWORD_ITER);
            byte[] keyBytes = derivedPassword.GetBytes(KEY_SIZE / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int byteCount = 0;

            MemoryStream memStream = null;
            CryptoStream cryptoStream = null;
            try
            {
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes);
                memStream = new MemoryStream(cipherTextBytes);
                cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read);
                byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
            }
            finally
            {
                closeStream(memStream);
                closeStream(cryptoStream);
                symmetricKey.Clear();
            }
        }

        public static char[] createSesionKey(PublicKey key, char[] powKey)
        {
            char[] bas = key.body.ToCharArray();
            char[] res = new char[countChars];
            for (int i = 0; i < powKey.Length; i++)
            {
                res[i] = CriptoUtils.powMod(bas[i], (int)powKey[i], key.mod);
            }
            return res;
        }

        private static void closeStream(Stream stream)
        {
            try
            {
                if (stream != null) stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                //ignored this exception
            }
        }
    }
}
