using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;
class Program
{
    static void Main()
    {
        MenuManager menuManager = new MenuManager();

        menuManager.Run();
        Console.WindowHeight = 20;
        Console.WindowWidth = 50;
    }

    class MenuManager
    {
        public void Login()
        {
            string connectionString = "server=127.0.0.1;user id=root;password=;database=bank;";
            DatabaseConnector dbConnector = new DatabaseConnector(connectionString);
            Menu showMenu = new Menu();

            try
            {
                dbConnector.OpenConnection();

                // Logowanie
                Console.Write("Podaj login: ");
                string login = Console.ReadLine();

                Console.Write("Podaj hasło: ");
                string password = GetPasswordInput();

                if (Autoryzacja(dbConnector, login, password))
                {
                    Console.WriteLine("Logowanie powiodło się!");

                    // Menu użytkownika
                    while (true)
                    {
                        showMenu.GoMenu(login);
                    }
                }
                else
                {
                    Console.WriteLine("Logowanie nie powiodło się.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd połączenia z bazą danych: {ex.Message}");
            }
            finally
            {
                dbConnector.CloseConnection();
            }
        }

        static bool Autoryzacja(DatabaseConnector dbConnector, string login, string password)
        {
            string query = $"SELECT * FROM uzytkownicy WHERE login='{login}' AND haslo='{password}'";
            MySqlCommand cmd = new MySqlCommand(query, dbConnector.GetConnection());
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                return reader.HasRows;
            }
        }

        private List<string> menuOptions = new List<string> { "Zaloguj się", "Utwórz konto", "Wyjdź" };
        private int selectedOption = 0;

        public void Run()
        {
            ConsoleKeyInfo key;

            do
            {
                Console.Clear();
                DisplayMenu();

                key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedOption = (selectedOption - 1 + menuOptions.Count) % menuOptions.Count;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedOption = (selectedOption + 1) % menuOptions.Count;
                        break;

                    case ConsoleKey.Enter:
                        HandleMenuSelection();
                        break;
                }

            } while (key.Key != ConsoleKey.Escape);
        }

        private void DisplayMenu()
        {
            Console.WriteLine("╔════════════════════════════════╗");
            Console.WriteLine("║                                ║");

            for (int i = 0; i < menuOptions.Count; i++)
            {
                Console.Write("║");
                if (i == selectedOption)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.Write("  ");
                }

                Console.WriteLine($"{menuOptions[i],-27}   ║");
                Console.ResetColor();
            }

            Console.WriteLine("╚════════════════════════════════╝");
        }

        private void HandleMenuSelection()
        {
            Console.Clear();

            switch (selectedOption)
            {
                case 0:
                    // Log in
                    Login();
                    break;

                case 1:
                    CreateAccount();
                    break;

                case 2:
                    Environment.Exit(0);
                    break;
            }

            Console.ReadKey();
        }

        private void CreateAccount()
        {
            Console.WriteLine("Wpisz swoje informacje dotyczące konta.");

            Console.Write("Login: ");
            string login = Console.ReadLine();

            Console.Write("Password: ");
            string password = GetPasswordInput();

            Console.Write("Imię: ");
            string firstName = Console.ReadLine();

            Console.Write("Nazwisko: ");
            string lastName = Console.ReadLine();

            // Generowanie losowego numeru konta składającego się z 10 cyfr
            Random random = new Random();
            long accountNumber = GenerateRandomAccountNumber();

            // Zapisywanie informacji o użytkowniku do bazy danych
            SaveUserToDatabase(login, password, firstName, lastName, accountNumber);

            Console.WriteLine("Konto utworzone pomyślnie!");
            Console.ReadKey();
        }
        private long GenerateRandomAccountNumber()
        {
            long minValue = 1000000000;
            long maxValue = 9999999999;

            Random random = new Random();
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            long longValue = BitConverter.ToInt64(buffer, 0);

            return (longValue % (maxValue - minValue)) + minValue;
        }

        private void SaveUserToDatabase(string login, string password, string firstName, string lastName, long accountNumber)
        {
            string connectionString = "server=127.0.0.1;user id=root;password=;database=bank;";
            DatabaseConnector dbConnector = new DatabaseConnector(connectionString);

            try
            {
                dbConnector.OpenConnection();

                // Sprawdź, czy użytkownik o podanym loginie już istnieje
                if (IsUserExists(dbConnector, login))
                {
                    Console.WriteLine("Użytkownik o podanym loginie już istnieje. Wybierz inny login.");
                    return;
                }

                // Dodaj nowego użytkownika do tabeli "uzytkownicy"
                string userQuery = $"INSERT INTO uzytkownicy (login, haslo, rola) " +
                                   $"VALUES ('{login}', '{password}', 'user')";
                MySqlCommand userCmd = new MySqlCommand(userQuery, dbConnector.GetConnection());
                userCmd.ExecuteNonQuery();

                // Pobierz id nowo dodanego użytkownika
                string getIdQuery = $"SELECT id FROM uzytkownicy WHERE login = '{login}'";
                MySqlCommand getIdCmd = new MySqlCommand(getIdQuery, dbConnector.GetConnection());
                int userId = Convert.ToInt32(getIdCmd.ExecuteScalar());

                // Dodaj nowe konto do tabeli "konto"
                string accountQuery = $"INSERT INTO konta (id, imie, nazwisko, nr_konta, saldo) " +
                                      $"VALUES ({userId}, '{firstName}', '{lastName}', '{accountNumber}', 0)";
                MySqlCommand accountCmd = new MySqlCommand(accountQuery, dbConnector.GetConnection());
                accountCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas tworzenia konta: {ex.Message}");
            }
            finally
            {
                dbConnector.CloseConnection();
            }
        }

        private bool IsUserExists(DatabaseConnector dbConnector, string login)
        {
            string query = $"SELECT COUNT(*) FROM uzytkownicy WHERE login='{login}'";
            MySqlCommand cmd = new MySqlCommand(query, dbConnector.GetConnection());
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }


        private string GetPasswordInput()
        {
            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Enter)
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                    else if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Escape &&
                             key.Key != ConsoleKey.Tab && !key.Key.ToString().StartsWith("F") &&
                             key.Modifiers != ConsoleModifiers.Alt && key.Modifiers != ConsoleModifiers.Control)
                    {
                        password += key.KeyChar;
                        Console.Write("*"); // Znak haszowanie hasła
                    }
                }

            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
}