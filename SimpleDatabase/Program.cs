// See https://aka.ms/new-console-template for more information
using Bogus;
using SimpleDatabase;
using System.Diagnostics;
using Database = SimpleDatabase.Database;

Console.InputEncoding = System.Text.Encoding.UTF8;
Console.OutputEncoding = System.Text.Encoding.UTF8;


var userGenerator = new UserGenarator();
var databaseService = new Database(DatabaseConfig.ConnectionString);

ConsoleHelper.WriteHeader();

await CheckDatabaseConnection();
await CreateTable();

int userCount = ConsoleHelper.GetUserCount();
bool useBulkInsert = userCount >= DatabaseConfig.BulkInsertSettings.RecommendBulkInsertThreshold
    && ConsoleHelper.AskForBulkInsert();

ConsoleHelper.WriteInfo($"Генерація {userCount} користувачів...");
var users = userGenerator.GenerateUsers(userCount);

var stopwatch = Stopwatch.StartNew();

if (useBulkInsert)
{
    ConsoleHelper.WriteInfo("Використовується масова вставка...");
    await databaseService.BulkInsertUsersAsync(users);
}
else
{
    ConsoleHelper.WriteInfo("Використовується стандартна вставка...");
    await databaseService.InsertUsersAsync(users);
}

stopwatch.Stop();

int totalUsers = await databaseService.GetUserCountAsync();
ConsoleHelper.WriteStatistics(userCount, stopwatch.ElapsedMilliseconds, totalUsers);

ConsoleHelper.WaitForExit();

async Task CheckDatabaseConnection()
{
    ConsoleHelper.WriteInfo("Перевірка з'єднання з базою даних...");

    bool connectionSuccessful = await databaseService.TestConnectionAsync();

    if (!connectionSuccessful)
    {
        throw new InvalidOperationException("Не вдалося підключитися до бази даних. Перевірте рядок підключення.");
    }

    ConsoleHelper.WriteSuccess("З'єднання з базою даних успішне.");
}

async Task CreateTable()
{
    ConsoleHelper.WriteInfo("Підготовка таблиці Users...");
    await databaseService.CreateTableIfNotExistsAsync();
    ConsoleHelper.WriteSuccess("Таблиця Users готова до використання.");
}
