using System;
using System.Collections.Generic;
using System.Linq;
using Pr1_10._09;

namespace GMWOG_Marketplace
{
    class Program
    {
        private static MarketplaceEntities db = new MarketplaceEntities();
        private static Users currentUser = null;

        static void Main(string[] args)
        {
            Console.WriteLine("=== Добро пожаловать в GMWOG Маркетплейс! ===");

            while (true)
            {
                if (currentUser == null)
                {
                    ShowMainMenu();
                }
                else
                {
                    ShowUserMenu();
                }
            }
        }

        static void ShowMainMenu()
        {
            Console.WriteLine("\n=== Главное меню ===");
            Console.WriteLine("1. Просмотр товаров");
            Console.WriteLine("2. Регистрация");
            Console.WriteLine("3. Вход в аккаунт");
            Console.WriteLine("4. Выход");
            Console.Write("Выберите действие: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ViewProducts();
                    break;
                case "2":
                    Register();
                    break;
                case "3":
                    Login();
                    break;
                case "4":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }

        static void ShowUserMenu()
        {
            Console.WriteLine($"\n=== Добро пожаловать, {currentUser.Username}! ===");
            Console.WriteLine("1. Просмотр товаров");
            Console.WriteLine("2. Корзина");
            Console.WriteLine("3. Мои заказы");
            Console.WriteLine("4. Выйти из аккаунта");
            Console.Write("Выберите действие: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ViewProducts();
                    break;
                case "2":
                    ShowCart();
                    break;
                case "3":
                    ViewOrders();
                    break;
                case "4":
                    currentUser = null;
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
        }

        static void ViewProducts()
        {
            Console.WriteLine("\n=== Все товары ===");
            var products = db.Products.ToList();

            if (!products.Any())
            {
                Console.WriteLine("Товары отсутствуют.");
                return;
            }

            foreach (var product in products)
            {
                Console.WriteLine($"{product.ProductId}. {product.Name} - {product.Price:C} (В наличии: {product.StockQuantity})");
                Console.WriteLine($"   {product.Description}");
                Console.WriteLine();
            }

            if (currentUser != null)
            {
                Console.Write("Добавить товар в корзину? (ID товара или 0 для отмены): ");
                if (int.TryParse(Console.ReadLine(), out int productId) && productId > 0)
                {
                    AddToCart(productId);
                }
            }
        }

        static void Register()
        {
            Console.WriteLine("\n=== Регистрация ===");
            Console.Write("Имя пользователя: ");
            var username = Console.ReadLine();

            if (db.Users.Any(u => u.Username == username))
            {
                Console.WriteLine("Пользователь с таким именем уже существует!");
                return;
            }

            Console.Write("Пароль: ");
            var password = Console.ReadLine();
            Console.Write("Подтвердите пароль: ");
            var confirmPassword = Console.ReadLine();

            if (password != confirmPassword)
            {
                Console.WriteLine("Пароли не совпадают!");
                return;
            }

            Console.Write("Email: ");
            var email = Console.ReadLine();

            var newUser = new User
            {
                Username = username,
                Password = password, // В реальном приложении нужно хэшировать
                Email = email,
                CreatedDate = DateTime.Now
            };

            db.Users.Add(newUser);
            db.SaveChanges();

            Console.WriteLine("Регистрация успешна! Теперь вы можете войти в аккаунт.");
        }

        static void Login()
        {
            Console.WriteLine("\n=== Вход в аккаунт ===");
            Console.Write("Имя пользователя: ");
            var username = Console.ReadLine();
            Console.Write("Пароль: ");
            var password = Console.ReadLine();

            var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                currentUser = user;
                Console.WriteLine($"Успешный вход! Добро пожаловать, {user.Username}!");
            }
            else
            {
                Console.WriteLine("Неверное имя пользователя или пароль!");
            }
        }

        static void AddToCart(int productId)
        {
            var product = db.Products.Find(productId);
            if (product == null)
            {
                Console.WriteLine("Товар не найден!");
                return;
            }

            if (product.StockQuantity <= 0)
            {
                Console.WriteLine("Товар отсутствует на складе!");
                return;
            }

            var existingCartItem = db.CartItems.FirstOrDefault(ci =>
                ci.UserId == currentUser.UserId && ci.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity++;
            }
            else
            {
                var newCartItem = new CartItem
                {
                    UserId = currentUser.UserId,
                    ProductId = productId,
                    Quantity = 1,
                    AddedDate = DateTime.Now
                };
                db.CartItems.Add(newCartItem);
            }

            db.SaveChanges();
            Console.WriteLine("Товар добавлен в корзину!");
        }

        static void ShowCart()
        {
            Console.WriteLine("\n=== Корзина ===");
            var cartItems = db.CartItems
                .Where(ci => ci.UserId == currentUser.UserId)
                .Join(db.Products,
                      ci => ci.ProductId,
                      p => p.ProductId,
                      (ci, p) => new { CartItem = ci, Product = p })
                .ToList();

            if (!cartItems.Any())
            {
                Console.WriteLine("Корзина пуста.");
                return;
            }

            decimal total = 0;
            foreach (var item in cartItems)
            {
                var itemTotal = item.Product.Price * item.CartItem.Quantity;
                total += itemTotal;
                Console.WriteLine($"{item.Product.Name} - {item.Product.Price:C} x {item.CartItem.Quantity} = {itemTotal:C}");
            }
            Console.WriteLine($"Общая сумма: {total:C}");

            Console.WriteLine("\n1. Купить все товары из корзины");
            Console.WriteLine("2. Купить отдельный товар");
            Console.WriteLine("3. Удалить товар из корзины");
            Console.WriteLine("0. Назад");
            Console.Write("Выберите действие: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    PurchaseAllFromCart(cartItems);
                    break;
                case "2":
                    PurchaseSingleItem();
                    break;
                case "3":
                    RemoveFromCart();
                    break;
            }
        }

        static void PurchaseAllFromCart(dynamic cartItems)
        {
            var pickupPoint = SelectPickupPoint();
            if (pickupPoint == null) return;

            // Проверка наличия товаров
            foreach (var item in cartItems)
            {
                var product = db.Products.Find(item.Product.ProductId);
                if (product.StockQuantity < item.CartItem.Quantity)
                {
                    Console.WriteLine($"Недостаточно товара '{product.Name}' на складе!");
                    return;
                }
            }

            // Создание заказа
            var order = new Order
            {
                UserId = currentUser.UserId,
                PointId = pickupPoint.PointId,
                OrderDate = DateTime.Now,
                TotalAmount = 0
            };
            db.Orders.Add(order);
            db.SaveChanges();

            decimal totalAmount = 0;

            // Добавление товаров в заказ
            foreach (var item in cartItems)
            {
                var product = db.Products.Find(item.Product.ProductId);
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = item.CartItem.Quantity,
                    Price = product.Price
                };
                db.OrderItems.Add(orderItem);

                // Обновление количества на складе
                product.StockQuantity -= item.CartItem.Quantity;

                totalAmount += product.Price * item.CartItem.Quantity;

                // Удаление из корзины
                db.CartItems.Remove(item.CartItem);
            }

            order.TotalAmount = totalAmount;
            db.SaveChanges();

            Console.WriteLine($"Заказ #{order.OrderId} успешно создан! Общая сумма: {totalAmount:C}");
        }

        static void PurchaseSingleItem()
        {
            Console.Write("Введите ID товара для покупки: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Неверный ID!");
                return;
            }

            var cartItem = db.CartItems.FirstOrDefault(ci =>
                ci.UserId == currentUser.UserId && ci.ProductId == productId);

            if (cartItem == null)
            {
                Console.WriteLine("Товар не найден в корзине!");
                return;
            }

            var product = db.Products.Find(productId);
            if (product.StockQuantity < cartItem.Quantity)
            {
                Console.WriteLine("Недостаточно товара на складе!");
                return;
            }

            var pickupPoint = SelectPickupPoint();
            if (pickupPoint == null) return;

            // Создание заказа для одного товара
            var order = new Order
            {
                UserId = currentUser.UserId,
                PointId = pickupPoint.PointId,
                OrderDate = DateTime.Now,
                TotalAmount = product.Price * cartItem.Quantity
            };
            db.Orders.Add(order);
            db.SaveChanges();

            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = productId,
                Quantity = cartItem.Quantity,
                Price = product.Price
            };
            db.OrderItems.Add(orderItem);

            // Обновление склада и корзины
            product.StockQuantity -= cartItem.Quantity;
            db.CartItems.Remove(cartItem);
            db.SaveChanges();

            Console.WriteLine($"Товар '{product.Name}' успешно куплен! Заказ #{order.OrderId}");
        }

        static PickupPoint SelectPickupPoint()
        {
            Console.WriteLine("\n=== Выберите пункт выдачи ===");
            var pickupPoints = db.PickupPoints.ToList();

            foreach (var point in pickupPoints)
            {
                Console.WriteLine($"{point.PointId}. {point.Address} ({point.WorkingHours})");
            }

            Console.Write("Выберите ПВЗ (ID): ");
            if (int.TryParse(Console.ReadLine(), out int pointId))
            {
                var selectedPoint = db.PickupPoints.Find(pointId);
                if (selectedPoint != null)
                {
                    return selectedPoint;
                }
            }

            Console.WriteLine("Неверный выбор ПВЗ!");
            return null;
        }

        static void RemoveFromCart()
        {
            Console.Write("Введите ID товара для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Неверный ID!");
                return;
            }

            var cartItem = db.CartItems.FirstOrDefault(ci =>
                ci.UserId == currentUser.UserId && ci.ProductId == productId);

            if (cartItem != null)
            {
                db.CartItems.Remove(cartItem);
                db.SaveChanges();
                Console.WriteLine("Товар удален из корзины!");
            }
            else
            {
                Console.WriteLine("Товар не найден в корзине!");
            }
        }

        static void ViewOrders()
        {
            Console.WriteLine("\n=== Мои заказы ===");
            var orders = db.Orders
                .Where(o => o.UserId == currentUser.UserId)
                .OrderByDescending(o => o.OrderDate)
                .Join(db.PickupPoints,
                      o => o.PointId,
                      pp => pp.PointId,
                      (o, pp) => new { Order = o, PickupPoint = pp })
                .ToList();

            if (!orders.Any())
            {
                Console.WriteLine("У вас пока нет заказов.");
                return;
            }

            foreach (var orderInfo in orders)
            {
                Console.WriteLine($"Заказ #{orderInfo.Order.OrderId} от {orderInfo.Order.OrderDate:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"Сумма: {orderInfo.Order.TotalAmount:C}");
                Console.WriteLine($"ПВЗ: {orderInfo.PickupPoint.Address}");

                var orderItems = db.OrderItems
                    .Where(oi => oi.OrderId == orderInfo.Order.OrderId)
                    .Join(db.Products,
                          oi => oi.ProductId,
                          p => p.ProductId,
                          (oi, p) => new { OrderItem = oi, Product = p })
                    .ToList();

                foreach (var item in orderItems)
                {
                    Console.WriteLine($"  - {item.Product.Name} x {item.OrderItem.Quantity} = {item.OrderItem.Price * item.OrderItem.Quantity:C}");
                }
                Console.WriteLine();
            }
        }
    }
}