namespace Bank_App
{
    internal class Admin : User
    {
        public Admin(string username, string password) : base(username, password)
        {
        }

        public MenuResult DisplayAdminMenu()
        {
            Menu adminMenu = new Menu(new List<MenuOption>
            {
                new MenuOption("Lägg till ny användare i systemet", () => AddUser()),
                new MenuOption("Ge pengar till användare", () => AddBalance()),
                new MenuOption("Justera kreditränta.", () =>
                {
                    Bank.AdjustCreditRate();
                    return MenuResult.Continue;
                }),
                new MenuOption("Uppdatera växelkurs", () => UpdateExchangeRate()),
                new MenuOption("Visa växelkurser", () =>
                {
                    Bank.ExchangeRates.ShowRate();
                    UserInput.WaitForUserConfirm();
                    return MenuResult.Continue;
                } )

            }, "Admin panelen");
            adminMenu.Display();
            return MenuResult.Continue;
        }

        // Admin skriver in dagens kurs. SEK är basen (alltid 1) och kan inte ändras.
        private MenuResult UpdateExchangeRate()
        {
            Bank.ExchangeRates.ShowRate();
            Console.WriteLine("Vilken valuta vill du uppdatera?");
            List<Currency> currencies = Enum.GetValues<Currency>().Where(c => c != Currency.SEK).ToList();
            Currency? chosenCurrency = UserInput.UserGetObjectFromList(currencies, c => c.ToString());
            if (chosenCurrency == null)
            {
                return MenuResult.Continue;
            }
            Console.WriteLine($"Ange ny kurs: hur många SEK är 1 {chosenCurrency}?");
            decimal newRate = UserInput.GetDecimal();
            if (Bank.ExchangeRates.UpdateRate(chosenCurrency.Value, newRate))
            {
                Console.WriteLine($"Kursen för {chosenCurrency} har uppdaterats.");
            }
            else
            {
                Console.WriteLine("Kursen kunde inte uppdateras. Den måste vara större än 0.");
            }
            UserInput.WaitForUserConfirm();
            return MenuResult.Continue;
        }

        private MenuResult AddUser()
        {
            Console.WriteLine("Vilken typ av användare vill du lägga till?");
            Console.WriteLine("1. Vanlig användare.");
            Console.WriteLine("2. Admin användare.");
            int userInput;
            while (true)
            {
                userInput = UserInput.GetInt();

                if (userInput == 1 || userInput == 2)
                {
                    break;
                }
            }
            bool isAdmin;
            switch (userInput)
            {
                case 1:
                    isAdmin = false;
                    break;
                case 2:
                    isAdmin = true;
                    break;
                default:
                    isAdmin = false;
                    break;
            }
            Console.WriteLine("Ange ett användarnamn för kontot:");
            string username = UserInput.GetString();

            Console.WriteLine("Ange ett lösenord för kontot:");
            string password = UserInput.GetString();

            if (Bank.UserDatabase.AddUser(username, password, isAdmin))
            {
                Console.WriteLine($"Kontot {username} har skapats!");
            }
            else
            {
                Console.WriteLine($"Kontot {username} kunde inte skapas då användarnamnet redan existerar.");
            }
            return MenuResult.Continue;
        }

        private MenuResult AddBalance()
        {
            Console.WriteLine("Vilken användare ska få pengar?");
            string userString = UserInput.GetString();
            User? targetedUser = Bank.UserDatabase.GetUserWithUsername(userString);

            if (targetedUser == null)
            {
                Console.WriteLine($"Användaren finns inte i systemet.:");
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            Console.WriteLine("Vilket av deras konton ska få pengar?:");
            BankAccount? targetedAccount = targetedUser.UserSelectAccount();

            if (targetedAccount == null)
            {
                Console.WriteLine($"Inget konto hittades.");
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            Console.WriteLine("Hur mycket saldo ska läggas till?:");
            decimal addAmmount = UserInput.GetDecimal();

            targetedAccount.Deposit(addAmmount);
            Console.WriteLine($"{addAmmount} saldo har tilldelats till \"{targetedAccount.Name}\"");
            UserInput.WaitForUserConfirm();

            return MenuResult.Continue;
        }
    }
}
