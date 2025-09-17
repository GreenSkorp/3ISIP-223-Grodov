using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabotata

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
    public int TotalWords { get; set; }
    public int WordsWithoutConjunctionsNumbers { get; set; }
    public string ShortestWord { get; set; }
    public string LongestWord { get; set; }
    public int SentencesCount { get; set; }
    public int VowelsCount { get; set; }
    public int ConsonantsCount { get; set; }
    public Dictionary<char, int> LetterFrequency { get; set; }
}


{
    class Program
    {
    private static List<Text> history = new List<Text>();
    private static HashSet<string> conjunctions = new HashSet<string>
        {
            "и", "а", "но", "да", "или", "либо", "то", "не", "ни",
            "чтобы", "как", "что", "когда", "пока", "если", "хотя",
            "потому", "так", "тоже", "также", "зато", "однако"
        };


    static void Main(string[] args)
        {



        }
    }
}
