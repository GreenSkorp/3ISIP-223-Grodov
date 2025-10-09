using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr1_10._09
{

    public class Players
    {
        public int xp = 100;
        public string name;
        public Players(string Name)
        {
            name = Name;
        }
        public void NewPlayer(string nameme)
        {
            this.name = nameme;
        }

    }
    public class Enemy
    {
        public int globalhp = 50; // Слизень - 0.4х | Гоблин - 0.6х     1 Уровень моба
                                  // Скелет - 1х | Маг - 1х             2 Уровень моба
                                  // Хоб-Гоблин - 2х | Архи-Маг 1.5х    3 Уровень моба

    }

    public class Boss
    {

    }
    public class Loot
    {}


    class Program
    {
        public Players Player;
        public int stage = 0;
        public int firstlvlchance = 6;  //При выпадении повышать след лвл на 1, из данного убирать 1
        public int secondlvlchance = 3; 
        public int thirdlvlchance = 1;  //При выпадении Все лвл на усходную (6-3-1)
        static void Main(string[] args)
        {
            Console.WriteLine("Введите имя игрока:");
            string nam = Console.ReadLine();
            var Player = new Players(nam);

        }
    }
}
