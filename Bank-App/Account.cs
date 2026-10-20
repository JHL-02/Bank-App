namespace Bank_App
{
    internal class Account
    {
        public decimal Balance { get; protected set; }
        public string Name { get; protected set; }

        public Account(string name)
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
    }
}
