using DataSecurity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DataSecurity.Windows
{
    public partial class AboutWindow : Window
    {
        private Config _cfg;

        public AboutWindow(Config cfg)
        {
            _cfg = cfg;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "О программе";

            txtBlockDevelop.Text = String.Format("Разработчик: {0}",_cfg.Programmist);
            txtBlockDevelopEmail.Text = String.Format("Почтовый адрес разработчика: {0}", _cfg.ProgrammistEmail);
            txtBlockProgramName.Text = String.Format("Название программы: \"{0}\" - хранение персональных данных в шифрованом виде.", _cfg.ProgrammName);
            txtBlockProgramVersion.Text = String.Format("Версия программы: {0}, {1}", _cfg.Version, _cfg.VersionType);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape || e.Key == Key.Enter)
            {
                this.Close();
            }
        }
    }
}
