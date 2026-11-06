namespace Bank_App
{
    // Represents a bank account that can calculate savings interest.
    internal class SavingsAccount : BankAccount
    {
        // Savings account.
        public SavingsAccount(User owner, string name, Currency currency = Currency.SEK) : base(owner, name, currency)// The currency is passed to BankAccount; otherwise a savings account would always use SEK.
        {
            AccountType = "Sparkonto";
        }
        public decimal savingsAccountRate { get; private set; } = 0.02m; // Standard savings rate for all customers. 2%

        // Savings interest.
        public void ShowSavingsInterest()
        {
            if (Balance > 0)
            {
                // Interest is calculated on the balance in the account's current currency.
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

        // Invalid input handling.
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

