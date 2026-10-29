namespace Bank_App
{
    internal static class Bank
    {
        public static UserDatabase UserDatabase { get; } = new UserDatabase();
        public static ExchangeRates ExchangeRates { get; } = new ExchangeRates();

        //public const int DefaultUserCreditScore = 100; I don't know what to do with this system -William
        public static decimal loanExtraCreditRate { get; private set; } = 0.05m; // Högre räntevärde för en lägre kreditvärdighet.
        public static decimal loanCreditRate { get; private set; } = 0.03m; // Standard lånränta på lånet. 3%

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

        public static decimal AdjustCreditRate()
        {
            Console.WriteLine(
                //$"Standardränta: {loanCreditRate:P} för alla kunder.\n" +
                //$"Ny kund börjar med: {DefaultUserCreditScore} i kreditvärdighet.\n" +
                "1. Öka räntenivå baserat på kreditvärdighet.\n" +
                "2. Minska räntenivå baserat på kreditvärdighet.\n" +
                "3. Valfri räntemängd.\n" +
                "4. Avsluta.");

            switch (UserInput.GetInt())
            {
                case 1:
                    Console.WriteLine(
                        $"Räntan baseras på kreditvärdighet. Kunder med kreditvärdighet under 60 poäng får {loanExtraCreditRate:P} ränta istället för {loanCreditRate:P}.\n" +
                        $"1. Addera mer på räntan.\n" +
                        $"2. Avsluta.\n");
                    switch (UserInput.GetInt())
                    {
                        case 1:
                            Console.WriteLine("Ange hur mycket räntan ska ökas (t.ex. 0.01 för 1 procentenhet):");
                            decimal increase = UserInput.GetDecimal();
                            if (increase > 0m)
                            {
                                loanExtraCreditRate += increase;
                                Console.WriteLine($"Den nya extra räntan är {loanExtraCreditRate:P}.");
                            }
                            else
                            {
                                Console.WriteLine("Ökningen måste vara mer än 0.");
                            }
                            break;
                        case 2:
                            break;
                    }
                    break;
                case 2:
                    Console.WriteLine(
                        $"Räntan baseras på kreditvärdighet. Kunder med kreditvärdighet under 60 poäng får {loanExtraCreditRate:P} ränta istället för {loanCreditRate:P}.\n" +
                        $"1. Reducera mer på räntan.\n" +
                        $"2. Avsluta.\n");
                    switch (UserInput.GetInt())
                    {
                        case 1:
                            Console.WriteLine("Ange hur mycket räntan ska minskas (t.ex. 0.01 för 1 procentenhet):");
                            decimal reduction = UserInput.GetDecimal();

                            if (reduction <= 0m)
                            {
                                Console.WriteLine("Minskningen måste vara mer än 0.");
                                break;
                            }

                            decimal newExtraRate = loanExtraCreditRate - reduction;

                            if (newExtraRate >= 0m)
                            {
                                loanExtraCreditRate = newExtraRate;
                                Console.WriteLine($"Den nya extra räntan är {loanExtraCreditRate:P}.");
                            }
                            else
                            {
                                Console.WriteLine("Räntan kan inte bli negativ.");
                            }
                            break;
                        case 2:
                            break;
                        default:
                            Console.WriteLine("Felaktig inmatning. Vänligen ange ett nummer mellan 1-2.");
                            break;
                    }
                    break;
                case 3:
                    Console.WriteLine("Ange ett nytt standardvärde för standardränta hos banken (t.ex. 0.01 för 1 procentenhet):");
                    decimal newRate = UserInput.GetDecimal();
                    if (newRate >= 0m && newRate <= 1m)
                    {
                        loanCreditRate = newRate;
                    }
                    else
                    {
                        Console.WriteLine("Talet kan bara vara mellan än 0-1. (t.ex. 0.01 för 1 procentenhet)");
                    }
                    break;
                case 4:
                    Console.Clear();
                    break;
                default:
                    Console.WriteLine("Felaktig inmatning. Vänligen ange ett nummer mellan 1-4.");
                    break;
            }
            return loanCreditRate;
        }
    }
}
