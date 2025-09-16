using System;
using System.Collections.Generic;
using System.Linq;

//  Создайте консольное приложение C# для учёта товаров в магазине. 
//  У товара должны быть следующее параметры:
//  Уникальный код(начинается с "1", должен автоматически ставиться при пополнении списка товаров)
//  Название
//  Цена
//  Количество
//  Остался ли ещё товар на складе
//  Категория (выбирается из имеющихся, задаются в коде, сделайте как минимум 3)
//  Мы можем работать с товаром через команды:
//  Добавить товар
//  Удалить товар
//  Заказать поставку товара
//  Продать товар
//  Поиск товаров (по коду, названию и категории). Необходимо выводить полную информацию о товаре.

//  Необходимо доработать программу, добавив следующие команды:
//  История продаж с возможностью отмены последней продажи. Для реализации этой функции, используйте Stack.
//  Отчёт о продажах. Отчёт должен выводить все проданные товары, количество штук и общую сумму продажи.



using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopInventory
{
    public enum Category
    {
        Electronics = 0,
        Clothing = 1,
        Food = 2,
        Books = 3,
        Sports = 4
    }

    public class Product
    {
        public string Code { get; private set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool InStock => Quantity > 0;
        public Category Category { get; set; }

        public Product(string name, decimal price, int quantity, Category category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название товара не может быть пустым");
            if (price < 0)
                throw new ArgumentException("Цена не может быть отрицательной");
            if (quantity < 0)
                throw new ArgumentException("Количество не может быть отрицательным");

            Code = GenerateCode();
            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
        }

        private static int _lastId = 1000;
        private static string GenerateCode()
        {
            _lastId++;
            return "1" + _lastId.ToString("D5");
        }

        public override string ToString()
        {
            return $"Код: {Code}, Название: {Name}, Цена: {Price}, " +
                   $"Количество: {Quantity}, В наличии: {(InStock ? "Да" : "Нет")}, " +
                   $"Категория: {Category}";
        }
    }

    public class Shop
    {
        private List<Product> _products = new List<Product>();
        private Stack<(Product product, int quantity)> _salesHistory = new Stack<(Product, int)>();
        private List<(Product product, int quantity, decimal total)> _salesReport = new List<(Product, int, decimal)>();

        public Shop()
        {
            InitializeTestData();
        }

        private void InitializeTestData()
        {
            _products.Add(new Product("Ноутбук", 50000, 10, Category.Electronics));
            _products.Add(new Product("Футболка", 1500, 50, Category.Clothing));
            _products.Add(new Product("Хлеб", 50, 100, Category.Food));
            _products.Add(new Product("Роман", 500, 30, Category.Books));
            _products.Add(new Product("Мяч", 2000, 20, Category.Sports));
        }

        public void AddProduct(string name, decimal price, int quantity, Category category)
        {
            try
            {
                var product = new Product(name, price, quantity, category);
                _products.Add(product);
                Console.WriteLine($"\nТовар добавлен: {product}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\nОшибка: {ex.Message}");
            }
        }

        public bool RemoveProduct(string code)
        {
            var product = _products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                _products.Remove(product);
                Console.WriteLine($"\nТовар {product.Name} удален");
                return true;
            }
            Console.WriteLine("\nТовар не найден!");
            return false;
        }

        public void OrderSupply(string code, int quantity)
        {
            if (quantity <= 0)
            {
                Console.WriteLine("Количество должно быть положительным!");
                return;
            }

            var product = _products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                product.Quantity += quantity;
                Console.WriteLine($"\nПоставка выполнена. Новое количество: {product.Quantity}");
            }
            else
            {
                Console.WriteLine("\nТовар не найден!");
            }
        }

        public bool SellProduct(string code, int quantity)
        {
            if (quantity <= 0)
            {
                Console.WriteLine("Количество должно быть положительным!");
                return false;
            }

            var product = _products.FirstOrDefault(p => p.Code == code);
            if (product != null)
            {
                if (product.Quantity >= quantity)
                {
                    product.Quantity -= quantity;
                    decimal total = product.Price * quantity;

                    _salesHistory.Push((product, quantity));
                    _salesReport.Add((product, quantity, total));

                    Console.WriteLine($"\nПродажа выполнена. Остаток: {product.Quantity}");
                    Console.WriteLine($"Общая сумма: {total}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"\nНедостаточно товара! Доступно: {product.Quantity}");
                    return false;
                }
            }
            Console.WriteLine("\nТовар не найден!");
            return false;
        }

        private void SalesHistoryMenu()
        {
            Console.Clear();
            Console.WriteLine("=== ИСТОРИЯ ПРОДАЖ ===");
            Console.WriteLine("1. Показать историю продаж");
            Console.WriteLine("2. Отменить последнюю продажу");
            Console.WriteLine("0. Назад");
            Console.Write("Выберите действие: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        ShowSalesHistory();
                        break;
                    case 2:
                        UndoLastSale();
                        break;
                    case 0:
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
            }
        }
        public void ShowSalesReport()
        {
            Console.Clear();
            Console.WriteLine("=== ОТЧЕТ О ПРОДАЖАХ ===");

            if (_salesReport.Any())
            {
                decimal totalRevenue = 0;
                int totalItems = 0;

                foreach (var sale in _salesReport)
                {
                    Console.WriteLine($"Товар: {sale.product.Name}, " +
                                    $"Количество: {sale.quantity}, " +
                                    $"Сумма: {sale.total}");

                    totalRevenue += sale.total;
                    totalItems += sale.quantity;
                }

                Console.WriteLine($"\nИтого: {totalItems} товаров на сумму {totalRevenue}");
            }
            else
            {
                Console.WriteLine("Продаж еще не было!");
            }
        }


        public void SearchProducts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("Поисковый запрос не может быть пустым!");
                return;
            }

            var results = _products.Where(p =>
                p.Code.Contains(searchTerm) ||
                p.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                p.Category.ToString().IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToList();

            if (results.Any())
            {
                Console.WriteLine("\nРезультаты поиска:");
                foreach (var product in results)
                {
                    Console.WriteLine(product);
                }
                Console.WriteLine($"\nНайдено товаров: {results.Count}");
            }
            else
            {
                Console.WriteLine("Товары не найдены!");
            }
        }

        public void ShowAllProducts()
        {
            Console.Clear();
            Console.WriteLine("=== ВСЕ ТОВАРЫ ===");
            if (_products.Any())
            {
                foreach (var product in _products)
                {
                    Console.WriteLine(product);
                }
                Console.WriteLine($"\nВсего товаров: {_products.Count}");
            }
            else
            {
                Console.WriteLine("Товаров нет!");
            }
        }

        public void UndoLastSale()
        {
            Console.Clear();
            Console.WriteLine("=== ОТМЕНА ПОСЛЕДНЕЙ ПРОДАЖИ ===");

            if (_salesHistory.Count > 0)
            {
                var lastSale = _salesHistory.Pop();
                lastSale.product.Quantity += lastSale.quantity;

                var saleToRemove = _salesReport.LastOrDefault(s =>
                    s.product.Code == lastSale.product.Code &&
                    s.quantity == lastSale.quantity);

                if (saleToRemove.product != null)
                {
                    _salesReport.Remove(saleToRemove);
                }

                Console.WriteLine($"Продажа отменена. Товар {lastSale.product.Name} " +
                                $"в количестве {lastSale.quantity} возвращен на склад.");
            }
            else
            {
                Console.WriteLine("История продаж пуста!");
            }
        }
        public void ShowSalesHistory()
        {
            Console.Clear();
            Console.WriteLine("=== ИСТОРИЯ ПРОДАЖ ===");

            if (_salesHistory.Count == 0)
            {
                Console.WriteLine("История продаж пуста!");
                return;
            }

            Console.WriteLine("Последние продажи (от последней к первой):");
            Console.WriteLine("----------------------------------------");

            int counter = 1;
            foreach (var sale in _salesHistory.Reverse())
            {
                decimal total = sale.product.Price * sale.quantity;
                Console.WriteLine($"{counter}. Товар: {sale.product.Name}, " +
                                $"Код: {sale.product.Code}, " +
                                $"Количество: {sale.quantity}, " +
                                $"Сумма: {total:C}, " +
                                $"Дата: {DateTime.Now:dd.MM.yyyy HH:mm}");
                counter++;
            }
        }



        class Program
        {
            static void Main(string[] args)
            {
                Shop shop = new Shop();
                int choice = -1;
                while (choice != 0)
                {
                    Console.Clear();
                    Console.WriteLine("=== СИСТЕМА УЧЕТА ТОВАРОВ ===");
                    Console.WriteLine("1. Добавить товар");
                    Console.WriteLine("2. Удалить товар");
                    Console.WriteLine("3. Заказать поставку");
                    Console.WriteLine("4. Продать товар");
                    Console.WriteLine("5. Поиск товаров");
                    Console.WriteLine("6. Показать все товары");
                    Console.WriteLine("7. История продаж");
                    Console.WriteLine("8. Отчет о продажах");
                    Console.WriteLine("0. Выход");
                    Console.Write("Выберите действие: ");
                    choice = Convert.ToInt32(Console.ReadLine());
                    switch (choice)
                    {
                        case 1: AddProductMenu(shop); break;
                        case 2: RemoveProductMenu(shop); break;
                        case 3: OrderSupplyMenu(shop); break;
                        case 4: SellProductMenu(shop); break;
                        case 5: SearchProductsMenu(shop); break;
                        case 6:
                            shop.ShowAllProducts();
                            WaitForKey();
                            break;
                        case 7:
                            shop.SalesHistoryMenu();
                            WaitForKey();
                            break;
                        case 8:
                            shop.ShowSalesReport();
                            WaitForKey();
                            break;
                        case 0: return;
                        default:
                            Console.WriteLine("Неверный выбор!");
                            WaitForKey();
                            break;
                    }

                }
            }



            static void WaitForKey()
            {
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }

            static void AddProductMenu(Shop shop)
            {
                Console.Clear();
                Console.WriteLine("=== ДОБАВЛЕНИЕ ТОВАРА ===");

                Console.Write("Название: ");
                string name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Название не может быть пустым!");
                    WaitForKey();
                    return;
                }

                Console.Write("Цена: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
                {
                    Console.WriteLine("Неверная цена!");
                    WaitForKey();
                    return;
                }

                Console.Write("Количество: ");
                if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 0)
                {
                    Console.WriteLine("Неверное количество!");
                    WaitForKey();
                    return;
                }

                Console.WriteLine("\nКатегории: ");
                foreach (var category in Enum.GetValues(typeof(Category)))
                {
                    Console.WriteLine($"{(int)category}. {category}");
                }

                Console.Write("Выберите категорию: ");
                if (int.TryParse(Console.ReadLine(), out int categoryId) &&
                    Enum.IsDefined(typeof(Category), categoryId))
                {
                    shop.AddProduct(name, price, quantity, (Category)categoryId);
                }
                else
                {
                    Console.WriteLine("Неверная категория!");
                }

                WaitForKey();
            }

            static void RemoveProductMenu(Shop shop)
            {
                Console.Clear();
                Console.WriteLine("=== УДАЛЕНИЕ ТОВАРА ===");
                Console.Write("Введите код товара: ");
                string code = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(code))
                {
                    Console.WriteLine("Код не может быть пустым!");
                    WaitForKey();
                    return;
                }

                shop.RemoveProduct(code);
                WaitForKey();
            }

            static void OrderSupplyMenu(Shop shop)
            {
                Console.Clear();
                Console.WriteLine("=== ЗАКАЗ ПОСТАВКИ ===");
                Console.Write("Введите код товара: ");
                string code = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(code))
                {
                    Console.WriteLine("Код не может быть пустым!");
                    WaitForKey();
                    return;
                }

                Console.Write("Количество для поставки: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    shop.OrderSupply(code, quantity);
                }
                else
                {
                    Console.WriteLine("Неверное количество!");
                }

                WaitForKey();
            }

            static void SellProductMenu(Shop shop)
            {
                Console.Clear();
                Console.WriteLine("=== ПРОДАЖА ТОВАРА ===");
                Console.Write("Введите код товара: ");
                string code = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(code))
                {
                    Console.WriteLine("Код не может быть пустым!");
                    WaitForKey();
                    return;
                }

                Console.Write("Количество для продажи: ");
                if (int.TryParse(Console.ReadLine(), out int quantity) && quantity > 0)
                {
                    shop.SellProduct(code, quantity);
                }
                else
                {
                    Console.WriteLine("Неверное количество!");
                }

                WaitForKey();
            }

            static void SearchProductsMenu(Shop shop)
            {
                Console.Clear();
                Console.WriteLine("=== ПОИСК ТОВАРОВ ===");
                Console.Write("Введите код, название или категорию: ");
                string searchTerm = Console.ReadLine();

                shop.SearchProducts(searchTerm);
                WaitForKey();
            }
        }
    }
}