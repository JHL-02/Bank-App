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
            Message.WriteLine(new Message("Vilken valuta vill du uppdatera?", MessageType.INPUT));
            List<Currency> currencies = Enum.GetValues<Currency>().Where(c => c != Currency.SEK).ToList();
            Currency? chosenCurrency = UserInput.UserGetObjectFromList(currencies, c => c.ToString());
            if (chosenCurrency == null)
            {
                return MenuResult.Continue;
            }
            Message.WriteLine(new Message($"Ange ny kurs: hur många SEK är 1 {chosenCurrency}?", MessageType.INPUT));
            decimal newRate = UserInput.GetDecimal();
            if (Bank.ExchangeRates.UpdateRate(chosenCurrency.Value, newRate))
            {
                Message.WriteLine(new Message($"Kursen för {chosenCurrency} har uppdaterats."));
            }
            else
            {
                Message.WriteLine(new Message("Kursen kunde inte uppdateras. Den måste vara större än 0.", MessageType.ERROR));
            }
            UserInput.WaitForUserConfirm();
            return MenuResult.Continue;
        }

        private MenuResult AddUser()
        {
            Message.WriteLine(new Message("Vilken typ av användare vill du lägga till?", MessageType.INPUT));
            Message.WriteLine(new Message
                (
                "1. Vanlig användare.\n" +
                "2. Admin användare."
                ));
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
            Message.WriteLine(new Message("Ange ett användarnamn för kontot:", MessageType.INPUT));
            string username = UserInput.GetString();

            Message.WriteLine(new Message("Ange ett lösenord för kontot:", MessageType.INPUT));
            string password = UserInput.GetString();

            if (Bank.UserDatabase.AddUser(username, password, isAdmin))
            {
                Message.WriteLine(new Message($"Kontot {username} har skapats!"));
            }
            else
            {
                Message.WriteLine(new Message($"Kontot {username} kunde inte skapas då användarnamnet redan existerar.", MessageType.ERROR));
            }
            return MenuResult.Continue;
        }

        private MenuResult AddBalance()
        {
            Message.WriteLine(new Message("Vilken användare ska få pengar?", MessageType.INPUT));
            string userString = UserInput.GetString();
            User? targetedUser = Bank.UserDatabase.GetUserWithUsername(userString);

            if (targetedUser == null)
            {
                Message.WriteLine(new Message($"Användaren finns inte i systemet.:", MessageType.ERROR));
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            Message.WriteLine(new Message("Vilket av deras konton ska få pengar?:", MessageType.INPUT));
            BankAccount? targetedAccount = targetedUser.UserSelectAccount();

            if (targetedAccount == null)
            {
                Message.WriteLine(new Message($"Inget konto hittades.", MessageType.ERROR));
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            Message.WriteLine(new Message($"Hur mycket saldo ska läggas till? ({targetedAccount.displayCurrency}):", MessageType.INPUT));
            decimal addAmmount = UserInput.GetDecimal();

            targetedAccount.Deposit(addAmmount);
            Message.WriteLine(new Message($"{targetedAccount.getCurrencyDisplayString(addAmmount)} har tilldelats till \"{targetedAccount.Name}\""));
            UserInput.WaitForUserConfirm();

            return MenuResult.Continue;
        }
    }
}
