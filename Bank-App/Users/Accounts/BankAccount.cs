using System.Data.Common;
using System.Transactions;

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
        public Stack<(string AccountName, decimal Amount, DateTime TransactionTime, bool IsSender)> PendingTransactionsList = new();


        public BankAccount(User owner, string name, Currency currency = Currency.SEK)
        {
            AccountType = "Privatkonto";
            accountOwner = owner;
            string newName = name.Trim();
            if (newName.Length > MaxNameLength)
            {
                throw new ArgumentOutOfRangeException(name);
            }

            Name = newName;
            displayCurrency = currency;
            Balance = Bank.ExchangeRates.Convert(500.0m, Currency.SEK, currency); //500 SEK räknas om till kontots valuta. USD-konto börjar med cirka 47,62 dollar, inte 500 dollar.
        }

        public decimal GetBalance()
        {
            return Balance;
        }

        public void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                Balance += amount;
            }
        }

        public bool Withdraw(decimal amount)
        {

            if (amount <= 0m || amount > Balance)
                return false;

            Balance -= amount;
            return true;
        }
        public bool Transactions(BankAccount targetedAccount, decimal transferAmount)
        {
            if (targetedAccount == this)
            {
                Message.WriteLine(new Message("Du kan inte överföra till samma konto.", MessageType.ERROR));
                return false;
            }

            if (transferAmount <= 0)
            {
                Message.WriteLine(new Message("Du kan inte överföra 0 eller mindre pengar.", MessageType.ERROR));
                return false;
            }

            if (transferAmount > Balance || Balance <= 0)
            {
                Message.WriteLine(new Message("För litet saldo", MessageType.ERROR));
                return false;
            }
            else
            {
                decimal receivedAmount = Bank.ExchangeRates.Convert(transferAmount, displayCurrency, targetedAccount.displayCurrency);
                if (receivedAmount <= 0m)
                {
                    Console.WriteLine("Summan är för liten för att växlas.");
                    return false;
                }

                DateTime transferedTime = DateTime.Now.AddMinutes(15);
                Withdraw(transferAmount);
                targetedAccount.AddPendingTransaction(Name,receivedAmount, transferedTime, false); //false = mottagaren får den växlade summan.
                AddPendingTransaction(targetedAccount.Name, transferAmount, transferedTime, true);
                
                return true;
            }
        }

        public void AddPendingTransaction(string accountName,decimal transferAmount, DateTime transferedTime, bool isSender)
        {
            if (transferAmount <= 0m)
                throw new ArgumentOutOfRangeException(nameof(transferAmount));

            PendingTransactionsList.Push((accountName, transferAmount, transferedTime, isSender));
        }

        //setDisplayCurrency är borttagen.
        // Valutan ska inte gå att byta efter att kontot har skapats.

        public void ExecuteTransactions()
        {
            if (PendingTransactionsList.Count < 1)
            {
                return;
            }

            while (PendingTransactionsList.Count > 0)
            {
                var transaction = PendingTransactionsList.Pop();
                if (!transaction.IsSender)
                {
                    Deposit(transaction.Amount);
                }
                
                if (transaction.IsSender)
                {
                    TransactionsHistory.Add($"{transaction.AccountName} tog emot {transaction.Amount}kr från dig {DateTime.Now}");
                }
                else
                {
                    TransactionsHistory.Add($"{transaction.AccountName} skickade {transaction.Amount}kr till dig {DateTime.Now}");
                }
                
                
                
            }
            //accountOwner.MarkTransactionsUpdated();
        }

        public string getCurrencyDisplayString(decimal number)
        {
            string  formatted = number.ToString("F2");
            switch (displayCurrency)
            {
                case Currency.SEK:
                    return $"{formatted} kr";
                case Currency.USD:
                    return $"${formatted}";
                case Currency.EUR:
                    return $"€{formatted}";
                default:
                    return formatted;
            }
            //Talet är redan i kontots valuta.
            // Vi växlar inte en gång till, för då blir summan fel.
        }
    }
}
