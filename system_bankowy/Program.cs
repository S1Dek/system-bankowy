using System;
using System.Collections.Generic;
using System.Reflection;
using system_bankowy;

class Program
{
    static void Main()
    {
        Menu showMenu = new Menu();
        string fileName = "login.txt";
        string filePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\", fileName));
        Loging loging = new Loging(filePath);

        while (true)
        {
            string login = Console.ReadLine();
            string password = Console.ReadLine();
            if (loging.VerifyCredentials(login, password))
            {
                showMenu.GoMenu();
            }
            else
            {
                Console.WriteLine("Błędne dane logowania. Spróbuj ponownie.");
            }
        }
    }
}