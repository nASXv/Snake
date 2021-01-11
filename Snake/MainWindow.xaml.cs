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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZmeykaLib;
using System.Globalization;




namespace Snake
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int record = 0;

        public MainWindow()
        {
            InitializeComponent();

            App.LanguageChanged += LanguageChanged;

            CultureInfo currLang = App.Language;

            //Заполняем меню смены языка:
            menuLanguage.Items.Clear();
            foreach(var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                menuLanguage.Items.Add(menuLang);
            }
        }
        ZmeykaClass game;

        private void Play(object sender, RoutedEventArgs e)
        {
            if(game == null)
                game = new ZmeykaClass(GameArea, text_Score, text_Record);
            else game.Start();

            btn_Play.Content =  "RESTART";
        }
        private void Exit(object sender, RoutedEventArgs e) {
            System.Windows.Application.Current.Shutdown();
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            game.Input(sender, e);
        }

        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            //Отмечаем нужный пункт смены языка как выбранный язык
            foreach(MenuItem i in menuLanguage.Items)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
        }

        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if(mi != null)
            {
                CultureInfo lang = mi.Tag as CultureInfo;
                if(lang != null)
                {
                    App.Language = lang;
                }
            }

        }
    }
}

