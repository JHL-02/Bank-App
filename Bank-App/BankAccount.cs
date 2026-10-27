using System.Transactions;

namespace Bank_App
{
    internal class BankAccount
    {
        public decimal Balance { get; protected set; }
        public Currency displayCurrency { get; protected set; } = Currency.SEK;
        public string Name { get; protected set; }
        public const int MaxNameLength = 20;
        public const int MaxAccountAmount = 10;
       // public DateTime TransactionTime { get; set; }
        
        public List<string> TransactionsHistory = new List<string>();
        public List<(BankAccount Account, decimal Amount, DateTime TransactionTime)> TransactionList = new();
        

        public BankAccount(string name)
        {
            if (name.Trim().Length > MaxNameLength)
            {
                throw new ArgumentOutOfRangeException(name);
            }

            Name = name.Trim();
            Balance = 0.0m;
        }

        public void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                Balance += amount;
            }
        }

        public void Withdraw(decimal amount)
        {
            if (amount > 0)
            {
                Balance -= amount;
            }
        }
        public bool Transactions(BankAccount targetedAccount, decimal transferAmount)
        {
            if (transferAmount > Balance || Balance <= 0)
            {
                Console.WriteLine("För litet saldo");
                return false;
            }
            else
            {
                DateTime realTime = DateTime.Now;
                DateTime transferedTime = realTime.AddMinutes(15);
                Balance -= transferAmount;
                TransactionList.Add((targetedAccount, transferAmount, transferedTime));
                //TransactionsHistory.Add($"{Name} skickade {transferAmount}kr till {targetedAccount.Name} {transferedTime}");
                return true;
            }
        }
      
        public void setDisplayCurrency(Currency newCurrency)
        {
            displayCurrency = newCurrency;
        }
      
        public void ExecuteTransactions()
        {
            var transactions = TransactionList.Where(t => t.TransactionTime <= DateTime.Now).ToList();
            foreach (var transaction in transactions)
            {
                transaction.Account.Deposit(transaction.Amount);
                TransactionList.Remove(transaction);
                TransactionsHistory.Add($"{Name} skickade {transaction.Amount}kr till {transaction.Account.Name} {DateTime.Now}");
            }
        }

        public string getCurrencyDisplayString()
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
            decimal convertedBalance = Bank.ExchangeRates.Convert(Balance, Currency.SEK, displayCurrency);

            return placeUnitBefore ? $"{currencyUnit}{convertedBalance}" : $"{convertedBalance}{currencyUnit}";
        }
    }
}
