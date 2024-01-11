using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;

public class Menu
{
    private static int selectedOption = 0;
    public static int menuOption = 0;
    public static string login;
    private static List<List<string>> menuOptions = new List<List<string>>
    {
        new List<string> { "Wyswietl Wszystkie Dane", "", "" },
        new List<string> { "Wykonaj Wplate", "", "Wyloguj" },
        new List<string> { "Wykonaj Wyplate", "" },
        new List<string> { "Zarzadzanie Kontami", "" },
        new List<string> { "Wyloguj", "Wyloguj" }
    };

    private static List<Action<DatabaseConnector, string>> menuFunctions = new List<Action<DatabaseConnector, string>>
    {
        (dbConnector, login) => DisplayAllData(dbConnector, login),
        (dbConnector, login) => MakeDeposit(dbConnector, login),
        (dbConnector, login) => MakeWithdrawal(dbConnector, login),
        (dbConnector, login) => ManageAccounts(dbConnector, login),
    };

    public void GoMenu(string login)
    {
        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();
            DisplayMenu();
            ConsoleKeyInfo key = Console.ReadKey();
            HandleKeyPress(key, login);
        }
    }

    private static void DisplayMenu()
    {
        int centerRow = (Console.WindowHeight - menuOptions.Count) / 2;

        for (int i = 0; i < menuOptions.Count; i++)
        {
            if (i == selectedOption)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            if (menuOptions[i].Count > menuOption)
            {
                Console.SetCursorPosition((Console.WindowWidth - menuOptions[i][menuOption].Length) / 2, centerRow + i);
                Console.Write(menuOptions[i][menuOption]);
            }

            Console.ResetColor();
        }

        Console.WriteLine();
    }
    private static void HandleKeyPress(ConsoleKeyInfo key, string login)
    {
        string connectionString = "server=127.0.0.1;user id=root;password=;database=bank;";
        DatabaseConnector dbConnector = new DatabaseConnector(connectionString);

        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                selectedOption = (selectedOption == 0) ? menuOptions.Count - 1 : selectedOption - 1;
                break;

            case ConsoleKey.DownArrow:
                selectedOption = (selectedOption == menuOptions.Count - 1) ? 0 : selectedOption + 1;
                break;

            case ConsoleKey.Enter:
                for (int i = 0; i < menuOptions.Count; i++)
                {
                    if (menuOptions[i].Count > menuOption && menuOptions[i][menuOption] == "Wyloguj")
                    {
                        if (selectedOption == i)
                        {
                            Console.Clear();
                            Console.WriteLine("Wylogowano. Naciśnij dowolny klawisz aby wyjść z programu...");
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                        else
                        {
                            Console.Clear();
                            if (menuOptions[selectedOption].Count > menuOption)
                            {
                                Console.WriteLine($"Wybrano: {menuOptions[selectedOption][menuOption]}. Naciśnij dowolny klawisz...");
                                Options(dbConnector, login);
                                Console.ReadKey();
                            }
                        }
                        break;
                    }
                }
                break;
        }
    }

    private static void Options(DatabaseConnector dbConnector, string login)
    {
        menuFunctions[selectedOption].Invoke(dbConnector, login);
    }

    static void DisplayAllData(DatabaseConnector dbConnector, string login)
    {
        try
        {
            dbConnector.OpenConnection();
            // Pobieranie danych użytkownika na podstawie loginu
            string query = $"SELECT * FROM konta INNER JOIN uzytkownicy ON konta.id = uzytkownicy.id WHERE uzytkownicy.login = '{login}'";
            MySqlCommand cmd = new MySqlCommand(query, dbConnector.GetConnection());

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Imię: {reader["imie"]}");
                        Console.WriteLine($"Nazwisko: {reader["nazwisko"]}");
                        Console.WriteLine($"Numer konta: {reader["nr_konta"]}");
                        Console.WriteLine($"Saldo: {reader["saldo"]}");
                        Console.WriteLine($"Login: {reader["login"]}");
                        Console.WriteLine($"Rola: {reader["rola"]}");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Nie znaleziono danych dla danego użytkownika.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd odczytu danych użytkownika: {ex.Message}");
        }
        finally
        {
            dbConnector.CloseConnection();
        }
    }
    private static void MakeDeposit(DatabaseConnector dbConnector, string login)
    {
        try
        {
            // Pobieranie aktualnego salda użytkownika
            string getBalanceQuery = $"SELECT saldo FROM konta INNER JOIN uzytkownicy ON konta.id = uzytkownicy.id WHERE uzytkownicy.login = '{login}'";
            MySqlCommand getBalanceCmd = new MySqlCommand(getBalanceQuery, dbConnector.GetConnection());

            dbConnector.OpenConnection();

            object currentBalanceObj = getBalanceCmd.ExecuteScalar();

            if (currentBalanceObj != null && decimal.TryParse(currentBalanceObj.ToString(), out decimal currentBalance))
            {
                // Wprowadzanie kwoty depozytu
                Console.Write("Wprowadź kwotę depozytu: ");
                if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal depositAmount))
                {
                    // Obliczanie saldo po depozycie
                    decimal newBalance = currentBalance + depositAmount;

                    // Aktualizuje saldo w bazie danych
                    string updateBalanceQuery = $"UPDATE konta SET saldo = {newBalance.ToString(CultureInfo.InvariantCulture)} WHERE id = (SELECT id FROM uzytkownicy WHERE login = '{login}')";
                    MySqlCommand updateBalanceCmd = new MySqlCommand(updateBalanceQuery, dbConnector.GetConnection());

                    int rowsAffected = updateBalanceCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Depozyt w wysokości {depositAmount} został pomyślnie dodany. Nowe saldo: {newBalance}");
                    }
                    else
                    {
                        Console.WriteLine("Błąd podczas aktualizacji salda.");
                    }
                }
                else
                {
                    Console.WriteLine("Nieprawidłowa kwota depozytu.");
                }
            }
            else
            {
                Console.WriteLine($"Nie można uzyskać aktualnego salda dla użytkownika o loginie '{login}'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas dokonywania depozytu: {ex.Message}");
        }
        finally
        {
            dbConnector.CloseConnection();
        }
    }




    private static void MakeWithdrawal(DatabaseConnector dbConnector, string login)
    {
        try
        {
            // Pobieranie aktualnego salda użytkownika
            string getBalanceQuery = $"SELECT saldo FROM konta INNER JOIN uzytkownicy ON konta.id = uzytkownicy.id WHERE uzytkownicy.login = '{login}'";
            MySqlCommand getBalanceCmd = new MySqlCommand(getBalanceQuery, dbConnector.GetConnection());

            dbConnector.OpenConnection();

            object currentBalanceObj = getBalanceCmd.ExecuteScalar();

            if (currentBalanceObj != null && decimal.TryParse(currentBalanceObj.ToString(), out decimal currentBalance))
            {
                // Wprowadzanie kwoty wypłaty
                Console.Write("Wprowadź kwotę wypłaty: ");
                if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal depositAmount))
                {
                    // Obliczanie saldo po wypłacie
                    decimal newBalance = currentBalance - depositAmount;

                    // Aktualizuje saldo w bazie danych
                    string updateBalanceQuery = $"UPDATE konta SET saldo = {newBalance.ToString(CultureInfo.InvariantCulture)} WHERE id = (SELECT id FROM uzytkownicy WHERE login = '{login}')";
                    MySqlCommand updateBalanceCmd = new MySqlCommand(updateBalanceQuery, dbConnector.GetConnection());

                    int rowsAffected = updateBalanceCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"Depozyt w wysokości {depositAmount} został pomyślnie dodany. Nowe saldo: {newBalance}");
                    }
                    else
                    {
                        Console.WriteLine("Błąd podczas aktualizacji salda.");
                    }
                }
                else
                {
                    Console.WriteLine("Nieprawidłowa kwota depozytu.");
                }
            }
            else
            {
                Console.WriteLine($"Nie można uzyskać aktualnego salda dla użytkownika o loginie '{login}'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas dokonywania depozytu: {ex.Message}");
        }
        finally
        {
            dbConnector.CloseConnection();
        }
    }
    private static void ManageAccounts(DatabaseConnector dbConnector, string login)
    {
        Console.WriteLine("");
    }
}
