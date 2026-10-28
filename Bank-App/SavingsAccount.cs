namespace Bank_App
{
    internal class SavingsAccount : BankAccount
    {
        // Sparkonto
        public SavingsAccount(User owner, string name) : base(owner, name)
        {
            AccountType = "Sparkonto";
        }
        public decimal savingsAccountRate { get; private set; } = 0.02m; // Standard sparränta för alla kunder. 2%

        // Sparränta
        public void ShowSavingsInterest()
        {
            ExecuteTransactions();
            if (Balance > 0)
            {
                decimal yearlyReturn = Balance * savingsAccountRate;
                Console.WriteLine($"Bankens sparränta: {savingsAccountRate:P}. Avkastning per år:{getCurrencyDisplayString(yearlyReturn):C}.");
            }
            else
            {
                Console.WriteLine("Det finns inga pengar på ditt sparkonto.");
            }
        }

        // Felinmatning
        public int IntInput()
        {
            int userMenuOutput;
            while (!int.TryParse(Console.ReadLine(), out userMenuOutput))
            {
                Console.WriteLine("Felaktig inmatning. Ange ett giltigt heltal: ");
            }
            return userMenuOutput;
        }
        public decimal DecimalInput() 
        {
            decimal userDecimalInput;
            while (!decimal.TryParse(Console.ReadLine(), out userDecimalInput) || userDecimalInput < 0m)
            {
                Console.WriteLine("Felaktig inmatning. Ange ett giltigt decimaltal (t.ex. 0.03):");
            }
            return userDecimalInput;
        }
    }
}

