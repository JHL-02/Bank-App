namespace Bank_App.Menu
{
    internal abstract class Menu
    {
        protected List<MenuOption> Options { get; } = new List<MenuOption>();
        private string LeaveMenuString { get; }

        protected Menu(string leaveMenuString = "Tillbaka.")
        {
            LeaveMenuString = leaveMenuString;

            InitilizeOptions();
        }

        protected abstract void InitilizeOptions();

        public void Display()
        {
            bool exit = false;
            while (!exit)
            {
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
                    exit = true;
                }
                else
                {
                    Options[userPick - 1].Execute();
                }
            }
        }
    }
}
