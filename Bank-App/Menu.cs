using System.Drawing;

namespace Bank_App
{
    internal class Menu
    {
        protected List<MenuOption> Options { get; } = new List<MenuOption>();
        private string Title { get; }
        private string LeaveMenuString { get; }
        private ConsoleColor defaultColor = ConsoleColor.Cyan;
        public Menu(List<MenuOption> options, string title, string leaveMenuString = "Tillbaka.")
        {
            Title = title;
            LeaveMenuString = leaveMenuString;
            Options.AddRange(options);
        }

        public MenuResult Display()
        {
            MenuResult menuResult = MenuResult.Continue;
            while (true)
            {
                DisplayMenuHeader(Title, defaultColor);
                for (int i = 0; i < Options.Count; i++)
                {
                    if (Options[i].Color is ConsoleColor color)
                    {
                        Console.ForegroundColor = color;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }
                    Console.WriteLine($"| {i + 1}. {Options[i].Title, -21} |");
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"| {Options.Count + 1}. {LeaveMenuString, -21} |");
                Console.ForegroundColor = defaultColor;
                Console.WriteLine("+--------------------------+");
                Console.ResetColor();

                int userPick;

                while (true)
                {
                    userPick = UserInput.GetInt("Du kan bara skriva heltal.");

                    if (userPick >= 1 && userPick <= Options.Count + 1)
                    {
                        break;
                    }

                    Console.WriteLine("Du behöver välja ett alternativ.");
                }

                if (userPick == Options.Count + 1)
                {
                    return MenuResult.Exit;
                }

                menuResult = Options[userPick - 1].Execute();

                if (menuResult != MenuResult.Continue)
                { 
                    return menuResult;
                }
            }
        }
        private static void DisplayMenuHeader(string title, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("+--------------------------+");
            Console.WriteLine($"| {title, -24} |");
            Console.WriteLine("+--------------------------+");
            Console.ResetColor();
        }
    }
}
