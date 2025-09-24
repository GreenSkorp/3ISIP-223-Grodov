using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        static void Main(string[] args)
        {



        }
    }
    public class Book
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public int Price { get; set; }

        public Book(string fullName, string login, string genre, int year, int price)
        {
            Name = fullName;
            Author = login;
            Genre = genre;
            Year = year;
            Price = price;
        }

        public void Print()
        {
            Console.WriteLine($" {Name} {Author} {Genre} {Year} {Price}");
        }
    }
}
