using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=sql.bsite.net\\MSSQL2016;Database=kostikok_VideoCinema;User Id=kostikok_VideoCinema;Password=12345;TrustServerCertificate=True;";

            string query = "SELECT UserID, FirstName, LastName, Email FROM Users";
        Register:
            Console.WriteLine("1. Зарегестрироваться");
            Console.WriteLine("2. Войти");
            Console.WriteLine("3. Выйти");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterUser(connectionString);
                    goto Start;
                case "2":
                    LoginUser(connectionString);
                    goto Start;
                case "3":
                    break;
                default:
                    Console.WriteLine("Ошибка. Попробуйте снова");
                    goto Register;
            }
        Start:
            Console.WriteLine("Выберите действие: \n" +
                "1. Посмотреть таблицу\n" +
                "2. Добавить нового пользователя\n" +
                "3. Обновить существующего пользователя \n" +
                "4. Удалить существующего пользователся\n" +
                "5. Выйти");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand(query, connection);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-30}", "ID", "First Name", "Last Name", "Email");
                            Console.WriteLine(new string('-', 65));

                            while (reader.Read())
                            {

                                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-30}", reader["UserID"], reader["FirstName"], reader["LastName"], reader["Email"]);
                            }
                        }
                    }
                    goto Start;
                case "2":
                    Console.WriteLine("\nAdding a new user...");
                    Console.Write("Enter First Name: ");
                    string firstName = Console.ReadLine();
                    Console.Write("Enter Last Name: ");
                    string lastName = Console.ReadLine();
                    Console.Write("Enter Email: ");
                    string email = Console.ReadLine();
                    Console.Write("Enter Password: ");
                    string password = Console.ReadLine();

                    string insertQuery = "INSERT INTO Users (FirstName, LastName, Email, PasswordHash) VALUES (@FirstName, @LastName, @Email, @PasswordHash)";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();

                            SqlCommand command = new SqlCommand(insertQuery, connection);

                            command.Parameters.AddWithValue("@FirstName", firstName);
                            command.Parameters.AddWithValue("@LastName", lastName);
                            command.Parameters.AddWithValue("@Email", email);
                            command.Parameters.AddWithValue("@PasswordHash", password);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine("Пользователь добавлен успешно");
                            }
                            else
                            {
                                Console.WriteLine("Ошибка добавления пользователя.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Ошибка: " + ex.Message);
                        }
                        Console.WriteLine();
                        goto Start;
                    }
                case "3":
                    UpdateUserData(connectionString);
                    goto Start;
                case "4":
                    DeleteUserData(connectionString);
                    goto Start;
                case "5":
                    break;

            }
        }
        static void RegisterUser(string connectionString)
        {
            Console.WriteLine("\nРегистрация нового пользователя: ");
            Console.Write("Введите имя: ");
            string firstName = Console.ReadLine();
            Console.Write("Введите фамилию: ");
            string lastName = Console.ReadLine();
            Console.Write("Введите почту: ");
            string email = Console.ReadLine();
            Console.Write("введите пароль: ");
            string password = Console.ReadLine();

            string insertQuery = "INSERT INTO Users (FirstName, LastName, Email, PasswordHash) VALUES (@FirstName, @LastName, @Email, @PasswordHash)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(insertQuery, connection);

                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@PasswordHash", password);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Пользователь зарегестрирован успешно!");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка регистрации пользователя.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }
        }
        static void LoginUser(string connectionString)
        {
Login:
            Console.WriteLine("\nВход...");
            Console.Write("Enter Email: ");
            string email = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            string selectQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(selectQuery, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@PasswordHash", password);
                    int count = (int)command.ExecuteScalar();
                    if (count > 0)
                    {
                        Console.WriteLine("Вход выполнен успешно!");
                    }
                    else
                    {
                        Console.WriteLine("Неправильноя почта или пароль.");
                        goto Login;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
        static void UpdateUserData(string connectionString)
        {
            Console.WriteLine("\nОбновляем данные пользователя");

            Console.Write("Введите почту: ");
            string email = Console.ReadLine();
            Console.Write("Введите имя: ");
            string newFirstName = Console.ReadLine();
            Console.Write("Введите фамилию: ");
            string newLastName = Console.ReadLine();
            Console.Write("EВведите пароль: ");
            string newPassword = Console.ReadLine();

            string updateQuery = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, PasswordHash = @PasswordHash WHERE Email = @Email";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(updateQuery, connection);

                    command.Parameters.AddWithValue("@FirstName", newFirstName);
                    command.Parameters.AddWithValue("@LastName", newLastName);
                    command.Parameters.AddWithValue("@PasswordHash", newPassword);
                    command.Parameters.AddWithValue("@Email", email);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Данные пользователя успешно обновлены!");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка добавления данных пользователя.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }
        }
        static void DeleteUserData(string connectionString)
        {
            Console.WriteLine("\nУдаление данных пользователя");
            Console.Write("Введите почту: ");
            string email = Console.ReadLine();
            string deleteQuery = "DELETE FROM Users WHERE Email = @Email";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(deleteQuery, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Данные пользователя успешно удалены!");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка удаления данных пользователя.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }
        }
    }
}