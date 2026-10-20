namespace Bank_App.Menu
{
    internal sealed class StartMenu : Menu
    {
        internal StartMenu() : base("Avsluta")
        {
        }

        protected override void InitilizeOptions()
        {
            Options.Add(new MenuOption("Skriv ut", Test));
            Options.Add(new MenuOption("Testmeny", EnterTestMenu));
        }

        private void Test()
        {
            Console.WriteLine("Skriver ut TEST");
        }

        private void EnterTestMenu()
        {
            var testMenu = new TestMenu();
            testMenu.Display();
        }
    }
}
