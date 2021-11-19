using DataSecurity.Models;
using DataSecurity.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace DataSecurity.Windows
{
    public partial class RegistrationWindow : Window
    {
        private BindingList<User> _list;

        private User _user;

        private Config _cfg;

        public RegistrationWindow(BindingList<User> list, Config cfg)
        {
            _cfg = cfg;

            _list = list;

            InitializeComponent();
        }

        private void frmRegistration_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "Регистрация";
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {            
            string name =  txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();
            string rePassword = txtRePassword.Password.Trim();

            bool isError = false;
            string message = "";
            if (String.IsNullOrEmpty(name))
            {
                isError = true;
                message += "\nПустое поле \"Имя пользователя\"";
            }
            if (name.Length < 3)
            {
                isError = true;
                message += "\nНедопустимое количество символов в поле \"Имя пользователя\"";
            }
            if (String.IsNullOrEmpty(email))
            {
                isError = true;
                message += "\nПустое поле \"Почтовый адрес\"";
            }
            if(!Regex.IsMatch(email, @"^([a-zA-Z0-9_\-])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$"))
            {
                isError = true;
                message = "Некорректный почтовый адрес";
            }
            if (String.IsNullOrEmpty(password))
            {
                isError = true;
                message += "\nПустое поле \"Пароль\"";
            }
            if (password.Length < 4)
            {
                isError = true;
                message += "\nНедопустимое количество символов в поле \"Пароль\"";
            }
            if (String.IsNullOrEmpty(rePassword))
            {
                isError = true;
                message += "\nПустое поле \"Пароль еще раз\"";
            }

            if(password != rePassword)
            {
                isError = true;
                message += "\nПароли не совпадают!!!";
            }
            if (rePassword.Length < 4)
            {
                isError = true;
                message += "\nНедопустимое количество символов в поле \"Пароль еще раз\"";
            }

            if (isError == true)
            {
                MessageBox.Show(message, "Отказ", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }


            User existUser = _list.SingleOrDefault(item => item.Email == email);

            if(existUser != null)
            {
                MessageBox.Show("Пользователь уже зарегистрирован!", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            _user = new User { Name = name, Email = email, Password = password, Fakers = new BindingList<Faker>(), DateRegistration = DateTime.Now };

            if(!String.IsNullOrEmpty(_cfg.SMTP_HOST) &&
                !String.IsNullOrEmpty(_cfg.SMTP_PORT.ToString()) &&
                !String.IsNullOrEmpty(_cfg.SMTP_USER) &&
                !String.IsNullOrEmpty(_cfg.SMTP_PASSWORD))
            {
                
                    new SendMail(_cfg.SMTP_HOST, _cfg.SMTP_PORT, _cfg.SMTP_USER, _cfg.SMTP_PASSWORD, _cfg.SMTP_TIMEOUT, _cfg.SMTP_SSL, SendError)
                    .InitAddress(_cfg.SMTP_USER, "Джин живущий в программе", email)
                    .AddText(_cfg.ProgrammName, String.Format("<p>Здравствуйте, <b>{0}</b></p><p>Создана учетная запись: \"{1}\"</p><p>пароль: {2}</p>", name, email, password))
                    .send();
                
            }

            MessageBox.Show("Вы зарегистрировались!", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
            
            this.DialogResult = true;
        }

        public User GetUser() => _user;

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SendError(string result)
        {
            Dispatcher.Invoke(() => {
                MessageBox.Show(result, "Ошибка отправки", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        private void frmRegistration_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }
    }
}
