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


namespace ShopInventory
{
    public enum Category
    {
        Electronics,
        Clothing,
        Food,
        Books,
        Sports
    }

    public class Product
    {
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

            Name = name;
            Price = price;
            Quantity = quantity;
            Category = category;
        }


        public override string ToString()
        {
            return $"Название: {Name}, Цена: {Price:C}, " +
                   $"Количество: {Quantity}, В наличии: {(InStock ? "Да" : "Нет")}, " +
                   $"Категория: {Category}";
        }
    }




}
