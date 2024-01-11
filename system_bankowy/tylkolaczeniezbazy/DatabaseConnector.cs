using MySql.Data.MySqlClient;
using System;

public class DatabaseConnector
{
    private MySqlConnection connection;

    public DatabaseConnector(string connectionString)
    {
        connection = new MySqlConnection(connectionString);
    }

    public void OpenConnection()
    {
        if (connection.State == System.Data.ConnectionState.Closed)
        {
            connection.Open();
        }
    }

    public void CloseConnection()
    {
        if (connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
        }
    }

    public MySqlConnection GetConnection()
    {
        return connection;
    }

    // Dodaj inne metody do obsługi bazy danych, np. wykonanie zapytania SQL, dodawanie danych itp.
}