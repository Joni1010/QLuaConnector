using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketObject
{
    /// <summary>
    /// Портфель
    /// </summary>
    public class Portfolio
    {
        /// <summary> Торговый счет портфеля </summary>
        public Account Account = null;
        /// <summary> Баланс по позициям предыдущей торговой сессии </summary>
        public decimal LastPositionBalance = 0;
        /// <summary> Баланс по позициям   </summary>
        public decimal PositionBalance = 0;
        /// <summary> Баланс текущих свободных средств </summary>
        public decimal CurrentBalance = 0;
        /// <summary> Тип лимита </summary>
        public int LimitKind;
        /// <summary> Предыдущий баланс </summary>
        public decimal PrevBalance = 0;
        /// <summary> Общий баланс </summary>
        public decimal Balance = 0;
        /// <summary> Общая вариационная маржа </summary>
        public decimal VarMargin = 0;
        /// <summary> Реально начисленная в ходе клиринга вариационная маржа. Отображается с точностью до 2 двух знаков. При этом, в поле "varmargin" транслируется вариационная маржа, рассчитанная с учетом установленных границ изменения цены   </summary>
        public decimal RealMargin = 0;
        /// <summary> Относительная величина изменения стоимости всех позиций клиента </summary>
        public decimal RateChange = 0;
        /// <summary> Накопленный купонный доход(NUMBER) </summary>
        public decimal Accruedint = 0;
        /// <summary> Биржевые сборы  (NUMBER) </summary>
        public decimal Commission = 0;
        /// <summary> Тэг расчетов  (STRING) для акций  </summary>
        public string Tag = "";
        /// <summary> Клиент </summary>
        public Client Client = null;
    }
}
