using System;
using System.Collections.Generic;
using System.Reflection;
using system_bankowy;
using System.IO;
using System.Linq;
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
        Console.WindowHeight = 20;
        Console.WindowWidth = 50;

        MenuManager menuManager = new MenuManager();
        menuManager.Run();
    }

    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        // Add additional user-related information if needed
    }

    class MenuManager
    {
        private enum LoggedInMenuOption
        {
            CheckAccountBalance,
            Deposit,
            Withdraw,
            ConvertCurrency,
            Logout
        }
        private List<string> menuOptions;
        private int selectedOption;
        private User currentUser; // To store the logged-in user

        public MenuManager()

        {

            menuOptions = new List<string>
            {
                "Zaloguj",
                "Załóż konto",
                "Wyjście"
            };

            selectedOption = 0;
        }

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
            Console.WriteLine("║        Wybierz opcję:          ║");

            for (int i = 0; i < menuOptions.Count; i++)
            {
                Console.Write("║");
                if (i == selectedOption)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("➤ ");
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
        private bool IsValidLogin(List<User> users, string username, string password)
        {
            // Validate login credentials against the loaded user data
            return users.Any(user => user.Username == username && user.Password == password);
        }
        private void Login()
        {
            Console.WriteLine("Enter your login credentials.");

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = GetPasswordInput();

            // Load user data from file
            List<User> users;
            LoadUsersFromFile(out users);

            // Validate login credentials
            if (IsValidLogin(users, username, password))
            {
                Console.WriteLine("Login successful!");
                currentUser = new User { Username = username, Password = password };
                // Once logged in, display the logged-in menu
                RunLoggedInMenu();
            }
            else
            {
                Console.WriteLine("Invalid login credentials. Please try again.");
            }
        }

        private bool IsValidLogin(string username, string password)
        {
            // Add your logic to validate the login credentials (e.g., check against a database)
            // For demonstration purposes, use a dummy check
            return username == "user" && password == "password";
        }

        private void CreateAccount()
        {
            Console.WriteLine("Enter your information to create an account.");

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = GetPasswordInput();

            // Save user information to a file
            SaveUserToFile(username, password);

            Console.WriteLine("Account created successfully!");
            Console.ReadKey();
        }

        private void SaveUserToFile(string username, string password)
        {
            string filePath = "user_data.txt";

            // Create or append to the user data file
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine($"{username},{password}");
            }
        }

        private void LoadUsersFromFile(out List<User> users)
        {
            users = new List<User>();
            string filePath = "user_data.txt";

            // Check if the user data file exists
            if (File.Exists(filePath))
            {
                // Read user data from the file and populate the users list
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] userData = line.Split(',');
                    if (userData.Length == 2)
                    {
                        users.Add(new User { Username = userData[0], Password = userData[1] });
                    }
                }
            }

        }

        private void RunLoggedInMenu()
        {
            ConsoleKeyInfo key;

            do
            {
                Console.Clear();
                DisplayLoggedInMenu();

                key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedOption = (selectedOption - 1 + Enum.GetValues(typeof(LoggedInMenuOption)).Length) % Enum.GetValues(typeof(LoggedInMenuOption)).Length;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedOption = (selectedOption + 1) % Enum.GetValues(typeof(LoggedInMenuOption)).Length;
                        break;

                    case ConsoleKey.Enter:
                        HandleLoggedInMenuSelection();
                        break;
                }

            } while (key.Key != ConsoleKey.Escape);
        }

        private void DisplayLoggedInMenu()
        {
            Console.WriteLine($"╔════════════════════════════════╗");
            Console.WriteLine($"║      Logged-In Menu ({currentUser.Username})     ║");

            var loggedInOptions = new List<string>
            {
                "Sprawdź stan konta",
                "Wpłata",
                "Wypłata",
                "Przewalutowanie",
                "Wyloguj"
            };

            for (int i = 0; i < loggedInOptions.Count; i++)
            {
                Console.Write("║");
                if (i == selectedOption)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("➤ ");
                }
                else
                {
                    Console.Write("  ");
                }

                Console.WriteLine($"{loggedInOptions[i],-27}     ║");
                Console.ResetColor();
            }

            Console.WriteLine($"╚════════════════════════════════╝");
        }

        private void HandleLoggedInMenuSelection()
        {
            switch (selectedOption)
            {
                case 0:
                    CheckAccountBalance();
                    break;

                case 1:
                    Deposit();
                    break;

                case 2:
                    Withdraw();
                    break;

                case 3:
                    ConvertCurrency();
                    break;

                case 4:
                    currentUser = null; // Logout the user
                    return; // Return to the main menu
            }

            Console.ReadKey();
        }

        private void CheckAccountBalance()
        {
            // Implement checking account balance functionality
            Console.WriteLine($"Account balance for {currentUser.Username} is: $X.XX"); // Replace with actual balance
        }

        private void Deposit()
        {
            // Implement deposit functionality
            Console.WriteLine("Enter the amount to deposit:");
            // Read user input and perform deposit logic
        }

        private void Withdraw()
        {
            // Implement withdrawal functionality
            Console.WriteLine("Enter the amount to withdraw:");
            // Read user input and perform withdrawal logic
        }

        private void ConvertCurrency()
        {
            // Implement currency conversion functionality
            Console.WriteLine("Enter the amount to convert:");
            // Read user input and perform currency conversion logic
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
                        Console.Write("\b \b"); // Remove the last entered character
                    }
                    else if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Escape &&
                             key.Key != ConsoleKey.Tab && !key.Key.ToString().StartsWith("F") &&
                             key.Modifiers != ConsoleModifiers.Alt && key.Modifiers != ConsoleModifiers.Control)
                    {
                        password += key.KeyChar;
                        Console.Write("*");
                    }
                }

            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine(); // Move to a new line after entering the password
            return password;
        }
    }
}