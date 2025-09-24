using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Pr1_10._09
{
    /*Создайте консольное приложение C# для учёта книг в библиотеке.
    У книги должны быть следующие параметры: 
    Уникальный идентификатор(генерируется автоматически при добавлении). 
    * Название.
    * Автор.
    * Жанр(можно выбрать из заданных в коде вариантов, не менее трёх). 
    * Год издания.
    * Цена.
    Мы можем работать с книгами через команды: 
    * Добавить книгу (запросить все параметры у пользователя, идентификатор назначается автоматически). 
    * Удалить книгу по идентификатору.
    * Найти книги (по названию, автору, жанру, должны быть все варианты поиска книги) и выводить полную информацию.
    * Отсортировать книги по названию или году (должны быть обе команды).
    * Вывести самую дорогую и самую дешёвую книгу.
    * Сгруппировать книги по авторам и вывести количество книг каждого автора.*/


    class Program
    {
        private static List<Book> books = new List<Book>()
        {
                new Book(10, "Гиря Пети" , "Жак Поль", "Фантастика", 2020, 100),
                new Book(12, "Гиря Пети" , "Петя", "Детектив", 2024, 1000),
                new Book(11, "Перо в небе" , "Жак Поль", "Фантастика", 2021, 300)

        };
        private static int nextId = 100;
        private static readonly List<string> availableGenres = new List<string> { "Фантастика", "Детектив", "Роман", "Научная литература", "Исторический" };
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система учета книг ===");
            string choise;
            while (true)
            {
               
                Console.Clear();
                ShowMenu();
                choise= Console.ReadLine();
                Console.WriteLine("");
                switch (choise)
                {
                    case "0":
                        Console.WriteLine("Выход из программы.");
                        break;
                    case "1":
                        AddBook();
                        break;
                    case "2":
                        DeleteBook();
                        break;
                    case "3":
                        FindBook();
                        break;
                    case "4":
                        SortingBooks();
                        break;
                    case "5":
                        DorogoDeshevo();
                        break;
                    case "6":
                        AuthorBlockBook();
                        break;
                    case "7":
                        BookList();
                        break;
                    case "8":
                        AddManyBook();
                        break;



                }
            }
        }
        static void ShowMenu() {
            Console.WriteLine("\n=== МЕНЮ ===");
            Console.WriteLine("1. Добавить книгу");
            Console.WriteLine("2. Удалить книгу по id");
            Console.WriteLine("3. Найти книгу");
            Console.WriteLine("4. Отсортировать книги");
            Console.WriteLine("5. Показать самую дорогую и дешёвую книгу");
            Console.WriteLine("6. Сгруппировать книги по авторам");
            Console.WriteLine("7. Показать все книги");
            Console.WriteLine("8. Добавить блок книг");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");
        }
        static void AddBook()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОЙ КНИГИ ===");

            Console.WriteLine("Введите данные книги (Через запятую данные: Название книги, Автор, Жанр, Год, Цена): ");
            string[] book = Console.ReadLine().Split(new char[] { ',' });
            for (int i = 1; i < book.Length; i++)
            {
                book[i] = book[i].Trim();
            }
            if (book.Length != 5) { Console.WriteLine("Неверный формат ввода данных"); WaitEnter(); return; }

            Book newBook = new Book(nextId++, book[0], book[1], book[2], Convert.ToInt32(book[3]), Convert.ToInt32(book[4]));
            books.Add(newBook);
            Console.WriteLine($"Книга успешно добавлена! ID: {newBook.Id}");
            WaitEnter();
        }
        static void AddManyBook()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ БЛОКА КНИГ ===");
            Console.WriteLine("Как много книг в блоке:");
            int num = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите данные книг в разных строках (Через запятую данные: Название книги, Автор, Жанр, Год, Цена): ");
            for (int j =0; j<num;j++) {string[] book = Console.ReadLine().Split(new char[] { ',' });
            for (int i = 1; i < book.Length; i++)
            {
                book[i] = book[i].Trim();
            }
            if (book.Length != 5) { Console.WriteLine("Неверный формат ввода данных"); WaitEnter(); return; }

            Book newBook = new Book(nextId++, book[0], book[1], book[2], Convert.ToInt32(book[3]), Convert.ToInt32(book[4]));
                books.Add(newBook);
            }
            Console.WriteLine("Блок книг успешно добавлен!");
            WaitEnter();
        }
        static void DeleteBook()
        {
            Console.Write("Введите ID книги которую хотите удалить: ");
            int code = Convert.ToInt32(Console.ReadLine());
            var book = books.FirstOrDefault(p=>p.Id == code);
            books.Remove(book);
            Console.WriteLine($"Успешное удаление книги с Кодом {code}");
            WaitEnter() ;
        }
        static void BookList()
        {
            foreach (Book book in books)
            {
                book.Print();

            }
            WaitEnter();

        }
        static void FindBook()
        {

            List<Book> sortbooks = new List<Book>();
            while (true)
            {
                Console.WriteLine("===МЕНЮ ВЫБОРА СОРТИРОВКИ===");
            Console.WriteLine("1. По Названию.");
            Console.WriteLine("2. По Автору");
            Console.WriteLine("3. По Жанру.");
            Console.WriteLine("0. Выход");
            string choise = Console.ReadLine();
            Console.WriteLine("");


            switch (choise)
            {
                case "0":
                    return;

                case "1":
                        Console.WriteLine("Введите название для поиска: ");
                        string name = Console.ReadLine();
                    sortbooks = books.Where(p => p.Name == name).ToList();
                    break;
                case "2":
                        Console.WriteLine("Введите автора для поиска: ");
                        string author = Console.ReadLine();
                        sortbooks = books.Where(p => p.Author == author).ToList();
                        break;
                case "3":
                        Console.WriteLine("Введите жанр для поиска: ");
                        string genre = Console.ReadLine();
                        sortbooks = books.Where(p => p.Genre == genre).ToList();
                        break;
            }
            Console.WriteLine("Найденные книги:");
            foreach (var book in sortbooks)
            {
                book.Print();
            }
            WaitEnter();
        }
        }
        
        static void SortingBooks()
        {

            List<Book> sortbooks = new List<Book>();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===МЕНЮ ВЫБОРА ПОИСКА===");
                Console.WriteLine("1. По Названию.");
                Console.WriteLine("2. По Году");
                Console.WriteLine("0. Выход");
                string choise = Console.ReadLine();
                Console.WriteLine("");


                switch (choise)
                {
                    case "0":
                        return;

                    case "1":
                        sortbooks = books.OrderBy(p => p.Name).ToList();
                        break;
                    case "2":
                        sortbooks = books.OrderBy(p => p.Year).ToList();
                        break;
                    case "3":
                        FindBook();
                        break;
                }
                Console.WriteLine("\nОтсортированный список книг:");
                foreach (var book in sortbooks)
                {
                    book.Print();
                }
                WaitEnter();
            }
        }
        static void DorogoDeshevo()
        {
            int min = books.Min(p => p.Price);
            int max = books.Max(p => p.Price);
            Book less = books.First(p => p.Price == min);
            Book more = books.First(p => p.Price == max);
            Console.WriteLine("Самая дорогая книга:");
            more.Print();
            Console.WriteLine("Самая дешевая книга:");
            less.Print();
            WaitEnter();
        }
        static void AuthorBlockBook()
        {
            var booksByAuthor = books.GroupBy(b => b.Author)
                                        .OrderBy(g => g.Key); // Сортировка по имени автора

            foreach (var authorGroup in booksByAuthor)
            {
                Console.WriteLine($"\n АВТОР: {authorGroup.Key}");
;
                Console.WriteLine($"Всего книг: {authorGroup.Count()}");


                int counter = 1;
                foreach (var book in authorGroup.OrderBy(b => b.Year))
                {
                    Console.WriteLine($"{counter}. {book.Name}");
                    Console.WriteLine($"   Жанр: {book.Genre} | Год: {book.Year} | Цена: {book.Price} руб.");
                    counter++;
                }
            }
            WaitEnter();
        }

        static void WaitEnter()
        {
            Console.WriteLine("Для продолжение нажмите Enter...");
            Console.Read();
        }

    }
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public int Price { get; set; }

        public Book(int id, string fullName, string login, string genre, int year, int price)
        {
            Id = id;
            Name = fullName;
            Author = login;
            Genre = genre;
            Year = year;
            Price = price;
        }

        public void Print()
        {
            Console.WriteLine($" Код:{this.Id} | Название: {this.Name}, Автор: {this.Author}, Жанр: {this.Genre}, {this.Year} год издания, цена {this.Price}$.");
        }




    }
}
