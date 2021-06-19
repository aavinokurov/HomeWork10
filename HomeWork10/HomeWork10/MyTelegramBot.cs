using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace HomeWork10
{
    public class MyTelegramBot
    {
        #region Поля

        /// <summary>
        /// Окно программы
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// Телеграм бот
        /// </summary>
        private TelegramBotClient bot;

        /// <summary>
        /// Текущий клиент
        /// </summary>
        private User currentUser;

        #endregion

        #region Свойства

        /// <summary>
        /// Список пользователей
        /// </summary>
        public ObservableCollection<User> Users { get; private set; }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Создать телеграм бота
        /// </summary>
        /// <param name="window">Рабочее окно</param>
        /// <param name="token">Токен для создания бота</param>
        public MyTelegramBot(MainWindow window, string token = "")
        {
            this.mainWindow = window;
            this.Users = new ObservableCollection<User>();

            bot = new TelegramBotClient(token);

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
        }

        #endregion

        #region Методы

        private void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs messageEvent)
        {
            long id = messageEvent.Message.Chat.Id;
            string firstName = messageEvent.Message.Chat.FirstName;

            mainWindow.Dispatcher.Invoke(() =>
            {
                int index = CheckUser(id);

                if (index < 0)
                {
                    Users.Add(new User(id, firstName));
                    index = Users.Count - 1;
                }

                currentUser = Users[index];
            });

            if (messageEvent.Message.Text != null)
            {
                string messageText = messageEvent.Message.Text;

                mainWindow.Dispatcher.Invoke(() =>
                {
                    currentUser.AddMessage(new Message(firstName, messageText, DateTime.Now));
                });

                if (currentUser.isDownload)
                {
                    if (currentUser.DownloadFolder == @"Download")
                    {
                        CheckTypeFile(messageText);
                    }
                    else
                    {
                        CheckFile(messageText);
                    }
                }

                if (!currentUser.isDownload && (messageText == @"/Start" || messageText == @"/start"))
                {
                    SendStartMessage();
                }

                if (!currentUser.isDownload && (messageText == @"/Download" || messageText == @"/download"))
                {
                    currentUser.isDownload = true;
                    SelectTypeFile();
                }
            }
            else
            {
                if (messageEvent.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
                {
                    DownloadDocument(messageEvent);
                }

                if (messageEvent.Message.Type == Telegram.Bot.Types.Enums.MessageType.Voice)
                {
                    DownloadVoice(messageEvent);
                }

                if (messageEvent.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
                {
                    DownloadPhoto(messageEvent);
                }
            }
        }

        /// <summary>
        /// Возращает индекс пользователя в списке. Если его нет, то -1
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <returns></returns>
        private int CheckUser(long id)
        {
            for (int i = 0; i < Users.Count; i++)
            {
                if (Users[i].Id == id)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Отправить сообщение пользователю
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        /// <param name="Id">Id пользователя</param>
        public void SendMessage(string text, User user)
        {            
            bot.SendTextMessageAsync(user.Id, text);

            mainWindow.Dispatcher.Invoke(() =>
            {
                user.AddMessage(new Message("Cloud Storage", text, DateTime.Now));
            });
        }

        /// <summary>
        /// Скачать файл на диск
        /// </summary>
        /// <param name="fileId">Id файла</param>
        /// <param name="fileName">Имя файла</param>
        public async void DownLoadFile(string fileId, string fileName)
        {
            var file = await bot.GetFileAsync(fileId);
            FileStream fs = new FileStream($@"{currentUser.DownloadFolder}/{fileName}", FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }

        /// <summary>
        /// Скачать документ
        /// </summary>
        /// <param name="messageEvent"></param>
        public void DownloadDocument(Telegram.Bot.Args.MessageEventArgs messageEvent)
        {
            DownLoadFile(messageEvent.Message.Document.FileId, @"Document/" + messageEvent.Message.Document.FileName);

            mainWindow.Dispatcher.Invoke(() =>
            {
                currentUser.AddMessage(new Message(currentUser.FirstName, "Я отправил документ", DateTime.Now));
            });

            SendMessage("Я скачал данный файл", currentUser);
        }

        /// <summary>
        /// Скачать аудиосообщение
        /// </summary>
        /// <param name="messageEvent"></param>
        public void DownloadVoice(Telegram.Bot.Args.MessageEventArgs messageEvent)
        {
            DownLoadFile(messageEvent.Message.Voice.FileId, $@"Voice/{DateTime.Now.ToString("yyyy.MM.dd HH_mm_ss")}.ogg");

            mainWindow.Dispatcher.Invoke(() =>
            {
                currentUser.AddMessage(new Message(currentUser.FirstName, "Я отправил аудиосообщение", DateTime.Now));
            });

            SendMessage("Я скачал данное аудиосообщение", currentUser);
        }        
        
        /// <summary>
        /// Скачать фото
        /// </summary>
        /// <param name="messageEvent"></param>
        public void DownloadPhoto(Telegram.Bot.Args.MessageEventArgs messageEvent)
        {
            DownLoadFile(messageEvent.Message.Photo[messageEvent.Message.Photo.Length - 1].FileId, $@"Photo/{DateTime.Now.ToString("yyyy.MM.dd HH_mm_ss")}.jpg");

            mainWindow.Dispatcher.Invoke(() =>
            {
                currentUser.AddMessage(new Message(currentUser.FirstName, "Я отправил фото", DateTime.Now));
            });

            SendMessage("Я скачал данное фото", currentUser);
        }

        /// <summary>
        /// Отправить сообщение на команду /Start
        /// </summary>
        public void SendStartMessage()
        {
            string textMessage = $"Вот что я могу:\n" +
                                 $"1) Сохраняю любой отправленный файл, аудиосообщение или фото\n" +
                                 $"2) На команду /Download отправляю тебе любой сохраненный файл\n" +
                                 $"3) Обычные сообщения я просто повторяю\n" +
                                 $"Давай начнем!\n";

            if (currentUser != null)
            {
                SendMessage(textMessage, currentUser);
            }
        }

        /// <summary>
        /// Выбрать тип файла
        /// </summary>
        public void SelectTypeFile()
        {
            string sendMessage = "Выбери тип файла:\n" +
                                 "1 - Документ\n" +
                                 "2 - Аудиосообщение\n" +
                                 "3 - Фото";

            if (currentUser != null)
            {
                SendMessage(sendMessage, currentUser);
            }
        }

        /// <summary>
        /// Проверка типа файла
        /// </summary>
        /// <param name="message">Сообщение</param>
        public void CheckTypeFile(string message)
        {
            int answer;

            if (Int32.TryParse(message, out answer) && answer > 0 && answer < 4)
            {
                switch (answer)
                {
                    case 1:
                        currentUser.DownloadFolder += @"\Document";
                        break;
                    case 2:
                        currentUser.DownloadFolder += @"\Voice";
                        break;
                    case 3:
                        currentUser.DownloadFolder += @"\Photo";
                        break;
                }

                SelectFile();
            }
            else
            {
                SendMessage("Такого типа файла нет(", currentUser);
                currentUser.isDownload = false;
            }
        }

        /// <summary>
        /// Выбор файла для скачивания
        /// </summary>
        /// <param name="messageEvent"></param>
        public void SelectFile()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(currentUser.DownloadFolder);
            var namesFile = directoryInfo.GetFiles();
            string messageFile;

            if (namesFile.Length > 0)
            {
                messageFile = "Выбери файл для скачивания:\n";

                for (int i = 0; i < namesFile.Length; i++)
                {
                    messageFile += $"{i + 1} - {namesFile[i].Name}\n";
                }
            }
            else
            {
                messageFile = "Файлов для скачивания нет.";
                currentUser.isDownload = false;
                currentUser.DownloadFolder = @"Download";
            }

            SendMessage(messageFile, currentUser);
        }

        /// <summary>
        /// Проверка выбранного файла
        /// </summary>
        /// <param name="messageEvent"></param>
        public void CheckFile(string message)
        {
            int answer;

            DirectoryInfo directoryInfo = new DirectoryInfo(currentUser.DownloadFolder);
            var files = directoryInfo.GetFiles();

            if (Int32.TryParse(message, out answer) && answer > 0 && answer <= files.Length)
            {
                switch (directoryInfo.Name)
                {
                    case "Document":
                        SendDocument(files[answer - 1].FullName, files[answer - 1].Name);
                        break;
                    case "Photo":
                        SendPhoto(files[answer - 1].FullName);
                        break;
                    case "Voice":
                        SendVoice(files[answer - 1].FullName);
                        break;
                }
            }
            else
            {
                SendMessage("Такого файла нет(", currentUser);
            }

            currentUser.isDownload = false;
            currentUser.DownloadFolder = @"Download";
        }

        /// <summary>
        /// Отправить файл с диска
        /// </summary>
        /// <param name="chatId">ID чата</param>
        /// <param name="fileName">Имя файла</param>
        public async void SendDocument(string path, string fileName)
        {
            using (var fs = File.OpenRead(path))
            {
                Telegram.Bot.Types.InputFiles.InputOnlineFile iof =
                    new Telegram.Bot.Types.InputFiles.InputOnlineFile(fs);

                iof.FileName = fileName;

                var send = await bot.SendDocumentAsync(currentUser.Id, iof);
            }
        }

        /// <summary>
        /// Отправить аудиосообщение с диска
        /// </summary>
        /// <param name="chatId">ID чата</param>
        /// <param name="fileName">Имя файла</param>
        public async void SendVoice(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                Telegram.Bot.Types.InputFiles.InputOnlineFile iof =
                    new Telegram.Bot.Types.InputFiles.InputOnlineFile(fs);

                var send = await bot.SendVoiceAsync(currentUser.Id, iof);
            }
        }

        /// <summary>
        /// Отправить фото с диска
        /// </summary>
        /// <param name="chatId">ID чата</param>
        /// <param name="fileName">Имя файла</param>
        public async void SendPhoto(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                Telegram.Bot.Types.InputFiles.InputOnlineFile iof =
                    new Telegram.Bot.Types.InputFiles.InputOnlineFile(fs);

                var send = await bot.SendPhotoAsync(currentUser.Id, iof);
            }
        }

        /// <summary>
        /// Сериализация истории сообщений
        /// </summary>
        public void jsonSerializationHistoryMessage()
        {
            string result = JsonConvert.SerializeObject(Users);

            File.WriteAllText($@"MessageHistory/{DateTime.Now.ToString("dd MM yyyy HH mm ss")}.json", result);
        }

        #endregion
    }
}
