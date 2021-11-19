using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DataSecurity.Data;
using DataSecurity.Models;


namespace DataSecurity.Windows
{
    public partial class MainWindow : Window
    {
        private Core core;

        public MainWindow()
        {
            InitializeComponent();
        }        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                core = new Core();
                core.sessionTimeOut = ResetData;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Title = String.Format("{0} v.{1}", core.cfg.ProgrammName, core.cfg.Version);

            AuthSession();        
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            core.Save();
        }

        /// <summary>
        /// Выбрать факер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeFakers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ResetFaker();

            core.Faker = null;

            object obj = treeFakers.SelectedItem;

            renderInformation(obj);

            core.AuthTimerefresh();
        }

        /// <summary>
        /// Вывод информации
        /// </summary>
        /// <param name="obj"></param>
        private void renderInformation(object obj)
        {
            if (obj != null && obj is Faker)
            {
                core.Faker = (Faker)treeFakers.SelectedItem;

                StackPanel stackMainPanelVertical = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness { Top = 5 }
                };
                StackPanel stackMainPanelHorizontal = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness { Top = 5 , Left = 0, Right = 0, Bottom = 0},
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                TextBox txtName = new TextBox() 
                { IsReadOnly = true, Width = 500, TextWrapping = TextWrapping.WrapWithOverflow, FontSize = 18, Padding = new Thickness(10), Margin = new Thickness { Left = 4, Right = 4, Top = 5, Bottom = 0 } };
                txtName.Text = String.Format("Запись: {0}", core.Faker.Name);
                stackMainPanelHorizontal.Children.Add(txtName);

                TextBox txtDateCreated = new TextBox()
                { IsReadOnly = true, Width = 500, TextWrapping = TextWrapping.WrapWithOverflow, FontSize = 18, Padding = new Thickness(10), Margin = new Thickness { Left = 4, Right = 4, Top = 5, Bottom = 0 } };
                txtDateCreated.Text = String.Format("Запись создана: {0}", core.Faker.DateTimeCreated);
                stackMainPanelHorizontal.Children.Add(txtDateCreated);

                if(core.Faker.DateTimeUpdate != new DateTime())
                {
                    TextBox txtDateUpdate = new TextBox()
                    { IsReadOnly = true, Width = 500, TextWrapping = TextWrapping.WrapWithOverflow, FontSize = 18, Padding = new Thickness(10), Margin = new Thickness { Left = 4, Right = 4, Top = 5, Bottom = 0 } };
                    txtDateUpdate.Text = String.Format("Запись изменена: {0}", core.Faker.DateTimeUpdate);
                    stackMainPanelHorizontal.Children.Add(txtDateUpdate);
                }
                

                stackMainPanelVertical.Children.Add(stackMainPanelHorizontal);
                if(core.Faker.Type != FakerType.Types.FOLDERS)
                    stackMainPanelVertical.Children.Add(dynamicRenderControls(core.Faker));
                
                gridMainContainer.Children.Add(stackMainPanelVertical);

                {
                    StackPanel stackPanelVertical = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(5)
                    };
                    foreach (Faker f in core.Faker.Fakers)
                    {
                        if (f.Type != FakerType.Types.FOLDERS)
                            stackPanelVertical.Children.Add(dynamicRenderControls(f));
                    }
                    gridContainer.Children.Add(stackPanelVertical);
                }                
            }
        }

        /// <summary>
        /// Динамическое создание кнопок и текстовых полей
        /// </summary>
        /// <param name="faker"></param>
        /// <returns></returns>
        private StackPanel dynamicRenderControls(Faker faker)
        {
            dynamic textInput = null;
            Button btnCopy = null, btnViewPass = null, btnUrl = null, btnRDP = null, btnCMD = null, btnShared = null;
            BitmapImage bitimgCopy, bitimgView, bitimgUrl, bitimgRDP, bitimgCMD, bitimgShared;
            {
                bitimgCMD = new BitmapImage();
                bitimgCMD.BeginInit();
                bitimgCMD.UriSource = new Uri(@"/Files/cmd.ico", UriKind.RelativeOrAbsolute);
                bitimgCMD.EndInit();

                bitimgShared = new BitmapImage();
                bitimgShared.BeginInit();
                bitimgShared.UriSource = new Uri(@"/Files/shared2.ico", UriKind.RelativeOrAbsolute);
                bitimgShared.EndInit();

                bitimgCopy = new BitmapImage();
                bitimgCopy.BeginInit();
                bitimgCopy.UriSource = new Uri(@"/Files/copy.png", UriKind.RelativeOrAbsolute);
                bitimgCopy.EndInit();

                bitimgView = new BitmapImage();
                bitimgView.BeginInit();
                bitimgView.UriSource = new Uri(@"/Files/eye.png", UriKind.RelativeOrAbsolute);
                bitimgView.EndInit();

                bitimgUrl = new BitmapImage();
                bitimgUrl.BeginInit();
                bitimgUrl.UriSource = new Uri(@"/Files/ie3.png", UriKind.RelativeOrAbsolute);
                bitimgUrl.EndInit();

                bitimgRDP = new BitmapImage();
                bitimgRDP.BeginInit();
                bitimgRDP.UriSource = new Uri(@"/Files/RDP.png", UriKind.RelativeOrAbsolute);
                bitimgRDP.EndInit();
            }            

            switch (faker.Type)
            {
                case FakerType.Types.PIN:
                case FakerType.Types.PASSWORD:
                case FakerType.Types.ACCOUNT:
                case FakerType.Types.EMAIL:
                case FakerType.Types.HOSTNAME:
                case FakerType.Types.IPADDRESS:
                case FakerType.Types.RDP:
                case FakerType.Types.PORT:
                case FakerType.Types.PHONE:
                case FakerType.Types.SERIALNUMBER:
                case FakerType.Types.URL:
                    Image img = new Image();
                    img.Stretch = Stretch.Fill;
                    img.Source = bitimgCopy;

                    textInput = new TextBox();
                    textInput.Width = 500;
                    textInput.IsReadOnly = true;
                    textInput.ToolTip = faker.Name;
                    btnCopy = new Button();
                    btnCopy.Click += delegate (object s, RoutedEventArgs e1) {
                        btnCopy_Click(s, e1, faker.Data);
                    };
                    btnCopy.Padding = new Thickness(0);
                    btnCopy.Height = 44;
                    btnCopy.Width = 44;
                    btnCopy.Content = img;
                    btnCopy.ToolTip = "Копировать в буфер обмена";
                    btnCopy.FontSize = 18;
                    btnCopy.Margin = new Thickness { Left = 4, Right = 4, Top = 0, Bottom = 0 };
                    btnCopy.Background = Brushes.Transparent;
                    btnCopy.BorderBrush = Brushes.Transparent;                    
                    break;
                default:
                    textInput = new TextBlock();
                    break;
            }

            if (faker.Type == FakerType.Types.URL)
            {
                Image imgUrl = new Image();
                imgUrl.Stretch = Stretch.Fill;
                imgUrl.Source = bitimgUrl;

                btnUrl = new Button();
                btnUrl.Padding = new Thickness(0);
                btnUrl.Height = 44;
                btnUrl.Width = 44;
                btnUrl.Content = imgUrl;
                btnUrl.ToolTip = "Пройти по ссылке";
                btnUrl.FontSize = 18;
                btnUrl.Margin = new Thickness { Left = 4, Right = 4, Top = 0, Bottom = 0 };
                btnUrl.Background = Brushes.Transparent;
                btnUrl.BorderBrush = Brushes.Transparent;
                btnUrl.Click += delegate (object s, RoutedEventArgs e1)
                {
                    System.Diagnostics.Process.Start(faker.Data);
                };
            }

            if (faker.Type == FakerType.Types.PIN || faker.Type == FakerType.Types.PASSWORD)
            {
                Image imgView = new Image();
                imgView.Stretch = Stretch.Fill;
                imgView.Source = bitimgView;

                btnViewPass = new Button();
                btnViewPass.Padding = new Thickness(0);
                btnViewPass.Height = 44;
                btnViewPass.Width = 44;
                btnViewPass.Content = imgView;
                btnViewPass.ToolTip = "Подглядеть пароль";
                btnViewPass.FontSize = 18;
                btnViewPass.Margin = new Thickness { Left = 4, Right = 4, Top = 0, Bottom = 0 };
                btnViewPass.Background = Brushes.Transparent;
                btnViewPass.BorderBrush = Brushes.Transparent;
                btnViewPass.PreviewMouseLeftButtonDown += delegate (object s, MouseButtonEventArgs e1)
                {
                    textInput.Text = faker.Data;
                };
                btnViewPass.PreviewMouseLeftButtonUp += delegate (object s, MouseButtonEventArgs e1)
                {
                    textInput.Text = $"{faker.Name}: ******";
                };
            }

            if(faker.Type == FakerType.Types.RDP)
            {
                Image imgRDP = new Image();
                imgRDP.Stretch = Stretch.Fill;
                imgRDP.Source = bitimgRDP;

                btnRDP = new Button();
                btnRDP.Padding = new Thickness(0);
                btnRDP.Height = 44;
                btnRDP.Width = 44;
                btnRDP.Content = imgRDP;
                btnRDP.ToolTip = "Подключиться";
                btnRDP.FontSize = 18;
                btnRDP.Margin = new Thickness { Left = 4, Right = 4, Top = 0, Bottom = 0 };
                btnRDP.Background = Brushes.Transparent;
                btnRDP.BorderBrush = Brushes.Transparent;
                btnRDP.Click += delegate (object s, RoutedEventArgs e1)
                {
                    string host;
                    int port = 3389;

                    string[] array = faker.Data.Split(':');
                    if(array.Length == 2)
                    {
                        host = array[0];
                        port = int.Parse(array[1]);
                    } else
                    {
                        host = faker.Data;
                    }

                    Process process = new Process();
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "mstsc",
                        Arguments = $"/v:{ host }:{ port }"
                    };

                    process.Start();
                    process.WaitForExit();
                };
            }

            if(faker.Type == FakerType.Types.IPADDRESS || faker.Type == FakerType.Types.HOSTNAME)
            {
                Image imgCMD = new Image();
                imgCMD.Stretch = Stretch.Fill;
                imgCMD.Source = bitimgCMD;

                Image imgShared = new Image();
                imgShared.Stretch = Stretch.Fill;
                imgShared.Source = bitimgShared;

                btnCMD = new Button();
                btnCMD.Padding = new Thickness(0);
                btnCMD.Height = 44;
                btnCMD.Width = 44;
                btnCMD.Content = imgCMD;
                btnCMD.ToolTip = "Пингануть";
                btnCMD.FontSize = 18;
                btnCMD.Margin = new Thickness { Left = 4, Right = 4, Top = 0, Bottom = 0 };
                btnCMD.Background = Brushes.Transparent;
                btnCMD.BorderBrush = Brushes.Transparent;
                btnCMD.Click += delegate (object s, RoutedEventArgs e1)
                {
                    Process process = new Process();
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd",
                        Arguments = $@"/c ping { faker.Data }"
                    };
                    process.Start();
                };

                btnShared = new Button();
                btnShared.Padding = new Thickness(0);
                btnShared.Height = 44;
                btnShared.Width = 44;
                btnShared.Content = imgShared;
                btnShared.ToolTip = "Открыть как шару";
                btnShared.FontSize = 18;
                btnShared.Margin = new Thickness { Left = 4, Right = 4, Top = 0, Bottom = 0 };
                btnShared.Background = Brushes.Transparent;
                btnShared.BorderBrush = Brushes.Transparent;
                btnShared.Click += delegate (object s, RoutedEventArgs e1)
                {
                    Process process = new Process();
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "explorer",
                        Arguments = $@"\\{ faker.Data }"
                    };
                    process.Start();
                };
            }

            textInput.TextWrapping = TextWrapping.WrapWithOverflow;
            textInput.FontSize = 18;
            textInput.Padding = new Thickness(10);
            textInput.Margin = new Thickness { Left = 4, Right = 4, Top = 0, Bottom = 0 };
            textInput.Text = String.Format("{0}: {1}", FakerType.getTypeValue(faker.Type) ,(isPassword(faker.Type)) ? "******" : faker.Data);

            StackPanel stackPanelHorizontal = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness { Top = 5 }
            };

            stackPanelHorizontal.Children.Add(textInput);

            if (btnCopy != null)
            {
                stackPanelHorizontal.Children.Add(btnCopy);
            }
            if (btnViewPass != null)
            {
                stackPanelHorizontal.Children.Add(btnViewPass);
            }
            if (btnUrl != null)
            {
                stackPanelHorizontal.Children.Add(btnUrl);
            }
            if (btnRDP != null)
            {
                stackPanelHorizontal.Children.Add(btnRDP);
            }
            if (btnShared != null)
            {
                stackPanelHorizontal.Children.Add(btnShared);
            }
            if (btnCMD != null)
            {
                stackPanelHorizontal.Children.Add(btnCMD);
            }
            return stackPanelHorizontal;
        }

        /// <summary>
        /// Скопировать в буфер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="text"></param>
        private void btnCopy_Click(object sender, RoutedEventArgs e, string text)
        {
            core.AuthTimerefresh();

            Clipboard.SetText(text);
        }

        /// <summary>
        /// Проверка на тип записи "ПАРОЛЬ" или "пин"
        /// </summary>
        /// <param name="fakerType"></param>
        /// <returns></returns>
        private bool isPassword(FakerType.Types type)
        {
            return (type == FakerType.Types.PASSWORD || type == FakerType.Types.PIN);
        }                

        /// <summary>
        /// Сбросить факер
        /// </summary>
        private void ResetFaker()
        {  
            gridMainContainer.Children.Clear();
            gridContainer.Children.Clear();
        }

        /// <summary>
        /// Удалить факер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDestroy_Click(object sender, RoutedEventArgs e)
        {
            SecurityWindow frm = new SecurityWindow(core.User.Password);
            frm.Owner = this;
            Nullable<bool> result = frm.ShowDialog();

            if (result == true)
            {
                if (treeFakers.SelectedItem != null && MessageBox.Show("Удалить выбраную запись?", "Удалить", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        if (core.DeleteFaker())
                        {
                            core.Save();
                            core.AuthTimerefresh();
                        }
                        else
                        {
                            MessageBox.Show("Запись удалить не возможно!", "Отказ", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
                       
        }

        /// <summary>
        /// Экспорт в CSV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void export_Click(object sender, RoutedEventArgs e)
        {
            SecurityWindow frm = new SecurityWindow(core.User.Password);
            frm.Owner = this;
            Nullable<bool> res = frm.ShowDialog();

            if(res == true)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Data";
                dlg.DefaultExt = ".json";
                dlg.Filter = "data json (.json)|*.json";

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    core.ExportFaker(dlg.FileName);
                }
            }
            
        }

        /// <summary>
        /// Импортировать
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void import_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Data";
            dlg.DefaultExt = ".json";
            dlg.Filter = "data json (.json) | *.json";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                core.ImportFaker(dlg.FileName);
            }
        }

       /// <summary>
       /// Сохранить данные
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!core.Save())
                {
                    MessageBox.Show("Невозможно сохранить данные", "Отказ", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Создать факер через контекстное меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextAdd_Click(object sender, RoutedEventArgs e)
        {
            EditFakerWindow frm = new EditFakerWindow(CoreIcons.GetIcons(), false, core.cfg);
            CreateFakerInCore(frm);
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow frm = new AboutWindow(core.cfg);
            frm.Owner = this;
            frm.ShowDialog();
        }

        /// <summary>
        /// Создать копию данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            SecurityWindow frm = new SecurityWindow(core.User.Password);
            frm.Owner = this;
            Nullable<bool> result = frm.ShowDialog();

            if(result == true)
            {
                core.CreateBackup();
                core.AuthTimerefresh();
                MessageBox.Show("Создание архива завершено", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Создать факер через кнопку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            EditFakerWindow frm = new EditFakerWindow(CoreIcons.GetIcons(), true, core.cfg);
            CreateFakerInCore(frm);
        }

        /// <summary>
        /// Создать факер в ядре
        /// </summary>
        /// <param name="frm"></param>
        private void CreateFakerInCore(EditFakerWindow frm)
        {
            frm.Owner = this;
            bool? result = frm.ShowDialog();
            if (result == true)
            {
                if(core.AddFaker(frm.Getfaker(), frm.IsAddRoot()))
                {
                    core.Save();
                    core.Faker = null;

                    ResetFaker();

                    object obj = treeFakers.SelectedItem;

                    renderInformation(obj);

                    core.AuthTimerefresh();
                }
                else
                {
                    MessageBox.Show("Невозможно создать запись", "Отказ", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }            
        }

        /// <summary>
        /// Изменить факер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void edit_Click(object sender, RoutedEventArgs e)
        {
            if(core.Faker != null)
            {
                EditFakerWindow frm = new EditFakerWindow(CoreIcons.GetIcons(), false, core.Faker, core.cfg);
                frm.Owner = this;
                bool? result = frm.ShowDialog();
                if(result == true)
                {
                    Faker faker = frm.Getfaker();
                    if(core.UpdateFaker(faker))
                    {
                        core.Save();
                        core.Faker = null;

                        ResetFaker();

                        object obj = treeFakers.SelectedItem;

                        renderInformation(obj);

                        core.AuthTimerefresh();
                    }
                    else
                    {
                        MessageBox.Show("Невозможно обновить запись", "Отказ", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }                    
                }
            }
        }

        private void CloseSession(object sender, RoutedEventArgs e)
        {
            core.Save();
            core.CloseSession();

            ResetData();
        }

        private void ResetData()
        {
            ResetFaker();
            treeFakers.ItemsSource = null;
            AuthSession();
        }

        private void AuthSession()
        {
            ResetFaker();

            AuthWindow frm = new AuthWindow(core);
            frm.Owner = this;

            bool? result = frm.ShowDialog();
            if (result != true)
            {
                this.Close();
                return;
            }

            if (core.IsAuth(frm.getUser()))
            {
                treeFakers.ItemsSource = core.GetFakers();
                this.Title = String.Format("Вход выполнен: {0}",  core.User.Email);
                treeFakers.Focus();
            }
        }       
    }
}