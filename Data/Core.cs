using System;
using System.Linq;
using System.ComponentModel;
using System.IO;
using DataSecurity.Models;
using System.Windows.Threading;
using DataSecurity.Modules;

namespace DataSecurity.Data
{
    public class Core
    {
        public delegate void SessionTimeOut();

        public Config cfg = new Config();

        private BindingList<User> _users = new BindingList<User>();

        private readonly CoreStorage _storage;

        private DispatcherTimer timerSessionOut;

        private Voice voice;

        public BindingList<User> GetUsers() => _users;

        public BindingList<Faker> GetFakers() => _user.Fakers;

        /// <summary>
        ///  Текущий пользователь
        /// </summary>
        private User _user;

        /// <summary>
        /// Текущий факер
        /// </summary>
        private Faker _faker;

        public Faker Faker
        {
            get { return _faker; }
            set { _faker = value; }
        }
        public User User
        {
            get { return _user; }
        }

        public SessionTimeOut sessionTimeOut;

        // Конструктор
        public Core()
        {
            {
                new CoreIcons().Generate();
            }

            _storage = new CoreStorage(cfg);
            _storage.SharedSecret = cfg.SharedSecret;
            _storage.LoadConfig();

            _users = (BindingList<User>)_storage.Load<User>() ?? new BindingList<User>();
            
            this.test();
            {
                timerSessionOut = new DispatcherTimer();
                timerSessionOut.Tick += new EventHandler(timerSessionOut_Tick);
                timerSessionOut.Interval = new TimeSpan(0, 0, 1);
            }
            {
                voice = new Voice(cfg.SPEAK_ON);
                voice.setVoice(voice.VoiceList[0]);
            }
        }

        /// <summary>
        /// Оборвать сессию по простою
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerSessionOut_Tick(object sender, EventArgs e)
        {
            if(_user != null)
            {
                TimeSpan diff = DateTime.Now.TimeOfDay - _user.TimeAuth;
                if (diff > cfg.TimeoutSession)
                {
                    Save();
                    CloseSession();
                    timerSessionOut.Stop();
                    if(sessionTimeOut != null)
                        sessionTimeOut();
                }
            }
        }

        /// <summary>
        /// Тестовые данные
        /// </summary>
        private void test()
        {
            User existUser = _users.SingleOrDefault(item => item.Email == "admin@gmail.com");
            if (existUser == null)
            {
                //string pass = Security.Crypt("admin", cfg.SharedSecret);
                _users.Add(new User()
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    Password = "admin",
                    Fakers = new BindingList<Faker>
                    {
                        new Faker(Guid.NewGuid(), "Email", FakerType.Types.FOLDERS, Path.GetFullPath(@"Images\folder.png")),
                        new Faker(Guid.NewGuid(), "Sites", FakerType.Types.FOLDERS, Path.GetFullPath(@"Images\folder.png")),
                        new Faker(Guid.NewGuid(), "Servers", FakerType.Types.FOLDERS, Path.GetFullPath(@"Images\folder.png")),
                    },
                });
            }
        }

        /// <summary>
        /// Обновить таймер аутинтификации
        /// </summary>
        internal void AuthTimerefresh()
        {
            if(_user != null)
                _user.TimeAuth = DateTime.Now.TimeOfDay;
        }

        internal bool IsAuth(User user)
        {
            this._user = _users.FirstOrDefault(dataUser => dataUser == user);
            bool result = (this._user != null) ? true : false;
            if (result)
            {
                timerSessionOut.Start();

                voice.SpeakAsync($"Привет {_user.Name}!, Вы начали новую сессию.");
            }

            return result;
        }

        internal void AddUser(User user)
        {
            _users.Add(user);
        }

        /// <summary>
        /// Сохранить данные
        /// </summary>
        /// <param name="isForcibly">Принудительно</param>
        /// <returns></returns>
        public bool Save(bool isForcibly = false)
        {
            if(isForcibly || _user != null)
            {
                _storage.SharedSecret = cfg.SharedSecret;
                _storage.Save(_users);
                _storage.SaveConfig();
                return true;
            }
            else
                return false;
        }        

        /// <summary>
        /// Удалить факер
        /// </summary>
        /// <param name="faker"></param>
        /// <param name="fakers"></param>
        /// <returns></returns>
        public bool DeleteFaker()
        {
            if (_faker != null && _user != null)
            {
                return Delete(_faker, _user.Fakers);
            }
            else
                return false;
        }

        /// <summary>
        /// Удалить факер
        /// </summary>
        /// <param name="faker"></param>
        /// <param name="fakers"></param>
        /// <returns></returns>
        private bool Delete(Faker faker, BindingList<Faker> fakers)
        {
            if (_user != null && _faker != null)
            {
                bool r = fakers.Remove(faker);
                if (r) return r;

                foreach (Faker f in fakers)
                {
                    r = Delete(faker, f.Fakers);
                    if (r) return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Добавить новый факер
        /// </summary>
        /// <param name="faker"></param>
        internal bool AddFaker(Faker faker, bool isAddRoot)
        {
            if(_user != null && isAddRoot)
            {
                _user.Fakers.Add(faker);
                return true;
            }
            else if(_user != null && _faker != null && !isAddRoot)
            {
                _faker.Fakers.Add(faker);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Изменение значений факера
        /// </summary>
        /// <param name="faker"></param>
        internal bool UpdateFaker(Faker faker)
        {
            if (_user != null && _faker != null)
            {
                Faker.Name = faker.Name;
                Faker.Data = faker.Data;
                Faker.Type = faker.Type;
                Faker.Img = faker.Img;
                Faker.DateTimeUpdate = DateTime.Now;
                return true;
            }
            else
                return false;
        }

        internal void CloseSession()
        {
            if(_user != null)
            {
                _user = null;
                _faker = null;
            }
        }

        public void CreateBackup()
        {
            _storage.CompressionData();
        }

        internal void ExportFaker(string filename)
        {
            _storage.Export(filename, _faker);
        }

        internal void ImportFaker(string fileName)
        {            
            Faker f = (Faker)_storage.Import<Faker>(fileName);
            _user.Fakers.Add(f);
        }
    }
}