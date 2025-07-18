using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDatabase;

public static class ConsoleHelper
{
    public static int GetUserCount()
    {
        while (true)
        {
            Console.Write("Введіть кількість користувачів для генерації: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int count) && count > 0)
            {
                return count;
            }
            WriteError("Будь ласка, введіть коректне число більше 0.");
        }
    }

    public static bool AskForBulkInsert()
    {
        Console.WriteLine("Для великої кількості записів рекомендується використовувати масову вставку.");
        Console.Write("Використовувати масову вставку? (y/n): ");
        string input = Console.ReadLine()?.ToLower();
        return input == "y" || input == "yes" || input == "так" || input == "т";
    }

    public static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($" {message}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($" {message}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void WriteInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($" {message}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void WriteWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($" {message}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void WriteHeader()
    {
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine("    Генератор користувачів з Bogus");
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine();
    }

    public static void WriteStatistics(int userCount, long elapsedMilliseconds, int totalUsers)
    {
        Console.WriteLine();
        WriteSuccess($"Успішно додано {userCount} користувачів у базу даних!");
        var elapsed = TimeSpan.FromMilliseconds(elapsedMilliseconds);
        Console.WriteLine($"Час виконання: {elapsedMilliseconds} мс ({elapsed:mm\\:ss\\.fff})");
        Console.WriteLine($"Швидкість: {(userCount / elapsed.TotalSeconds):F2} користувачів/сек");
        Console.WriteLine($"Загальна кількість користувачів у БД: {totalUsers}");
    }

    public static void WaitForExit()
    {
        Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
        Console.ReadKey();
    }

    
    public static void WriteSearchHeader()
    {
        Console.WriteLine();
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine("    Пошук та перегляд користувачів");
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine();
    }

    public static int GetSearchMenuChoice()
    {
        while (true)
        {
            Console.WriteLine("Оберіть дію:");
            Console.WriteLine("1. Переглянути всіх користувачів");
            Console.WriteLine("2. Пошук за іменем");
            Console.WriteLine("3. Пошук за прізвищем");
            Console.WriteLine("4. Пошук за email");
            Console.WriteLine("5. Загальний пошук");
            Console.WriteLine("6. Пошук за ID");
            Console.WriteLine("0. Повернутися до головного меню");
            Console.Write("Ваш вибір: ");

            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 6)
            {
                return choice;
            }
            WriteError("Будь ласка, введіть число від 0 до 6.");
        }
    }

    public static string GetSearchTerm(string fieldName)
    {
        while (true)
        {
            Console.Write($"Введіть {fieldName} для пошуку: ");
            string input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(input))
            {
                return input;
            }
            WriteError($"Будь ласка, введіть {fieldName}.");
        }
    }

    public static int GetUserId()
    {
        while (true)
        {
            Console.Write("Введіть ID користувача: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int id) && id > 0)
            {
                return id;
            }
            WriteError("Будь ласка, введіть коректне число більше 0.");
        }
    }

    public static void DisplayUsers(List<User> users, long searchTimeMs)
    {
        if (users.Count == 0)
        {
            WriteWarning("Користувачі не знайдені.");
            WriteInfo($"Час пошуку: {searchTimeMs} мс");
            return;
        }

        Console.WriteLine();
        WriteSuccess($"Знайдено {users.Count} користувачів:");
        WriteInfo($"Час пошуку: {searchTimeMs} мс");

        Console.WriteLine();
        Console.WriteLine($"{"ID",-5} {"Ім'я",-15} {"Прізвище",-15} {"Email",-30}");
        Console.WriteLine(new string('-', 65));

        foreach (var user in users)
        {
            Console.WriteLine($"{user.Id,-5} {user.FirstName,-15} {user.LastName,-15} {user.Email,-30}");
        }
        Console.WriteLine();
    }

    public static void DisplayUser(User user, long searchTimeMs)
    {
        if (user == null)
        {
            WriteWarning("Користувач не знайдений.");
            WriteInfo($"Час пошуку: {searchTimeMs} мс");
            return;
        }

        Console.WriteLine();
        WriteSuccess("Користувач знайдений:");
        WriteInfo($"Час пошуку: {searchTimeMs} мс");

        Console.WriteLine();
        Console.WriteLine($"ID: {user.Id}");
        Console.WriteLine($"Ім'я: {user.FirstName}");
        Console.WriteLine($"Прізвище: {user.LastName}");
        Console.WriteLine($"Email: {user.Email}");
        Console.WriteLine();
    }

    public static (int limit, int offset) GetPaginationParams()
    {
        Console.WriteLine("Налаштування пагінації:");

        int limit = 50; // За замовчуванням
        Console.Write($"Кількість записів на сторінку (за замовчуванням {limit}): ");
        string limitInput = Console.ReadLine();
        if (int.TryParse(limitInput, out int parsedLimit) && parsedLimit > 0)
        {
            limit = parsedLimit;
        }

        int offset = 0; // За замовчуванням
        Console.Write($"Пропустити записів (за замовчуванням {offset}): ");
        string offsetInput = Console.ReadLine();
        if (int.TryParse(offsetInput, out int parsedOffset) && parsedOffset >= 0)
        {
            offset = parsedOffset;
        }

        return (limit, offset);
    }

    public static bool AskToContinue()
    {
        Console.WriteLine();
        Console.Write("Бажаете продовжити пошук? (y/n): ");
        string input = Console.ReadLine()?.ToLower();
        return input == "y" || input == "yes" || input == "так" || input == "т";
    }

    public static int GetMainMenuChoice()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=".PadRight(50, '='));
            Console.WriteLine("    Головне меню");
            Console.WriteLine("=".PadRight(50, '='));
            Console.WriteLine();
            Console.WriteLine("1. Додати нових користувачів");
            Console.WriteLine("2. Переглянути та шукати користувачів");
            Console.WriteLine("0. Вийти");
            Console.Write("Ваш вибір: ");

            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice) && choice >= 0 && choice <= 2)
            {
                return choice;
            }
            WriteError("Будь ласка, введіть число від 0 до 2.");
        }
    }
}