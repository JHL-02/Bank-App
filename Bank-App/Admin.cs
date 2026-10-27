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
                new MenuOption("Ge pengar till användare", () => AddBalance())
            }, "Admin panelen");
            adminMenu.Display();
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
