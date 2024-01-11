using MySql.Data.MySqlClient;
using System;

class ManageAccounts
{
    private DatabaseConnector dbConnector;

    public ManageAccounts(DatabaseConnector connector)
    {
        dbConnector = connector;
    }

    public void DodajNoweKonto(string imie, string nazwisko, decimal saldo, string login, string haslo, string rola)
    {
        try
        {
            // Dodaj nowe konto
            string insertKontoQuery = "INSERT INTO konta (imie, nazwisko, saldo) VALUES (@Imie, @Nazwisko, @Saldo);";
            MySqlCommand insertKontoCmd = new MySqlCommand(insertKontoQuery, dbConnector.GetConnection());
            insertKontoCmd.Parameters.AddWithValue("@Imie", imie);
            insertKontoCmd.Parameters.AddWithValue("@Nazwisko", nazwisko);
            insertKontoCmd.Parameters.AddWithValue("@Saldo", saldo);

            insertKontoCmd.ExecuteNonQuery();

            // Pobierz id dodanego konta
            int idKonta = (int)insertKontoCmd.LastInsertedId;

            // Dodaj nowego użytkownika z informacjami o koncie
            string insertUzytkownikQuery = "INSERT INTO uzytkownicy (id_konta, login, haslo, rola) VALUES (@IdKonta, @Login, @Haslo, @Rola);";
            MySqlCommand insertUzytkownikCmd = new MySqlCommand(insertUzytkownikQuery, dbConnector.GetConnection());
            insertUzytkownikCmd.Parameters.AddWithValue("@IdKonta", idKonta);
            insertUzytkownikCmd.Parameters.AddWithValue("@Login", login);
            insertUzytkownikCmd.Parameters.AddWithValue("@Haslo", haslo);
            insertUzytkownikCmd.Parameters.AddWithValue("@Rola", rola);

            insertUzytkownikCmd.ExecuteNonQuery();

            Console.WriteLine("Nowe konto zostało dodane.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas dodawania nowego konta: {ex.Message}");
        }
    }

    public void EdytujKonto(int idKonta, string imie, string nazwisko, decimal saldo)
    {
        try
        {
            // Edytuj istniejące konto
            string updateKontoQuery = "UPDATE konta SET imie = @Imie, nazwisko = @Nazwisko, saldo = @Saldo WHERE id = @Id;";
            MySqlCommand updateKontoCmd = new MySqlCommand(updateKontoQuery, dbConnector.GetConnection());
            updateKontoCmd.Parameters.AddWithValue("@Imie", imie);
            updateKontoCmd.Parameters.AddWithValue("@Nazwisko", nazwisko);
            updateKontoCmd.Parameters.AddWithValue("@Saldo", saldo);
            updateKontoCmd.Parameters.AddWithValue("@Id", idKonta);

            int rowsAffected = updateKontoCmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Konto zostało zaktualizowane.");
            }
            else
            {
                Console.WriteLine("Nie znaleziono konta do aktualizacji.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas aktualizacji konta: {ex.Message}");
        }
    }

    public void UsunKonto(int idKonta)
    {
        try
        {
            // Usuń konto
            string deleteKontoQuery = "DELETE FROM konta WHERE id = @Id;";
            MySqlCommand deleteKontoCmd = new MySqlCommand(deleteKontoQuery, dbConnector.GetConnection());
            deleteKontoCmd.Parameters.AddWithValue("@Id", idKonta);

            int rowsAffected = deleteKontoCmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Konto zostało usunięte.");
            }
            else
            {
                Console.WriteLine("Nie znaleziono konta do usunięcia.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas usuwania konta: {ex.Message}");
        }
    }

    public void WyswietlWszystkieKonta()
    {
        try
        {
            dbConnector.OpenConnection();

            // Kod do odczytu danych wszystkich kont
            string query = "SELECT * FROM konta";
            MySqlCommand cmd = new MySqlCommand(query, dbConnector.GetConnection());
            MySqlDataReader dataReader = cmd.ExecuteReader();

            Console.WriteLine("Wszystkie konta:");

            while (dataReader.Read())
            {
                Console.WriteLine($"ID: {dataReader["id"]}, Imie: {dataReader["imie"]}, Nazwisko: {dataReader["nazwisko"]}, Saldo: {dataReader["saldo"]}");
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