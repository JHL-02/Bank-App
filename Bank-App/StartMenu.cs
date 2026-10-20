namespace Bank_App
{
    internal sealed class StartMenu : Menu
    {
        internal StartMenu() : base("Avsluta")
        {
        }

        protected override void InitilizeOptions()
        {
            Options.Add(new MenuOption("Skriv ut", Test));
        }

        private void Test()
        {
            Console.WriteLine("Skriver ut TEST");
        }
    }
}
