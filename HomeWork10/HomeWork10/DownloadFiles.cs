using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork10
{
    public static class DownloadFiles
    {
        /// <summary>
        /// Получить сохраненные файлы
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<FileSystemInfo> GetFiles()
        {
            var directoryInfo = new DirectoryInfo("Download").GetDirectories();
            ObservableCollection<FileSystemInfo> result = new ObservableCollection<FileSystemInfo>();

            foreach (var item in directoryInfo)
            {
                var tempFileSystemInfo = new FileSystemInfo { FileType = item.Name };

                var directoryInfoFileSystemInfo = new DirectoryInfo($@"Download\{tempFileSystemInfo.FileType}").GetFiles();

                foreach (var file in directoryInfoFileSystemInfo)
                {
                    tempFileSystemInfo.Files = new List<FileInfoName>();
                    tempFileSystemInfo.Files.Add(new FileInfoName { FileName = file.Name });
                }

                result.Add(tempFileSystemInfo);

            }

            return result;
        }
    }
}
