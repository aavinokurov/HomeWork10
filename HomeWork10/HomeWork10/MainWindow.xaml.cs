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

namespace HomeWork10
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyTelegramBot myTelegramBot;

        User currentUser;

        public MainWindow()
        {
            InitializeComponent();

            myTelegramBot = new MyTelegramBot(this);

            UserList.ItemsSource = myTelegramBot.Users;
        }

        private void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentUser = (sender as ListBox).SelectedItem as User;
            MsgList.ItemsSource = currentUser.Messages;
        }

        private void btnMsgSend_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser != null)
            {
                myTelegramBot.SendMessage(txtMsgSend.Text, currentUser);
            }
        }

        private void MenuItem_jsonSerialization(object sender, RoutedEventArgs e)
        {
            myTelegramBot.jsonSerializationHistoryMessage();
        }

        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Files(object sender, RoutedEventArgs e)
        {
            FilesInfo filesInfo = new FilesInfo();
            filesInfo.Show();
        }
    }
}
