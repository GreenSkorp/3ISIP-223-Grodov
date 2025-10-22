using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr1_10._09
{
    class AutoService
    {
        private Random random = new Random();
        private int balance = 1000; // начальный баланс
        private Dictionary<string, int> Storage = new Dictionary<string, int>(); // склад: деталь -> количество
        private Dictionary<string, int> partPrices = new Dictionary<string, int>(); // цены деталей
        private List<Order> Orders;
        private int currentStage = 0; // текущий этап (клиент)
        private int successfulRepairs = 0; // счетчик успешных ремонтов

        // Штрафы и настройки
        private const int Otkaz = 50; // штраф за отказ
        private const int WrongPart = 100; // штраф за неправильную деталь

        private void InitializeParts()
        {
            // Инициализация деталей и их цен
            partPrices["Двигатель"] = 300;
            partPrices["Тормозные колодки"] = 50;
            partPrices["Аккумулятор"] = 80;
            partPrices["Шины"] = 60;
            partPrices["Фары"] = 40;
            partPrices["Трансмиссия"] = 200;
            partPrices["Топливный насос"] = 70;
            partPrices["Стартер"] = 90;

            // Начальный склад
            Storage["Двигатель"] = 1;
            Storage["Тормозные колодки"] = 3;
            Storage["Аккумулятор"] = 2;
            Storage["Шины"] = 4;
            Storage["Фары"] = 2;
            Storage["Трансмиссия"] = 1;
            Storage["Топливный насос"] = 2;
            Storage["Стартер"] = 1;
        }


        public void Start()
        {
            this.InitializeParts();

            Console.WriteLine("=== ДОБРО ПОЖАЛОВАТЬ В АВТОСЕРВИС! ===");
            Console.WriteLine($"Начальный баланс: {balance} руб.");
            Console.WriteLine("Удачи в бизнесе!\n");

            while (true)
            {
                currentStage++;

                // Показываем этап и меню подготовки
                ShowStagePreparation();

                // Генерируем клиента для этого этапа

                //Customer customer = GenerateCustomer();
                //ProcessCustomer(customer);

                // Проверяем условие окончания игры
                if (balance < 0)
                {
                    Console.WriteLine("\n=== ИГРА ОКОНЧЕНА ===");
                    Console.WriteLine("Вы разорились!");
                    Console.WriteLine($"Всего этапов: {currentStage}");
                    Console.WriteLine($"Успешных ремонтов: {successfulRepairs}");
                    break;
                }

                if (currentStage >= 20) // ограничение по количеству этапов
                {
                    Console.WriteLine("\n=== ИГРА ОКОНЧЕНА ===");
                    Console.WriteLine("Рабочий день завершен!");
                    Console.WriteLine($"Итоговый баланс: {balance} руб.");
                    Console.WriteLine($"Успешных ремонтов: {successfulRepairs} из {currentStage}");
                    break;
                }
            }
        }
        public void WriteColorLine(string txr, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(txr);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void WriteColor(string txr, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(txr);
            Console.ForegroundColor = ConsoleColor.White;
        }
        private void ShowStagePreparation()
        {
            Console.Clear();
            Console.WriteLine($"\n=========================================");
            WriteColorLine($"          Заказ {currentStage}", ConsoleColor.DarkYellow);
            WriteColorLine($"     Баланс: {balance} руб.", ConsoleColor.DarkYellow);
            Console.WriteLine($"=========================================\n");
            OrderCheck();

            bool preparationComplete = false;
            while (!preparationComplete)
            {
                WriteColorLine("Подготовка к клиенту - выберите действие:", ConsoleColor.DarkCyan);
                Console.WriteLine("1 - Показать склад");
                Console.WriteLine("2 - Закупить запчасти");
                Console.WriteLine("3 - Перейти к клиенту");
                WriteColor("Выбранный пункт: ", ConsoleColor.Yellow);
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowStorage();
                        break;
                    case "2":
                        ShowPurchaseMenu();
                        break;
                    case "3":
                        NewClient();
                        preparationComplete = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }

        private void NewClient()
        {
            int randomIndex = random.Next(partPrices.Count);
            string BrokenPart = partPrices.Keys.ElementAt(randomIndex);
            int BrokenPrice = partPrices[BrokenPart];
            Console.Clear();

            Console.WriteLine($"\n=========================================");
            WriteColorLine($"          Сломано - {BrokenPart}", ConsoleColor.DarkRed);
            WriteColorLine($"     Выручка при выполнении: {BrokenPrice*1,4} руб.", ConsoleColor.DarkYellow);
            Console.WriteLine($"=========================================\n");
            bool Choise = false;
            while (!Choise) {
                Console.WriteLine("1. Выполнить заказ");
                Console.WriteLine("2. Отказаться от заказа");

            }

            Console.ReadLine();

        }

        private void ShowStorage()
        {
            Console.WriteLine("\nСклад: Предмет -> Количество на складе");
            foreach (KeyValuePair<string, int> pair in Storage)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
            Console.WriteLine();

        }
        private void OrderCheck()
        {

        }
        private void ShowPurchaseMenu()
        {
            int i = 1;
            Order order = new Order(currentStage + 2);
            List<string> parts = new List<string>();
            bool choise = false;
            ShowStorage();
            WriteColorLine("\nЗакупка: Предмет -> Цена", ConsoleColor.DarkYellow);
                foreach (KeyValuePair<string, int> pair in partPrices)
                {
                    Console.WriteLine($"{i}. {pair.Key}: {pair.Value}");
                    i++;
                    parts.Add(pair.Key);
                }
                Console.WriteLine("0. Закончить покупку") ;
            while (!choise)
            {




                WriteColor("Выбранный часть для заказа: ", ConsoleColor.Yellow);
                int chos = Convert.ToInt32(Console.ReadLine());
                if (chos == 0) { WriteColorLine("Покупка завершена", ConsoleColor.DarkGreen); choise = true; }
                else
                {
                    Console.Write("Введите кол-во товара для покупки: "); int nums = Convert.ToInt32(Console.ReadLine()); 
                    string part = parts[chos - 1];
                    order.Add(part, nums);
                    WriteColorLine($"В заказ добавлено - {part}: {nums}", ConsoleColor.DarkGreen);
                    balance = balance - partPrices[part]*nums;
                    Console.WriteLine($"Баланс: {balance}");
                }
            }
        }



    }
    class Order
    {
        public int Stage;
        public Dictionary<string, int> ordering = new Dictionary<string, int>();
        public Order(int stage)
        {
            Stage = stage;
        }
        public void WriteColorLine(string txr, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(txr);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void WriteColor(string txr, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(txr);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void Add(string part, int num)
        {
            ordering[part] = num;
        }
        public void List()
        {
            foreach (KeyValuePair<string, int> pair in ordering)
            {
                WriteColorLine("Товары в заказе", ConsoleColor.Blue);
                Console.WriteLine($"{pair.Key}: {pair.Value}");

            }
        }
    }



    class Program
    {

        static void Main(string[] args)
        {
            AutoService service = new AutoService();
            service.Start();
        }
    }
}
