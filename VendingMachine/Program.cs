using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachine
{
    class Program
    {
        private static Machine m;
        private static Wallet u;
        static void Main(string[] args)
        {
            Random gen = new Random();
            m = new Machine(new string[] { "Кексы", "Печенье", "Вафли" }, new uint[] { 50, 10, 30 }, new uint[] { 4, 3, 10 });
            u = new Wallet(150, gen);

            while (true)
            {
                ShowCurState();
                ShowTip();
                int s = GetCom();
                if (s == -2)
                    break;
            }
        }

        public static void ShowCurState()
        {
            Console.Write(m.ToString());
            Console.WriteLine("У вас: " + u.Total + ".");
        }

        public static void ShowTip()
        {
            Console.Write("Выберете действие:\n1. Внести деньги \n2. Выбрать товар\n3. Забрать сдачу\n4. Выход\n");
        }

        public static int GetCom()
        {
            string tmp = Console.ReadLine();
            switch (tmp)
            {
                case "1":
                    InsertMoney();
                    break;
                case "2":
                    ChooseItem();
                    break;
                case "3":
                    GetChange();
                    break;
                case "4":
                    return -2;
                default:
                    return -1;
            }
            return 0;
        }

        public static void InsertMoney()
        {
            try
            {
                Console.Write("Введите сумму, которую вы хотите внести: ");
                string tmp = Console.ReadLine();
                uint a = uint.Parse(tmp);
                Wallet tmpw = u.GetMoney(a);
                m.PutMoney(tmpw);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void ChooseItem()
        {
            try
            {
                Console.Write("Выберете номер товара: ");
                string tmp = Console.ReadLine();
                uint a = uint.Parse(tmp);
                m.GetItem(a);
                Console.WriteLine("Успешно");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void GetChange()
        {
            u.AddMoney(m.ReturnChange());
        }
    }
}
