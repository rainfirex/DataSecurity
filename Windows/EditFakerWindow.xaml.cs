using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataSecurity.Models;

namespace DataSecurity.Windows
{
    public partial class EditFakerWindow : Window
    {
        const string ROOT = "Корень";
        const string SELECT_ITEM = "Выбранная запись";

        private List<Icon> _icons;

        private Faker _faker;

        private Config _cfg;

        internal Faker Getfaker() => _faker;

        private bool _isAddRoot;

        public bool IsAddRoot() => _isAddRoot;

        public EditFakerWindow(List<Icon> icons, bool isAddRoot, Config cfg)
        {
            _isAddRoot = isAddRoot;
            _icons = icons;
            _cfg = cfg;

            InitializeComponent();
        }

        public EditFakerWindow(List<Icon> icons, bool isAddRoot, Faker faker, Config cfg) : this(icons, isAddRoot, cfg)
        {
            _faker = faker;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cmbIcons.ItemsSource = _icons;
            cmbTypes.ItemsSource = FakerType.TTypes.Values.ToArray();
            cmbNodeName.Items.Add(ROOT);
            cmbNodeName.Items.Add(SELECT_ITEM);

            cmbNodeName.Text = (_isAddRoot) ? ROOT : SELECT_ITEM;

            if (_faker != null)
            {
                txtName.Text = _faker.Name;
                txtData.Text = _faker.Data;
                cmbIcons.SelectedItem = _icons.SingleOrDefault(item => item.Name == _faker.Img);
                cmbTypes.Text = FakerType.getTypeValue(_faker.Type);

                btnAdd.Content = "Изменить";
                this.Title = "Изменить запись";
                groupIsAddRoot.IsEnabled = false;

                if (_faker.Type == FakerType.Types.FOLDERS)
                    txtName.Foreground = (isValid(cmbTypes.SelectedItem.ToString(), _faker.Name)) ? Brushes.Green : Brushes.Red;
                else
                    txtName.Foreground = Brushes.Black;

                txtData.Foreground = (isValid(cmbTypes.SelectedItem.ToString(), _faker.Data)) ? Brushes.Green : Brushes.Red;
            }
            else
            {
                btnAdd.Content = "Создать";
                this.Title = "Создать запись";
                groupIsAddRoot.IsEnabled = true;
                cmbTypes.Text = _cfg.LastTypeFaker;
                cmbIcons.SelectedItem = _icons.SingleOrDefault(item => item.Path == _cfg.LastIconFaker);
            }
        }

        private void txtData_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cmbTypes.SelectedItem == null) return;
            string type = cmbTypes.SelectedItem.ToString();
            string data = txtData.Text.Trim();
            txtData.Foreground = (isValid(type, data))? Brushes.Green : Brushes.Red;
        }        

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value;
            FakerType.TTypes.TryGetValue(FakerType.Types.FOLDERS, out value);
            if (cmbTypes.Text == value)
            {
                txtName.Foreground =
                (isValid(value, txtName.Text.Trim())) ? Brushes.Green : Brushes.Red;
            }
            else
            {
                txtName.Foreground = Brushes.Black;
            }
        }

        private void cmbTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTypes.SelectedItem == null) return;

            string type = cmbTypes.SelectedItem.ToString();
            string data = txtData.Text.Trim();

            var selectType = FakerType.TTypes.SingleOrDefault(item => item.Value == type);

            if(String.IsNullOrEmpty(txtName.Text.Trim()) || isDefaultTypeValue(txtName.Text.Trim()))
                txtName.Text = selectType.Value;


            txtData.Foreground = (isValid(type, data)) ? Brushes.Green : Brushes.Red;

            dinamicTitle.Header = selectType.Value;
            dinamicTitle.Visibility = Visibility.Visible;

            switch (selectType.Key)
            {
                case FakerType.Types.FOLDERS:
                    dinamicTitle.Visibility = Visibility.Hidden;
                    break;
                case FakerType.Types.ACCOUNT:
                case FakerType.Types.PASSWORD:
                case FakerType.Types.ADDRESS:
                case FakerType.Types.DATE:
                case FakerType.Types.DATETIME:
                case FakerType.Types.EMAIL:
                case FakerType.Types.HOSTNAME:
                case FakerType.Types.IPADDRESS:
                case FakerType.Types.NUMBER:
                case FakerType.Types.SERIALNUMBER:
                case FakerType.Types.URL:
                case FakerType.Types.USERNAME:
                case FakerType.Types.PHONE:
                case FakerType.Types.PIN:                    
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Проверка значение на default
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private bool isDefaultTypeValue(string v)
        {
            var def = FakerType.TTypes.FirstOrDefault(item => item.Value == v);
            return def.Value == v;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            bool isInvalid = false;
            string message = null;

            if (cmbNodeName.Text.Trim().Length <= 0 && groupIsAddRoot.IsEnabled == true)
            {
                isInvalid = true;
                message = "Не выбрано место добавление.\n";
            }

            if (cmbTypes.Text.Trim().Length <= 0)
            {
                isInvalid = true;
                message = "Тип записи не выбран.\n";
            }

            if (cmbIcons.SelectedValue == null)
            {
                isInvalid = true;
                message += "Иконка не выбрана.\n";
            }

            if (txtName.Text.Trim().Length <= 0)
            {
                isInvalid = true;
                message += "Название не заполнено.\n";
            }

            string typeDirectory = null;
            FakerType.TTypes.TryGetValue(FakerType.Types.FOLDERS, out typeDirectory);
            if (txtData.Text.Trim().Length <= 0 && cmbTypes.Text != typeDirectory)
            {
                isInvalid = true;
                message += $"Поле \"{cmbTypes.Text.Trim()}\" не заполнено.";
            }

            if (isInvalid)
            {
                MessageBox.Show(message, "Отказ", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            else
            {
                Guid id = (_faker != null) ? _faker.Id : Guid.NewGuid();
                string type = cmbTypes.Text.Trim();
                string name = txtName.Text.Trim();
                string data = txtData.Text.Trim();
                Icon ico = (Icon)cmbIcons.SelectedItem;

                _faker = new Faker(id, name, FakerType.getTypeEnum(type), ico.Name, data);

                _cfg.LastTypeFaker = cmbTypes.Text;
                _cfg.LastIconFaker = ico.Path;

                this.DialogResult = true;
            }
        }

        /// <summary>
        /// Прогнать через регулярку
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool isValid(string type, string data)
        {
            var tType = FakerType.TTypes.SingleOrDefault(item => item.Value == type);
            var mask = FakerType.TMask.SingleOrDefault(item => item.Key.ToString() == tType.Key.ToString());
            return Regex.IsMatch(data, mask.Value, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        private void cmbNodeName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbNodeName.SelectedItem != null)
            {
                string type = cmbNodeName.SelectedItem.ToString();
                if (type == ROOT)
                {
                    _isAddRoot = true;
                }
                else
                {
                    _isAddRoot = false;
                }
            }            
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }
    }
}
