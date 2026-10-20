namespace Bank_App
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
        }
    }
}
