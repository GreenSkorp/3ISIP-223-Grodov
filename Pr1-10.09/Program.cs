using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr1_10._09
{
    class AutoService
    {
        private Random random = new Random();
        private int balance = 1000;
        private Dictionary<string, int> Storage = new Dictionary<string, int>();
        private Dictionary<string, int> partPrices = new Dictionary<string, int>();
        private List<Order> Orders = new List<Order>();
        private List<Order> pendingOrders = new List<Order>();
        private int currentStage = 0;
        private int successfulRepairs = 0;

        private const int Otkaz = 50;
        private const int WrongPart = 100;

        private void InitializeParts()
        {
            partPrices["Двигатель"] = 300;
            partPrices["Тормозные колодки"] = 50;
            partPrices["Аккумулятор"] = 80;
            partPrices["Шины"] = 60;
            partPrices["Фары"] = 40;
            partPrices["Трансмиссия"] = 200;
            partPrices["Топливный насос"] = 70;
            partPrices["Стартер"] = 90;

            Storage["Двигатель"] = 1;
            Storage["Тормозные колодки"] = 3;
            Storage["Аккумулятор"] = 2;
            Storage["Шины"] = 4;
            Storage["Фары"] = 2;
            Storage["Трансмиссия"] = 1;
            Storage["Топливный насос"] = 2;
            Storage["Стартер"] = 1;
        }

        // Используем существующую модель БД
        private void SaveGameResultsSimple()
        {
            try
            {
                using (var db = new AutoServeceEntities()) // Замените на ваш DbContext
                {
                    // Создаем игрока
                    var player = new Player
                    {
                        PlayerName = $"Игрок_{DateTime.Now:yyyyMMdd_HHmmss}",
                        Balance = this.balance
                    };
                    db.Player.Add(player);
                    db.SaveChanges(); // Сохраняем чтобы получить PlayerID

                    // Сохраняем детали и склад
                    foreach (var storageItem in Storage)
                    {
                        string partName = storageItem.Key;
                        int quantity = storageItem.Value;

                        // Находим или создаем деталь в таблице Parts
                        var part = db.Parts.FirstOrDefault(p => p.PartName == partName);
                        if (part == null)
                        {
                            // Если детали нет в БД - создаем ее
                            part = new Parts
                            {
                                PartName = partName,
                                PartPrice = partPrices[partName] // Используем цену из игры
                            };
                            db.Parts.Add(part);
                            db.SaveChanges(); // Сохраняем чтобы получить PartsID
                        }

                        // Создаем запись на складе игрока
                        var playerPart = new PlayersParts
                        {
                            PlayerID = player.PlayerID,
                            PartsID = part.PartsID,
                            Count = quantity
                        };
                        db.PlayersParts.Add(playerPart);
                    }

                    db.SaveChanges();
                    Console.WriteLine("\n=== ВСЕ ДАННЫЕ СОХРАНЕНЫ В БАЗЕ ДАННЫХ ===");
                    Console.WriteLine($"Игрок: {player.PlayerName}");
                    Console.WriteLine($"Баланс: {player.Balance}");
                    Console.WriteLine($"Деталей на складе: {Storage.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения в БД: {ex.Message}");
            }
        }

        // Альтернативный вариант - если нужно сохранять только статистику игры
        private void SaveGameResults()
        {
            try
            {
                using (var db = new AutoServeceEntities()) // Замените на ваш существующий DbContext
                {
                    var player = new Player
                    {
                        PlayerName = $"Игрок_{DateTime.Now:yyyyMMdd_HHmmss}",
                        Balance = balance
                    };
                    db.Player.Add(player);
                    db.SaveChanges();

                    Console.WriteLine($"\nРезультаты сохранены! ID игрока: {player.PlayerID}");
                    Console.WriteLine($"Имя: {player.PlayerName}");
                    Console.WriteLine($"Баланс: {player.Balance}");
                    Console.WriteLine($"Этапы: {currentStage}, Успешные ремонты: {successfulRepairs}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения в БД: {ex.Message}");
            }
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
                ShowStagePreparation();

                if (balance < 0)
                {
                    Console.WriteLine("\n=== ИГРА ОКОНЧЕНА ===");
                    Console.WriteLine("Вы разорились!");
                    Console.WriteLine($"Всего этапов: {currentStage}");
                    Console.WriteLine($"Успешных ремонтов: {successfulRepairs}");
                    SaveGameResultsSimple(); // Используем упрощенный вариант
                    break;
                }

                if (currentStage >= 20)
                {
                    Console.WriteLine("\n=== ИГРА ОКОНЧЕНА ===");
                    Console.WriteLine("Рабочий день завершен!");
                    Console.WriteLine($"Итоговый баланс: {balance} руб.");
                    Console.WriteLine($"Успешных ремонтов: {successfulRepairs} из {currentStage}");
                    SaveGameResultsSimple(); // Используем упрощенный вариант
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
        private void OrderCheck()
        {
            if (pendingOrders.Count > 0)
            {
                WriteColorLine($"Ожидается поставок: {pendingOrders.Count}", ConsoleColor.Cyan);

                // Показываем детали и таймеры всех ожидающих заказов
                foreach (var order in pendingOrders)
                {
                    WriteColorLine($"Заказ №{order.Stage} (доставка через {2 - order.DeliveryTimer} клиентов):", ConsoleColor.Blue);
                    foreach (var item in order.ordering)
                    {
                        Console.WriteLine($"  {item.Key}: {item.Value} шт.");
                    }
                }
            }
            else
            {
                WriteColorLine("Активных заказов нет", ConsoleColor.Gray);
            }
        }
        private void NewClient()
        {
            // СНАЧАЛА ПРОВЕРЯЕМ ПОСТАВКИ (теперь для каждого заказа отдельно)
            ProcessDeliveries();

            int randomIndex = random.Next(partPrices.Count);
            string BrokenPart = partPrices.Keys.ElementAt(randomIndex);
            int BrokenPrice = partPrices[BrokenPart];
            int reward = (int)(BrokenPrice * 1.4);

            Console.Clear();
            Console.WriteLine($"\n=========================================");
            WriteColorLine($"          Сломано - {BrokenPart}", ConsoleColor.DarkRed);
            WriteColorLine($"     Выручка при выполнении: {reward} руб.", ConsoleColor.DarkYellow);

            // ПОКАЗЫВАЕМ СТАТУС ВСЕХ ЗАКАЗОВ
            if (pendingOrders.Count > 0)
            {
                WriteColorLine($"     Ожидается поставок: {pendingOrders.Count}", ConsoleColor.Cyan);
                foreach (var order in pendingOrders)
                {
                    WriteColorLine($"     Заказ №{order.Stage}: через {2 - order.DeliveryTimer} клиентов", ConsoleColor.Cyan);
                }
            }

            Console.WriteLine($"=========================================\n");

            bool choiceMade = false;
            while (!choiceMade)
            {
                Console.WriteLine("1. Выполнить заказ");
                Console.WriteLine("2. Отказаться от заказа");
                WriteColor("Выберите действие: ", ConsoleColor.Yellow);
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ProcessRepair(BrokenPart, reward);
                        choiceMade = true;
                        break;
                    case "2":
                        balance -= Otkaz;
                        WriteColorLine($"Вы отказались от заказа. Штраф: {Otkaz} руб.", ConsoleColor.Red);
                        WriteColorLine($"Текущий баланс: {balance} руб.", ConsoleColor.Yellow);
                        choiceMade = true;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор! Попробуйте снова.");
                        break;
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private void ProcessDeliveries()
        {
            if (pendingOrders.Count > 0)
            {
                List<Order> deliveredOrders = new List<Order>();

                // Обновляем таймеры и проверяем доставку для КАЖДОГО заказа
                foreach (var order in pendingOrders)
                {
                    order.DeliveryTimer++; // Увеличиваем таймер каждого заказа

                    WriteColorLine($"Заказ №{order.Stage}: до поставки {2 - order.DeliveryTimer} клиентов", ConsoleColor.Cyan);

                    // Если заказ готов к доставке
                    if (order.DeliveryTimer >= 2)
                    {
                        deliveredOrders.Add(order);
                    }
                }

                // Доставляем готовые заказы
                if (deliveredOrders.Count > 0)
                {
                    foreach (var order in deliveredOrders)
                    {
                        WriteColorLine($"\n=== ПОСТАВКА ЗАКАЗА №{order.Stage} ===", ConsoleColor.Green);
                        foreach (var item in order.ordering)
                        {
                            string part = item.Key;
                            int quantity = item.Value;

                            if (Storage.ContainsKey(part))
                                Storage[part] += quantity;
                            else
                                Storage[part] = quantity;

                            WriteColorLine($"Получено: {part} - {quantity} шт.", ConsoleColor.Green);
                        }
                        pendingOrders.Remove(order); // Удаляем доставленный заказ
                    }
                    WriteColorLine("Заказы доставлены на склад!", ConsoleColor.Green);
                }
            }
        }
        private void ProcessRepair(string brokenPart, int reward)
        {
            Console.WriteLine("\nВыберите деталь для замены:");

            // Показываем доступные детали на складе
            int index = 1;
            var availableParts = new List<string>();

            foreach (var part in Storage.Where(p => p.Value > 0))
            {
                Console.WriteLine($"{index}. {part.Key} (в наличии: {part.Value})");
                availableParts.Add(part.Key);
                index++;
            }

            Console.WriteLine("0. Отменить ремонт");
            WriteColor("Выберите деталь: ", ConsoleColor.Yellow);

            if (int.TryParse(Console.ReadLine(), out int partChoice) && partChoice > 0 && partChoice <= availableParts.Count)
            {
                string selectedPart = availableParts[partChoice - 1];

                if (selectedPart == brokenPart)
                {
                    // Правильная деталь - успешный ремонт
                    Storage[selectedPart]--; // Уменьшаем количество на складе
                    balance += reward; // Получаем оплату
                    successfulRepairs++;

                    WriteColorLine($"\nРемонт выполнен успешно!", ConsoleColor.Green);
                    WriteColorLine($"Получено: {reward} руб.", ConsoleColor.Green);
                    WriteColorLine($"Текущий баланс: {balance} руб.", ConsoleColor.Yellow);
                }
                else
                {
                    // Неправильная деталь - штраф
                    Storage[selectedPart]--; // Все равно тратим деталь
                    balance -= WrongPart; // Штраф за неправильную деталь

                    WriteColorLine($"\nОшибка! Нужно было заменить: {brokenPart}", ConsoleColor.Red);
                    WriteColorLine($"Вы выбрали: {selectedPart}", ConsoleColor.Red);
                    WriteColorLine($"Штраф: {WrongPart} руб.", ConsoleColor.Red);
                    WriteColorLine($"Текущий баланс: {balance} руб.", ConsoleColor.Yellow);
                }
            }
            else if (partChoice == 0)
            {
                // Отмена ремонта
                balance -= Otkaz;
                WriteColorLine($"Ремонт отменен. Штраф: {Otkaz} руб.", ConsoleColor.Red);
                WriteColorLine($"Текущий баланс: {balance} руб.", ConsoleColor.Yellow);
            }
            else
            {
                // Неверный ввод - считаем как отказ
                balance -= Otkaz;
                WriteColorLine($"Неверный выбор. Штраф: {Otkaz} руб.", ConsoleColor.Red);
                WriteColorLine($"Текущий баланс: {balance} руб.", ConsoleColor.Yellow);
            }
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
        class Order
        {
            public int Stage;
            public Dictionary<string, int> ordering = new Dictionary<string, int>();
            public int DeliveryTimer; // Таймер доставки для КАЖДОГО заказа

            public Order(int stage)
            {
                Stage = stage;
                DeliveryTimer = 0; // Начинаем с 0
            }

            // Остальные методы без изменений
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

        private void ShowPurchaseMenu()
        {
            int i = 1;
            Order order = new Order(currentStage);
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
            Console.WriteLine("0. Закончить покупку");

            while (!choise)
            {
                WriteColor("Выберите деталь для заказа: ", ConsoleColor.Yellow);

                if (int.TryParse(Console.ReadLine(), out int chos))
                {
                    if (chos == 0)
                    {
                        if (order.ordering.Count > 0)
                        {
                            // КАЖДЫЙ НОВЫЙ ЗАКАЗ получает свой таймер
                            order.DeliveryTimer = 0; // начинаем с 0
                            pendingOrders.Add(order);
                            WriteColorLine($"Заказ создан! Придет на склад через 2 клиента", ConsoleColor.DarkGreen);
                        }
                        else
                        {
                            WriteColorLine("Покупка отменена", ConsoleColor.DarkGreen);
                        }
                        choise = true;
                    }
                    else if (chos > 0 && chos <= parts.Count)
                    {
                        Console.Write("Введите кол-во товара для покупки: ");
                        if (int.TryParse(Console.ReadLine(), out int nums) && nums > 0)
                        {
                            string part = parts[chos - 1];
                            int totalCost = partPrices[part] * nums;

                            if (balance >= totalCost)
                            {
                                order.Add(part, nums);
                                balance -= totalCost;

                                WriteColorLine($"Добавлено в заказ - {part}: {nums} шт.", ConsoleColor.DarkGreen);
                                WriteColorLine($"Списано: {totalCost} руб.", ConsoleColor.Green);
                                WriteColorLine($"Баланс: {balance} руб.", ConsoleColor.Yellow);
                            }
                            else
                            {
                                WriteColorLine("Недостаточно средств!", ConsoleColor.Red);
                            }
                        }
                        else
                        {
                            WriteColorLine("Неверное количество!", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        WriteColorLine("Неверный выбор детали!", ConsoleColor.Red);
                    }
                }
                else
                {
                    WriteColorLine("Неверный ввод!", ConsoleColor.Red);
                }
            }
        }



    }
    class Order
    {
        public int Stage;
        public Dictionary<string, int> ordering = new Dictionary<string, int>();
        public int DeliveryTimer;

        public Order(int stage)
        {
            Stage = stage;
            DeliveryTimer = 0;
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
