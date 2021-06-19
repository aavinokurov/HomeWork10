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

namespace HomeWork10
{
    /// <summary>
    /// Логика взаимодействия для FilesInfo.xaml
    /// </summary>
    public partial class FilesInfo : Window
    {
        public FilesInfo()
        {
            InitializeComponent();

            var files = DownloadFiles.GetFiles();

            FileType.ItemsSource = files;
        }

        private void FileType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentFileType = (sender as ListBox).SelectedItem as FileSystemInfo;
            FileName.ItemsSource = currentFileType.Files;
        }
    }
}
