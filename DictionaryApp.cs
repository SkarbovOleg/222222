using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DictionaryApp
{
    // Класс для представления словаря
    public class Dictionary
    {
        public string Name { get; set; }
        public string FromLanguage { get; set; }
        public string ToLanguage { get; set; }
        public Dictionary<string, List<string>> Words { get; set; }

        public Dictionary()
        {
            Words = new Dictionary<string, List<string>>();
        }

        public Dictionary(string name, string fromLang, string toLang) : this()
        {
            Name = name;
            FromLanguage = fromLang;
            ToLanguage = toLang;
        }

        public void AddWord(string word, string translation)
        {
            if (!Words.ContainsKey(word))
            {
                Words[word] = new List<string>();
            }
            Words[word].Add(translation);
        }

        public bool UpdateWord(string oldWord, string newWord)
        {
            if (Words.ContainsKey(oldWord) && !Words.ContainsKey(newWord))
            {
                Words[newWord] = Words[oldWord];
                Words.Remove(oldWord);
                return true;
            }
            return false;
        }

        public bool UpdateTranslation(string word, string oldTranslation, string newTranslation)
        {
            if (Words.ContainsKey(word))
            {
                int index = Words[word].IndexOf(oldTranslation);
                if (index != -1)
                {
                    Words[word][index] = newTranslation;
                    return true;
                }
            }
            return false;
        }

        public bool DeleteWord(string word)
        {
            return Words.Remove(word);
        }

        public bool DeleteTranslation(string word, string translation)
        {
            if (Words.ContainsKey(word) && Words[word].Count > 1)
            {
                return Words[word].Remove(translation);
            }
            return false;
        }

        public List<string> Search(string word)
        {
            if (Words.ContainsKey(word))
            {
                return Words[word];
            }
            return null;
        }

        public void ExportToFile(string filePath, string word)
        {
            if (Words.ContainsKey(word))
            {
                var result = new { Word = word, Translations = Words[word] };
                string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
        }
    }

    // Класс для управления словарями
    public class DictionaryManager
    {
        private List<Dictionary> dictionaries = new List<Dictionary>();
        private string dataPath = "dictionaries.json";

        public DictionaryManager()
        {
            LoadData();
        }

        public void CreateDictionary(string name, string fromLang, string toLang)
        {
            if (dictionaries.Any(d => d.Name == name))
            {
                Console.WriteLine("Словарь с таким именем уже существует!");
                return;
            }
            dictionaries.Add(new Dictionary(name, fromLang, toLang));
            SaveData();
            Console.WriteLine($"Словарь '{name}' создан успешно!");
        }

        public Dictionary GetDictionary(string name)
        {
            return dictionaries.FirstOrDefault(d => d.Name == name);
        }

        public void ListDictionaries()
        {
            if (dictionaries.Count == 0)
            {
                Console.WriteLine("Нет доступных словарей.");
                return;
            }
            Console.WriteLine("\nДоступные словари:");
            foreach (var dict in dictionaries)
            {
                Console.WriteLine($"- {dict.Name} ({dict.FromLanguage}->{dict.ToLanguage})");
            }
        }

        public void AddWord(string dictName, string word, string translation)
        {
            var dict = GetDictionary(dictName);
            if (dict == null)
            {
                Console.WriteLine("Словарь не найден!");
                return;
            }
            dict.AddWord(word, translation);
            SaveData();
            Console.WriteLine("Слово добавлено успешно!");
        }

        public void UpdateWord(string dictName, string oldWord, string newWord)
        {
            var dict = GetDictionary(dictName);
            if (dict == null)
            {
                Console.WriteLine("Словарь не найден!");
                return;
            }
            if (dict.UpdateWord(oldWord, newWord))
            {
                SaveData();
                Console.WriteLine("Слово обновлено успешно!");
            }
            else
            {
                Console.WriteLine("Не удалось обновить слово.");
            }
        }

        public void UpdateTranslation(string dictName, string word, string oldTranslation, string newTranslation)
        {
            var dict = GetDictionary(dictName);
            if (dict == null)
            {
                Console.WriteLine("Словарь не найден!");
                return;
            }
            if (dict.UpdateTranslation(word, oldTranslation, newTranslation))
            {
                SaveData();
                Console.WriteLine("Перевод обновлен успешно!");
            }
            else
            {
                Console.WriteLine("Не удалось обновить перевод.");
            }
        }

        public void DeleteWord(string dictName, string word)
        {
            var dict = GetDictionary(dictName);
            if (dict == null)
            {
                Console.WriteLine("Словарь не найден!");
                return;
            }
            if (dict.DeleteWord(word))
            {
                SaveData();
                Console.WriteLine("Слово удалено успешно!");
            }
            else
            {
                Console.WriteLine("Слово не найдено.");
            }
        }

        public void DeleteTranslation(string dictName, string word, string translation)
        {
            var dict = GetDictionary(dictName);
            if (dict == null)
            {
                Console.WriteLine("Словарь не найден!");
                return;
            }
            if (dict.DeleteTranslation(word, translation))
            {
                SaveData();
                Console.WriteLine("Перевод удален успешно!");
            }
            else
            {
                Console.WriteLine("Не удалось удалить перевод. Возможно, это последний перевод.");
            }
        }

        public void SearchWord(string dictName, string word)
        {
            var dict = GetDictionary(dictName);
            if (dict == null)
            {
                Console.WriteLine("Словарь не найден!");
                return;
            }
            var translations = dict.Search(word);
            if (translations != null && translations.Count > 0)
            {
                Console.WriteLine($"Переводы слова '{word}':");
                foreach (var t in translations)
                {
                    Console.WriteLine($"- {t}");
                }
            }
            else
            {
                Console.WriteLine("Слово не найдено.");
            }
        }

        public void ExportWord(string dictName, string word, string filePath)
        {
            var dict = GetDictionary(dictName);
            if (dict == null)
            {
                Console.WriteLine("Словарь не найден!");
                return;
            }
            dict.ExportToFile(filePath, word);
            Console.WriteLine($"Экспорт выполнен в файл {filePath}");
        }

        private void SaveData()
        {
            try
            {
                string json = JsonSerializer.Serialize(dictionaries, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(dataPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(dataPath))
                {
                    string json = File.ReadAllText(dataPath);
                    dictionaries = JsonSerializer.Deserialize<List<Dictionary>>(json) ?? new List<Dictionary>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки: {ex.Message}");
                dictionaries = new List<Dictionary>();
            }
        }
    }

    // Главный класс программы
    class Program
    {
        static DictionaryManager manager = new DictionaryManager();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool exit = false;
            while (!exit)
            {
                ShowMainMenu();
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": CreateDictionaryMenu(); break;
                    case "2": ManageDictionaryMenu(); break;
                    case "3": ListDictionariesMenu(); break;
                    case "4": SearchInDictionaryMenu(); break;
                    case "5": ExportWordMenu(); break;
                    case "6": exit = true; break;
                    default: Console.WriteLine("Неверный выбор!"); break;
                }
            }
        }

        static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("=== ГЛАВНОЕ МЕНЮ ===");
            Console.WriteLine("1. Создать словарь");
            Console.WriteLine("2. Работа со словарем");
            Console.WriteLine("3. Список словарей");
            Console.WriteLine("4. Поиск слова");
            Console.WriteLine("5. Экспорт слова");
            Console.WriteLine("6. Выход");
            Console.Write("Выберите действие: ");
        }

        static void CreateDictionaryMenu()
        {
            Console.Clear();
            Console.WriteLine("=== СОЗДАНИЕ СЛОВАРЯ ===");
            Console.Write("Введите имя словаря: ");
            string name = Console.ReadLine();
            Console.Write("Введите язык источника (например, Английский): ");
            string from = Console.ReadLine();
            Console.Write("Введите язык перевода (например, Русский): ");
            string to = Console.ReadLine();
            manager.CreateDictionary(name, from, to);
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void ManageDictionaryMenu()
        {
            Console.Clear();
            manager.ListDictionaries();
            Console.Write("Выберите словарь (введите имя): ");
            string dictName = Console.ReadLine();

            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine($"=== РАБОТА СО СЛОВАРЕМ: {dictName} ===");
                Console.WriteLine("1. Добавить слово");
                Console.WriteLine("2. Заменить слово");
                Console.WriteLine("3. Заменить перевод");
                Console.WriteLine("4. Удалить слово");
                Console.WriteLine("5. Удалить перевод");
                Console.WriteLine("6. Вернуться назад");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Console.Write("Введите слово: ");
                        string word = Console.ReadLine();
                        Console.Write("Введите перевод: ");
                        string trans = Console.ReadLine();
                        manager.AddWord(dictName, word, trans);
                        break;
                    case "2":
                        Console.Write("Введите старое слово: ");
                        string oldWord = Console.ReadLine();
                        Console.Write("Введите новое слово: ");
                        string newWord = Console.ReadLine();
                        manager.UpdateWord(dictName, oldWord, newWord);
                        break;
                    case "3":
                        Console.Write("Введите слово: ");
                        string wordForUpdate = Console.ReadLine();
                        Console.Write("Введите старый перевод: ");
                        string oldTrans = Console.ReadLine();
                        Console.Write("Введите новый перевод: ");
                        string newTrans = Console.ReadLine();
                        manager.UpdateTranslation(dictName, wordForUpdate, oldTrans, newTrans);
                        break;
                    case "4":
                        Console.Write("Введите слово для удаления: ");
                        string wordToDelete = Console.ReadLine();
                        manager.DeleteWord(dictName, wordToDelete);
                        break;
                    case "5":
                        Console.Write("Введите слово: ");
                        string wordForDeleteTrans = Console.ReadLine();
                        Console.Write("Введите перевод для удаления: ");
                        string transToDelete = Console.ReadLine();
                        manager.DeleteTranslation(dictName, wordForDeleteTrans, transToDelete);
                        break;
                    case "6":
                        back = true;
                        continue;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        break;
                }
                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        static void ListDictionariesMenu()
        {
            Console.Clear();
            manager.ListDictionaries();
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void SearchInDictionaryMenu()
        {
            Console.Clear();
            manager.ListDictionaries();
            Console.Write("Выберите словарь (введите имя): ");
            string dictName = Console.ReadLine();
            Console.Write("Введите слово для поиска: ");
            string word = Console.ReadLine();
            manager.SearchWord(dictName, word);
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void ExportWordMenu()
        {
            Console.Clear();
            manager.ListDictionaries();
            Console.Write("Выберите словарь (введите имя): ");
            string dictName = Console.ReadLine();
            Console.Write("Введите слово для экспорта: ");
            string word = Console.ReadLine();
            Console.Write("Введите путь для сохранения (например, export.json): ");
            string path = Console.ReadLine();
            manager.ExportWord(dictName, word, path);
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}