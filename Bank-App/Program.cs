namespace Bank_App
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
    


        }

        public static void Transactions()
        {
            bool isRunning = true;
            decimal bankAcount = 200;
            decimal savingsAcount = 600;

            Console.WriteLine("Vilket konto vill du flytta från?");

            string input = Console.ReadLine().ToUpper();
            while (isRunning)
            {
                if (input == "BANKKONTO")
                {
                    Console.WriteLine("Hur mycket vill du föra över?");
                    string transferAmount = Console.ReadLine();
                    decimal amount = decimal.Parse(transferAmount);

                    if (amount < bankAcount)
                    {
                        bankAcount += amount;
                        savingsAcount -= amount;
                        Console.WriteLine($"summa på bank konto: {bankAcount}");
                        Console.WriteLine($"summa på spar konto: {savingsAcount}");
                    }
                    else
                    {
                        Console.WriteLine("finns inte så mycket pengar på kontot");
                        Console.WriteLine("testa igen");

                    }

                }
                else if (input == "SPARKONTO")
                {
                    Console.WriteLine("Hur mycket vill du föra över?");
                    string transferAmount = Console.ReadLine();
                    decimal amount = decimal.Parse(transferAmount);

                    if (amount < savingsAcount)
                    {
                        savingsAcount += amount;
                        bankAcount -= amount;
                        Console.WriteLine($"summa på sparkonto: {savingsAcount}");
                        Console.WriteLine($"summa på bankkonto: {bankAcount}");
                    }
                    else
                    {
                        Console.WriteLine("finns inte så mycket pengar på kontot");
                        Console.WriteLine("testa igen");
                    }
                }
            }
            



        }
    }
}
