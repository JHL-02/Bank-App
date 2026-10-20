namespace Bank_App
{
    internal class Menu
    {
        protected List<MenuOption> Options { get; } = new List<MenuOption>();
        private string Title { get; }
        private string LeaveMenuString { get; }
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
                Console.WriteLine($"---{Title}---");
                for (int i = 0; i < Options.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Options[i].Title}");
                }
                Console.WriteLine($"{Options.Count + 1}. {LeaveMenuString}.");

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
    }
}
