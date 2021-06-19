using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork10
{
    public class Message
    {
        #region Свойства

        /// <summary>
        /// Имя отправителя сообщения
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Время сообщения
        /// </summary>
        public DateTime Date { get; private set; }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Новое сообщение
        /// </summary>
        /// <param name="text">Текст сообщения</param>
        /// <param name="date">Время сообщения</param>
        public Message(string firstName, string text, DateTime date)
        {
            this.FirstName = firstName;
            this.Text = text;
            this.Date = date;
        }

        #endregion
    }
}
