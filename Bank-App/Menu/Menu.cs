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
            while (true)
            {
                int width = Math.Max(
                    24,
                    Math.Max(
                        Title.Length,
                        Math.Max(
                            LeaveMenuString.Length + 3,
                            Options.Max(o => o.Title.Length + 3)
                        )
                    )
                );

                DisplayMenuHeader(Title, defaultColor, width);

                for (int i = 0; i < Options.Count; i++)
                {
                    Console.ForegroundColor = Options[i].Color ?? ConsoleColor.Yellow;

                    Console.WriteLine($"| {i + 1}. {Options[i].Title.PadRight(width - 3)} |");
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"| 0. {LeaveMenuString.PadRight(width - 3)} |");

                Console.ForegroundColor = defaultColor;
                Console.WriteLine("+" + new string('-', width + 2) + "+");

                Console.ResetColor();

                int userPick;

                while (true)
                {
                    userPick = UserInput.GetInt();

                    if (userPick >= 0 && userPick <= Options.Count)
                        break;

                    Message.WriteLine(new Message("Du behöver välja ett alternativ."));
                }

                if (userPick == 0)
                    return MenuResult.Exit;

                MenuResult menuResult = Options[userPick - 1].Execute();

                if (menuResult != MenuResult.Continue)
                    return menuResult;
            }
        }

        private static void DisplayMenuHeader(string title, ConsoleColor color, int width)
        {
            Console.ForegroundColor = color;

            string border = "+" + new string('-', width + 2) + "+";

            Console.WriteLine(border);
            Console.WriteLine($"| {title.PadRight(width)} |");
            Console.WriteLine(border);

            Console.ResetColor();
        }
    }
}
