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

        //Класс для управления консольным меню
        public class ConsoleMenu
        {
            private UniversitySystem _university;

            public ConsoleMenu(UniversitySystem university)
            {
                _university = university;
            }

            public void ShowMainMenu()
            {
                while (true)
                {
                    Console.WriteLine("\n=== СИСТЕМА УПРАВЛЕНИЯ УНИВЕРСИТЕТОМ ===");
                    Console.WriteLine("1. Управление студентами");
                    Console.WriteLine("2. Управление преподавателями");
                    Console.WriteLine("3. Управление курсами");
                    Console.WriteLine("4. Просмотр информации");
                    Console.WriteLine("5. Операции");
                    Console.WriteLine("0. Выход");
                    Console.Write("Выберите пункт меню: ");

                    var choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1": ShowStudentMenu(); break;
                        case "2": ShowTeacherMenu(); break;
                        case "3": ShowCourseMenu(); break;
                        case "4": ShowInfoMenu(); break;
                        case "5": ShowOperationsMenu(); break;
                        case "0": return;
                        default: Console.WriteLine("Неверный выбор!"); break;
                    }
                }
            }

            //Меню управления студентами
            private void ShowStudentMenu()
            {
                while (true)
                {
                    Console.WriteLine("\n--- Управление студентами ---");
                    Console.WriteLine("1. Добавить студента");
                    Console.WriteLine("2. Просмотреть всех студентов");
                    Console.WriteLine("3. Найти студента по ID");
                    Console.WriteLine("4. Записать студента на курс");
                    Console.WriteLine("5. Выставить оценку");
                    Console.WriteLine("0. Назад");
                    Console.Write("Выберите пункт: ");

                    var choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            try
                            {
                                Console.Write("Имя: ");
                                var name = Console.ReadLine();
                                Console.Write("Возраст: ");
                                var age = int.Parse(Console.ReadLine());
                                Console.Write("Контакты: ");
                                var contacts = Console.ReadLine();
                                _university.AddStudent(name, age, contacts);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка: {ex.Message}");
                            }
                            break;

                        case "2":
                            var students = _university.GetAllStudents();
                            Console.WriteLine("\n--- Все студенты ---");
                            foreach (var studdent in students)
                            {
                                studdent.DisplayInfo();
                                Console.WriteLine("---");
                            }
                            break;

                        case "3":
                            Console.Write("Введите ID студента: ");
                            var studentId = Console.ReadLine();
                            var student = _university.GetStudent(studentId);
                            if (student != null)
                                student.DisplayInfo();
                            else
                                Console.WriteLine("Студент не найден");
                            break;

                        case "4":
                            try
                            {
                                Console.Write("ID студента: ");
                                var sId = Console.ReadLine();
                                Console.Write("ID курса: ");
                                var cId = Console.ReadLine();
                                _university.EnrollStudentInCourse(sId, cId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка: {ex.Message}");
                            }
                            break;

                        case "5":
                            try
                            {
                                Console.Write("ID студента: ");
                                var studId = Console.ReadLine();
                                Console.Write("ID курса: ");
                                var courseId = Console.ReadLine();
                                Console.Write("Оценка (0-100): ");
                                var grade = double.Parse(Console.ReadLine());
                                _university.AddGradeToStudent(studId, courseId, grade);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка: {ex.Message}");
                            }
                            break;

                        case "0": return;
                        default: Console.WriteLine("Неверный выбор!"); break;
                    }
                }
            }


            private void ShowTeacherMenu()
            {
                while (true)
                {
                    Console.WriteLine("\n--- Управление преподавателями ---");
                    Console.WriteLine("1. Добавить преподавателя");
                    Console.WriteLine("2. Просмотреть всех преподавателей");
                    Console.WriteLine("3. Назначить преподавателя на курс");
                    Console.WriteLine("0. Назад");
                    Console.Write("Выберите пункт: ");

                    var choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            try
                            {
                                Console.Write("Имя: ");
                                var name = Console.ReadLine();
                                Console.Write("Возраст: ");
                                var age = int.Parse(Console.ReadLine());
                                Console.Write("Контакты: ");
                                var contacts = Console.ReadLine();
                                Console.Write("Специализация: ");
                                var spec = Console.ReadLine();
                                _university.AddTeacher(name, age, contacts, spec);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка: {ex.Message}");
                            }
                            break;

                        case "2":
                            var teachers = _university.GetAllTeachers();
                            Console.WriteLine("\n--- Все преподаватели ---");
                            foreach (var teacher in teachers)
                            {
                                teacher.DisplayInfo();
                                Console.WriteLine("---");
                            }
                            break;

                        case "3":
                            try
                            {
                                Console.Write("ID преподавателя: ");
                                var tId = Console.ReadLine();
                                Console.Write("ID курса: ");
                                var cId = Console.ReadLine();
                                _university.AssignTeacherToCourse(tId, cId);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка: {ex.Message}");
                            }
                            break;

                        case "0": return;
                        default: Console.WriteLine("Неверный выбор!"); break;
                    }
                }
            }

            private void ShowCourseMenu()
            {
                while (true)
                {
                    Console.WriteLine("\n--- Управление курсами ---");
                    Console.WriteLine("1. Создать курс");
                    Console.WriteLine("2. Просмотреть все курсы");
                    Console.WriteLine("3. Просмотреть детали курса");
                    Console.WriteLine("0. Назад");
                    Console.Write("Выберите пункт: ");

                    var choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            try
                            {
                                Console.Write("Название курса: ");
                                var name = Console.ReadLine();
                                Console.Write("Описание: ");
                                var desc = Console.ReadLine();
                                Console.Write("Макс. студентов (по умолчанию 30): ");
                                var max = Console.ReadLine();
                                if (string.IsNullOrEmpty(max))
                                    _university.AddCourse(name, desc);
                                else
                                    _university.AddCourse(name, desc, int.Parse(max));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка: {ex.Message}");
                            }
                            break;

                        case "2":
                            var courses = _university.GetAllCourses();
                            Console.WriteLine("\n--- Все курсы ---");
                            foreach (var coursse in courses)
                            {
                                coursse.DisplayInfo();
                                Console.WriteLine("---");
                            }
                            break;

                        case "3":
                            Console.Write("Введите ID курса: ");
                            var courseId = Console.ReadLine();
                            var course = _university.GetCourse(courseId);
                            if (course != null)
                            {
                                course.DisplayInfo();
                                Console.WriteLine("\nСтуденты на курсе:");
                                foreach (var student in course.GetStudents())
                                {
                                    Console.WriteLine($"- {student.Name} (ID: {student.Id})");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Курс не найден");
                            }
                            break;

                        case "0": return;
                        default: Console.WriteLine("Неверный выбор!"); break;
                    }
                }
            }

            // Меню просмотра информации
            private void ShowInfoMenu()
            {
                Console.WriteLine("\n--- Просмотр информации ---");
                Console.WriteLine("1. Все студенты");
                Console.WriteLine("2. Все преподаватели");
                Console.WriteLine("3. Все курсы");
                Console.WriteLine("4. Статистика университета");
                Console.Write("Выберите пункт: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        var students = _university.GetAllStudents();
                        Console.WriteLine($"\nВсего студентов: {students.Count}");
                        foreach (var student in students)
                        {
                            student.DisplayInfo();
                            Console.WriteLine("Курсы студента:");
                            foreach (var course in student.Courses)
                            {
                                Console.WriteLine($"- {course.Name}");
                            }
                            Console.WriteLine("---");
                        }
                        break;

                    case "2":
                        var teachers = _university.GetAllTeachers();
                        Console.WriteLine($"\nВсего преподавателей: {teachers.Count}");
                        foreach (var teacher in teachers)
                        {
                            teacher.DisplayInfo();
                            Console.WriteLine("Курсы преподавателя:");
                            foreach (var course in teacher.Courses)
                            {
                                Console.WriteLine($"- {course.Name}");
                            }
                            Console.WriteLine("---");
                        }
                        break;

                    case "3":
                        var courses = _university.GetAllCourses();
                        Console.WriteLine($"\nВсего курсов: {courses.Count}");
                        foreach (var course in courses)
                        {
                            course.DisplayInfo();
                            Console.WriteLine("---");
                        }
                        break;

                    case "4":
                        Console.WriteLine("\n=== СТАТИСТИКА УНИВЕРСИТЕТА ===");
                        Console.WriteLine($"Студентов: {_university.GetAllStudents().Count}");
                        Console.WriteLine($"Преподавателей: {_university.GetAllTeachers().Count}");
                        Console.WriteLine($"Курсов: {_university.GetAllCourses().Count}");

                        var allStudents = _university.GetAllStudents();
                        if (allStudents.Count > 0)
                        {
                            var avgGrade = allStudents.Average(s => s.CalculateAverageGrade());
                            Console.WriteLine($"Средний балл по университету: {avgGrade:F2}");
                        }
                        break;
                }
            }

            //Меню операций
            private void ShowOperationsMenu()
            {
                Console.WriteLine("\n--- Операции ---");
                Console.WriteLine("1. Записать студента на курс");
                Console.WriteLine("2. Назначить преподавателя на курс");
                Console.WriteLine("3. Выставить оценку студенту");
                Console.Write("Выберите пункт: ");

                var choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.Write("ID студента: ");
                            var sId = Console.ReadLine();
                            Console.Write("ID курса: ");
                            var cId = Console.ReadLine();
                            _university.EnrollStudentInCourse(sId, cId);
                            break;

                        case "2":
                            Console.Write("ID преподавателя: ");
                            var tId = Console.ReadLine();
                            Console.Write("ID курса: ");
                            var courseId = Console.ReadLine();
                            _university.AssignTeacherToCourse(tId, courseId);
                            break;

                        case "3":
                            Console.Write("ID студента: ");
                            var studId = Console.ReadLine();
                            Console.Write("ID курса: ");
                            var crsId = Console.ReadLine();
                            Console.Write("Оценка (0-100): ");
                            var grade = double.Parse(Console.ReadLine());
                            _university.AddGradeToStudent(studId, crsId, grade);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }



        class Program
        {
            static void Main(string[] args)
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;


                var university = new UniversitySystem();
                var menu = new ConsoleMenu(university);

                InitializeTestData(university);

                Console.WriteLine("Добро пожаловать в систему управления университетом!");
                menu.ShowMainMenu();

                Console.WriteLine("Программа завершена. До свидания!");
            }

            static void InitializeTestData(UniversitySystem university)
            {

                university.AddStudent("Иван Петров", 20, "ivan@mail.ru");
                university.AddStudent("Мария Сидорова", 19, "maria@mail.ru");
                university.AddStudent("Алексей Козлов", 21, "alex@mail.ru");

                university.AddTeacher("Дмитрий Орлов", 45, "orlov@university.ru", "Математика");
                university.AddTeacher("Елена Васнецова", 38, "vasnecova@university.ru", "Программирование");

                university.AddCourse("Высшая математика", "Основы высшей математики", 25);
                university.AddCourse("C# программирование", "Изучение языка C# и .NET", 20);
                university.AddCourse("Базы данных", "Основы проектирования и работы с БД", 15);

                try
                {
                    university.AssignTeacherToCourse("T001", "C001"); // Орлов -> Высшая математика
                    university.AssignTeacherToCourse("T002", "C002"); // Васнецова -> C# программирование
                    university.AssignTeacherToCourse("T002", "C003"); // Васнецова -> Базы данных

                    university.EnrollStudentInCourse("S001", "C001");
                    university.EnrollStudentInCourse("S001", "C002");
                    university.EnrollStudentInCourse("S002", "C002");
                    university.EnrollStudentInCourse("S003", "C001");
                    university.EnrollStudentInCourse("S003", "C003");


                    university.AddGradeToStudent("S001", "C001", 85);
                    university.AddGradeToStudent("S001", "C002", 92);
                    university.AddGradeToStudent("S002", "C002", 78);
                    university.AddGradeToStudent("S003", "C001", 88);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при инициализации тестовых данных: {ex.Message}");
                }
            }
        }
    }
}
