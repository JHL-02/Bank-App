namespace Bank_App
{
    internal class BankAccount
    {
        public decimal Balance { get; protected set; }
        public Currency displayCurrency { get; protected set; } = Currency.SEK;
        public string Name { get; protected set; }
        public const int MaxNameLength = 20;
        public const int MaxAccountAmount = 10;
        
        public List<string> transactionsHistory = new List<string>();

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
                Balance -= transferAmount;
                targetedAccount.Balance += transferAmount;
                transactionsHistory.Add($"{Name} skickade {transferAmount}kr till {targetedAccount.Name} {DateTime.Now}");
                return true;
            }
        }
        public void setDisplayCurrency(Currency newCurrency)
        {
            displayCurrency = newCurrency;
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
