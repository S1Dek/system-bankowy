using MySql.Data.MySqlClient;
using System;

public class DataReader
{
    private DatabaseConnector dbConnector;

    public DataReader(DatabaseConnector connector)
    {
        dbConnector = connector;
    }

    public void WyswietlWszystkieDane()
    {
        try
        {
            dbConnector.OpenConnection();

            // Kod do odczytu danych
            string query = "SELECT * FROM konta";
            MySqlCommand cmd = new MySqlCommand(query, dbConnector.GetConnection());
            MySqlDataReader dataReader = cmd.ExecuteReader();

            Console.WriteLine("Wszystkie dane z tabeli:");

            while (dataReader.Read())
            {
                Console.WriteLine($"imie: {dataReader["imie"]}, nazwisko: {dataReader["nazwisko"]},NR_konta:{dataReader["NR_konta"]},Saldo: {dataReader["Saldo"]}");
                // Dodaj inne kolumny, jeśli są
            }

            dataReader.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd odczytu danych: {ex.Message}");
        }
        finally
        {
            dbConnector.CloseConnection();
        }
    }
}
