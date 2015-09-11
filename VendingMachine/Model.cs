using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine
{
    class Machine
    {
        private Wallet _wal;
        private string[] _itemsNames;
        private uint[] _itemsPrices;
        private uint[] _itemsNums;
        private uint UserMoney;

        /// <summary>
        /// Массивы товаров. Все массивы должны быть одинаковой длины.
        /// </summary>
        /// <param name="itemsNames">Массив имен товаров</param>
        /// <param name="itemsPrices">Массив цен на товары</param>
        /// <param name="itemsNums"Массив количеств товаров></param>
        public Machine(string[] itemsNames, uint[] itemsPrices, uint[] itemsNums)
        {
            if (itemsNames.Length != itemsNums.Length || itemsNames.Length != itemsPrices.Length)
                throw new ArgumentException("Array lengths have to be equal.");
            _itemsNames = (string[])itemsNames.Clone();
            _itemsPrices = (uint[])itemsPrices.Clone();
            _itemsNums = (uint[])itemsNums.Clone();
            _wal = new Wallet();
        }

        /// <summary>
        /// Внести деньги в автомат
        /// </summary>
        /// <param name="m">Кошелек с монетами</param>
        public void PutMoney(Wallet m)
        {
            UserMoney = m.Total;
            _wal.AddMoney(m);
        }

        /// <summary>
        /// Выбрать товар. Кидает исключение в случае неудачи.
        /// </summary>
        /// <param name="id">Номер товара</param>
        public void GetItem(uint id)
        {
            if (id < 0 || id > _itemsPrices.Length - 1)
                throw new IndexOutOfRangeException();
            if (_itemsNums[id] < 1)
                throw new ArgumentException("Product is not available.");
            if (UserMoney < _itemsPrices[id])
                throw new ArgumentException("Not enough money.");
            _itemsNums[id]--;
            UserMoney -= _itemsPrices[id];
        }

        /// <summary>
        /// Возвращает сдачу
        /// </summary>
        /// <returns>Кошелек со сдачей</returns>
        public Wallet ReturnChange()
        {
            return _wal.GetMoney(UserMoney);
        }

        /// <summary>
        /// Представление состояния аппарата в виде текста
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string tmp = "";
            tmp += string.Format("{0,5} {1,20}, {2,10}, {3,5}\n", "Номер", "Название", "Цена", "Количество");
            for(int i=0;i<_itemsNames.Length;i++)
            {
                tmp += string.Format("{0,5} {1,20}, {2,10}, {3,5}\n", i, _itemsNames[i], _itemsPrices[i], _itemsNums[i]);
            }

            tmp += "Внесено: " + UserMoney + "\n";
            return tmp;
        }
    }

    /// <summary>
    /// Кошелек. Структура, хранящая денежные средства в виде монет.
    /// </summary>
    struct Wallet
    {
        private uint[] _values = { 1, 2, 5, 10 }; //Сортированный массив номиналов монет
        public uint[] Coins = { 0, 0, 0, 0 }; //Массив количеств монет

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Wallet()
        {
        }

        /// <summary>
        /// Создает кошелек из массива количеств монет
        /// </summary>
        /// <param name="coins">Массив количеств монет</param>
        public Wallet(uint[] coins)
        {
            if (coins.Length != _values.Length)
                throw new ArgumentException("Array of invalid length.");
            Coins = (uint[])coins.Clone();
        }

        /// <summary>
        /// Создает кошелек с определенной суммой из случайных монет
        /// </summary>
        /// <param name="total">Сумма</param>
        /// <param name="gen">ГСЧ</param>
        public Wallet(uint total, Random gen)
        {
            int res = 0;
            while (res != -1)
            {
                total -= (uint)res;
                res = AddRandomCoin(total, gen);
            }
        }

        /// <summary>
        /// Создает кошелек с определенной суммой по оптимальному алгоритму
        /// </summary>
        /// <param name="total">Сумма</param>
        public Wallet(uint total)
        {
            int res = 0;
            while (res != -1)
            {
                total -= (uint)res;
                res = AddCoin(total);
            }
        }

        /// <summary>
        /// Извлекает из кошелька заданную сумму оптимальным способом
        /// </summary>
        /// <param name="total">Сумма</param>
        /// <returns>Кошелек с заданной суммой</returns>
        public Wallet GetMoney(uint total)
        {
            Wallet tmp = new Wallet();
            int res = -1;
            while (res != -1)
            {
                if (res != -1)
                {
                    total -= _values[res];
                    Coins[res]--;
                    tmp.Coins[res]++;
                }
                res = ChooseCoin(total);
            }
            return tmp;
        }

        /// <summary>
        /// Внести деньги в кошелек
        /// </summary>
        /// <param name="w">Кошелек из которого переносятся деньги</param>
        public void AddMoney(Wallet w)
        {
            for (int i = 0; i < Coins.Length; i++)
            {
                Coins[i] += w.Coins[i];
            }
        }

        /// <summary>
        /// Общая сумма в кошельке
        /// </summary>
        public uint Total
        {
            get
            {
                uint t = 0;
                for (int i = 0; i < Coins.Length; i++)
                {
                    t += _values[i] * Coins[i];
                }
                return t;
            }
        }

        /// <summary>
        /// Выбирает оптимальную монету из доступных в кошельке
        /// </summary>
        /// <param name="total">Сумма, которую нужно набрать</param>
        /// <returns>Номер номинала монеты</returns>
        private int ChooseCoin(uint total)
        {
            if (total <= 0)
                return -1;

            int e = _values.Length;
            while (_values[e - 1] > total || _values[e - 1] == 0)
            {
                e--;
                if (e < 0)
                    throw new ArgumentOutOfRangeException("It's impossible to get asked Sum with available coins.");
            }
            return e - 1;
        }

        /// <summary>
        /// Добавляет в кошелек оптимальную монету для набора суммы
        /// </summary>
        /// <param name="total">Сумма</param>
        /// <returns>Номинал монеты</returns>
        private int AddCoin(uint total)
        {
            if (total <= 0)
                return -1;

            if (total < _values[0])
                throw new ArgumentOutOfRangeException("It's impossible to create Wallet with current value with allowed coins.");

            int e = _values.Length;
            while (_values[e - 1] > total)
            {
                e--;
            }

            int r = e - 1;
            Coins[r]++;
            return (int)_values[r];
        }

        /// <summary>
        /// Добавляет в кошелек случайную монету для набора суммы
        /// </summary>
        /// <param name="total">Сумма</param>
        /// <param name="gen">ГСЧ</param>
        /// <returns>Номинал монеты</returns>
        private int AddRandomCoin(uint total, Random gen)
        {
            if (total < _values[0])
                return -1;

            int e = _values.Length;
            while (_values[e - 1] > total)
            {
                e--;
            }

            int r = gen.Next(e);
            Coins[r]++;
            return (int)_values[r];
        }

    }
}
