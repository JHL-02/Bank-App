namespace Bank_App
{
    internal class TimerService
    {
        public static event Action? OnTimerTick;

        public static async Task TransactionsTimer(Action transactionMethod)
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                transactionMethod();

                OnTimerTick?.Invoke();
            }
        }

        public static async Task ShowNotification(string message)
        {
            int oldX = Console.CursorLeft;
            int oldY = Console.CursorTop;

            int x = Console.WindowWidth - message.Length - 4;
            int y = 0;

            Console.SetCursorPosition(x, y);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[ {message} ]");
            Console.ResetColor();

            Console.SetCursorPosition(oldX, oldY);

            await Task.Delay(3000);

            Console.SetCursorPosition(x, y);
            Console.Write(new string(' ', message.Length + 4));

            Console.SetCursorPosition(oldX, oldY);
        }
    }
}
