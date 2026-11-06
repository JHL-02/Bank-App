namespace Bank_App
{
    // Identifies the purpose and default presentation of a message.
    enum MessageType
    {
        STANDARD,
        ERROR,
        INPUT
    }
    // Formats and displays bordered console messages with optional colors.
    internal class Message
    {
        public string Text { get; init; }
        public MessageType MsgType { get; init; }
        public ConsoleColor? Color { get; private set; } = ConsoleColor.Cyan;
        public Message(string text, MessageType msgType = MessageType.STANDARD)
        {
            Text = text;
            MsgType = msgType;

            if (MsgType == MessageType.ERROR)
            {
                Color = ConsoleColor.Red;
            }
            if (MsgType == MessageType.INPUT)
            {
                Color = ConsoleColor.Yellow;
            }
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
                Console.ForegroundColor = (ConsoleColor)message.Color;
            }

            string[] lines = message.Text.Split('\n');

            int width = Math.Max(24, lines.Max(line => line.TrimEnd('\r').Length));

            string border = "+" + new string('-', width + 2) + "+";

            Console.WriteLine(border);

            foreach (string line in lines)
            {
                string cleanLine = line.TrimEnd('\r');
                Console.WriteLine($"| {cleanLine.PadRight(width)} |");
            }

            if (message.MsgType == MessageType.INPUT)
            {
                Console.WriteLine($"| {"".PadRight(width)} |");
                Console.WriteLine(border);

                // Move cursor back inside the box
                Console.SetCursorPosition(2, Console.CursorTop - 2);
            }
            else
            {
                Console.WriteLine(border);
            }

            Console.ForegroundColor = oldColor;
        }
    }
}
