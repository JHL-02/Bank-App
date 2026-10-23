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
            if (_accounts.Any(a => a.Name == name) || name.Length > BankAccount.MaxNameLength || _accounts.Count < BankAccount.MaxAccountAmount)
            {
                return false;
            }

            _accounts.Add(new BankAccount(name));
            return true;
        }
        public BankAccount? GetAccountWithName(string name)
        {
            return _accounts.Find(a => a.Name == name);
        }

        private MenuResult UserCreateAccount()
        {
            if (_accounts.Count >= BankAccount.MaxAccountAmount)
            {
                Console.WriteLine($"Du kan inte skapa mer än {BankAccount.MaxAccountAmount} konton.");
                UserInput.WaitForUserConfirm();
            }

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
                    menuOptions.Add(new MenuOption($"Konto: {account.Name}", () => DisplayAccountMenu(account)));
                }

                Menu accountMenu = new Menu(menuOptions, "Kontohantering");
                menuResult = accountMenu.Display();
            } while (menuResult == MenuResult.Rebuild);

            return MenuResult.Continue;
        }

        public MenuResult DisplayAccountMenu(BankAccount account)
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
                }),
                new MenuOption("Överföra pengar", () => StartUserTransaction(account))
            }, account.Name);
            accountMenu.Display();

            return MenuResult.Continue;
        }

        //Allows other classes to see the password for validation
        public bool CheckPassword(string password)
        {
            return Password == password;
        }
        private MenuResult StartUserTransaction(BankAccount account)
        {
            Console.WriteLine("Vill du föra över til dig själv?");
            if (UserInput.GetYesOrNo())
            {
                Console.WriteLine("Vilket konto vill du föra över till?");
                string input = UserInput.GetString();

                if (_accounts.Any(a => a.Name == input))
                {
                    BankAccount? targetedAccount = _accounts.Find(a => a.Name == input);
                    if (targetedAccount != null)
                    {
                        Console.WriteLine("Hur mycket vill du föra över?:");
                        decimal transferAmount = UserInput.GetDecimal("Du behöver skriva in en siffra.");

                        account.Transactions(targetedAccount, transferAmount);
                    }
                }
                else
                {
                    Console.WriteLine($"Du har inget konto som heter {input}.");
                    UserInput.WaitForUserConfirm();
                }
            }
            else
            {
                Console.WriteLine("Vilken användare vill du föra över till?");
                string input = UserInput.GetString();

                User? targetedUser = Bank.UserDatabase.GetUserWithUsername(input);
                if (targetedUser != null)
                {
                    Console.WriteLine("Ange namnet på kontot som du vill föra över till:");
                    string userAccountTargetString = UserInput.GetString("Felaktig inmatning.");

                    BankAccount? targetedAccount = targetedUser.GetAccountWithName(userAccountTargetString);

                    if (targetedAccount != null)
                    {
                        Console.WriteLine("Hur mycket vill du föra över?:");
                        decimal transferAmount = UserInput.GetDecimal("Du behöver skriva in en siffra.");

                        if (account.Transactions(targetedAccount, transferAmount))
                        {
                            Console.WriteLine($"Du förde över {transferAmount} till {targetedAccount.Name}.");
                        }
                        else
                        {
                            Console.WriteLine("Det gick inte att föra över.");
                        }
                        UserInput.WaitForUserConfirm();
                    }
                    else
                    {
                        Console.WriteLine($"Kunde inte hitta något konto med namnet {userAccountTargetString}");
                        UserInput.WaitForUserConfirm();
                    }
                }
                else
                {
                    Console.WriteLine($"Kunde inte hitta en användare med namnet {input}.");
                    UserInput.WaitForUserConfirm();
                }
            }
            return MenuResult.Continue;
        }

    }
}

