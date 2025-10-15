using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr1_10._09
{
    public class Game
    {
        
    }
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
        // Шансы 10 - 6 - 3 - 1
        // 1-10 11-16 17-19  20
        Random random = new Random();

        public int LootChance()
        {

            int i = random.Next(1,21);
            return i;
        }
        public Weapon GetRandomWeapon()
        {

            int chance = LootChance();
            if (chance <= 10)
            {
                
            }
            if (chance <= 16 && chance > 10)
            {

            }
            if (chance <= 19 && chance > 16)
            {

            }
            if (chance == 20)
            {

            }
            return null;
        }
        public void GetRandomArmor()
        {

        }
        public void GetRandomPotions()
        {

        }

    }

    public class Weapon : Loot  
    {
        public string Name;
        public int ATK;           //  0-15 16-30 31-45 46-60
        public double CritChance; // 10-20 20-35 35-50 50-75

        public Weapon(int atk,double critchanse) {
            ATK = atk;
            CritChance = critchanse;
            if (atk >= 15) Name = "Острый Мечь";
            if (atk<=30 && atk > 15) Name = "Закаленный Мечь";
            if (atk <= 45 && atk > 30) Name = "Пробужденный Мечь";
            if (atk <= 60 && atk > 45) Name = "Сокрушитель тьмы";

        }
    }
    public class Armor : Loot
    {
        public string Name;
        public int AddHP;    //     15 - 40  41 - 65   66 - 90   91 - 125
        public double Deff;  // %    0 - 5    6 - 15   16 - 30   31 - 50
        public double DodgeChance;// 0 - 5   6 - 12.5 12.5 - 20  21 - 30    

        public Armor(int addhp, double deff, double dodgechance) {
            AddHP = addhp;
            Deff = deff;
            DodgeChance = dodgechance;
            if (AddHP >= 15) Name = "Одеяния новичка";
            if (AddHP <= 30 && AddHP > 15) Name = "Кожанные доспехи";
            if (AddHP <= 45 && AddHP > 30) Name = "Латные доспехи";
            if (AddHP <= 60 && AddHP > 45) Name = "Доспехи падшего короля";
        }
    }
    public class Potion : Loot
    {
        public string Name;
        public int AddHP;               // 10 - 30 31 - 60 61 - 100 101 - 150
        public double HPrechargechance; // 5 - 15  16 - 25 26 - 35  36 - 50

        public Potion(int addhp, double hprech) {
            AddHP = addhp;
            HPrechargechance = hprech;
            if (AddHP >= 30) Name = "Слабое зелье лечения";
            if (AddHP <= 60 && AddHP > 30) Name = "Обычное зелье лечения";
            if (AddHP <= 100 && AddHP > 60) Name = "Сильное зелье лечения";
            if (AddHP <= 150 && AddHP > 100) Name = "Божественное зелье лечения";
        }
    }

  
    class Program
    {
  public static void WaitEnter()
        {
            Console.WriteLine("Нажмите Enter для продолжения ...");
            Console.ReadLine();
            Console.Clear();
            
        }
        public static void WriteColor(string text, ConsoleColor color)
        {
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = original;
        }
        public static void WriteLineColor(string text, ConsoleColor color)
        {
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = original;
        }
        public static void NewStage(int Stage)
        {
            
            WriteLineColor($"Следующий уровень - {Stage++}!",ConsoleColor.Cyan);

            
        }

        public Random random = new Random();
        public Players Player;
        public int stage = 0;





        static void Main(string[] args)
        {
            int stage = 0;
            Console.WriteLine("Введите имя игрока:");
            NewStage(stage);

            string nam = Console.ReadLine();
            var Player = new Players(nam);
            WaitEnter();
            NewStage(stage);


        }
    }
}
