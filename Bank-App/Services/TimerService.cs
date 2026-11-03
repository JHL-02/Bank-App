namespace Bank_App
{
    internal class TimerService
    {
        public static event Action? OnTimerTick;

        public static async Task TransactionsTimer(Action transactionMethod)
        {
            while (true)
            {
                transactionMethod();
                await Task.Delay(TimeSpan.FromSeconds(5));

                OnTimerTick?.Invoke();
            }
        }
    }
}
