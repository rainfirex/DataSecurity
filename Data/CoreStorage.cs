using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using DataSecurity.Models;
using DataSecurity.Modules;
using Newtonsoft.Json;
using Pinger.Modules;

namespace DataSecurity.Data
{
    class CoreStorage
    {
        private Config _cfg;

        private IniFile INI;

        private const string FILE = "database";

        private const string FILE_SHARED_SECRET = "key";

        private const string FILE_CONFIG = "config.ini";

        private const string DIRECTORY_DATA = "data";

        private const string DIRECTORY_BACKUP = "backup";

        private string _sharedSecret;

        private readonly static string PATH_DATABASE = $"{Environment.CurrentDirectory}\\{DIRECTORY_DATA}\\{FILE}";

        private readonly static string PATH_SHARED_SECRET = $"{Environment.CurrentDirectory}\\{DIRECTORY_DATA}\\{FILE_SHARED_SECRET}";

        private readonly static string PATH_CONFIG = $"{Environment.CurrentDirectory}\\{DIRECTORY_DATA}\\{FILE_CONFIG}";

        public String SharedSecret
        {
            get { return _sharedSecret; }
            set { _sharedSecret = value; }
        }

        // Конструктор
        public CoreStorage(Config cfg, string sharedSecret = null)
        {
            _cfg = cfg;

            _sharedSecret = sharedSecret;

            if (!Directory.Exists($"{Environment.CurrentDirectory}\\{DIRECTORY_DATA}"))
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\{DIRECTORY_DATA}");

            if (!Directory.Exists($"{Environment.CurrentDirectory}\\{DIRECTORY_BACKUP}"))
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\{DIRECTORY_BACKUP}");

            INI = new IniFile(PATH_CONFIG);
        }

        /// <summary>
        /// Сохранить объект
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sharedSecret"></param>
        public void Save(object data, string sharedSecret = "123n09@9j_!#4-1")
        {
            if (!String.IsNullOrEmpty(_sharedSecret))
                sharedSecret = _sharedSecret;

            using (StreamWriter wr = File.CreateText(PATH_DATABASE))
            {
                string output = JsonConvert.SerializeObject(data);
                wr.WriteLine(Security.Crypt(output, sharedSecret));
            }
        }

        /// <summary>
        /// Прочитать объект
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sharedSecret"></param>
        /// <returns></returns>
        public BindingList<T> Load<T>(string sharedSecret = "123n09@9j_!#4-1")
        {
            if (!File.Exists(PATH_DATABASE))
            {
                return null;
            }

            if (!String.IsNullOrEmpty(_sharedSecret))
                sharedSecret = _sharedSecret;

            using (StreamReader rd = File.OpenText(PATH_DATABASE))
            {
                string text = Security.DeCrypt(rd.ReadToEnd(), sharedSecret);
                return JsonConvert.DeserializeObject<BindingList<T>>(text);
            }
        }

        /// <summary>
        /// Загрузка файла ключа 
        /// </summary>
        /// <returns></returns>
        public static string LoadSharedSecret()
        {
            if (File.Exists(PATH_SHARED_SECRET))
            {
                return File.ReadAllText(path: PATH_SHARED_SECRET);
            }
            return null;
        }

        public void SaveConfig()
        {
            INI.Write("main", "last-email-auth", _cfg.LastEmailAuth);
            INI.Write("main", "timeout-session", _cfg.TimeoutSession.ToString());
            INI.Write("main", "sharedkey", _cfg.SharedSecret);
            INI.Write("main", "use-last-email-auth", _cfg.IsUseLastEmailAuth.ToString());
            INI.Write("main", "speakon", _cfg.SPEAK_ON.ToString());

            INI.Write("faker", "last-ico", _cfg.LastIconFaker);
            INI.Write("faker", "last-type", _cfg.LastTypeFaker);

            INI.Write("smtp", "host", _cfg.SMTP_HOST);
            INI.Write("smtp", "port", _cfg.SMTP_PORT.ToString());
            INI.Write("smtp", "ssl", _cfg.SMTP_SSL.ToString());
            INI.Write("smtp", "timeout", _cfg.SMTP_TIMEOUT.ToString());
            INI.Write("smtp", "user", _cfg.SMTP_USER);
            INI.Write("smtp", "password", _cfg.SMTP_PASSWORD);
        }

        public void LoadConfig()
        {
            if (INI.KeyExists("last-email-auth", "main")) _cfg.LastEmailAuth  = INI.ReadINI("main", "last-email-auth");
            if (INI.KeyExists("timeout-session", "main")) _cfg.TimeoutSession = TimeSpan.Parse(INI.ReadINI("main", "timeout-session"));
            if (INI.KeyExists("sharedkey", "main"))       _cfg.SharedSecret   = INI.ReadINI("main", "sharedkey");
            if (INI.KeyExists("use-last-email-auth", "main")) _cfg.IsUseLastEmailAuth = Boolean.Parse(INI.ReadINI("main", "use-last-email-auth"));
            if (INI.KeyExists("speakon", "main")) _cfg.SPEAK_ON = Boolean.Parse(INI.ReadINI("main", "speakon"));


            if (INI.KeyExists("last-ico", "faker")) _cfg.LastIconFaker = INI.ReadINI("faker", "last-ico");
            if (INI.KeyExists("last-type", "faker")) _cfg.LastTypeFaker = INI.ReadINI("faker", "last-type");

            if (INI.KeyExists("host", "smtp")) _cfg.SMTP_HOST = INI.ReadINI("smtp", "host");
            if (INI.KeyExists("port", "smtp")) _cfg.SMTP_PORT = int.Parse(INI.ReadINI("smtp", "port"));
            if (INI.KeyExists("ssl", "smtp")) _cfg.SMTP_SSL = Boolean.Parse(INI.ReadINI("smtp", "ssl"));
            if (INI.KeyExists("timeout", "smtp")) _cfg.SMTP_TIMEOUT = int.Parse(INI.ReadINI("smtp", "timeout"));
            if (INI.KeyExists("user", "smtp")) _cfg.SMTP_USER = INI.ReadINI("smtp", "user");
            if (INI.KeyExists("password", "smtp")) _cfg.SMTP_PASSWORD = INI.ReadINI("smtp", "password");

        }

        /// <summary>
        /// Архивировать файлы
        /// </summary>
        public void CompressionData()
        {
            string source = $"{Environment.CurrentDirectory}\\{DIRECTORY_DATA}";
            string destination = $"{Environment.CurrentDirectory}\\{DIRECTORY_BACKUP}\\{DateTime.Now.ToShortDateString()}.zip";
            if (File.Exists(destination))
                File.Delete(destination);

            ZipFile.CreateFromDirectory(source, destination, CompressionLevel.Optimal, false);
        }

        /// <summary>
        /// Разархивировать файлы
        /// </summary>
        /// <param name="path"></param>
        public void DeCompressionData(string path)
        {
            if(path != null)
            {
                string destination = $"{Environment.CurrentDirectory}\\{DIRECTORY_DATA}";
                ZipFile.ExtractToDirectory(path, destination);
            }            
        }

        /// <summary>
        /// Импорт ветки
        /// </summary>
        /// <param name="fileName"></param>
        public object Import<T>(string fileName)
        {
            object obj;
            using (StreamReader rd = File.OpenText(fileName))
            {
                string text = rd.ReadToEnd();
                obj =  JsonConvert.DeserializeObject<T>(text);
            }
            return obj;
        }

        /// <summary>
        /// Экспорт ветки
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        public void Export(string filename, object data)
        {
            using (StreamWriter wr = File.CreateText(filename))
            {
                string output = JsonConvert.SerializeObject(data);
                wr.WriteLine(output);
            }
        }

    }
}
