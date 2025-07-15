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
}