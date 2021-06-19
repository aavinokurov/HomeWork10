using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork10
{
    public class User
    {
        #region Свойства

        /// <summary>
        /// Id клиента
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Лист сообщений
        /// </summary>
        public ObservableCollection<Message> Messages { get; private set; }

        /// <summary>
        /// Загружает ли пользователь файл
        /// </summary>
        public bool isDownload { get; set; } = false;

        /// <summary>
        /// Папка для скачивания
        /// </summary>
        public string DownloadFolder { get; set; } = @"Download";

        #endregion

        #region Конструкторы

        /// <summary>
        /// Добавить пользователя
        /// </summary>
        /// <param name="id">Id пользователя</param>
        /// <param name="firstName">Имя пользователя</param>
        public User(long id, string firstName)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.Messages = new ObservableCollection<Message>();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавить новое сообщение
        /// </summary>
        /// <param name="message">Сообщение</param>
        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }

        #endregion
    }
}
