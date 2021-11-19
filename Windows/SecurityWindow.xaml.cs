using System.Windows;
using System.Windows.Input;

namespace DataSecurity.Windows
{
    public partial class SecurityWindow : Window
    {
        private string currentPassword;

        public SecurityWindow(string password)
        {
            currentPassword = password;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblError.Visibility = Visibility.Hidden;
            this.Title = "Проверка безопасности";
            txtPassword.Focus();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            checkPassword();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }

            if(e.Key == Key.Enter)
            {
                checkPassword();
            }
        }

        /// <summary>
        /// Проверить пароль
        /// </summary>
        private void checkPassword()
        {
            if (currentPassword.Equals(txtPassword.Password.Trim()))
            {
                this.DialogResult = true;
                lblError.Visibility = Visibility.Hidden;
            }
            else
            {
                lblError.Visibility = Visibility.Visible;
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}
