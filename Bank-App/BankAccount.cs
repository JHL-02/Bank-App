namespace Bank_App
{
    internal class BankAccount
    {
        protected decimal Balance;
        public Currency displayCurrency { get; protected set; } = Currency.SEK;
        public string Name { get; protected set; }
        public const int MaxNameLength = 20;
        public const int MaxAccountAmount = 10;
        public string AccountType { get; init; }

        // public DateTime TransactionTime { get; set; }
        public User accountOwner { get; init; }

        public List<string> TransactionsHistory = new List<string>();
        public List<(decimal Amount, DateTime TransactionTime)> PendingTransactionsList = new();


        public BankAccount(User owner, string name)
        {
            AccountType = "Privatkonto";
            accountOwner = owner;
            string newName = name.Trim();
            if (newName.Length > MaxNameLength)
            {
                throw new ArgumentOutOfRangeException(name);
            }

            Name = newName;
            Balance = 500.0m;
        }

        public decimal GetBalance()
        {
            ExecuteTransactions();
            return Balance;
        }

        public void Deposit(decimal amount)
        {
            ExecuteTransactions();

            if (amount > 0)
            {
                Balance += amount;
            }
        }

        public void Withdraw(decimal amount)
        {
            ExecuteTransactions();

            if (amount > 0)
            {
                Balance -= amount;
            }
        }
        public bool Transactions(BankAccount targetedAccount, decimal transferAmount)
        {
            ExecuteTransactions();
            if (transferAmount > Balance || Balance <= 0)
            {
                Console.WriteLine("För litet saldo");
                return false;
            }
            else
            {
                DateTime realTime = DateTime.Now;
                DateTime transferedTime = realTime.AddMinutes(1);
                Balance -= transferAmount;
                targetedAccount.AddPendingTransaction(transferAmount, transferedTime);
                TransactionsHistory.Add($"Ditt konto {Name} skickade {transferAmount}kr till {targetedAccount.Name} {transferedTime}");
                return true;
            }
        }

        public void AddPendingTransaction(decimal transferAmount, DateTime transferedTime)
        {
            PendingTransactionsList.Add((transferAmount, transferedTime));
        }

        public void setDisplayCurrency(Currency newCurrency)
        {
            displayCurrency = newCurrency;
        }

        public void ExecuteTransactions()
        {
            var transactions = PendingTransactionsList.Where(t => t.TransactionTime <= DateTime.Now).ToList();
            foreach (var transaction in transactions)
            {
                Balance += transaction.Amount;
                PendingTransactionsList.Remove(transaction);
                TransactionsHistory.Add($"??? skickade {transaction.Amount}kr till dig {DateTime.Now}");
            }
        }

        public string getCurrencyDisplayString(decimal number)
        {
            string currencyUnit;
            bool placeUnitBefore = false;
            switch (displayCurrency)
            {
                case Currency.SEK:
                    currencyUnit = "kr";
                    break;
                case Currency.USD:
                    currencyUnit = "$";
                    placeUnitBefore = true;
                    break;
                case Currency.EUR:
                    currencyUnit = "€";
                    placeUnitBefore = true;
                    break;
                default:
                    currencyUnit = "";
                    break;
            }
            decimal convertedBalance = Bank.ExchangeRates.Convert(number, Currency.SEK, displayCurrency);

            return placeUnitBefore ? $"{currencyUnit}{convertedBalance}" : $"{convertedBalance}{currencyUnit}";
        }
    }
}
