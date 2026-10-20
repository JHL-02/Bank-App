namespace Bank_App
{
    internal class SavingsAccount : Account
    {
        //Sparkonto och ränta: Som användare vill jag kunna öppna sparkonto i banken och när jag sätter in dem få se hur mycket ränta jag kommer kunna få på pengarna jag sätter in.  - KLAR
        //Lån och räntekostnad: Som användare vill jag kunna låna pengar av banken och direkt få se hur mycket ränta jag kommer behöva betala på det lånet. - KLAR
            protected List<SavingsAccount> savingAccountsList = new List<SavingsAccount>();
            public SavingsAccount(string name, int banknumber, decimal balance, decimal creditscore) : base(name)
            {
                userCreditScore = creditscore;
                creditscore = 0.0m;
                userSavings += balance;
                ShowSavingsInterest();
            }

            //  Lån och räntekostnad: Som användare vill jag kunna låna pengar av banken och direkt få se hur mycket ränta jag kommer behöva betala på det lånet.
            //  Kreditgräns(Max 5x): Som bankägare vill jag begränsa hur mycket varje kund kan låna av banken så att de maximalt kan låna 5 ggr de pengar de redan har hos banken.

            private int loanLimit = 0;  // Den här ska egentligen till bankkontoklassen.
            public decimal userSavings { get; protected set; }
            public decimal userCreditScore { get; protected set; }
            public decimal userBalance { get; protected set; }
            public decimal creditRate = 0.06m; // Ränta på lånet. 6%
            public void ShowSavingsInterest()
            {
                decimal yearlyReturn = userSavings * 0.025m;
                Console.WriteLine($"Sparräntan är 2.5% och ger dig en avkastning per år: {yearlyReturn:C}");
            }
            public void UserLoan(decimal loanAmount) // Den här ska egentligen till bankkontoklassen. 
            {
                decimal creditLimit = (userSavings * 5);
                decimal yearlyInterest = loanAmount * creditRate; // Årsränta
                decimal loanSum = yearlyInterest + loanAmount;

                if (loanAmount > creditLimit || loanLimit >= 5)
                {
                    Console.WriteLine("Ditt lån har inte blivit godkänt av banken.");
                }
                else
                {
                    loanLimit++;
                    Console.WriteLine($"Din ansökan för ett belopp på {loanAmount} har blivit godkänd. Årsränta: {yearlyInterest}. Totalkostnad: {loanSum}.");
                    Console.WriteLine("Vilket konto vill du att pengarna skall överföras till? \n1. Privatkonto 2. Sparkonto");

                    int userMenuOutput;
                    while (!int.TryParse(Console.ReadLine(), out userMenuOutput))
                    {
                        Console.WriteLine("Felaktig inmatning. Vänligen ange 1 eller 2. ");
                    }
                    switch (userMenuOutput)
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
        // man ska kunna välja olika räntor vid menyn?
        public decimal AdjustCreditRate()
        {
            Console.WriteLine($"Nuvarande ränta: {creditRate}. Anpassa ränta efter kreditvärdighet.");
            return creditRate;

        }


        }
    }

