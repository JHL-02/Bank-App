namespace Bank_App
{
    internal class User
    {
        public string Username { get; init; }
        private string Password { get; init; }
        private readonly List<BankAccount> _accounts = new List<BankAccount>();
        public IReadOnlyCollection<BankAccount> Accounts => _accounts.AsReadOnly();
        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        private bool AddAccount(string name)
        {
            if (_accounts.Any(a => a.Name == name) || name.Length > BankAccount.MaxNameLength)
            {
                return false;
            }

            _accounts.Add(new BankAccount(name));
            return true;
        }

        private MenuResult UserCreateAccount()
        {
            Console.WriteLine($"Ange ett namn för det nya kontot (max {BankAccount.MaxNameLength} tecken, inga mellanslag):");

            string input;
            string proposedName;
            while (true)
            {
                input = UserInput.GetString("Du behöver skriva in något ett giltigt kontonamn.");
                proposedName = input.Trim();

                if (proposedName.Length <= BankAccount.MaxNameLength)
                {
                    break;
                }

                Console.WriteLine($"Ditt namn är längre än {BankAccount.MaxNameLength} tecken.");
            }

            Console.WriteLine($"Är du säker på kontonamnet \"{proposedName}\"?");
            if (!UserInput.GetYesOrNo())
            {
                Console.WriteLine("Avbryter.");
                return MenuResult.Continue;
            }
            

            if (AddAccount(proposedName))
            {
                Console.WriteLine("Ditt konto har skapats.");
                UserInput.WaitForUserConfirm();
                return MenuResult.Rebuild;
            }
            Console.WriteLine("Ditt konto kunde inte skapas.");
            UserInput.WaitForUserConfirm();
            return MenuResult.Continue;
        }

        public MenuResult DisplayAccountMenu()
        {
            MenuResult menuResult;

            do
            {
                List<MenuOption> menuOptions = new List<MenuOption>();

                menuOptions.Add(new MenuOption("Lägg till nytt konto.", () => UserCreateAccount()));

                foreach (BankAccount account in _accounts)
                {
                    menuOptions.Add(new MenuOption($"Konto: {account.Name}", () => DisplayAccountInfoMenu(account)));
                }

                Menu accountMenu = new Menu(menuOptions, "Kontohantering");
                menuResult = accountMenu.Display();
            } while (menuResult == MenuResult.Rebuild);

            return MenuResult.Continue;
        }

        public MenuResult DisplayAccountInfoMenu(BankAccount account)
        {
            Menu accountMenu = new Menu(new List<MenuOption>
            {
                new MenuOption("Visa info.", () =>
                {
                    Console.WriteLine("------");
                    Console.WriteLine($"Kontonamn: {account.Name}");
                    Console.WriteLine($"Saldo: {account.Balance}");
                    UserInput.WaitForUserConfirm();
                    return MenuResult.Continue;
                })
            }, account.Name);
            accountMenu.Display();

            return MenuResult.Continue;
        }
    }
}
