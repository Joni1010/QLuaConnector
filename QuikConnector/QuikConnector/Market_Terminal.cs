using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketObject
{
    /// <summary>
    /// Класс торгового терминала.
    /// Содержит необходимые параметры по терминалу.
    /// </summary>
    public class MarketTerminal
    {
        /// <summary> Версия терминала </summary>
        public string Version;
        /// <summary> Дата торгов </summary>
        public DateTime TradeDate;
        /// <summary> Время сервера </summary>
        public TimeSpan ServerTime;
        /// <summary> Время последней, полученной с сервера, записи </summary>
        public TimeSpan LastRecordTime;
        private bool _isConnect;
        /// <summary> соединение ("установлено"/"не установлено") </summary>
        public bool Connect
        {
            set
            {
                bool tmp = value;
                if (tmp != this._isConnect)
                {
                    if (tmp)
                        if (this.OnConnected != null) OnConnected(this);
                    if (!tmp)
                        if (this.OnDisconnected != null) OnDisconnected(this);
                    this._isConnect = tmp;
                }
            }
            get { return this._isConnect; }
        }
        /// <summary> IP-адрес сервера </summary>
        public string IpServerAddr;
        /// <summary> Порт сервера </summary>
        public string Port;
        /// <summary> Описание сервера </summary>
        public string ServerDescription;
        /// <summary> Пользователь </summary>
        public string User;
        /// <summary> ID пользователя </summary>
        public string UserID;
        /// <summary> Организация </summary>
        public string Organization;
        /// <summary> Текущее время </summary>
        public TimeSpan Localtime;
        /// <summary> Время на связи </summary>
        public TimeSpan ConnectionTime;
        ///////////////////////////////////////////////////
        public delegate void TerminalEvent(object Object);
        /// <summary> Событие подключения терминала </summary>
        public event TerminalEvent OnConnected;
        /// <summary> Событие отключения терминала </summary>
        public event TerminalEvent OnDisconnected;
    }
}
