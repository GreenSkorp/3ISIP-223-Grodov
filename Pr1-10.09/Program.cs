using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace Pr1_10._09
{


    public abstract class Person
    {
        private string _name;
        private int _age;
        private string _contactInfo;


        public string Id { get; private set; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Имя не может быть пустым");
                _name = value;
            }
        }

        public int Age
        {
            get => _age;
            set
            {
                if (value < 16 || value > 100)
                    throw new ArgumentException("Возраст должен быть от 16 до 100 лет");
                _age = value;
            }
        }

        public string ContactInfo
        {
            get => _contactInfo;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Контактная информация не может быть пустой");
                _contactInfo = value;
            }
        }


        protected Person(string id, string name, int age, string contactInfo)
        {
            Id = id;
            Name = name;
            Age = age;
            ContactInfo = contactInfo;
        }

        public abstract void DisplayInfo();
    }


    public interface IGradeable
    {
        void AddGrade(string courseId, double grade);
        double CalculateAverageGrade();
        Dictionary<string, List<double>> GetGrades();
    }


    class Program
    {
        static void Main(string[] args)
        {



        }
    }
}
