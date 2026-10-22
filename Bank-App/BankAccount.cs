namespace Bank_App
{
    internal class BankAccount
    {
        public decimal Balance { get; protected set; }
        public string Name { get; protected set; }
        public const int MaxNameLength = 20;

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
        public virtual void Transactions(BankAccount targetedAccount, decimal transferAmount)
        {
            if (transferAmount > Balance || Balance <= 0)
            {
                Console.WriteLine("För litet saldo");
            }
            else
            {
                Balance -= transferAmount;
                targetedAccount.Balance += transferAmount;

            }
        }

    }
}
