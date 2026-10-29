namespace Bank_App
{
    internal class User
    {
        public string Username { get; init; }
        private string Password { get; init; }
        private readonly List<BankAccount> _accounts = new List<BankAccount>();
        public IReadOnlyCollection<BankAccount> Accounts => _accounts.AsReadOnly();
        public decimal userLoanDebt { get; protected set; }
        public int userCreditScore
        {
            get
            {
                if (userLoanDebt == 0)
                    return 100;

                return Math.Clamp(
                    (int)( Math.Round((GetTotalBalance() - userLoanDebt) * 25 / userLoanDebt )),
                    0,
                    100
                );
            }
        } // Temporary solution?
        public User(string username, string password)
        {
            Username = username;
            Password = password;
            userLoanDebt = 0.0m;
            //userCreditScore = Bank.DefaultUserCreditScore;
        }

        private bool AddAccount(string name, bool isSavingsAccount = false)
        {
            if (_accounts.Any(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase) || name.Length > BankAccount.MaxNameLength || _accounts.Count >= BankAccount.MaxAccountAmount))
            {
                return false;
            }
            if (isSavingsAccount)
            {
                _accounts.Add(new SavingsAccount(this, name));
                return true;
            }
            _accounts.Add(new BankAccount(this, name));
            return true;
        }
        public decimal GetTotalBalance() => _accounts.Sum(account => account.GetBalance());

        public decimal CalculateCreditRate()
        {
            if (userCreditScore < 60)
            {
                return Bank.loanExtraCreditRate;
            }
            return Bank.loanCreditRate;
        }

        // You always get a year interest for each loan?
        public MenuResult UserLoan()
        {
            Console.WriteLine("Hur mycket pengar vill du långa av banken? (SEK):");
            decimal loanAmount = UserInput.GetDecimal("Du behöver skriva in en siffra.");

            if (loanAmount <= 0m)
            {
                Console.WriteLine("Summan är för låg.");
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            decimal ownCapital = GetTotalBalance() - userLoanDebt;
            decimal creditLimit = ownCapital * 5;
            if (creditLimit < 0m)
            {
                creditLimit = 0m;
            }
            decimal yearlyInterest = loanAmount * CalculateCreditRate();
            decimal loanSum = yearlyInterest + loanAmount;

            if (loanAmount > creditLimit || userCreditScore <= 20)
            {
                Console.WriteLine("Ditt lån har inte blivit godkänt av banken.");
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            Console.WriteLine($"Din ansökan för ett belopp på {loanAmount:C} har blivit godkänd.");
            Console.WriteLine($"Årsränta: {yearlyInterest:C}. Totalkostnad: {loanSum:C}.");

            Console.WriteLine("Vilket konto vill du att pengarna ska överföras till?");
            BankAccount? bankAccount = UserSelectAccount();

            if (bankAccount == null)
            {
                Console.WriteLine("Du har inga konton.");
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            bankAccount.Deposit(loanAmount);
            if (bankAccount.displayCurrency != Currency.SEK)
            {
                Console.WriteLine("Konverterar valuta.");
            }
            Console.WriteLine($"Pengarna har överförts till {bankAccount.Name}.");

            userLoanDebt += loanSum;
            return MenuResult.Continue;
        }
        public MenuResult PayBackUserLoan()
        {
            if (userLoanDebt <= 0m)
            {
                Console.WriteLine("Du har inget att betala tillbaka.");
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            Console.WriteLine("Vilket konto vill du betala med?");
            BankAccount? payingAccount = UserSelectAccount();

            if (payingAccount == null)
            {
                Console.WriteLine("Du har inga konton.");
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            Console.WriteLine($"Hur mycket vill du betala tillbaka? ({payingAccount.displayCurrency.ToString()}):");
            decimal amount = Bank.ExchangeRates.Convert(UserInput.GetDecimal("Du behöver skriva in en siffra."), payingAccount.displayCurrency, Currency.SEK);

            if (amount <= 0m)
            {
                Console.WriteLine("Summan är för låg.");
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            decimal newAmount = userLoanDebt - amount;
            decimal userPays = Math.Min(amount, userLoanDebt);

            if (payingAccount.GetBalance() < userPays)
            {
                Console.WriteLine("För lågt saldo på konto.");
                return MenuResult.Continue;
            }

            if (!payingAccount.Withdraw(userPays))
            {
                Console.WriteLine("FEL: Det gick inte att ta pengar ur kontot.");
                return MenuResult.Continue;
            }

            userLoanDebt -= userPays;

            Console.WriteLine($"Du har betalat tillbaka {payingAccount.getCurrencyDisplayString(userPays)} till banken.");
            UserInput.WaitForUserConfirm();
            return MenuResult.Continue;
        }

        public BankAccount? GetAccountWithName(string name)
        {
            return _accounts.Find(a => string.Equals(a.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private MenuResult UserCreateAccount()
        {
            if (_accounts.Count >= BankAccount.MaxAccountAmount)
            {
                Console.WriteLine($"Du kan inte skapa mer än {BankAccount.MaxAccountAmount} konton.");
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }
            Console.WriteLine("1. Bankkonto\n2. Sparkonto");
            int accountTypeInput;
            while (true)
            {
                accountTypeInput = UserInput.GetInt();

                if (accountTypeInput == 1 || accountTypeInput == 2)
                {
                    break;
                }
                Console.WriteLine("Välj ett alternativ ur listan.");
            }
            bool isSavingsAccount = accountTypeInput == 2;


            Console.WriteLine($"Ange ett namn för det nya kontot (max {BankAccount.MaxNameLength} tecken, inga mellanslag):");
            string input;
            string proposedName;
            while (true)
            {
                input = UserInput.GetString("Du behöver skriva in ett giltigt kontonamn.");

                if (input.Any(char.IsWhiteSpace))
                {
                    Console.WriteLine("Kontonamnet får inte innehålla mellanslag.");
                    continue;
                }

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


            if (AddAccount(proposedName, isSavingsAccount))
            {
                Console.WriteLine("Ditt konto har skapats.");
                UserInput.WaitForUserConfirm();
                return MenuResult.Rebuild;
            }
            Console.WriteLine("Ditt konto kunde inte skapas då namnet redan används.");
            UserInput.WaitForUserConfirm();
            return MenuResult.Continue;
        }

        public MenuResult DisplayAccountMenu()
        {
            MenuResult menuResult;

            if (this is Admin admin)
            {
                Console.WriteLine("Vill du logga in som admin?");
                if (UserInput.GetYesOrNo())
                {
                    Console.WriteLine("Loggar in som admin.");
                    admin.DisplayAdminMenu();
                    return MenuResult.Continue;
                }

                Console.WriteLine("Loggar in som användare.");
            }

            do
            {
                List<MenuOption> menuOptions = new List<MenuOption>();

                menuOptions.Add(new MenuOption("Lägg till nytt konto.", () => UserCreateAccount()));

                menuOptions.Add(new MenuOption("Visa användaruppgifter.", () => DisplayUserInfo()));

                menuOptions.Add(new MenuOption("Låna pengar av banken.", UserLoan));
                menuOptions.Add(new MenuOption("Betala pengar tillbaka till banken.", PayBackUserLoan));

                foreach (BankAccount account in _accounts)
                {
                    menuOptions.Add(new MenuOption($"Konto: {account.Name}", () => DisplayAccountMenu(account)));
                }

                Menu accountMenu = new Menu(menuOptions, $"Kontohantering - {Username}");
                menuResult = accountMenu.Display();
            } while (menuResult == MenuResult.Rebuild);

            return MenuResult.Continue;
        }

        private MenuResult DisplayUserInfo()
        {
            Console.WriteLine("------");
            Console.WriteLine($"Namn: N/A");
            Console.WriteLine($"Email: N/A");
            Console.WriteLine($"Telefon: N/A");
            Console.WriteLine($"Du är skyldig att betala tillbaka {userLoanDebt}kr till banken.");
            Console.WriteLine($"Din kreditvärdighet är {userCreditScore}");
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
                    Console.WriteLine($"Kontotyp: {account.AccountType}");
                    Console.WriteLine($"Valuta: {account.displayCurrency.ToString()}");
                    Console.WriteLine($"Saldo: {account.getCurrencyDisplayString(account.GetBalance())}");
                    if (account is SavingsAccount savingsAccount)
                    {
                        savingsAccount.ShowSavingsInterest();
                    }
                    UserInput.WaitForUserConfirm();
                    return MenuResult.Continue;
                }),
                new MenuOption("Överföra pengar", () => StartUserTransaction(account)),
                new MenuOption("Transaktions historik", () => TransactionHistory(account)),
                new MenuOption("Ändra valuta", () => UserChangeAccountCurrency(account))
            }, account.Name);
            accountMenu.Display();

            return MenuResult.Continue;
        }

        private MenuResult UserChangeAccountCurrency(BankAccount account)
        {
            Currency? chosenCurrency = UserInput.UserGetObjectFromList(Enum.GetValues<Currency>().ToList<Currency>(), type => type.ToString());
            if (chosenCurrency.HasValue)
            {
                account.setDisplayCurrency(chosenCurrency.Value);
            }
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
                BankAccount? targetedAccount = UserSelectAccount();

                if (targetedAccount != null && targetedAccount != account)
                {
                    Console.WriteLine($"Hur mycket vill du föra över?: ({account.displayCurrency.ToString()})");
                    decimal transferAmount = Bank.ExchangeRates.Convert(UserInput.GetDecimal("Du behöver skriva in en siffra."), account.displayCurrency, Currency.SEK);

                    account.Transactions(targetedAccount, transferAmount);
                }
                else if (targetedAccount == account)
                {
                    Console.WriteLine("Du kan inte föra över till samma konto");
                    UserInput.WaitForUserConfirm();
                }
                else
                {
                    Console.WriteLine($"Inget konto hittades.");
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
                        Console.WriteLine($"Hur mycket vill du föra över? ({account.displayCurrency.ToString()}):");
                        decimal transferAmount = Bank.ExchangeRates.Convert(UserInput.GetDecimal("Du behöver skriva in en siffra."), account.displayCurrency, Currency.SEK);

                        if (account.Transactions(targetedAccount, transferAmount))
                        {
                            Console.WriteLine($"Du förde över {account.getCurrencyDisplayString(transferAmount)} till {targetedAccount.Name}.");
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
        private MenuResult TransactionHistory(BankAccount account)
        {
            account.ExecuteTransactions();

            if (account.TransactionsHistory.Count == 0)
            {
                Console.WriteLine("Din transaktions historik är tom.");
                return MenuResult.Continue;
            }
            foreach (var transaction in account.TransactionsHistory)
            {
                Console.WriteLine(transaction);
            }
            if (account.PendingTransactionsList.Count > 0)
            {
                Console.WriteLine("\n--- Väntande överföringar ---");
                foreach (var pending in account.PendingTransactionsList)
                {
                    Console.WriteLine($"{account.getCurrencyDisplayString(pending.Amount)} till {account.Name}, genomförs kl {pending.TransactionTime:HH:mm:ss}");
                }
            }
            return MenuResult.Continue;
        }

        public BankAccount? UserSelectAccount() => UserInput.UserGetObjectFromList<BankAccount>(_accounts, a => a.Name);
    }
}