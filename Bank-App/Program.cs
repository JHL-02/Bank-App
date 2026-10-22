namespace Bank_App
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            //Login code
            /*
            var database = new UserDatabase();
            database.AddUser("JL", "password123");
            
            var login = new Login(database);
            login.TryLogin();

            Console.WriteLine("Succees?");
            */

            
            Bank.UserDatabase.AddUser("StandardUser", "1234");

            var startMenu = new Menu(new List<MenuOption>
            {
                new MenuOption("Konto", () =>
                {
                    Bank.UserDatabase.GetUserWithUsername("StandardUser")?.DisplayAccountMenu();
                    return MenuResult.Continue;
                })
            }, "Välkommen till bankappen","Avsluta");
            startMenu.Display();
            
            //Transactions();
            TransactionToOthers();
            
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
        public static void TransactionToOthers()
        {
            Dictionary<string, string> bankUser1 = new Dictionary<string, string>();
            Dictionary<string, string> bankUser2 = new Dictionary<string, string>();
            string user1 = "Daniel";
            string user2 = "Joakim";

            string bankId1 = "1996";
            string bankId2 = "1993";
            decimal user1Balance = 500;
            decimal user2Balance = 300;

            bankUser1.Add(user1, bankId1);
            bankUser2.Add(user2, bankId2);

            Console.WriteLine($"{user1} vill skicka pengar till {user2}");

            Console.WriteLine($"skriv in {user2} konto nummer");

            string input = Console.ReadLine();

            if (input == bankId2 && bankUser2.ContainsValue(bankId2))
            {
                Console.WriteLine("Hur mycket pengar vill du skicka?");
                string tranferAmount = Console.ReadLine();
                decimal amount = decimal.Parse(tranferAmount);

                user2Balance += amount;
                user1Balance -= amount;

                Console.WriteLine(user2Balance + " " + user1Balance);
            }
            else
            {
                Console.WriteLine("Fel inmatning");
            }
        }
    }
}
