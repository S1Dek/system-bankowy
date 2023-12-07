using System;
public class Menu
{
    static int selectedOption = 0;
    static string[] menuOptions = { "Opcja 1", "Opcja 2", "Opcja 3", "Wyloguj" };
    delegate void MenuOption();

    static MenuOption[] optionActions = { Option1, Option2, Option3, Logout };
    public void GoMenu()
    {
        Console.CursorVisible = false;

        while (true)
        {
            Console.Clear();
            DisplayMenu();

            ConsoleKeyInfo key = Console.ReadKey();
            HandleKeyPress(key);
        }
    }
    static void DisplayMenu()
    {
        int centerRow = (Console.WindowHeight - menuOptions.Length) / 2;

        for (int i = 0; i < menuOptions.Length; i++)
        {
            if (i == selectedOption)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.SetCursorPosition((Console.WindowWidth - menuOptions[i].Length) / 2, centerRow + i);
            Console.WriteLine(menuOptions[i]);
            Console.ResetColor();
        }
    }
    static void HandleKeyPress(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                selectedOption = (selectedOption == 0) ? menuOptions.Length - 1 : selectedOption - 1;
                break;

            case ConsoleKey.DownArrow:
                selectedOption = (selectedOption == menuOptions.Length - 1) ? 0 : selectedOption + 1;
                break;

            case ConsoleKey.Enter:
                if (selectedOption == menuOptions.Length - 1)
                {
                    Console.Clear();
                    Console.WriteLine("Wylogowano. Naciśnij dowolny klawisz aby wyjść z programu...");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine($"Wybrano: {menuOptions[selectedOption]}. Naciśnij dowolny klawisz...");
                    Console.ReadKey();
                }
                break;
        }
    }
    static void Option1()
    {
        Console.Clear();
        Console.WriteLine("Wybrano opcję 1. Naciśnij dowolny klawisz...");
        Console.ReadKey();
    }

    static void Option2()
    {
        Console.Clear();
        Console.WriteLine("Wybrano opcję 2. Naciśnij dowolny klawisz...");
        Console.ReadKey();
    }

    static void Option3()
    {
        Console.Clear();
        Console.WriteLine("Wybrano opcję 3. Naciśnij dowolny klawisz...");
        Console.ReadKey();
    }

    static void Logout()
    {
        Console.Clear();
        Console.WriteLine("Wylogowano. Naciśnij dowolny klawisz aby wyjść z programu...");
        Console.ReadKey();
        Environment.Exit(0);
    }
}
