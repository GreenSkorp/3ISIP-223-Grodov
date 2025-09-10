
//Задание
//Создать консольное приложение для подсчета потраченных за день средств.
//Пользователь вводит количество операций, которые будут записаны.
//Можно внести от 2 до 40 операций.
//Дальше, пользователь по шаблону(Название услуги или товара; Количество денег) вводит траты.
//Валюта - рубли.
//Пример: (Влажные салфетки "Лента"; 235)
//После заполнения всех трат, пользователь должен увидеть следующее меню:
//1.Вывод данных
//2.Статистика(среднее, максимальное, минимальное, сумма)
//3.Сортировка по цене(пузырьковая сортировка)
//4.Конвертация валюты(пользователь вводит курс или выбирает из списка)
//5.Поиск по названию 0.Выход
//Выбор пунктов меню осуществляется по соответствующей цифре.

using System;
using System.Globalization;

class ExpenseTracker
{
    class Expense
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    static void Main()
    {
        Console.WriteLine("=== ТРЕКЕР РАСХОДОВ ===");

        int count;
        do
        {
            Console.Write("Введите количество операций (2-40): ");
        } while (!int.TryParse(Console.ReadLine(), out count) || count < 2 || count > 40);

        Expense[] expenses = new Expense[count];

        Console.WriteLine("\nВведите данные о расходах (формат: Название; Сумма):");
        for (int i = 0; i < count; i++)
        {
            Console.Write($"{i + 1}. ");
            string input = Console.ReadLine();

            string[] parts = input.Split(';');
            if (parts.Length == 2 && decimal.TryParse(parts[1].Trim(), out decimal amount))
            {
                expenses[i] = new Expense
                {
                    Name = parts[0].Trim(),
                    Amount = amount
                };
            }
            else
            {
                Console.WriteLine("Неверный формат! Попробуйте снова.");
                i--;
            }
        }

        // Главное меню
        bool running = true;
        while (running)
        {
            Console.WriteLine("\n=== МЕНЮ ===");
            Console.WriteLine("1. Вывод данных");
            Console.WriteLine("2. Статистика");
            Console.WriteLine("3. Сортировка по цене");
            Console.WriteLine("4. Конвертация валюты");
            Console.WriteLine("5. Поиск по названию");
            Console.WriteLine("0. Выход");

            Console.Write("Выберите пункт: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowData(expenses);
                    break;
                case "2":
                    ShowStatistics(expenses);
                    break;
                case "3":
                    BubbleSort(expenses);
                    Console.WriteLine("Данные отсортированы по цене!");
                    break;
                case "4":
                    ConvertCurrency(expenses);
                    break;
                case "5":
                    SearchByName(expenses);
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }
    }

    static void ShowData(Expense[] expenses)
    {
        Console.WriteLine("\n=== ВАШИ РАСХОДЫ ===");
        for (int i = 0; i < expenses.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {expenses[i].Name} - {expenses[i].Amount} руб.");
        }
    }

    static void ShowStatistics(Expense[] expenses)
    {
        decimal total = 0;
        decimal max = decimal.MinValue;
        decimal min = decimal.MaxValue;

        foreach (var expense in expenses)
        {
            total += expense.Amount;
            if (expense.Amount > max) max = expense.Amount;
            if (expense.Amount < min) min = expense.Amount;
        }

        decimal average = total / expenses.Length;

        Console.WriteLine("\n=== СТАТИСТИКА ===");
        Console.WriteLine($"Общая сумма: {total} руб.");
        Console.WriteLine($"Средняя сумма: {average:F2} руб.");
        Console.WriteLine($"Максимальная трата: {max} руб.");
        Console.WriteLine($"Минимальная трата: {min} руб.");
    }

    static void BubbleSort(Expense[] expenses)
    {
        for (int i = 0; i < expenses.Length - 1; i++)
        {
            for (int j = 0; j < expenses.Length - i - 1; j++)
            {
                if (expenses[j].Amount > expenses[j + 1].Amount)
                {
                    var temp = expenses[j];
                    expenses[j] = expenses[j + 1];
                    expenses[j + 1] = temp;
                }
            }
        }
    }

    static void ConvertCurrency(Expense[] expenses)
    {
        bool running = true;
        string dollar = "82";
        string en = "11";
        while (running)
        {
            Console.WriteLine("\n=== Выбор конвертации ===");
            Console.WriteLine("1. В Доллары");
            Console.WriteLine("2. В Йены");
            Console.WriteLine("3. Ввести свой курс валюты");

            Console.WriteLine("0. Выход");

            Console.Write("Выберите пункт: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Введите курс конвертации (рубли в другую валюту): ");
                    if (decimal.TryParse(dollar, out decimal rate) && rate > 0)
                    {
                        Console.WriteLine("\n=== КОНВЕРТИРОВАННЫЕ СУММЫ ===");
                        foreach (var expense in expenses)
                        {
                            decimal converted = expense.Amount / rate;
                            Console.WriteLine($"{expense.Name} - {converted:F2} (по курсу {rate})");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный курс!");
                    }
                    break;
                case "2":
                    Console.Write("Введите курс конвертации (рубли в другую валюту): ");
                    if (decimal.TryParse(en, out rate) && rate > 0)
                    {
                        Console.WriteLine("\n=== КОНВЕРТИРОВАННЫЕ СУММЫ ===");
                        foreach (var expense in expenses)
                        {
                            decimal converted = expense.Amount / rate;
                            Console.WriteLine($"{expense.Name} - {converted:F2} (по курсу {rate})");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный курс!");
                    }
                    break;
                case "3":
                    Console.Write("Введите курс конвертации (рубли в другую валюту): ");
                    if (decimal.TryParse(Console.ReadLine(), out  rate) && rate > 0)
                    {
                        Console.WriteLine("\n=== КОНВЕРТИРОВАННЫЕ СУММЫ ===");
                        foreach (var expense in expenses)
                        {
                            decimal converted = expense.Amount / rate;
                            Console.WriteLine($"{expense.Name} - {converted:F2} (по курсу {rate})");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный курс!");
                    }
                    break;
                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }
    }

    static void SearchByName(Expense[] expenses)
    {
        Console.Write("Введите название для поиска: ");
        string searchTerm = Console.ReadLine().ToLower();

        Console.WriteLine("\n=== РЕЗУЛЬТАТЫ ПОИСКА ===");
        bool found = false;

        foreach (var expense in expenses)
        {
            if (expense.Name.ToLower().Contains(searchTerm))
            {
                Console.WriteLine($"{expense.Name} - {expense.Amount} руб.");
                found = true;
            }
        }

        if (!found)
        {
            Console.WriteLine("Ничего не найдено!");
        }
    }
}