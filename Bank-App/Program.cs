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
            
            Bank.UserDatabase.AddUser("User1", "1234");
            Bank.UserDatabase.AddUser("User2", "1234");


            Menu startMenu = new Menu(new List<MenuOption>
            {
                new MenuOption("Logga in som User1", () =>
                {
                    Bank.UserDatabase.GetUserWithUsername("User1")?.DisplayAccountMenu();
                    return MenuResult.Continue;
                }),
                new MenuOption("Logga in som User2", () =>
                {
                    Bank.UserDatabase.GetUserWithUsername("User2")?.DisplayAccountMenu();
                    return MenuResult.Continue;
                })
            }, "Välkommen till bankappen","Avsluta");
            startMenu.Display();
        }
    }
}
