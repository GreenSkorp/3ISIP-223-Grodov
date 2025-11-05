using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//1-Просмотр товара, Вход в акк, Рег акка, выход программы
//Вход - Ввести Логин и пароль
//Рег - Ввеси Логин, Пароль и повтор пароля

//Просмотр товара - Всех заказаных впринцепе(Order)
//После входа - Показать Ордер этого пользователя, Корзину(Внутри сделать оформления заказа(переход из Cart в Order))

namespace Pr1_10._09
{
    public class MarketPlace
    {
        private static MarketplaceEntities db = Core.Market;
        private Dictionary<string, int> productsprice = new Dictionary<string, int>();
        private Users currentUser = null;

        private void InitializeProducts()
        {
            productsprice["Рюкзак"] = 300;
            productsprice["Пенал"] = 50;
            productsprice["Тетрадь"] = 20;
            productsprice["Ручка"] = 60;
            productsprice["Карандаш"] = 40;
            productsprice["Корректор"] = 200;
            productsprice["Ластик"] = 70;
            productsprice["Точилка"] = 90;

            // Добавляем товары в базу данных если их нет
            if (!db.Products.Any())
            {
                foreach (var product in productsprice)
                {
                    db.Products.Add(new Products
                    {
                        Name = product.Key,
                        Price = product.Value
                    });
                }
                db.SaveChanges();
            }
        }

        private void InitializePickupPoints()
        {
            if (!db.PointOrders.Any())
            {
                var points = new List<string>
                {
                    "ул. Ленина, д. 10",
                    "пр. Мира, д. 25",
                    "ул. Центральная, д. 5",
                    "ул. Школьная, д. 15"
                };

                foreach (var address in points)
                {
                    db.PointOrders.Add(new PointOrders { Address = address });
                }
                db.SaveChanges();
            }
        }

        public void Run()
        {
            InitializeProducts();
            InitializePickupPoints();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== МАРКЕТПЛЕЙС ===");
                Console.WriteLine("1. Просмотр товаров");
                Console.WriteLine("2. Регистрация");
                Console.WriteLine("3. Вход в аккаунт");
                Console.WriteLine("4. Выход");

                if (currentUser != null)
                {
                    Console.WriteLine($"\nВы вошли как: {currentUser.UserName}");
                    Console.WriteLine("5. Личный кабинет");
                }

                Console.Write("\nВыберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowProducts();
                        break;
                    case "2":
                        Register();
                        break;
                    case "3":
                        Login();
                        break;
                    case "4":
                        return;
                    case "5":
                        if (currentUser != null)
                            UserMenu();
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowProducts()
        {
            Console.Clear();
            Console.WriteLine("=== ВСЕ ТОВАРЫ ===\n");

            var products = db.Products.ToList();
            foreach (var product in products)
            {
                Console.WriteLine($"{product.ProductID}. {product.Name} - {product.Price} руб.");
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void Register()
        {
            Console.Clear();
            Console.WriteLine("=== РЕГИСТРАЦИЯ ===\n");

            Console.Write("Введите имя пользователя: ");
            var username = Console.ReadLine();

            if (db.Users.Any(u => u.UserName == username))
            {
                Console.WriteLine("Пользователь с таким именем уже существует!");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите пароль: ");
            var password = Console.ReadLine();

            Console.Write("Подтвердите пароль: ");
            var confirmPassword = Console.ReadLine();

            if (password != confirmPassword)
            {
                Console.WriteLine("Пароли не совпадают!");
                Console.ReadKey();
                return;
            }

            var newUser = new Users
            {
                UserName = username,
                Password = password,
                RegisterDate = DateTime.Now
            };

            db.Users.Add(newUser);
            db.SaveChanges();

            Console.WriteLine("Регистрация успешна!");
            Console.ReadKey();
        }

        private void Login()
        {
            Console.Clear();
            Console.WriteLine("=== ВХОД В АККАУНТ ===\n");

            Console.Write("Введите имя пользователя: ");
            var username = Console.ReadLine();

            Console.Write("Введите пароль: ");
            var password = Console.ReadLine();

            currentUser = db.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);

            if (currentUser == null)
            {
                Console.WriteLine("Неверное имя пользователя или пароль!");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Вход выполнен успешно!");
                Console.ReadKey();
            }
        }

        private void UserMenu()
        {
            while (currentUser != null)
            {
                Console.Clear();
                Console.WriteLine($"=== ЛИЧНЫЙ КАБИНЕТ ({currentUser.UserName}) ===\n");
                Console.WriteLine("1. Просмотр моих заказов");
                Console.WriteLine("2. Корзина");
                Console.WriteLine("3. Добавить товар в корзину");

                Console.WriteLine("5. Выйти из аккаунта");

                Console.Write("\nВыберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowUserOrders();
                        break;
                    case "2":
                        ShowCart();
                        break;
                    case "3":
                        AddToCart();
                        break;
                    case "4":
                        AddToCart();
                        break;
                    case "5":
                        currentUser = null;
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowUserOrders()
        {
            Console.Clear();
            Console.WriteLine("=== МОИ ЗАКАЗЫ ===\n");

            var orders = db.Order
                .Where(o => o.UserID == currentUser.UserID)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.PointOrders)
                .Include(o => o.OrderCart)
                .ToList();

            if (!orders.Any())
            {
                Console.WriteLine("У вас нет заказов.");
            }
            else
            {
                foreach (var order in orders)
                {
                    Console.WriteLine($"Заказ #{order.OrderID} от {order.OrderDate:dd.MM.yyyy HH:mm}");
                    Console.WriteLine($"ПВЗ: {order.PointOrders.Address}");
                    Console.WriteLine($"Общая сумма: {order.TatalAmount} руб.");
                    Console.WriteLine("Товары:");

                    var orderItems = db.OrderCart
                        .Where(oc => oc.OrderID == order.OrderID)
                        .Include(oc => oc.Products)
                        .ToList();

                    foreach (var item in orderItems)
                    {
                        Console.WriteLine($"  - {item.Products.Name} x{item.Count} = {item.TotalPrice} руб.");
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Нажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void ShowCart()
        {
            Console.Clear();
            Console.WriteLine("=== КОРЗИНА ===\n");

            var cartItems = db.Cart
                .Where(c => c.UserID == currentUser.UserID)
                .Include(c => c.Products)
                .ToList();

            if (!cartItems.Any())
            {
                Console.WriteLine("Корзина пуста.");
            }
            else
            {
                decimal totalAmount = 0;
                foreach (var item in cartItems)
                {
                    Console.WriteLine($"{item.Products.Name} x{item.Count} = {item.TotalPrice} руб.");
                    totalAmount += item.TotalPrice;
                }
                Console.WriteLine($"\nОбщая сумма: {totalAmount} руб.");

                Console.WriteLine("\n1. Оформить все товары");
                Console.WriteLine("2. Удалить товар из корзины");
                Console.WriteLine("3. Купить один товар из корзины");
                Console.WriteLine("4. Назад");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CheckoutAll(cartItems);
                        break;
                    case "2":
                        RemoveFromCart();
                        break;
                    case "3":
                        BuyOneFromCart(cartItems);
                        break;
                    case "4":
                        // Просто возвращаемся назад
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void AddToCart()
        {
            Console.Clear();
            Console.WriteLine("=== ДОБАВЛЕНИЕ ТОВАРА В КОРЗИНУ ===\n");

            var products = db.Products.ToList();
            foreach (var product in products)
            {
                Console.WriteLine($"{product.ProductID}. {product.Name} - {product.Price} руб.");
            }

            Console.Write("\nВведите ID товара: ");
            if (int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.Write("Введите количество: ");
                if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
                {
                    var product = db.Products.Find(productId);
                    if (product != null)
                    {
                        var existingCartItem = db.Cart
                            .FirstOrDefault(c => c.UserID == currentUser.UserID && c.ProductID == productId);

                        if (existingCartItem != null)
                        {
                            existingCartItem.Count += count;
                            existingCartItem.TotalPrice = existingCartItem.Count * product.Price;
                        }
                        else
                        {
                            var cartItem = new Cart
                            {
                                UserID = currentUser.UserID,
                                ProductID = productId,
                                Count = count,
                                TotalPrice = count * product.Price
                            };
                            db.Cart.Add(cartItem);
                        }

                        db.SaveChanges();
                        Console.WriteLine("Товар добавлен в корзину!");
                    }
                    else
                    {
                        Console.WriteLine("Товар не найден!");
                    }
                }
                else
                {
                    Console.WriteLine("Неверное количество!");
                }
            }
            else
            {
                Console.WriteLine("Неверный ID товара!");
            }

            Console.WriteLine("Нажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void RemoveFromCart()
        {
            Console.Write("Введите ID товара для удаления: ");
            if (int.TryParse(Console.ReadLine(), out int productId))
            {
                var cartItem = db.Cart
                    .FirstOrDefault(c => c.UserID == currentUser.UserID && c.ProductID == productId);

                if (cartItem != null)
                {
                    db.Cart.Remove(cartItem);
                    db.SaveChanges();
                    Console.WriteLine("Товар удален из корзины!");
                }
                else
                {
                    Console.WriteLine("Товар не найден в корзине!");
                }
            }
            else
            {
                Console.WriteLine("Неверный ID товара!");
            }
            Console.ReadKey();
        }

        private void BuyOneFromCart(List<Cart> cartItems)
        {
            Console.Clear();
            Console.WriteLine("=== ПОКУПКА ОДНОГО ТОВАРА ИЗ КОРЗИНЫ ===\n");

            if (!cartItems.Any())
            {
                Console.WriteLine("Корзина пуста!");
                Console.WriteLine("Нажмите любую клавишу для возврата...");
                Console.ReadKey();
                return;
            }

            // Показываем товары в корзине
            Console.WriteLine("Товары в вашей корзине:");
            foreach (var item in cartItems)
            {
                Console.WriteLine($"{item.ProductID}. {item.Products.Name} x{item.Count} - {item.TotalPrice} руб.");
            }

            Console.Write("\nВведите ID товара для покупки: ");
            if (int.TryParse(Console.ReadLine(), out int productId))
            {
                var cartItem = db.Cart
                    .FirstOrDefault(c => c.UserID == currentUser.UserID && c.ProductID == productId);

                if (cartItem != null)
                {
                    // Показываем доступные пункты выдачи
                    var pickupPoints = db.PointOrders.ToList();
                    Console.WriteLine("\nДоступные пункты выдачи:");
                    foreach (var point in pickupPoints)
                    {
                        Console.WriteLine($"{point.PointID}. {point.Address}");
                    }

                    Console.Write("\nВыберите пункт выдачи: ");
                    if (int.TryParse(Console.ReadLine(), out int pointId))
                    {
                        var selectedPoint = db.PointOrders.Find(pointId);
                        if (selectedPoint != null)
                        {
                            // Создаем заказ для одного товара
                            var order = new Order
                            {
                                UserID = currentUser.UserID,
                                PointID = pointId,
                                OrderDate = DateTime.Now,
                                TatalAmount = cartItem.TotalPrice
                            };

                            db.Order.Add(order);
                            db.SaveChanges();

                            // Добавляем товар в заказ
                            var orderCart = new OrderCart
                            {
                                OrderID = order.OrderID,
                                ProdictID = cartItem.ProductID,
                                Count = cartItem.Count,
                                TotalPrice = cartItem.TotalPrice
                            };
                            db.OrderCart.Add(orderCart);

                            // Удаляем товар из корзины
                            db.Cart.Remove(cartItem);
                            db.SaveChanges();

                            Console.WriteLine($"\nТовар успешно куплен! Номер заказа: {order.OrderID}");
                            Console.WriteLine($"Пункт выдачи: {selectedPoint.Address}");
                        }
                        else
                        {
                            Console.WriteLine("Пункт выдачи не найден!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор пункта выдачи!");
                    }
                }
                else
                {
                    Console.WriteLine("Товар не найден в корзине!");
                }
            }
            else
            {
                Console.WriteLine("Неверный ID товара!");
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void CheckoutAll(List<Cart> cartItems)
        {
            Console.Clear();
            Console.WriteLine("=== ОФОРМЛЕНИЕ ЗАКАЗА ===\n");

            var pickupPoints = db.PointOrders.ToList();
            Console.WriteLine("Доступные пункты выдачи:");
            foreach (var point in pickupPoints)
            {
                Console.WriteLine($"{point.PointID}. {point.Address}");
            }

            Console.Write("\nВыберите пункт выдачи: ");
            if (int.TryParse(Console.ReadLine(), out int pointId))
            {
                var selectedPoint = db.PointOrders.Find(pointId);
                if (selectedPoint != null)
                {
                    int totalAmount = cartItems.Sum(c => c.TotalPrice);

                    var order = new Order
                    {
                        UserID = currentUser.UserID,
                        PointID = pointId,
                        OrderDate = DateTime.Now,
                        TatalAmount = totalAmount
                    };

                    db.Order.Add(order);
                    db.SaveChanges();

                    // Переносим товары из корзины в заказ
                    foreach (var cartItem in cartItems)
                    {
                        var orderCart = new OrderCart
                        {
                            OrderID = order.OrderID,
                            ProdictID = cartItem.ProductID,
                            Count = cartItem.Count,
                            TotalPrice = cartItem.TotalPrice
                        };
                        db.OrderCart.Add(orderCart);

                        // Удаляем из корзины
                        db.Cart.Remove(cartItem);
                    }

                    db.SaveChanges();

                    Console.WriteLine($"Заказ успешно оформлен! Номер заказа: {order.OrderID}");
                }
                else
                {
                    Console.WriteLine("Пункт выдачи не найден!");
                }
            }
            else
            {
                Console.WriteLine("Неверный выбор пункта выдачи!");
            }

            Console.WriteLine("Нажмите любую клавишу для возврата...");
            Console.ReadKey();
        }
    }

    public class Core
    {
        public static MarketplaceEntities Market = new MarketplaceEntities();
    }

    class Program
    {
        static void Main(string[] args)
        {
            var marketplace = new MarketPlace();
            marketplace.Run();
        }
    }
}