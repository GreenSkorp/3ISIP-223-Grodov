using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr1_10._09
{
    public class Game
    {
        public Players Player;
        public int Stage = 1;
        private Random random = new Random();

        private void DisplayQuickStats()
        {
            
            WriteLineColor($"\nHP: {Player.HP}/{Player.MaxHP + Player.armor.AddHP} | Урон: {Player.weapon.ATK}", ConsoleColor.DarkGreen);
        }
        public void StartGame()
        {
            Console.WriteLine("Введите имя игрока:");
            string name = Console.ReadLine();

            Player = new Players(name);
            Player.weapon = new Weapon(10, 10); // Стартовое оружие
            Player.armor = new Armor(20, 5, 5); // Стартовая броня
            Player.HP = 120;
            Player.potions = new List<Potion> { new Potion(20, 10) }; // Стартовое зелье

            Console.WriteLine($"Добро пожаловать, {name}!");
            Console.WriteLine($"Ваше стартовое снаряжение: {Player.weapon.Name}, {Player.armor.Name}, 1 зелье лечения");
            WaitEnter();

            while (Player.HP > 0)
            {
                PlayStage();
                if (Player.HP <= 0) break;

                Stage++;
                if (Stage > 30) break; 
            }

            if (Player.HP > 0)
            {
                WriteLineColor("Поздравляем! Вы прошли игру!", ConsoleColor.Green);
            }
            else
            {
                WriteLineColor("Игра окончена! Вы погибли...", ConsoleColor.Red);
            }
        }

        private void PlayStage()
        {
            NewStage(Stage);

            if (Stage % 10 == 0) // Каждый 10 уровень - босс
            {
                FightBoss();
            }
            else
            {
                int encounter = random.Next(1, 4); // 1-3
                if (encounter == 1) // 33% шанс на сундук
                {
                    FindChest();
                }
                else // 66% шанс на монстра
                {
                    FightEnemy();

                    if (Player.HP > 0)
                    {
                        Console.WriteLine($"У вас осталось {Player.HP} HP");
                        WaitEnter();
                    }
                }
            }


        }

        private void FindChest()
        {
            WriteLineColor("Вы нашли сундук с сокровищами!", ConsoleColor.Yellow);

            Loot loot = new Loot();

            // Получаем случайное оружие или броню
            if (random.Next(2) == 0)
            {
                Weapon newWeapon = loot.GetRandomWeapon();
                Console.WriteLine($"В сундуке: {newWeapon.Name} (АТК: {newWeapon.ATK}, Крит: {newWeapon.CritChance}%)");

                Console.WriteLine("Хотите взять его? (д/н)");
                if (Console.ReadLine().ToLower() == "д")
                {
                    Player.weapon = newWeapon;
                    WriteLineColor($"Вы экипировали: {newWeapon.Name}", ConsoleColor.Green);
                    DisplayQuickStats();
                }
                else { WriteColor("Предмет был оставлен.", ConsoleColor.DarkYellow); }
            }
            else
            {
                Armor newArmor = loot.GetRandomArmor();
                Console.WriteLine($"В сундуке: {newArmor.Name} (+HP: {newArmor.AddHP}, Защ: {newArmor.Deff}%, Уклон: {newArmor.DodgeChance}%)");

                Console.WriteLine("Хотите взять его? (д/н)");
                if (Console.ReadLine().ToLower() == "д")
                {
                    // СОХРАНЯЕМ СТАРОЕ МАКСИМАЛЬНОЕ HP ДЛЯ СООБЩЕНИЯ
                    int oldMaxHP = Player.MaxHP + Player.armor.AddHP;

                    Player.armor = newArmor;

                    // УСТАНАВЛИВАЕМ HP В 100 + БОНУС НОВОЙ БРОНИ
                    Player.HP = Player.HP + newArmor.AddHP;

                    int newMaxHP = Player.MaxHP + Player.armor.AddHP;
                    WriteLineColor($"Вы экипировали: {newArmor.Name}", ConsoleColor.Green);
                    WriteLineColor($"Ваше здоровье теперь: {Player.HP}/{newMaxHP} HP", ConsoleColor.Green);

                    DisplayQuickStats();
                }
                else { WriteColor("Предмет был оставлен.", ConsoleColor.DarkYellow); }
            }

            // Получаем зелья (1-3 штуки)
            int potionCount = random.Next(1, 4);
            for (int i = 0; i < potionCount; i++)
            {
                Potion potion = loot.GetRandomPotion();
                Player.potions.Add(potion);
                Console.WriteLine($"Вы нашли: {potion.Name} (+{potion.AddHP} HP)");
            }
        }

        private void FightEnemy()
        {
            Enemy enemy = new Enemy(Stage);
            WriteLineColor($"На вас напал {enemy.Name}! (HP: {enemy.HP}, ATK: {enemy.ATK})", ConsoleColor.Red);

            bool playerFrozen = false;
            int freezeTurns = 0;

            while (enemy.HP > 0 && Player.HP > 0)
            {
                if (playerFrozen)
                {
                    WriteLineColor("Вы заморожены и пропускаете ход!", ConsoleColor.Blue);
                    playerFrozen = false;
                }
                else
                {
                    PlayerTurn(enemy);
                    if (enemy.HP <= 0) break;
                }

                EnemyTurn(enemy, ref playerFrozen, ref freezeTurns);
            }

            if (enemy.HP <= 0)
            {
                WriteLineColor($"Вы победили {enemy.Name}!", ConsoleColor.Green);
                // Награда за победу
                Player.HP = Math.Min(Player.MaxHP, Player.HP + 10); // Восстановление HP
                WriteLineColor("Вы восстановили 10 HP", ConsoleColor.Green);
            }
        }

        private void FightBoss()
        {
            Boss boss = new Boss(Stage);
            WriteLineColor($"БОСС: {boss.Name}! (HP: {boss.HP}, ATK: {boss.ATK})", ConsoleColor.DarkRed);

            while (boss.HP > 0 && Player.HP > 0)
            {
                PlayerTurn(boss);
                if (boss.HP <= 0) break;

                BossTurn(boss);
            }

            if (boss.HP <= 0)
            {
                WriteLineColor($"Вы победили босса {boss.Name}!", ConsoleColor.Green);
                WriteLineColor("ВЫ ПОЛУЧИЛИ ЛЕГЕНДАРНУЮ НАГРАДУ!", ConsoleColor.Yellow);
                Player.HP = Player.MaxHP; // Полное восстановление HP
            }
        }

        private void PlayerTurn(Enemy enemy)
        {
            DisplayQuickStats();
            Console.WriteLine("\nВаш ход:");
            Console.WriteLine("1 - Атаковать");
            Console.WriteLine("2 - Использовать зелье");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                int damage = Player.weapon.ATK;
                // Проверка на критический удар
                if (random.Next(100) < Player.weapon.CritChance)
                {
                    damage *= 2;
                    WriteLineColor("КРИТИЧЕСКИЙ УДАР!", ConsoleColor.Yellow);
                }

                enemy.HP -= damage;
                WriteLineColor($"Вы нанесли {damage} урона {enemy.Name}", ConsoleColor.Green);
            }
            else if (choice == "2")
            {
                if (Player.potions.Count > 0)
                {
                    Potion potion = Player.potions[0];
                    Player.potions.RemoveAt(0);
                    Player.HP = Math.Min(Player.MaxHP, Player.HP + potion.AddHP);
                    WriteLineColor($"Вы использовали {potion.Name} и восстановили {potion.AddHP} HP", ConsoleColor.Green);
                }
                else
                {
                    WriteLineColor("У вас нет зелий!", ConsoleColor.Red);
                    PlayerTurn(enemy); // Повтор хода
                }
            }
        }

        private void EnemyTurn(Enemy enemy, ref bool playerFrozen, ref int freezeTurns)
        {
            // Проверка уклонения
            if (random.Next(100) < Player.armor.DodgeChance)
            {
                WriteLineColor("Вы уклонились от атаки!", ConsoleColor.Cyan);
                return;
            }

            int damage = enemy.ATK;
            // Применение защиты брони
            damage = (int)(damage * (1 - Player.armor.Deff / 100));

            Player.HP -= damage;
            WriteLineColor($"{enemy.Name} наносит вам {damage} урона", ConsoleColor.Red);

            // Проверка спецспособности мага
            if (enemy.Type == "Маг" && random.Next(100) < enemy.FreezeChance)
            {
                playerFrozen = true;
                WriteLineColor("Маг заморозил вас на следующий ход!", ConsoleColor.Blue);
            }
        }

        private void BossTurn(Boss boss)
        {
            // Проверка уклонения
            if (random.Next(100) < Player.armor.DodgeChance)
            {
                WriteLineColor("Вы уклонились от атаки босса!", ConsoleColor.Cyan);
                return;
            }

            int damage = boss.ATK;
            // Применение защиты брони
            damage = (int)(damage * (1 - Player.armor.Deff / 100));

            Player.HP -= damage;
            WriteLineColor($"Босс {boss.Name} наносит вам {damage} урона", ConsoleColor.DarkRed);

            // Проверка спецспособностей боссов
            if (boss.Name == "Повелитель Тьмы" && random.Next(100) < boss.FreezeChance)
            {
                WriteLineColor("Повелитель Тьмы накладывает на вас проклятие! Вы теряете 15 HP!", ConsoleColor.DarkMagenta);
                Player.HP -= 15;
            }
            else if (boss.Name == "Король Демонов" && random.Next(100) < boss.FreezeChance)
            {
                WriteLineColor("Король Демонов оглушает вас! Вы пропускаете следующий ход!", ConsoleColor.DarkRed);
                // Здесь можно добавить логику пропуска хода, если нужно
            }
        }

        // Вспомогательные методы
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

        public void NewStage(int Stage)
        {
            WriteLineColor($"=== Уровень {Stage} ===", ConsoleColor.Cyan);
        }
    }

    public class Players
    {
        public int MaxHP = 100;
        public int HP = 100;
        public string Name;
        public Weapon weapon;

        public Armor armor;
        public List<Potion> potions;

        public Players(string name)
        {
            Name = name;
            potions = new List<Potion>();
        }
    }

    public class Enemy
    {
        public string Name;
        public int HP;
        public int ATK;
        public string Type;
        public double FreezeChance;

        public Enemy(int stage)
        {
            Random random = new Random();

            // Определяем веса для уровней мобов: 1 уровень - 7, 2 уровень - 3, 3 уровень - 1
            int[] levelWeights = { 7, 3, 1 }; // Сумма весов = 11

            // Выбираем уровень моба на основе весов
            int levelChoice = random.Next(1, 12); // 1-11

            int mobLevel;
            if (levelChoice <= 7) // 1-7 = 7/11 шанс
            {
                mobLevel = 1;
            }
            else if (levelChoice <= 10) // 8-10 = 3/11 шанс
            {
                mobLevel = 2;
            }
            else // 11 = 1/11 шанс
            {
                mobLevel = 3;
            }

            // Выбираем конкретного моба в зависимости от уровня
            string[] level1Mobs = { "Слизень", "Гоблин" };
            string[] level2Mobs = { "Скелет", "Маг" };
            string[] level3Mobs = { "Хоб-Гоблин", "Архи-Маг" };

            switch (mobLevel)
            {
                case 1:
                    Type = level1Mobs[random.Next(level1Mobs.Length)];
                    break;
                case 2:
                    Type = level2Mobs[random.Next(level2Mobs.Length)];
                    break;
                case 3:
                    Type = level3Mobs[random.Next(level3Mobs.Length)];
                    break;
            }

            Name = Type;

            // Базовая статистика в зависимости от типа (остается без изменений)
            switch (Type)
            {
                case "Слизень":
                    HP = random.Next(5, 30);
                    ATK = random.Next(5, 15);
                    FreezeChance = 0;
                    break;
                case "Гоблин":
                    HP = random.Next(25, 50);
                    ATK = random.Next(5, 25);
                    FreezeChance = 0;
                    break;
                case "Скелет":
                    HP = random.Next(50, 100);
                    ATK = random.Next(10, 30);
                    FreezeChance = 0;
                    break;
                case "Маг":
                    HP = random.Next(25, 60);
                    ATK = random.Next(20, 50);
                    FreezeChance = random.Next(10, 26); // 10-25%
                    break;
                case "Хоб-Гоблин":
                    HP = random.Next(100, 200);
                    ATK = random.Next(15, 40);
                    FreezeChance = 0;
                    break;
                case "Архи-Маг":
                    HP = random.Next(40, 80);
                    ATK = random.Next(30, 70);
                    FreezeChance = random.Next(15, 31); // 15-30%
                    break;
            }
        }
    }

    public class Boss : Enemy
    {
        public Boss(int stage) : base(stage)
        {
            Random random = new Random();
            string[] bossNames = { "Древний Дракон", "Повелитель Тьмы", "Король Демонов" };
            Name = bossNames[random.Next(bossNames.Length)];

            // Статистика в зависимости от типа босса
            switch (Name)
            {
                case "Древний Дракон":
                    HP = 270;
                    ATK = 25;
                    Type = "Босс";
                    FreezeChance = 0;
                    break;
                case "Повелитель Тьмы":
                    HP = 220;
                    ATK = 30;
                    Type = "Босс";
                    FreezeChance = 15; // 15% шанс наложить проклятие
                    break;
                case "Король Демонов":
                    HP = 250;
                    ATK = 28;
                    Type = "Босс";
                    FreezeChance = 10; // 10% шанс оглушить
                    break;
            }
        }
    }

    public class Loot
    {
        Random random = new Random();

        public int LootChance()
        {
            return random.Next(1, 21);
        }

        public Weapon GetRandomWeapon()
        {
            int chance = LootChance();
            if (chance <= 10)
            {
                return new Weapon(random.Next(5, 16), random.Next(10, 21));
            }
            if (chance <= 16 && chance > 10)
            {
                return new Weapon(random.Next(16, 31), random.Next(20, 36));
            }
            if (chance <= 19 && chance > 16)
            {
                return new Weapon(random.Next(31, 46), random.Next(35, 51));
            }
            if (chance == 20)
            {
                return new Weapon(random.Next(46, 61), random.Next(50, 76));
            }
            return new Weapon(10, 10);
        }

        public Armor GetRandomArmor()
        {
            int chance = LootChance();
            if (chance <= 10)
            {
                return new Armor(random.Next(15, 41), random.Next(0, 6), random.Next(0, 6));
            }
            if (chance <= 16 && chance > 10)
            {
                return new Armor(random.Next(41, 66), random.Next(6, 16), random.Next(6, 13));
            }
            if (chance <= 19 && chance > 16)
            {
                return new Armor(random.Next(66, 91), random.Next(16, 31), random.Next(13, 21));
            }
            if (chance == 20)
            {
                return new Armor(random.Next(91, 126), random.Next(31, 51), random.Next(21, 31));
            }
            return new Armor(20, 5, 5);
        }

        public Potion GetRandomPotion()
        {
            int chance = LootChance();
            if (chance <= 10)
            {
                return new Potion(random.Next(10, 31), random.Next(5, 16));
            }
            if (chance <= 16 && chance > 10)
            {
                return new Potion(random.Next(31, 61), random.Next(16, 26));
            }
            if (chance <= 19 && chance > 16)
            {
                return new Potion(random.Next(61, 101), random.Next(26, 36));
            }
            if (chance == 20)
            {
                return new Potion(random.Next(101, 151), random.Next(36, 51));
            }
            return new Potion(20, 10);
        }
    }

    public class Weapon : Loot
    {
        public string Name;
        public int ATK;
        public double CritChance;

        public Weapon(int atk, double critchance)
        {
            ATK = atk;
            CritChance = critchance;
            if (atk <= 15) Name = "Острый Мечь";
            else if (atk <= 30) Name = "Закаленный Мечь";
            else if (atk <= 45) Name = "Пробужденный Мечь";
            else Name = "Сокрушитель тьмы";
        }
    }

    public class Armor : Loot
    {
        public string Name;
        public int AddHP;
        public double Deff;
        public double DodgeChance;

        public Armor(int addhp, double deff, double dodgechance)
        {
            AddHP = addhp;
            Deff = deff;
            DodgeChance = dodgechance;
            if (addhp <= 40) Name = "Одеяния новичка";
            else if (addhp <= 65) Name = "Кожанные доспехи";
            else if (addhp <= 90) Name = "Латные доспехи";
            else Name = "Доспехи падшего короля";
        }
    }

    public class Potion : Loot
    {
        public string Name;
        public int AddHP;
        public double HPrechargechance;

        public Potion(int addhp, double hprech)
        {
            AddHP = addhp;
            HPrechargechance = hprech;
            if (addhp <= 30) Name = "Слабое зелье лечения";
            else if (addhp <= 60) Name = "Обычное зелье лечения";
            else if (addhp <= 100) Name = "Сильное зелье лечения";
            else Name = "Божественное зелье лечения";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.StartGame();
        }
    }
}