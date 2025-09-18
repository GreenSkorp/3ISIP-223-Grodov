using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Rabotata {

    /*
     Необходимо написать программу, которая будет принимать текст от пользователя и делать над ним определённые действия.
    Функциональные требования:

        Программа принимает от пользователя минимум 100 символов
        Подсчёт количества слов в тексте
        Поиск самого короткого слова
        Подсчёт количества предложений
        Подсчёт количества гласных и согласных букв
        Поиск самого длинного слова
        Создание статистики по частоте встречаемости каждой буквы
        Возможность продолжить работу с новым текстом
        Сохранение всей статистики в список
        Возможность вывести статистику по прошлым текстам

        Сделать подсчет слов в тексте, не считая союзы и числа
        Удаление букв, которые введёт пользователь. После удаления букв, необходимо сделать повторный подсчёт всех статистик по тексту


     */



    class Text
    {
        public string OriginalText { get; set; }
        public string[] Words { get; set; }
        public int TotalWords { get; set; }
        public int WordsWithoutConjunctions { get; set; }
        public string ShortestWord { get; set; }
        public string LongestWord { get; set; }
        public int SentencesCount { get; set; }
        public int VowelsCount { get; set; }
        public int ConsonantsCount { get; set; }
        public Dictionary<char, int> LetterFrequency { get; set; }
        public DateTime CreatedDate { get; set; }

    }


    class Program
    {
        private static List<Text> history = new List<Text>();
        private static string[] conjunctions = new string[]
{
            "и", "а", "но", "да", "или", "либо", "то", "не", "ни",
            "чтобы", "как", "что", "когда", "пока", "если", "хотя",
            "потому", "так", "тоже", "также", "зато", "однако"
};
        private static char[] sentenceSeparators = new char[] { '.', '!', '?' };
        private static char[] punctuation = new char[] { ',', '.', '!', '?', ':', ';', '-', '(', ')', '\"', '\'' };
        private static char[] vowels = new char[] { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я', 'А', 'Е', 'Ё', 'И', 'О', 'У', 'Ы', 'Э', 'Ю', 'Я' };


        static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=== Анализ текста ===");
                Console.WriteLine("1. Анализ нового текста");
                Console.WriteLine("2. Просмотр истории анализа");
                Console.WriteLine("3. Удаление букв из текущего текста");
                Console.WriteLine("4. Выход");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AnalyzeNewText();
                        break;
                    case "2":
                        ShowHistory();
                        break;
                    case "3":
                        RemoveLetters();
                        break;
                    case "4":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        Console.ReadKey();
                        break;
                }
            }
        }




        static void AnalyzeNewText()
        {
            string inputText = WriteText();
            if (inputText.Length < 100)
            {
                Console.WriteLine("Текст должен содержать минимум 100 символов!");
                Console.ReadKey();
                return;
            }

            Text text = new Text
            {
                OriginalText = inputText,
                CreatedDate = DateTime.Now
            };

            AnalyzeText(text);
            history.Add(text);

            DisplayResults(text);
            Console.ReadKey();
        }

        static string WriteText()
        {
            Console.WriteLine("Введите новый текст для обработки (минимум 100 символов): ");
            return Console.ReadLine();
        }

        static void AnalyzeText(Text text)
        {
            // Разделение на слова
            string[] words = text.OriginalText.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            text.Words = words.Select(w => new string(w.Where(c => !punctuation.Contains(c)).ToArray())).ToArray();
            text.TotalWords = text.Words.Length;

            // Подсчет слов без союзов и чисел
            text.WordsWithoutConjunctions = text.Words.Count(w =>
                !conjunctions.Contains(w.ToLower()) && !IsNumber(w));

            // Поиск самого короткого и длинного слова
            if (text.Words.Length > 0)
            {
                text.ShortestWord = text.Words.OrderBy(w => w.Length).First();
                text.LongestWord = text.Words.OrderByDescending(w => w.Length).First();
            }

            // Подсчет предложений
            text.SentencesCount = text.OriginalText.Split(sentenceSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Count(s => s.Trim().Length > 0);

            // Подсчет гласных и согласных
            text.VowelsCount = text.OriginalText.Count(c => vowels.Contains(char.ToLower(c)));
            text.ConsonantsCount = text.OriginalText.Count(c => char.IsLetter(c) && !vowels.Contains(char.ToLower(c)));

            // Статистика букв
            text.LetterFrequency = text.OriginalText
                .Where(char.IsLetter)
                .GroupBy(char.ToLower)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        static bool IsNumber(string word)
        {
            return word.All(char.IsDigit) || (word.StartsWith("-") && word.Substring(1).All(char.IsDigit));
        }

        static void DisplayResults(Text text)
        {
            Console.WriteLine("\n=== Результаты анализа ===");
            Console.WriteLine($"Общее количество слов: {text.TotalWords}");
            Console.WriteLine($"Количество слов (без союзов и чисел): {text.WordsWithoutConjunctions}");
            Console.WriteLine($"Самое короткое слово: {text.ShortestWord}");
            Console.WriteLine($"Самое длинное слово: {text.LongestWord}");
            Console.WriteLine($"Количество предложений: {text.SentencesCount}");
            Console.WriteLine($"Гласные буквы: {text.VowelsCount}");
            Console.WriteLine($"Согласные буквы: {text.ConsonantsCount}");

            Console.WriteLine("\nСтатистика букв:");
            foreach (var pair in text.LetterFrequency.OrderByDescending(p => p.Value))
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }
        }

        static void ShowHistory()
        {
            if (history.Count == 0)
            {
                Console.WriteLine("История пуста!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\n=== История анализа ===");
            for (int i = 0; i < history.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Текст от {history[i].CreatedDate:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"   Слов: {history[i].TotalWords}, Предложений: {history[i].SentencesCount}");
            }

            Console.Write("\nВведите номер текста для просмотра (0 - вернуться): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= history.Count)
            {
                DisplayResults(history[choice - 1]);
                Console.ReadKey();
            }
        }

        static void RemoveLetters()
        {
            if (history.Count == 0)
            {
                Console.WriteLine("Сначала проанализируйте текст!");
                Console.ReadKey();
                return;
            }

            Text currentText = history[history.Count - 1];
            Console.WriteLine($"Текущий текст: {currentText.OriginalText.Substring(0, Math.Min(100, currentText.OriginalText.Length))}...");

            Console.Write("Введите буквы для удаления (без пробелов): ");
            string lettersToRemove = Console.ReadLine();

            if (!string.IsNullOrEmpty(lettersToRemove))
            {
                string modifiedText = currentText.OriginalText;
                foreach (char letter in lettersToRemove)
                {
                    modifiedText = modifiedText.Replace(letter.ToString(), "").Replace(char.ToUpper(letter).ToString(), "");
                }

                Text newText = new Text
                {
                    OriginalText = modifiedText,
                    CreatedDate = DateTime.Now
                };

                AnalyzeText(newText);
                history.Add(newText);

                Console.WriteLine("\nТекст после удаления букв:");
                Console.WriteLine(modifiedText);
                DisplayResults(newText);
            }

            Console.ReadKey();
        }
    }
}
