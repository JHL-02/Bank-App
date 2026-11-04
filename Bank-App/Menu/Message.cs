using System.Drawing;

namespace Bank_App
{
    internal class Message
    {
        public string Text { get; init; }
        public ConsoleColor? Color { get; private set; } = ConsoleColor.Cyan;
        public Message(string text)
        {
            Text = text;
        }

        public Message SetColor(ConsoleColor? color)
        {
            Color = color;
            return this;
        }

        public static void WriteLine(Message message)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            if (message.Color != null)
            {
                Console.ForegroundColor = (ConsoleColor)message.Color!;
            }

            Console.WriteLine("+--------------------------+");
            Console.WriteLine($"| {message.Text,-24} |");
            Console.WriteLine("+--------------------------+");

            Console.ForegroundColor = oldColor;
        }
    }
}
