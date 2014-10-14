using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Messenger.utils
{
    public sealed class CriptoUtils
    {
        static readonly string salt = "Kosher";
        static readonly string hashAlgorithm = "SHA1";
        static readonly string initialVector = "OFRna73m*aze01xY";

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

        public static void Generic(ref char[] buf)
        {
            Random rand = new Random();
            for (int i = 0; i < 32; i++)
                buf[i] = (char)rand.Next(32, 255);
        }

        public static string Encrypt(string plainText, string password, int passwordIterations, int keySize)
        {
            if (string.IsNullOrEmpty(plainText)) return "";

            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
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

        public static string Decrypt(string cipherText, string password, int passwordIterations, int keySize)
        {
            if (string.IsNullOrEmpty(cipherText)) return "";

            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
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
