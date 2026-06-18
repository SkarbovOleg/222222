using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace QuizApp
{
    // Модели данных
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public List<QuizResult> Results { get; set; } = new List<QuizResult>();
    }

    public class QuizResult
    {
        public string Category { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime Date { get; set; }
        public int Rank { get; set; }
    }

    public class Question
    {
        public string Text { get; set; }
        public List<string> CorrectAnswers { get; set; } = new List<string>();
        public List<string> Options { get; set; } = new List<string>();
    }

    public class Quiz
    {
        public string Category { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }

    // Менеджер пользователей
    public class UserManager
    {
        private List<User> users = new List<User>();
        private string dataPath = "users.json";
        private User currentUser = null;

        public User CurrentUser
        {
            get { return currentUser; }
            private set { currentUser = value; }
        }

        public UserManager()
        {
            LoadUsers();
        }

        public bool Register(string login, string password, DateTime birthDate)
        {
            if (users.Any(u => u.Login == login))
            {
                return false;
            }
            var user = new User
            {
                Login = login,
                Password = password,
                BirthDate = birthDate
            };
            users.Add(user);
            SaveUsers();
            return true;
        }

        public bool Login(string login, string password)
        {
            var user = users.FirstOrDefault(u => u.Login == login && u.Password == password);
            if (user != null)
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public bool ChangePassword(string oldPassword, string newPassword)
        {
            if (CurrentUser == null || CurrentUser.Password != oldPassword)
                return false;
            CurrentUser.Password = newPassword;
            SaveUsers();
            return true;
        }

        public bool ChangeBirthDate(DateTime newDate)
        {
            if (CurrentUser == null)
                return false;
            CurrentUser.BirthDate = newDate;
            SaveUsers();
            return true;
        }

        public void AddResult(QuizResult result)
        {
            if (CurrentUser != null)
            {
                CurrentUser.Results.Add(result);
                SaveUsers();
            }
        }

        public List<QuizResult> GetUserResults()
        {
            return CurrentUser?.Results ?? new List<QuizResult>();
        }

        private void SaveUsers()
        {
            try
            {
                string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dataPath, json);
            }
            catch { }
        }

        private void LoadUsers()
        {
            try
            {
                if (File.Exists(dataPath))
                {
                    string json = File.ReadAllText(dataPath);
                    users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                }
            }
            catch
            {
                users = new List<User>();
            }
        }
    }

    // Менеджер викторин
    public class QuizManager
    {
        private List<Quiz> quizzes = new List<Quiz>();
        private string dataPath = "quizzes.json";

        public QuizManager()
        {
            LoadQuizzes();
            if (quizzes.Count == 0)
            {
                InitializeSampleQuizzes();
            }
        }

        private void InitializeSampleQuizzes()
        {
            // Викторина по истории
            var historyQuiz = new Quiz
            {
                Category = "История",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "В каком году началась Вторая мировая война?",
                        Options = new List<string> { "1939", "1940", "1941", "1945" },
                        CorrectAnswers = new List<string> { "1939" }
                    },
                    new Question
                    {
                        Text = "Кто был первым президентом США?",
                        Options = new List<string> { "Джордж Вашингтон", "Томас Джефферсон", "Авраам Линкольн", "Бенджамин Франклин" },
                        CorrectAnswers = new List<string> { "Джордж Вашингтон" }
                    },
                    new Question
                    {
                        Text = "Какие из этих стран были союзниками во Второй мировой войне?",
                        Options = new List<string> { "СССР", "США", "Германия", "Великобритания" },
                        CorrectAnswers = new List<string> { "СССР", "США", "Великобритания" }
                    },
                    new Question
                    {
                        Text = "В каком году была отменена крепостное право в России?",
                        Options = new List<string> { "1861", "1865", "1871", "1881" },
                        CorrectAnswers = new List<string> { "1861" }
                    }
                }
            };

            // Викторина по географии
            var geographyQuiz = new Quiz
            {
                Category = "География",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Какая река самая длинная в мире?",
                        Options = new List<string> { "Нил", "Амазонка", "Миссисипи", "Янцзы" },
                        CorrectAnswers = new List<string> { "Нил" }
                    },
                    new Question
                    {
                        Text = "Сколько материков на Земле?",
                        Options = new List<string> { "5", "6", "7", "8" },
                        CorrectAnswers = new List<string> { "6" }
                    },
                    new Question
                    {
                        Text = "Какая страна является самой большой по площади?",
                        Options = new List<string> { "Россия", "Канада", "Китай", "США" },
                        CorrectAnswers = new List<string> { "Россия" }
                    }
                }
            };

            // Викторина по биологии
            var biologyQuiz = new Quiz
            {
                Category = "Биология",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Какие из этих органов относятся к дыхательной системе?",
                        Options = new List<string> { "Легкие", "Сердце", "Трахея", "Печень" },
                        CorrectAnswers = new List<string> { "Легкие", "Трахея" }
                    },
                    new Question
                    {
                        Text = "Сколько хромосом у человека?",
                        Options = new List<string> { "44", "46", "48", "50" },
                        CorrectAnswers = new List<string> { "46" }
                    }
                }
            };

            quizzes.Add(historyQuiz);
            quizzes.Add(geographyQuiz);
            quizzes.Add(biologyQuiz);
            SaveQuizzes();
        }

        public List<string> GetCategories()
        {
            return quizzes.Select(q => q.Category).ToList();
        }

        public Quiz GetQuizByCategory(string category)
        {
            return quizzes.FirstOrDefault(q => q.Category == category);
        }

        public Quiz GetMixedQuiz()
        {
            var mixed = new Quiz { Category = "Смешанная" };
            var allQuestions = new List<Question>();
            var random = new Random();

            foreach (var quiz in quizzes)
            {
                foreach (var question in quiz.Questions)
                {
                    allQuestions.Add(question);
                }
            }

            var shuffled = allQuestions.OrderBy(x => random.Next()).Take(20).ToList();
            mixed.Questions = shuffled;
            return mixed;
        }

        public int CalculateScore(Quiz quiz, List<int> selectedAnswers)
        {
            int score = 0;
            for (int i = 0; i < quiz.Questions.Count && i < selectedAnswers.Count; i++)
            {
                var question = quiz.Questions[i];
                int selectedIndex = selectedAnswers[i];

                if (selectedIndex >= 0 && selectedIndex < question.Options.Count)
                {
                    string selectedOption = question.Options[selectedIndex];
                    if (question.CorrectAnswers.Contains(selectedOption))
                    {
                        score++;
                    }
                }
            }
            return score;
        }

        public int CalculateRank(string category, int score, int totalQuestions)
        {
            int percentage = (score * 100) / totalQuestions;
            if (percentage >= 90) return 1;
            if (percentage >= 80) return 5;
            if (percentage >= 70) return 10;
            if (percentage >= 60) return 15;
            return 20;
        }

        public void AddQuiz(Quiz quiz)
        {
            quizzes.Add(quiz);
            SaveQuizzes();
        }

        public bool DeleteQuiz(string category)
        {
            var quiz = quizzes.FirstOrDefault(q => q.Category == category);
            if (quiz != null)
            {
                quizzes.Remove(quiz);
                SaveQuizzes();
                return true;
            }
            return false;
        }

        public List<Quiz> GetAllQuizzes()
        {
            return quizzes;
        }

        public void SaveQuizzes()
        {
            try
            {
                string json = JsonSerializer.Serialize(quizzes, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dataPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения викторин: {ex.Message}");
            }
        }

        private void LoadQuizzes()
        {
            try
            {
                if (File.Exists(dataPath))
                {
                    string json = File.ReadAllText(dataPath);
                    quizzes = JsonSerializer.Deserialize<List<Quiz>>(json) ?? new List<Quiz>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки викторин: {ex.Message}");
                quizzes = new List<Quiz>();
            }
        }
    }

    // Административное приложение
    public class AdminApp
    {
        private UserManager userManager;
        private QuizManager quizManager;
        private const string AdminLogin = "admin";
        private const string AdminPassword = "admin123";

        public AdminApp(UserManager userManager, QuizManager quizManager)
        {
            this.userManager = userManager;
            this.quizManager = quizManager;
        }

        public void Run()
        {
            Console.Clear();
            Console.WriteLine("=== АДМИНИСТРАТИВНОЕ ПРИЛОЖЕНИЕ ===");
            Console.Write("Логин: ");
            string login = Console.ReadLine();
            Console.Write("Пароль: ");
            string password = Console.ReadLine();

            if (login != AdminLogin || password != AdminPassword)
            {
                Console.WriteLine("Неверный логин или пароль!");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
                return;
            }

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== АДМИНИСТРИРОВАНИЕ ВИКТОРИН ===");
                Console.WriteLine("1. Создать новую викторину");
                Console.WriteLine("2. Добавить вопрос в викторину");
                Console.WriteLine("3. Редактировать вопрос");
                Console.WriteLine("4. Удалить викторину");
                Console.WriteLine("5. Просмотреть все викторины");
                Console.WriteLine("6. Выход");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": CreateQuiz(); break;
                    case "2": AddQuestion(); break;
                    case "3": EditQuestion(); break;
                    case "4": DeleteQuiz(); break;
                    case "5": ListQuizzes(); break;
                    case "6": exit = true; break;
                    default: Console.WriteLine("Неверный выбор!"); break;
                }
            }
        }

        private void CreateQuiz()
        {
            Console.Clear();
            Console.Write("Введите название категории викторины: ");
            string category = Console.ReadLine();

            if (quizManager.GetQuizByCategory(category) != null)
            {
                Console.WriteLine("Викторина с таким названием уже существует!");
                Console.ReadKey();
                return;
            }

            var quiz = new Quiz { Category = category };

            Console.Write("Сколько вопросов добавить? ");
            if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    AddQuestionToQuiz(quiz, i + 1);
                }
                quizManager.AddQuiz(quiz);
                Console.WriteLine($"Викторина '{category}' создана успешно с {count} вопросами!");
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private void AddQuestion()
        {
            Console.Clear();
            ListQuizzes();
            Console.Write("Выберите викторину (введите название категории): ");
            string category = Console.ReadLine();
            var quiz = quizManager.GetQuizByCategory(category);
            if (quiz == null)
            {
                Console.WriteLine("Викторина не найдена!");
                Console.ReadKey();
                return;
            }
            AddQuestionToQuiz(quiz, quiz.Questions.Count + 1);
            quizManager.SaveQuizzes();
            Console.WriteLine("Вопрос добавлен успешно!");
            Console.ReadKey();
        }

        private void AddQuestionToQuiz(Quiz quiz, int number)
        {
            Console.WriteLine($"\nВопрос #{number}");
            Console.Write("Введите текст вопроса: ");
            string text = Console.ReadLine();
            var question = new Question { Text = text };

            Console.Write("Сколько вариантов ответа? ");
            if (int.TryParse(Console.ReadLine(), out int optionCount) && optionCount > 0)
            {
                for (int i = 0; i < optionCount; i++)
                {
                    Console.Write($"Вариант {i + 1}: ");
                    string option = Console.ReadLine();
                    if (!string.IsNullOrEmpty(option))
                    {
                        question.Options.Add(option);
                    }
                }
            }

            Console.Write("Сколько правильных ответов? ");
            if (int.TryParse(Console.ReadLine(), out int correctCount) && correctCount > 0)
            {
                for (int i = 0; i < correctCount; i++)
                {
                    Console.Write($"Правильный ответ {i + 1}: ");
                    string answer = Console.ReadLine();
                    if (!question.Options.Contains(answer))
                    {
                        Console.WriteLine("Ошибка: такого варианта нет! Попробуйте снова.");
                        i--;
                        continue;
                    }
                    question.CorrectAnswers.Add(answer);
                }
            }
            quiz.Questions.Add(question);
        }

        private void EditQuestion()
        {
            Console.Clear();
            ListQuizzes();
            Console.Write("Выберите викторину (введите название): ");
            string category = Console.ReadLine();
            var quiz = quizManager.GetQuizByCategory(category);
            if (quiz == null)
            {
                Console.WriteLine("Викторина не найдена!");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {quiz.Questions[i].Text}");
            }
            Console.Write("Выберите номер вопроса для редактирования: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= quiz.Questions.Count)
            {
                var question = quiz.Questions[index - 1];
                Console.Write($"Новый текст вопроса (было: {question.Text}): ");
                string newText = Console.ReadLine();
                if (!string.IsNullOrEmpty(newText))
                    question.Text = newText;

                Console.WriteLine("Редактирование ответов:");
                for (int i = 0; i < question.Options.Count; i++)
                {
                    Console.Write($"Вариант {i + 1} (было: {question.Options[i]}): ");
                    string newOption = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newOption))
                        question.Options[i] = newOption;
                }

                quizManager.SaveQuizzes();
                Console.WriteLine("Вопрос обновлен!");
            }
            Console.ReadKey();
        }

        private void DeleteQuiz()
        {
            Console.Clear();
            ListQuizzes();
            Console.Write("Выберите викторину для удаления (введите название): ");
            string category = Console.ReadLine();

            if (quizManager.DeleteQuiz(category))
            {
                Console.WriteLine("Викторина удалена успешно!");
            }
            else
            {
                Console.WriteLine("Викторина не найдена!");
            }
            Console.ReadKey();
        }

        private void ListQuizzes()
        {
            Console.WriteLine("\n=== ВСЕ ВИКТОРИНЫ ===");
            var quizzes = quizManager.GetAllQuizzes();
            if (quizzes.Count == 0)
            {
                Console.WriteLine("Нет доступных викторин.");
                return;
            }
            foreach (var quiz in quizzes)
            {
                Console.WriteLine($"- {quiz.Category} ({quiz.Questions.Count} вопросов)");
            }
        }
    }

    // Главный класс программы
    class Program
    {
        static UserManager userManager = new UserManager();
        static QuizManager quizManager = new QuizManager();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool exit = false;
            while (!exit)
            {
                if (userManager.CurrentUser == null)
                {
                    ShowAuthMenu();
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1": Login(); break;
                        case "2": Register(); break;
                        case "3":
                            var adminApp = new AdminApp(userManager, quizManager);
                            adminApp.Run();
                            break;
                        case "4": exit = true; break;
                        default: Console.WriteLine("Неверный выбор!"); break;
                    }
                }
                else
                {
                    ShowMainMenu();
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1": StartQuiz(); break;
                        case "2": ShowResults(); break;
                        case "3": ShowTop20(); break;
                        case "4": Settings(); break;
                        case "5":
                            userManager.Logout();
                            Console.WriteLine("Вы вышли из системы!");
                            break;
                        case "6": exit = true; break;
                        default: Console.WriteLine("Неверный выбор!"); break;
                    }
                }
            }
        }

        static void ShowAuthMenu()
        {
            Console.Clear();
            Console.WriteLine("=== ВИКТОРИНА ===");
            Console.WriteLine("1. Вход");
            Console.WriteLine("2. Регистрация");
            Console.WriteLine("3. Администрирование");
            Console.WriteLine("4. Выход");
            Console.Write("Выберите действие: ");
        }

        static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine($"=== ДОБРО ПОЖАЛОВАТЬ, {userManager.CurrentUser.Login}! ===");
            Console.WriteLine("1. Начать новую викторину");
            Console.WriteLine("2. Мои результаты");
            Console.WriteLine("3. Топ-20");
            Console.WriteLine("4. Настройки");
            Console.WriteLine("5. Выход из системы");
            Console.WriteLine("6. Выход из программы");
            Console.Write("Выберите действие: ");
        }

        static void Login()
        {
            Console.Clear();
            Console.WriteLine("=== ВХОД ===");
            Console.Write("Логин: ");
            string login = Console.ReadLine();
            Console.Write("Пароль: ");
            string password = Console.ReadLine();

            if (userManager.Login(login, password))
            {
                Console.WriteLine("Вход выполнен успешно!");
            }
            else
            {
                Console.WriteLine("Неверный логин или пароль!");
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void Register()
        {
            Console.Clear();
            Console.WriteLine("=== РЕГИСТРАЦИЯ ===");
            Console.Write("Логин: ");
            string login = Console.ReadLine();
            Console.Write("Пароль: ");
            string password = Console.ReadLine();
            Console.Write("Дата рождения (гггг-мм-дд): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime birthDate))
            {
                if (userManager.Register(login, password, birthDate))
                {
                    Console.WriteLine("Регистрация успешна!");
                }
                else
                {
                    Console.WriteLine("Такой логин уже существует!");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат даты!");
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void StartQuiz()
        {
            Console.Clear();
            Console.WriteLine("=== ВЫБОР ВИКТОРИНЫ ===");
            var categories = quizManager.GetCategories();

            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных викторин!");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }
            Console.WriteLine($"{categories.Count + 1}. Смешанная");
            Console.Write("Выберите викторину (номер): ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0)
            {
                Quiz quiz = null;
                if (choice <= categories.Count)
                {
                    quiz = quizManager.GetQuizByCategory(categories[choice - 1]);
                }
                else if (choice == categories.Count + 1)
                {
                    quiz = quizManager.GetMixedQuiz();
                }

                if (quiz != null && quiz.Questions.Count > 0)
                {
                    RunQuiz(quiz);
                }
                else
                {
                    Console.WriteLine("Викторина не найдена или пуста!");
                }
            }
            else
            {
                Console.WriteLine("Неверный выбор!");
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void RunQuiz(Quiz quiz)
        {
            Console.Clear();
            Console.WriteLine($"=== ВИКТОРИНА: {quiz.Category} ===");
            int questionCount = Math.Min(20, quiz.Questions.Count);
            Console.WriteLine($"Всего вопросов: {questionCount}");
            Console.WriteLine("Нажмите любую клавишу для начала...");
            Console.ReadKey();

            var userAnswers = new List<int>();
            for (int i = 0; i < questionCount; i++)
            {
                var question = quiz.Questions[i];
                Console.Clear();
                Console.WriteLine($"Вопрос {i + 1} из {questionCount}");
                Console.WriteLine($"Категория: {quiz.Category}");
                Console.WriteLine($"\n{question.Text}");
                Console.WriteLine("\nВарианты ответов:");
                for (int j = 0; j < question.Options.Count; j++)
                {
                    Console.WriteLine($"{j + 1}. {question.Options[j]}");
                }
                Console.Write("\nВведите номер правильного ответа: ");
                if (int.TryParse(Console.ReadLine(), out int answer) && answer > 0 && answer <= question.Options.Count)
                {
                    userAnswers.Add(answer - 1);
                }
                else
                {
                    Console.WriteLine("Неверный ответ! Пропускаем вопрос.");
                    userAnswers.Add(-1);
                    Console.ReadKey();
                }
            }

            int score = quizManager.CalculateScore(quiz, userAnswers);
            int rank = quizManager.CalculateRank(quiz.Category, score, questionCount);

            Console.Clear();
            Console.WriteLine("=== РЕЗУЛЬТАТЫ ===");
            Console.WriteLine($"Правильных ответов: {score} из {questionCount}");
            Console.WriteLine($"Процент правильных ответов: {(score * 100) / questionCount}%");
            Console.WriteLine($"Ваше место: {rank}");

            var result = new QuizResult
            {
                Category = quiz.Category,
                Score = score,
                TotalQuestions = questionCount,
                Date = DateTime.Now,
                Rank = rank
            };
            userManager.AddResult(result);

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void ShowResults()
        {
            Console.Clear();
            var results = userManager.GetUserResults();
            if (results.Count == 0)
            {
                Console.WriteLine("У вас пока нет результатов.");
            }
            else
            {
                Console.WriteLine("=== МОИ РЕЗУЛЬТАТЫ ===");
                int count = 1;
                foreach (var r in results.OrderByDescending(r => r.Date))
                {
                    Console.WriteLine($"{count}. Дата: {r.Date.ToShortDateString()}");
                    Console.WriteLine($"   Викторина: {r.Category}");
                    Console.WriteLine($"   Результат: {r.Score}/{r.TotalQuestions} ({(r.Score * 100) / r.TotalQuestions}%)");
                    Console.WriteLine($"   Место: {r.Rank}");
                    Console.WriteLine("-------------------------");
                    count++;
                }
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void ShowTop20()
        {
            Console.Clear();
            Console.WriteLine("=== ТОП-20 ===");
            var categories = quizManager.GetCategories();

            if (categories.Count == 0)
            {
                Console.WriteLine("Нет доступных викторин!");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }
            Console.Write("Выберите викторину для просмотра топа (номер): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= categories.Count)
            {
                string category = categories[choice - 1];
                Console.WriteLine($"\nТоп-20 по викторине '{category}':");

                Console.WriteLine("1. Игрок1 - 20/20 (100%)");
                Console.WriteLine("2. Игрок2 - 19/20 (95%)");
                Console.WriteLine("3. Игрок3 - 18/20 (90%)");
                Console.WriteLine("4. Игрок4 - 17/20 (85%)");
                Console.WriteLine("5. Игрок5 - 16/20 (80%)");
                Console.WriteLine("...");
                Console.WriteLine("\n(В полной версии здесь отображалась бы таблица из БД)");
            }
            else
            {
                Console.WriteLine("Неверный выбор!");
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void Settings()
        {
            Console.Clear();
            Console.WriteLine("=== НАСТРОЙКИ ===");
            Console.WriteLine("1. Сменить пароль");
            Console.WriteLine("2. Сменить дату рождения");
            Console.WriteLine("3. Назад");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Console.Write("Старый пароль: ");
                    string oldPass = Console.ReadLine();
                    Console.Write("Новый пароль: ");
                    string newPass = Console.ReadLine();
                    if (userManager.ChangePassword(oldPass, newPass))
                    {
                        Console.WriteLine("Пароль изменен успешно!");
                    }
                    else
                    {
                        Console.WriteLine("Неверный старый пароль!");
                    }
                    break;
                case "2":
                    Console.Write("Новая дата рождения (гггг-мм-дд): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime newDate))
                    {
                        if (userManager.ChangeBirthDate(newDate))
                        {
                            Console.WriteLine("Дата рождения изменена!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный формат даты!");
                    }
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Неверный выбор!");
                    break;
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}