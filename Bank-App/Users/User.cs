using System.Text;

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
                    (int)(Math.Round((GetTotalBalance() - userLoanDebt) * 25 / userLoanDebt)),
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
            Message.WriteLine(new Message("Hur mycket pengar vill du låna av banken? (SEK):", MessageType.INPUT));
            decimal loanAmount = UserInput.GetDecimal("Du behöver skriva in en siffra.");

            if (loanAmount <= 0m)
            {
                Message.WriteLine(new Message("Summan är för låg.", MessageType.ERROR));
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
                Message.WriteLine(new Message("Ditt lån har inte blivit godkänt av banken.", MessageType.ERROR));
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            Message.WriteLine(new Message
                (
                $"Din ansökan för ett belopp på {loanAmount:C} har blivit godkänd." +
                $"\nÅrsränta: {yearlyInterest:C}. Totalkostnad: {loanSum:C}."
                ));

            Message.WriteLine(new Message("Vilket konto vill du att pengarna ska överföras till?", MessageType.INPUT));
            BankAccount? bankAccount = UserSelectAccount();

            if (bankAccount == null)
            {
                Message.WriteLine(new Message("Du har inga konton."));
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            bankAccount.Deposit(loanAmount);
            if (bankAccount.displayCurrency != Currency.SEK)
            {
                Message.WriteLine(new Message("Konverterar valuta."));
            }
            Message.WriteLine(new Message($"Pengarna har överförts till {bankAccount.Name}."));

            userLoanDebt += loanSum;
            return MenuResult.Continue;
        }
        public MenuResult PayBackUserLoan()
        {
            if (userLoanDebt <= 0m)
            {
                Message.WriteLine(new Message("Du har inget att betala tillbaka."));
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            Message.WriteLine(new Message("Vilket konto vill du betala med?", MessageType.INPUT));
            BankAccount? payingAccount = UserSelectAccount();

            if (payingAccount == null)
            {
                Message.WriteLine(new Message("Du har inga konton.", MessageType.ERROR));
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            Message.WriteLine(new Message($"Hur mycket vill du betala tillbaka? ({payingAccount.displayCurrency.ToString()}):", MessageType.INPUT));
            decimal amount = Bank.ExchangeRates.Convert(UserInput.GetDecimal("Du behöver skriva in en siffra."), payingAccount.displayCurrency, Currency.SEK);

            if (amount <= 0m)
            {
                Message.WriteLine(new Message("Summan är för låg.", MessageType.ERROR));
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            decimal newAmount = userLoanDebt - amount;
            decimal userPays = Math.Min(amount, userLoanDebt);

            if (payingAccount.GetBalance() < userPays)
            {
                Message.WriteLine(new Message("För lågt saldo på konto.", MessageType.ERROR));
                return MenuResult.Continue;
            }

            if (!payingAccount.Withdraw(userPays))
            {
                Message.WriteLine(new Message("FEL: Det gick inte att ta pengar ur kontot.", MessageType.ERROR));
                return MenuResult.Continue;
            }

            userLoanDebt -= userPays;

            Message.WriteLine(new Message($"Du har betalat tillbaka {payingAccount.getCurrencyDisplayString(userPays)} till banken."));
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
                Message.WriteLine(new Message($"Du kan inte skapa mer än {BankAccount.MaxAccountAmount} konton."));
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }
            Message.WriteLine(new Message("1. Bankkonto\n2. Sparkonto"));
            int accountTypeInput;
            while (true)
            {
                accountTypeInput = UserInput.GetInt();

                if (accountTypeInput == 1 || accountTypeInput == 2)
                {
                    break;
                }
                Message.WriteLine(new Message("Välj ett alternativ ur listan.", MessageType.INPUT));
            }
            bool isSavingsAccount = accountTypeInput == 2;


            Message.WriteLine(new Message($"Ange ett namn för det nya kontot (max {BankAccount.MaxNameLength} tecken, inga mellanslag):", MessageType.INPUT));
            string input;
            string proposedName;
            while (true)
            {
                input = UserInput.GetString("Du behöver skriva in ett giltigt kontonamn.");

                if (input.Any(char.IsWhiteSpace))
                {
                    Message.WriteLine(new Message("Kontonamnet får inte innehålla mellanslag.", MessageType.ERROR));
                    continue;
                }

                proposedName = input.Trim();


                if (proposedName.Length <= BankAccount.MaxNameLength)
                {
                    break;
                }

                Message.WriteLine(new Message($"Ditt namn kan inte vara längre än {BankAccount.MaxNameLength} tecken.", MessageType.ERROR));
            }

            Message.WriteLine(new Message($"Är du säker på kontonamnet \"{proposedName}\"?", MessageType.INPUT));

            if (!UserInput.GetYesOrNo())
            {
                Message.WriteLine(new Message("Avbryter."));
                return MenuResult.Continue;
            }


            if (AddAccount(proposedName, isSavingsAccount))
            {
                Message.WriteLine(new Message("Ditt konto har skapats."));
                UserInput.WaitForUserConfirm();
                return MenuResult.Rebuild;
            }
            Message.WriteLine(new Message("Ditt konto kunde inte skapas då namnet redan används.", MessageType.ERROR));
            UserInput.WaitForUserConfirm();
            return MenuResult.Continue;
        }

        public MenuResult DisplayAccountMenu()
        {
            TimerService.OnTimerTick += NotifyUser;

            try
            {
                MenuResult menuResult;

                if (this is Admin admin)
                {
                    Message.WriteLine(new Message("Vill du logga in som admin?", MessageType.INPUT));
                    if (UserInput.GetYesOrNo())
                    {
                        admin.DisplayAdminMenu();
                        return MenuResult.Continue;
                    }
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
            finally
            {
                TimerService.OnTimerTick -= NotifyUser;
            }
        }

        private MenuResult DisplayUserInfo()
        {
            Message.WriteLine(new Message(
                $"Namn: N/A" +
                $"\nEmail: N/A" +
                $"\nTelefon: N/A" +
                $"\nDu är skyldig att betala tillbaka {userLoanDebt} till banken." +
                $"\nDin kreditvärdighet är {userCreditScore}"
                ));
            UserInput.WaitForUserConfirm();
            return MenuResult.Continue;
        }

        public MenuResult DisplayAccountMenu(BankAccount account)
        {
            Menu accountMenu = new Menu(new List<MenuOption>
            {
                new MenuOption("Visa info.", () =>
                {
                    Message.WriteLine(new Message(
                        $"Kontonamn: {account.Name}" +
                        $"\nKontotyp: {account.AccountType}" +
                        $"\nValuta: {account.displayCurrency.ToString()}" +
                        $"\nSaldo: {account.getCurrencyDisplayString(account.GetBalance())}"
                        ));
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
            Message.WriteLine(new Message("Vill du föra över til dig själv?", MessageType.INPUT));
            if (UserInput.GetYesOrNo())
            {
                Message.WriteLine(new Message("Vilket konto vill du föra över till?", MessageType.INPUT));
                BankAccount? targetedAccount = UserSelectAccount();

                if (targetedAccount != null && targetedAccount != account)
                {
                    Message.WriteLine(new Message($"Hur mycket vill du föra över?: ({account.displayCurrency.ToString()})"));
                    decimal transferAmount = Bank.ExchangeRates.Convert(UserInput.GetDecimal("Du behöver skriva in en siffra."), account.displayCurrency, Currency.SEK);

                    account.Transactions(targetedAccount, transferAmount);
                }
                else if (targetedAccount == account)
                {
                    Message.WriteLine(new Message("Du kan inte föra över till samma konto", MessageType.ERROR));
                    UserInput.WaitForUserConfirm();
                }
                else
                {
                    Message.WriteLine(new Message($"Inget konto hittades.", MessageType.ERROR));
                    UserInput.WaitForUserConfirm();
                }
            }
            else
            {
                Message.WriteLine(new Message("Vilken användare vill du föra över till?", MessageType.INPUT));
                string input = UserInput.GetString();

                User? targetedUser = Bank.UserDatabase.GetUserWithUsername(input);
                if (targetedUser != null)
                {
                    Message.WriteLine(new Message("Ange namnet på kontot som du vill föra över till:", MessageType.INPUT));
                    string userAccountTargetString = UserInput.GetString("Felaktig inmatning.");

                    BankAccount? targetedAccount = targetedUser.GetAccountWithName(userAccountTargetString);

                    if (targetedAccount != null)
                    {
                        Message.WriteLine(new Message($"Hur mycket vill du föra över? ({account.displayCurrency.ToString()}):", MessageType.INPUT));
                        decimal transferAmount = Bank.ExchangeRates.Convert(UserInput.GetDecimal("Du behöver skriva in en siffra."), account.displayCurrency, Currency.SEK);

                        if (account.Transactions(targetedAccount, transferAmount))
                        {
                            Message.WriteLine(new Message($"Du förde över {account.getCurrencyDisplayString(transferAmount)} till {targetedAccount.Name}."));
                        }
                        else
                        {
                            Message.WriteLine(new Message("Det gick inte att föra över.", MessageType.ERROR));
                        }
                        UserInput.WaitForUserConfirm();
                    }
                    else
                    {
                        Message.WriteLine(new Message($"Kunde inte hitta något konto med namnet {userAccountTargetString}", MessageType.ERROR));
                        UserInput.WaitForUserConfirm();
                    }
                }
                else
                {
                    Message.WriteLine(new Message($"Kunde inte hitta en användare med namnet {input}.", MessageType.ERROR));
                    UserInput.WaitForUserConfirm();
                }
            }
            return MenuResult.Continue;
        }
        private MenuResult TransactionHistory(BankAccount account)
        {
            if (account.TransactionsHistory.Count == 0 && account.PendingTransactionsList.Count == 0)
            {
                Message.WriteLine(new Message("Din transaktions historik är tom."));
                UserInput.WaitForUserConfirm();
                return MenuResult.Continue;
            }

            StringBuilder sb = new StringBuilder();
            if (account.PendingTransactionsList.Count > 0)
            {
                Message.WriteLine(new Message("\n--- Väntande överföringar ---"));

                foreach (var pending in account.PendingTransactionsList)
                {
                    if (sb.Length > 0)
                        sb.AppendLine();

                    if (pending.IsSender)
                    {
                        sb.Append($"{account.getCurrencyDisplayString(pending.Amount)} skickas till {pending.AccountName} kl {pending.TransactionTime:HH:mm:ss}");
                    }
                    else
                    {
                        sb.Append($"{account.getCurrencyDisplayString(pending.Amount)} skickas till {pending.AccountName} kl {pending.TransactionTime:HH:mm:ss}");
                    }
                }
                Message.WriteLine(new Message(sb.ToString()));
                sb.Clear();
            }
            foreach (var transaction in account.TransactionsHistory)
            {
                if (sb.Length > 0)
                    sb.AppendLine();

                sb.Append(string.Join(Environment.NewLine, transaction));
            }
            Message.WriteLine(new Message(sb.ToString()));
            return MenuResult.Continue;
        }

        public BankAccount? UserSelectAccount() => UserInput.UserGetObjectFromList<BankAccount>(_accounts, a => a.Name);

        //private int _updatedTransactions = 0;
        //public void MarkTransactionsUpdated()
        //{
        //    Interlocked.Exchange(ref _updatedTransactions, 1);
        //}
        private void NotifyUser()
        {
            _ = TimerService.ShowNotification($"Transaktioner uppdaterades.");
        }
    }
}