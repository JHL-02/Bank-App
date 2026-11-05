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
            if (Balance > 0)
            {
                decimal yearlyReturn = Balance * savingsAccountRate;
                Message.WriteLine(new Message
                    (
                    $"Bankens sparränta: {savingsAccountRate:P}.\n" +
                    $"Avkastning per år:{getCurrencyDisplayString(yearlyReturn):C}."
                    ));
            }
            else
            {
                Message.WriteLine(new Message("Det finns inga pengar på ditt sparkonto.", MessageType.ERROR));
            }
        }

        // Felinmatning
        public int IntInput()
        {
            int userMenuOutput;
            while (!int.TryParse(Console.ReadLine(), out userMenuOutput))
            {
                Message.WriteLine(new Message("Felaktig inmatning. Ange ett giltigt heltal: "));
            }
            return userMenuOutput;
        }
        public decimal DecimalInput() 
        {
            decimal userDecimalInput;
            while (!decimal.TryParse(Console.ReadLine(), out userDecimalInput) || userDecimalInput < 0m)
            {
                Message.WriteLine(new Message("Felaktig inmatning. Ange ett giltigt decimaltal (t.ex. 0.03):"));
            }
            return userDecimalInput;
        }
    }
}

