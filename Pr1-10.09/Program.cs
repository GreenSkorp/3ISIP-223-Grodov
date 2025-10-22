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
        private Dictionary<string, int> warehouse = new Dictionary<string, int>(); // склад: деталь -> количество
        private Dictionary<string, int> partPrices = new Dictionary<string, int>(); // цены деталей
        private int currentStage = 0; // текущий этап (клиент)
        private int successfulRepairs = 0; // счетчик успешных ремонтов

        // Штрафы и настройки
        private const int PENALTY_REFUSAL = 50; // штраф за отказ
        private const int PENALTY_WRONG_PART = 100; // штраф за неправильную деталь
        private const int SERVICE_FEE = 30; // плата за работу

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
            warehouse["Двигатель"] = 1;
            warehouse["Тормозные колодки"] = 3;
            warehouse["Аккумулятор"] = 2;
            warehouse["Шины"] = 4;
            warehouse["Фары"] = 2;
            warehouse["Трансмиссия"] = 1;
            warehouse["Топливный насос"] = 2;
            warehouse["Стартер"] = 1;
        }


        public void Start()
        {
            Console.WriteLine("=== ДОБРО ПОЖАЛОВАТЬ В АВТОСЕРВИС! ===");
            Console.WriteLine($"Начальный баланс: {balance} руб.");
            Console.WriteLine("Удачи в бизнесе!\n");

            while (true)
            {
                currentStage++;

                // Показываем этап и меню подготовки
                ShowStagePreparation();

                // Генерируем клиента для этого этапа
                Customer customer = GenerateCustomer();
                ProcessCustomer(customer);

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
        private void ShowStagePreparation()
        {
            Console.WriteLine($"\n=========================================");
            Console.WriteLine($"          ЭТАП {currentStage}");
            Console.WriteLine($"     Баланс: {balance} руб.");
            Console.WriteLine($"=========================================");

            bool preparationComplete = false;

            while (!preparationComplete)
            {
                Console.WriteLine("\nПодготовка к клиенту - выберите действие:");
                Console.WriteLine("1 - Показать склад");
                Console.WriteLine("2 - Закупить запчасти");
                Console.WriteLine("3 - Перейти к клиенту");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowWarehouse();
                        break;
                    case "2":
                        ShowPurchaseMenu();
                        break;
                    case "3":
                        preparationComplete = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }



    }
    class Client
    {

    }


    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
