using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SimpleDatabase;

public class Database
{
    private readonly string _connectionString;

    public Database(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return true;
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($" Помилка підключення: {ex.Message}");
            return false;
        }
    }

    public async Task CreateTableIfNotExistsAsync()
    {
        string createTableSql = @"
            CREATE TABLE IF NOT EXISTS users (
                id SERIAL PRIMARY KEY,
                firstname VARCHAR(50) NOT NULL,
                lastname VARCHAR(50) NOT NULL,
                email VARCHAR(100) NOT NULL
            );";

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(createTableSql, connection);
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($" Помилка при створенні таблиці: {ex.Message}", ex);
        }
    }

    public async Task InsertUsersAsync(List<User> users)
    {
        if (users == null || users.Count == 0)
            throw new ArgumentException("Список користувачів не може бути пустим", nameof(users));

        string insertSql = @"
            INSERT INTO users (firstname, lastname, email)
            VALUES (@FirstName, @LastName, @Email);";

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            foreach (var user in users)
            {
                using var command = new NpgsqlCommand(insertSql, connection, transaction);
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@Email", user.Email);

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($" Помилка при додаванні користувачів: {ex.Message}", ex);
        }
    }

    public async Task BulkInsertUsersAsync(List<User> users)
    {
        if (users == null || users.Count == 0)
            throw new ArgumentException("Список користувачів не може бути пустим", nameof(users));

        try
        {
            var dataTable = CreateDataTable(users);

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var writer = connection.BeginBinaryImport(
                "COPY users (firstname, lastname, email) FROM STDIN (FORMAT BINARY)");

            foreach (DataRow row in dataTable.Rows)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(row["FirstName"]);
                await writer.WriteAsync(row["LastName"]);
                await writer.WriteAsync(row["Email"]);
            }

            await writer.CompleteAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($" Помилка при масовій вставці користувачів: {ex.Message}", ex);
        }
    }

    public async Task<int> GetUserCountAsync()
    {
        string countSql = "SELECT COUNT(*) FROM users;";

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(countSql, connection);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Помилка при отриманні кількості користувачів: {ex.Message}", ex);
        }
    }

    
    public async Task<List<User>> GetAllUsersAsync(int limit = 100, int offset = 0)
    {
        string selectSql = @"
            SELECT id, firstname, lastname, email 
            FROM users 
            ORDER BY id 
            LIMIT @Limit OFFSET @Offset;";

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(selectSql, connection);
            command.Parameters.AddWithValue("@Limit", limit);
            command.Parameters.AddWithValue("@Offset", offset);

            var users = new List<User>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32("id"),
                    FirstName = reader.GetString("firstname"),
                    LastName = reader.GetString("lastname"),
                    Email = reader.GetString("email")
                });
            }

            return users;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Помилка при отриманні списку користувачів: {ex.Message}", ex);
        }
    }

    public async Task<List<User>> SearchUsersByFirstNameAsync(string firstName)
    {
        string searchSql = @"
            SELECT id, firstname, lastname, email 
            FROM users 
            WHERE LOWER(firstname) LIKE LOWER(@FirstName) 
            ORDER BY firstname, lastname;";

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(searchSql, connection);
            command.Parameters.AddWithValue("@FirstName", $"%{firstName}%");

            return await ExecuteSearchQuery(command);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Помилка при пошуку користувачів за іменем: {ex.Message}", ex);
        }
    }

    public async Task<List<User>> SearchUsersByLastNameAsync(string lastName)
    {
        string searchSql = @"
            SELECT id, firstname, lastname, email 
            FROM users 
            WHERE LOWER(lastname) LIKE LOWER(@LastName) 
            ORDER BY lastname, firstname;";

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(searchSql, connection);
            command.Parameters.AddWithValue("@LastName", $"%{lastName}%");

            return await ExecuteSearchQuery(command);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Помилка при пошуку користувачів за прізвищем: {ex.Message}", ex);
        }
    }

    public async Task<List<User>> SearchUsersByEmailAsync(string email)
    {
        string searchSql = @"
            SELECT id, firstname, lastname, email 
            FROM users 
            WHERE LOWER(email) LIKE LOWER(@Email) 
            ORDER BY email;";

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(searchSql, connection);
            command.Parameters.AddWithValue("@Email", $"%{email}%");

            return await ExecuteSearchQuery(command);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Помилка при пошуку користувачів за email: {ex.Message}", ex);
        }
    }

    public async Task<List<User>> SearchUsersAsync(string searchTerm)
    {
        string searchSql = @"
            SELECT id, firstname, lastname, email 
            FROM users 
            WHERE LOWER(firstname) LIKE LOWER(@SearchTerm) 
               OR LOWER(lastname) LIKE LOWER(@SearchTerm) 
               OR LOWER(email) LIKE LOWER(@SearchTerm) 
            ORDER BY firstname, lastname;";

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(searchSql, connection);
            command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

            return await ExecuteSearchQuery(command);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Помилка при загальному пошуку користувачів: {ex.Message}", ex);
        }
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        string selectSql = @"
            SELECT id, firstname, lastname, email 
            FROM users 
            WHERE id = @Id;";

        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(selectSql, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32("id"),
                    FirstName = reader.GetString("firstname"),
                    LastName = reader.GetString("lastname"),
                    Email = reader.GetString("email")
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Помилка при отриманні користувача за ID: {ex.Message}", ex);
        }
    }

    private async Task<List<User>> ExecuteSearchQuery(NpgsqlCommand command)
    {
        var users = new List<User>();
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetInt32("id"),
                FirstName = reader.GetString("firstname"),
                LastName = reader.GetString("lastname"),
                Email = reader.GetString("email")
            });
        }

        return users;
    }

    private DataTable CreateDataTable(List<User> users)
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("FirstName", typeof(string));
        dataTable.Columns.Add("LastName", typeof(string));
        dataTable.Columns.Add("Email", typeof(string));

        foreach (var user in users)
        {
            dataTable.Rows.Add(user.FirstName, user.LastName, user.Email);
        }

        return dataTable;
    }
}