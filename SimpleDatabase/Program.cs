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


while (true)
{
    int mainChoice = ConsoleHelper.GetMainMenuChoice();

    switch (mainChoice)
    {
        case 1:
            await AddNewUsers();
            break;
        case 2:
            await SearchAndViewUsers();
            break;
        case 0:
            ConsoleHelper.WriteInfo("До побачення!");
            return;
    }
}

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

async Task AddNewUsers()
{
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
}

async Task SearchAndViewUsers()
{
    while (true)
    {
        ConsoleHelper.WriteSearchHeader();
        int searchChoice = ConsoleHelper.GetSearchMenuChoice();

        var stopwatch = new Stopwatch();

        switch (searchChoice)
        {
            case 1: 
                var (limit, offset) = ConsoleHelper.GetPaginationParams();
                stopwatch.Start();

                var allUsers = await databaseService.GetAllUsersAsync(limit, offset);
                stopwatch.Stop();

                ConsoleHelper.DisplayUsers(allUsers, stopwatch.ElapsedMilliseconds);
                break;

            case 2: 
                string firstName = ConsoleHelper.GetSearchTerm("ім'я");
                stopwatch.Start();

                var usersByFirstName = await databaseService.SearchUsersByFirstNameAsync(firstName);
                stopwatch.Stop();

                ConsoleHelper.DisplayUsers(usersByFirstName, stopwatch.ElapsedMilliseconds);
                break;

            case 3: 
                string lastName = ConsoleHelper.GetSearchTerm("прізвище");
                stopwatch.Start();

                var usersByLastName = await databaseService.SearchUsersByLastNameAsync(lastName);
                stopwatch.Stop();

                ConsoleHelper.DisplayUsers(usersByLastName, stopwatch.ElapsedMilliseconds);
                break;

            case 4: 
                string email = ConsoleHelper.GetSearchTerm("email");
                stopwatch.Start();

                var usersByEmail = await databaseService.SearchUsersByEmailAsync(email);
                stopwatch.Stop();

                ConsoleHelper.DisplayUsers(usersByEmail, stopwatch.ElapsedMilliseconds);
                break;

            case 5:
                string searchTerm = ConsoleHelper.GetSearchTerm("текст для пошуку");
                stopwatch.Start();

                var foundUsers = await databaseService.SearchUsersAsync(searchTerm);
                stopwatch.Stop();

                ConsoleHelper.DisplayUsers(foundUsers, stopwatch.ElapsedMilliseconds);
                break;

            case 6: 
                int userId = ConsoleHelper.GetUserId();
                stopwatch.Start();

                var user = await databaseService.GetUserByIdAsync(userId);
                stopwatch.Stop();

                ConsoleHelper.DisplayUser(user, stopwatch.ElapsedMilliseconds);
                break;

            case 0: 
                return;
        }

        if (!ConsoleHelper.AskToContinue())
        {
            break;
        }
    }
}