using DataSecurity.Data;
using DataSecurity.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DataSecurity.Windows
{
    public partial class AuthWindow : Window
    {
        private User userAuth;

        private BindingList<User> _users;

        private string _sharedSecret;

        private Core _core;

        public AuthWindow(Core core)
        {
            _users = core.GetUsers();
            _sharedSecret = core.cfg.SharedSecret;
            _core = core;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblError.Visibility = Visibility.Hidden;
            this.Title = "Авторизация";

            if (_core.cfg.IsUseLastEmailAuth)
                txtEmail.Text = _core.cfg.LastEmailAuth;
            if (String.IsNullOrEmpty(txtEmail.Text))
                txtEmail.Focus();
            else
                txtPassword.Focus();
        }

        private void btnAuth_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^([a-zA-Z0-9_\-])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$"))
            {
                txtEmail.Focus();
                lblError.Content = "Некорректный почтовый адрес.";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            User user = _users.SingleOrDefault(item => item.Email == email);

            if(user == null)
            {
                lblError.Content = "Пользователь не найден.";
                lblError.Visibility = Visibility.Visible;
                return;
            }

            //string pass = Security.Crypt(password, _sharedSecret);
            if (user.Password != password)
            {
                lblError.Content = "Неверный пароль, попробуйте еще раз.";
                lblError.Visibility = Visibility.Visible;
                txtPassword.Password = "";
                txtPassword.Focus();
                return;
            }
            user.TimeAuth = DateTime.Now.TimeOfDay;
            userAuth = user;

            _core.cfg.LastEmailAuth = userAuth.Email;

            this.DialogResult = true;
        }

        private void lblRegistration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegistrationWindow frm = new RegistrationWindow(_users, _core.cfg);
            frm.Owner = this;
            if(frm.ShowDialog() == true)
            {
                User user = frm.GetUser();
                txtEmail.Text = user.Email;

                _core.AddUser(user);
                _core.Save(true);
            }
        }

        public User getUser() => this.userAuth;

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
