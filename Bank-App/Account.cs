namespace Bank_App
{
    internal class Account
    {
        public decimal Balance { get; protected set; }
        public string Name { get; protected set; }
        public int Banknumber { get; protected set; }
        public Account(string name, int banknumber)
        {
            Name = name;
            Banknumber = banknumber;

        }

        public virtual void Transactions(Account targetedAccount, decimal transferAmount)
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
