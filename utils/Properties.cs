using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using MessageException;
 
namespace Messenger.utils
{

    public sealed class Properties
    {
        private const string SERVER_PORT = "SERVER_PORT";
        private const string CLIENT_IP = "CLIENT_IP";
        private const string CLIENT_PORT = "CLIENT_PORT";
        //в формате a:x, где a принадлжеит [2-9], x принадлежит [2-40000]
        private const string PUBLIC_KEY = "PUBLIC_KEY";
        //не считываем из файла, а генерируем во время первого соединения
        private const string PRIVATE_KEY = "PRIVATE_KEY";
        //не считываем из файла, а генерируем во время первого соединения
        private const string SESSION_KEY = "SESSION_KEY";

        private const string FILE_CONFIG = "conf/conf.properties";

        public const string BEGIN = "begin";
        public const string YES_COMMUNICATION = "ok";
        public const string NO_COMMUNICATION = "no";
        public const string TEST_COMMUNICATON = "test";


        private static SortedDictionary<string, string> dictionary = new SortedDictionary<string, string>();
        
        //первоначальная загрузка параметров конфигурации сети
        public static void load()
        {
            if (string.IsNullOrWhiteSpace(FILE_CONFIG)) return;

            string line = null;
            StreamReader reader = new StreamReader(FILE_CONFIG);
            while ((line = reader.ReadLine()) != null)
            {
                string[] param = line.Split('=');
                if(param.Length == 2) 
                {
                    Console.WriteLine(param[0].Trim() + "   " + param[1].Trim());
                    dictionary.Add(param[0].Trim(), param[1].Trim());
                }
            }
        }

        public static string getServerPort()
        {
            return getValue(SERVER_PORT);       
        }

        public static string getClientIp()
        {
            return getValue(CLIENT_IP);
        }

        public static string getClientPort()
        {
            return getValue(CLIENT_PORT);
        }

        public static PublicKey getPublicKey()
        {
            string[] res = getValue(PUBLIC_KEY).Split(':');
            if (res.Length != 2 && res[0].Length != 32) throw new InitilizationException("invalid public key");
            int mod = int.Parse(res[1]);
            if (mod < 2 && mod > 44000) throw new InitilizationException("invalid public key");

            return new PublicKey(res[0], mod);
        }

        public static string getSessionKey()
        {
            return getValue(SESSION_KEY);
        }

        public static void setSessionKey(string key)
        {
            string value;
            if (dictionary.TryGetValue(SESSION_KEY, out value))
            {
                dictionary[SESSION_KEY] = key;
            }
            else
            {
                dictionary.Add(SESSION_KEY, key);
            }
        }


        public static string getPrivateKey()
        {
            return getValue(PRIVATE_KEY);
        }

        public static void setPrivateKey(string key)
        {
            string value;
            if (dictionary.TryGetValue(PRIVATE_KEY, out value))
            {
                dictionary[PRIVATE_KEY] = key;
            }
            else
            {
                dictionary.Add(PRIVATE_KEY, key);
            } 
        }

        private static string getValue(string name)
        {
            string value;
            if (dictionary.TryGetValue(name, out value)) return value;
            else throw new InitilizationException("Properties " + name + " not found");
        }

    }

    public struct PublicKey
    {
        public string body;
        public int mod;

        public PublicKey(string body, int mod)
        {
            this.body = body;
            this.mod = mod;
        }
    }
}
