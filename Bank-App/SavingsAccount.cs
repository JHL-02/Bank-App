namespace Bank_App
{
    internal class SavingsAccount : Account
    {
        //Sparkonto och ränta: Som användare vill jag kunna öppna sparkonto i banken och när jag sätter in dem få se hur mycket ränta jag kommer kunna få på pengarna jag sätter in.  - KLAR
        //Lån och räntekostnad: Som användare vill jag kunna låna pengar av banken och direkt få se hur mycket ränta jag kommer behöva betala på det lånet. - KLAR
        protected List<SavingsAccount> savingAccountsList = new List<SavingsAccount>();
        public SavingsAccount(string name, int banknumber) : base(name)
        {

        }

        //  Lån och räntekostnad: Som användare vill jag kunna låna pengar av banken och direkt få se hur mycket ränta jag kommer behöva betala på det lånet.
        //  Kreditgräns(Max 5x): Som bankägare vill jag begränsa hur mycket varje kund kan låna av banken så att de maximalt kan låna 5 ggr de pengar de redan har hos banken.

        private int loanLimit = 0;
        public decimal userSavings { get; protected set; } // Användarens pengar på sparkonto. 
        public decimal userBalance { get; protected set; } // Användarens pengar på privatkonto. 
        public decimal userCreditScore { get; private set; }  = 100m; // Standard kreditvärdighet för nya kunder. 
        public decimal savingsAccountRate { get; private set; } = 0.02m; // Standard sparränta för alla kunder. 2%
        public decimal userExtraCreditRate { get; private set; } = 0.05m; // Högre räntevärde för en lägre kreditvärdighet.
        public decimal loanCreditRate { get; private set; } = 0.03m; // Standard lånränta på lånet. 3%
        public void ShowSavingsInterest()
        {
            decimal yearlyReturn = userSavings * savingsAccountRate;
            Console.WriteLine($"Sparräntan är {savingsAccountRate:P} och ger dig en avkastning per år: {yearlyReturn:C}");
        }
        public void UserLoan(decimal loanAmount)
        {
            decimal creditLimit = (userSavings + userBalance) * 5;
            decimal yearlyInterest = loanAmount * CalculateCreditRate(); // Årsränta på privatlån. 
            decimal loanSum = yearlyInterest + loanAmount;

            if (loanAmount > creditLimit || userCreditScore <= 20)
            {
                Console.WriteLine("Ditt lån har inte blivit godkänt av banken.");
            }
            else
            {
                loanLimit++;
                userCreditScore = (userCreditScore - 20);
                Console.WriteLine($"Din ansökan för ett belopp på {loanAmount} har blivit godkänd. Årsränta: {yearlyInterest}. Totalkostnad: {loanSum}.");
                Console.WriteLine("Vilket konto vill du att pengarna skall överföras till? \n1. Privatkonto 2. Sparkonto");

                switch (IntInput())
                {
                    case 1:
                        userBalance += loanAmount;
                        Console.WriteLine("Pengarna har överförts till Privatkontot.");
                        break;
                    case 2:
                        userSavings += loanAmount;
                        Console.WriteLine($"Pengarna har överförts till Sparkontot. Nytt saldo: {userSavings}");

                        break;
                    default:
                        Console.WriteLine("Ogiltigt val. Välj 1 eller 2.");
                        break;
                }
            }

        }

        // som administratör vill jag kunna sätta regler för hur låneräntor ska beräknas beroende på olika faktorer(te.x användarens kredivärdighet)
        public decimal AdjustCreditRate()
        {
            Console.WriteLine($"Standardränta: {loanCreditRate:P} för alla kunder. Varje ny kund börjar med :{userCreditScore} i kreditvärdighet. " +
                $"1  Öka räntenivå baserat på kreditvärdighet. " + // se över denna
                $"2. Minska räntenivå baserat på kreditvärdighet. " + // se över denna
                $"3. Valfri räntemängd." + // BRA
                $"4. Avsluta.");

            switch (IntInput())
            {
                case 1:
                     Console.WriteLine($"Räntan baseras på kreditvärdighet. Kunder med kreditvärdighet under 60 poäng får {userExtraCreditRate:P}% ränta istället för {loanCreditRate:P}%.\n1. Addera mer på räntan. 2. Avsluta.");
                     switch(IntInput())
                    {
                        case 1:
                            Console.WriteLine("Ange ny räntemängd: (Decimalform 0.00)");
                            userExtraCreditRate += DecimalInput();
                            break;
                        case 2:
                            break;
                    }
                     break;

                case 2:
                    Console.WriteLine($"Räntan baseras på kreditvärdighet. Kunder med kreditvärdighet under 60 poäng får {userExtraCreditRate:P}% ränta istället för {loanCreditRate:P}%.\n1. Reducera mer på räntan. 2. Avsluta.");
                    switch (IntInput())
                    {
                        case 1:
                            Console.WriteLine("Ange ny räntemängd: (Decimalform 0.00)");
                            userExtraCreditRate -= DecimalInput();
                            break;
                        case 2:
                            break;
                    }
                    break; 
                case 3:
                    Console.WriteLine("Ange ett nytt standardvärde för räntan:");
                    decimal newRate = DecimalInput();
                    if (newRate <= 100)
                    {
                        loanCreditRate = newRate; // Använder värdet du precis samlade in
                    }
                    else
                    {
                        Console.WriteLine("Talet kan inte vara större än 100.");
                    }
                    break;
                case 4:
                    Console.Clear();
                    break;
            }
            return loanCreditRate;

        }
        public int IntInput() // Int input för menyer med felhantering.
        {
            int userMenuOutput;
            while (!int.TryParse(Console.ReadLine(), out userMenuOutput))
            {
                Console.WriteLine("Felaktig inmatning.");
            }
            return userMenuOutput;
        }
        public decimal DecimalInput() // Decimal input för menyer med felhantering.
        {
            decimal userDecimalInput;
            while (!decimal.TryParse(Console.ReadLine(), out userDecimalInput) || userDecimalInput < 0m)
            {
                Console.WriteLine("Felaktig inmatning. Ange ett giltigt decimaltal (t.ex. 0.03):");
            }
            return userDecimalInput;
        }
        public decimal CalculateCreditRate()
        {
            if (userCreditScore < 60)
            {
                return userExtraCreditRate;
            }
            return loanCreditRate;
        }
    }
}

