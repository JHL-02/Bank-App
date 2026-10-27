namespace Bank_App
{
    internal static class Bank
    {
        public static UserDatabase UserDatabase { get; } = new UserDatabase();
        public static ExchangeRates ExchangeRates { get; } = new ExchangeRates();
        private static Menu startMenu = new Menu(new List<MenuOption>
        {
            new MenuOption("Logga in.", () =>
            {
                if (Login.isLockedOut)
                {
                    Console.WriteLine("Du har gjort för många felförsök.");
                    return MenuResult.Continue;
                }

                var login = new Login(UserDatabase);
                User? user = login.TryLogin();

                if (user != null)
                {
                    user.DisplayAccountMenu();
                }
                return MenuResult.Continue;
            }),
            new MenuOption("Skippa inlogging till User0 (adminkonto)", () =>
            {
                UserDatabase.GetUserWithUsername("User0")?.DisplayAccountMenu();
                return MenuResult.Continue;
            }),
            new MenuOption("Skippa inlogging till User1", () =>
            {
                UserDatabase.GetUserWithUsername("User1")?.DisplayAccountMenu();
                return MenuResult.Continue;
            }),
            new MenuOption("Skippa inlogging till User2", () =>
            {
                UserDatabase.GetUserWithUsername("User2")?.DisplayAccountMenu();
                return MenuResult.Continue;
            })
        }, "Välkommen till bankappen", "Avsluta");

        public static void StartProgram()
        {
            UserDatabase.AddUser("User0", "1234", true);
            UserDatabase.AddUser("User1", "1234");
            UserDatabase.AddUser("User2", "1234");

            startMenu.Display();
        }
    }
}
