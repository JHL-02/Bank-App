namespace Bank_App
{
    internal class SavingsAccount : Account
    {
        // Sparkonto
        protected List<SavingsAccount> savingAccountsList = new List<SavingsAccount>();
        public SavingsAccount(string name, int banknumber) : base(name, banknumber)
        {

        }

        // Kapital
        public decimal userSavings { get; protected set; } // Användarens pengar på sparkonto. 
        public decimal userBalance { get; protected set; } // Användarens pengar på privatkonto. 
        public decimal userLoanDebt { get; private set; } // Användarens skuld hos banken.

        // Ränta & Kreditvärdighet
        public int userCreditScore { get; private set; }  = 100; // Standard kreditvärdighet för nya kunder. 
        public decimal savingsAccountRate { get; private set; } = 0.02m; // Standard sparränta för alla kunder. 2%
        public decimal loanExtraCreditRate { get; private set; } = 0.05m; // Högre räntevärde för en lägre kreditvärdighet.
        public decimal loanCreditRate { get; private set; } = 0.03m; // Standard lånränta på lånet. 3%
        
        // Sparränta
        public void ShowSavingsInterest()
        {
            if (userSavings > 0)
            {
                decimal yearlyReturn = userSavings * savingsAccountRate;
                Console.WriteLine($"Bankens sparränta: {savingsAccountRate:P}. Avkastning per år:{yearlyReturn:C}.");
            }
            else
            {
                Console.WriteLine("Det finns inga pengar på ditt sparkonto.");
            }

        }

        // Privatlån

        public void UserLoan(decimal loanAmount)
        {
            decimal creditLimit = (userSavings + userBalance) * 5 - userLoanDebt;
            if (creditLimit < 0m)
            {
                creditLimit = 0m;
            }
            decimal yearlyInterest = loanAmount * CalculateCreditRate();
            decimal loanSum = yearlyInterest + loanAmount;

            if (loanAmount > creditLimit || userCreditScore <= 20 || loanAmount <= 0m)
            {
                Console.WriteLine("Ditt lån har inte blivit godkänt av banken.");
                return;
            }

            Console.WriteLine($"Din ansökan för ett belopp på {loanAmount:C} har blivit godkänd.");
            Console.WriteLine($"Årsränta: {yearlyInterest:C}. Totalkostnad: {loanSum:C}.");

            int accountChoice;

            do
            {
                Console.WriteLine("Vilket konto vill du att pengarna ska överföras till?");
                Console.WriteLine("1. Privatkonto");
                Console.WriteLine("2. Sparkonto");

                accountChoice = IntInput();

                if (accountChoice != 1 && accountChoice != 2)
                {
                    Console.WriteLine("Ogiltigt val. Välj 1 eller 2.");
                }

            } while (accountChoice != 1 && accountChoice != 2);

            switch (accountChoice)
            {
                case 1:
                    userBalance += loanAmount;
                    Console.WriteLine("Pengarna har överförts till privatkontot.");
                    break;

                case 2:
                    userSavings += loanAmount;
                    Console.WriteLine($"Pengarna har överförts till sparkontot. Nytt saldo: {userSavings:C}");
                    break;
            }
            userLoanDebt += loanAmount;
            userCreditScore -= 20;
        }

        // Kreditränta
        public decimal CalculateCreditRate()
        {
            if (userCreditScore < 60)
            {
                return loanExtraCreditRate;
            }
            return loanCreditRate;
        }

        // Justera kreditränta. (Admin)
        public decimal AdjustCreditRate()
        {
            Console.WriteLine(
                $"Standardränta: {loanCreditRate:P} för alla kunder.\n" +
                $"Ny kund börjar med: {userCreditScore} i kreditvärdighet.\n" +
                "1. Öka räntenivå baserat på kreditvärdighet.\n" +
                "2. Minska räntenivå baserat på kreditvärdighet.\n" +
                "3. Valfri räntemängd.\n" +
                "4. Avsluta.");

            switch (IntInput())
            {
                case 1:
                     Console.WriteLine(
                         $"Räntan baseras på kreditvärdighet. Kunder med kreditvärdighet under 60 poäng får {loanExtraCreditRate:P} ränta istället för {loanCreditRate:P}.\n" +
                         $"1. Addera mer på räntan.\n" +
                         $"2. Avsluta.\n");
                     switch(IntInput())
                    {
                        case 1:
                            Console.WriteLine("Ange hur mycket räntan ska ökas (t.ex. 0.01 för 1 procentenhet):");
                            decimal increase = DecimalInput();
                            loanExtraCreditRate += increase;
                            Console.WriteLine($"Den nya extra räntan är {loanExtraCreditRate:P}.");
                            break;
                        case 2:
                            break;
                    }
                     break;
                case 2:
                    Console.WriteLine(
                        $"Räntan baseras på kreditvärdighet. Kunder med kreditvärdighet under 60 poäng får {loanExtraCreditRate:P} ränta istället för {loanCreditRate:P}.\n" +
                        $"1. Reducera mer på räntan.\n" +
                        $"2. Avsluta.\n");
                    switch (IntInput())
                    {
                        case 1:
                            Console.WriteLine("Ange hur mycket räntan ska minskas (t.ex. 0.01 för 1 procentenhet):");
                            decimal reduction = DecimalInput();
                            decimal newExtraRate = loanExtraCreditRate - reduction;

                            if (newExtraRate >= 0m)
                            {
                                loanExtraCreditRate = newExtraRate;
                                Console.WriteLine($"Den nya extra räntan är {loanExtraCreditRate:P}.");
                            }
                            else
                            {
                                Console.WriteLine("Räntan kan inte bli negativ.");
                            }
                            break;
                        case 2:
                            break;
                        default:
                            Console.WriteLine("Felaktig inmatning. Vänligen ange ett nummer mellan 1-2.");
                            break;
                    }
                    break; 
                case 3:
                    Console.WriteLine("Ange ett nytt standardvärde för standardränta hos banken:");
                    decimal newRate = DecimalInput();
                    if (newRate <= 1m)
                    {
                        loanCreditRate = newRate;
                    }
                    else
                    {
                        Console.WriteLine("Talet kan inte vara större än 100.");
                    }
                    break;
                case 4:
                    Console.Clear();
                    break;
                default:
                    Console.WriteLine("Felaktig inmatning. Vänligen ange ett nummer mellan 1-4.");
                    break;
            }
            return loanCreditRate;
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

