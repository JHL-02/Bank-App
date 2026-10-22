namespace Bank_App
{
    internal class BankAccount
    {
        public decimal Balance { get; protected set; }
        public string Name { get; protected set; }
        
        public BankAccount(string name)
        {
            Name = name;
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
                return true;
            }
        }

    }
}
