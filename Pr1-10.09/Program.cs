using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace Kucherenko223
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

        public abstract void DisplayInfo();  // Абстрактный метод - Только обявляется но не реализовывается, в наследуемых классах обязательно перезаписывается с помозью Override
    }

    public class Student : Person, IGradeable // Наследование абстрактного класса и элементов интерфейса оценок
    {
        private Dictionary<string, List<double>> _grades;
        private List<Course> _courses;

        public Student(string id, string name, int age, string contactInfo)
            : base(id, name, age, contactInfo)
        {
            _grades = new Dictionary<string, List<double>>();
            _courses = new List<Course>();
        }


        public IReadOnlyList<Course> Courses => _courses.AsReadOnly();  // Только чтение (курсов)

        public void EnrollInCourse(Course course) // Добавление ученика на курс
        {
            if (!_courses.Contains(course))
            {
                _courses.Add(course);
                course.AddStudent(this);
            }
        }


        public void AddGrade(string courseId, double grade)
        {
            if (grade < 0 || grade > 100)
                throw new ArgumentException("Оценка должна быть от 0 до 100");

            if (!_grades.ContainsKey(courseId))
                _grades[courseId] = new List<double>();

            _grades[courseId].Add(grade);
        }

        public double CalculateAverageGrade()
        {
            if (_grades.Count == 0) return 0;

            var allGrades = _grades.Values.SelectMany(g => g).ToList();   // Преобразует несколько "паралельных" коллекций(массивов) в один ( Пример Select - [[1,2,3],[4,5,6]]  |  SelectMany - [1,2,3,4,5,6] )
            return allGrades.Average(); // Average аналог среднего арифм.
        }

        public Dictionary<string, List<double>> GetGrades() =>
            new Dictionary<string, List<double>>(_grades);

        public override void DisplayInfo()
        {
            Console.WriteLine($"Студент: {Name} (ID: {Id})");
            Console.WriteLine($"Возраст: {Age}, Контакты: {ContactInfo}");
            Console.WriteLine($"Средний балл: {CalculateAverageGrade():F2}");
            Console.WriteLine($"Курсов: {_courses.Count}");
        }
    }


    public class Teacher : Person  //Наследование др класса
    {
        private List<Course> _courses;

        public string Specialization { get; set; }

        public Teacher(string id, string name, int age, string contactInfo, string specialization)
            : base(id, name, age, contactInfo)
        {
            Specialization = specialization;
            _courses = new List<Course>();
        }

        public IReadOnlyList<Course> Courses => _courses.AsReadOnly(); //Курсы, только для чтения

        public void AssignToCourse(Course course)
        {
            if (!_courses.Contains(course))
            {
                _courses.Add(course);
                course.AssignTeacher(this);
            }
        }


        public override void DisplayInfo()
        {
            Console.WriteLine($"Преподаватель: {Name} (ID: {Id})");
            Console.WriteLine($"Возраст: {Age}, Специализация: {Specialization}");
            Console.WriteLine($"Контакты: {ContactInfo}");
            Console.WriteLine($"Курсов: {_courses.Count}");
        }
    }

    public interface IGradeable     // Интерфейс - служит только для обьявление функций и переменных обязательных для существования и перезаписи в других классах
    {
        void AddGrade(string courseId, double grade);
        double CalculateAverageGrade();
        Dictionary<string, List<double>> GetGrades();
    }


    public class Course
    {
        // Приватные поля
        private string _name;
        private string _description;
        private List<Student> _students;
        private int _maxStudents; // для записи максимального кол-ва мест

        // Публичные свойства
        public string CourseId { get; private set; }
        public Teacher Teacher { get; private set; }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Название курса не может быть пустым");
                _name = value;
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Описание курса не может быть пустым");
                _description = value;
            }
        }

        // Свойство с ограничением максимального количества студентов
        public int MaxStudents
        {
            get => _maxStudents;
            set
            {
                if (value < 1)
                    throw new ArgumentException("Максимальное количество студентов должно быть положительным");
                _maxStudents = value;
            }
        }


        public int CurrentStudentsCount => _students.Count;
        public bool HasFreePlaces => _students.Count < _maxStudents;

        public Course(string courseId, string name, string description, int maxStudents = 30)
        {
            CourseId = courseId;
            Name = name;
            Description = description;
            MaxStudents = maxStudents;
            _students = new List<Student>();
        }


        public void AssignTeacher(Teacher teacher)
        {
            Teacher = teacher;
        }

        public void AddStudent(Student student)
        {
            if (_students.Count >= MaxStudents)
                throw new InvalidOperationException("Курс заполнен. Невозможно добавить больше студентов");

            if (!_students.Contains(student))
                _students.Add(student);
        }

        public IReadOnlyList<Student> GetStudents() => _students.AsReadOnly();

        public void DisplayInfo()
        {
            Console.WriteLine($"Курс: {Name} (ID: {CourseId})");
            Console.WriteLine($"Описание: {Description}");
            Console.WriteLine($"Преподаватель: {Teacher?.Name ?? "Не назначен"}");
            Console.WriteLine($"Студентов: {_students.Count}/{MaxStudents}");
            Console.WriteLine($"Свободных мест: {MaxStudents - _students.Count}");
        }


        public class UniversitySystem
        {
            private List<Student> _students;
            private List<Teacher> _teachers;
            private List<Course> _courses;

            public UniversitySystem()
            {
                _students = new List<Student>();
                _teachers = new List<Teacher>();
                _courses = new List<Course>();
            }


            public void AddStudent(string name, int age, string contactInfo)
            {
                var id = $"S{_students.Count + 1:000}";
                var student = new Student(id, name, age, contactInfo);
                _students.Add(student);
                Console.WriteLine($"Студент {name} добавлен с ID: {id}");
            }

            public Student GetStudent(string id) =>
                _students.FirstOrDefault(s => s.Id == id);

            public List<Student> GetAllStudents() => new List<Student>(_students);


            public void AddTeacher(string name, int age, string contactInfo, string specialization)
            {
                var id = $"T{_teachers.Count + 1:000}";
                var teacher = new Teacher(id, name, age, contactInfo, specialization);
                _teachers.Add(teacher);
                Console.WriteLine($"Преподаватель {name} добавлен с ID: {id}");
            }

            public Teacher GetTeacher(string id) =>
                _teachers.FirstOrDefault(t => t.Id == id);

            public List<Teacher> GetAllTeachers() => new List<Teacher>(_teachers);

            public void AddCourse(string name, string description, int maxStudents = 30)
            {
                var id = $"C{_courses.Count + 1:000}";
                var course = new Course(id, name, description, maxStudents);
                _courses.Add(course);
                Console.WriteLine($"Курс {name} создан с ID: {id}");
            }

            public Course GetCourse(string id) =>
                _courses.FirstOrDefault(c => c.CourseId == id);

            public List<Course> GetAllCourses() => new List<Course>(_courses);


            public void EnrollStudentInCourse(string studentId, string courseId)
            {
                var student = GetStudent(studentId);
                var course = GetCourse(courseId);

                if (student == null) throw new ArgumentException("Студент не найден");
                if (course == null) throw new ArgumentException("Курс не найден");

                student.EnrollInCourse(course);
                Console.WriteLine($"Студент {student.Name} записан на курс {course.Name}");
            }

            public void AssignTeacherToCourse(string teacherId, string courseId)
            {
                var teacher = GetTeacher(teacherId);
                var course = GetCourse(courseId);

                if (teacher == null) throw new ArgumentException("Преподаватель не найден");
                if (course == null) throw new ArgumentException("Курс не найден");

                teacher.AssignToCourse(course);
                Console.WriteLine($"Преподаватель {teacher.Name} назначен на курс {course.Name}");
            }


            public void AddGradeToStudent(string studentId, string courseId, double grade)
            {
                var student = GetStudent(studentId);
                if (student == null) throw new ArgumentException("Студент не найден");

                student.AddGrade(courseId, grade);
                Console.WriteLine($"Оценка {grade} добавлена студенту {student.Name} за курс {courseId}");
            }
        }


        class Program
        {
            static void Main(string[] args)
            {



            }
        }
    }
}
