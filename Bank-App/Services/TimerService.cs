namespace Bank_App
{
    // Runs periodic transaction processing and temporary console notifications.
    internal class TimerService
    {
        public static event Action? OnTimerTick;
        public static DateTime NextUpdate()
        {
            DateTime now = DateTime.Now;

            int nextQuarter = ((now.Minute / 15) + 1) * 15;

            return new DateTime(
                now.Year,
                now.Month,
                now.Day,
                now.Hour,
                0,
                0
            ).AddMinutes(nextQuarter);
        }

        public static async Task TransactionsTimer(Action transactionMethod)
        {
            while (true)
            {
                DateTime nextUpdate = NextUpdate();

                TimeSpan delay = nextUpdate - DateTime.Now;
                await Task.Delay(delay);
                transactionMethod();

                OnTimerTick?.Invoke();
            }
        }

        public static async Task TransactionsTimer(Action transactionMethod)
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(60));
                transactionMethod();

                OnTimerTick?.Invoke();
            }
        }

        public static async Task ShowNotification(string message)
        {
            int x = Math.Max(0, Console.WindowWidth - message.Length - 4);
            int y = 0;

            // Save cursor before drawing
            int oldX = Console.CursorLeft;
            int oldY = Console.CursorTop;

            // Draw notification
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"[ {message} ]");
            Console.ResetColor();

            // Restore cursor
            Console.SetCursorPosition(oldX, oldY);

            await Task.Delay(3000);

            // Get NEW cursor position
            int newX = Console.CursorLeft;
            int newY = Console.CursorTop;

            // Remove notification
            Console.SetCursorPosition(x, y);
            Console.Write(new string(' ', message.Length + 4));

            // Restore NEW cursor position
            Console.SetCursorPosition(newX, newY);
        }
    }
}
