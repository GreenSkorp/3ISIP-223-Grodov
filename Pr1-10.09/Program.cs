using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr1_10._09
{

    public class Players
    {
        protected int MaxHP = 100;
        public int HP = 100;
        public string Name;
        public Weapon weapon;
        public Armor armor;
        public List<Potion> potions;
        public Players(string name)
        {
            Name = name;
        }
        public void NewPlayer(string nameme)
        {
            this.Name = nameme;
        }

        public void NewArmor() { }
        public void NewWeapon() { }
        

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
    public class Loot  {



    }

    public class Weapon : Loot  
    {
        public string Name;
        public int ATK;           // 0-15 16-30 31-45 46-60
        public double CritChance;

        public Weapon(int atk,double critchanse) {
            ATK = atk;
            CritChance = critchanse;
            if (atk >= 15) Name = "Острый Мечь";
        }
    }
    public class Armor : Loot
    {
        public int AddHP;
        public double Deff;  // Процент поглащаемого урона
        public double DodgeChance;

        public Armor(int addhp, double dadgechance) { }
    }
    public class Potion : Loot
    {
        public int AddHP;
        public Potion() { }
    }


    class Program
    {
        public Random random = new Random();
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
