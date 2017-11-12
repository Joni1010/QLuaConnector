using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketObject;

namespace QuikControl
{
    public class MarketTools
    {
        /// <summary>
        /// Список событий, которые выполняются отложенно(При возникновении события). Прогружает события в очередях.
        /// </summary>
        public static List<MarketElemActivatorEvent> ListAllDeferredEventBase = new List<MarketElemActivatorEvent>();
        public static List<MarketElemActivatorEvent> ListAllDeferredEventTrades = new List<MarketElemActivatorEvent>();
        public static List<MarketElemActivatorEvent> ListAllDeferredEventMarkets = new List<MarketElemActivatorEvent>();

        /// <summary> Объект сделок </summary>
        public MarketElement<Trade> tTrades = new MarketElement<Trade>(ListAllDeferredEventTrades);
        /// <summary> Все сделки </summary>
        public IEnumerable<Trade> Trades { get { return tTrades.AsIEnumerable; } }
        /// <summary> Кол-во сделок </summary>
        public decimal CountTrades { get { return tTrades.Count; } }
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект "моих" сделок </summary>
        public MarketElement<MyTrade> tMyTrades = new MarketElement<MyTrade>(ListAllDeferredEventBase);
        /// <summary> Коллекция сделок </summary>
        public IEnumerable<MyTrade> MyTrades { get { return tMyTrades.AsIEnumerable; } }
        /// <summary> Кол-во сделок </summary>
        public decimal CountMyTrades { get { return tMyTrades.Count; } }
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект заявок </summary>
        public MarketElement<Order> tOrders = new MarketElement<Order>(ListAllDeferredEventBase);
        /// <summary> Все заявки </summary>
        public IEnumerable<Order> Orders { get { return tOrders.AsIEnumerable; } }
        /// <summary> Кол-во заявок </summary>
        public decimal CountOrders { get { return tOrders.Count; } }
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект стоп-заявок </summary>
        public MarketElement<StopOrder> tStopOrders = new MarketElement<StopOrder>(ListAllDeferredEventBase);
        /// <summary> Все стоп-заявки </summary>
        public IEnumerable<StopOrder> StopOrders { get { return tStopOrders.AsIEnumerable; } }
        /// <summary> Кол-во заявок </summary>
        public decimal CountStopOrders { get { return tStopOrders.Count; } }
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект инструментов </summary>
        public MarketElement<Securities> tSecurities = new MarketElement<Securities>(ListAllDeferredEventBase);
        /// <summary> Все инструменты </summary>
        public IEnumerable<Securities> Securities { get { return tSecurities.AsIEnumerable;} }
        /// <summary> Кол-во инструментов </summary>
        public decimal CountSec { get { return tSecurities.Count; } }
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект позиций </summary>
        public MarketElement<Position> tPositions = new MarketElement<Position>(ListAllDeferredEventBase);
        /// <summary> Все позиции </summary>
        public IEnumerable<Position> Positions { get { return tPositions.AsIEnumerable; } }
        /// <summary> Кол-во позиций </summary>
        public decimal CountPositions { get { return tPositions.Count; } }
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект поортфелей </summary>
        public MarketElement<Portfolio> tPortfolios = new MarketElement<Portfolio>(ListAllDeferredEventBase);
        /// <summary> Все поортфели </summary>
        public IEnumerable<Portfolio> Portfolios { get { return tPortfolios.AsIEnumerable; } }
        /// <summary> Кол-во поортфелей </summary>
        public decimal CountPortfolios { get { return tPortfolios.Count; } }
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект для стаканов </summary>
        //public ToolsQuote tQuote = new ToolsQuote();
        public MarketElement<Quote> tQuote = new MarketElement<Quote>(ListAllDeferredEventMarkets);
        /// <summary> Событие изменения стакана </summary>
        //public ToolsQuote.eventQuote OnChangeQuote { set { tQuote.OnQuote += value; } }

        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект для транзакций </summary>
        public ToolsTrans tTransaction = new ToolsTrans();

        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект для клиентов </summary>
        public MarketElement<Client> tClients = new MarketElement<Client>(ListAllDeferredEventBase);
        /// <summary> Коллекция всех клиентов </summary>
        public IEnumerable<Client> Clients { get { return tClients.AsIEnumerable; } }
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект для фирм </summary>
        public MarketElement<Firm> tFirms = new MarketElement<Firm>(ListAllDeferredEventBase);
        /// <summary> Коллекция фирм </summary>
        public IEnumerable<Firm> Firms { get { return tFirms.AsIEnumerable; } }
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект для классов </summary>
        public MarketElement<MarketClass> tClasses = new MarketElement<MarketClass>(ListAllDeferredEventBase);
        /// <summary> Все классы </summary>
        public IEnumerable<MarketClass> Classes { get { return tClasses.AsIEnumerable; } }
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary> Объект для счетов </summary>
        public MarketElement<Account> tAccounts = new MarketElement<Account>(ListAllDeferredEventBase);
        /// <summary> Коллекция счетов </summary>
        public IEnumerable<Account> Accounts { get { return tAccounts.AsIEnumerable; } }
        //////////////////////////////////////////////////////////////////////////////////



    }
}
