namespace Bank_App
{
    internal class MenuOption
    {
        public string Title { get; init; }
        public Func<MenuResult> Function { get; init; }
        public ConsoleColor? Color { get; private set; } = null;

        public MenuOption(string title, Func<MenuResult> function)
        {
            Title = title;
            Function = function;
        }

        public MenuResult Execute()
        {
            return Function();
        }

        public MenuOption SetColor(ConsoleColor? color)
        {
            Color = color;
            return this;
        }

    }
}