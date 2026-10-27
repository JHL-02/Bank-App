namespace Bank_App
{
    internal class ExchangeRates
    {
        // SEK is the base. Then we only store "how many SEK is 1 USD"
        // and "how many SEK is 1 EUR", not a rate for every pair.

        private Dictionary<Currency, decimal> rates = new Dictionary<Currency, decimal>
        {
            {Currency.SEK, 1.0m },
            {Currency.USD, 10.5m },
            {Currency.EUR, 11.0m }
        };

        public DateOnly UpdatedOn { get; private set; } = DateOnly.FromDateTime(DateTime.Today);


        // Admin writes today's rate. SEK stays 1. Zero or less is rejected.
        // Returns false when nothing was changed, so the menu can tell the admin.
        public bool UpdateRate(Currency currency, decimal newRate)
        {
            if (currency == Currency.SEK || newRate <= 0)
            {
                return false;
            }

            rates[currency] = newRate;
            UpdatedOn = DateOnly.FromDateTime(DateTime.Today);
            return true;
        }

        public decimal GetRate(Currency currency)
        {
            return rates[currency];
        }

        public decimal Convert(decimal amount, Currency fromCurrency, Currency toCurrency)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Summan kan inte vara negativ.");
            }

            if (fromCurrency == toCurrency)
            {
                return amount;
            }

            decimal amountInSek = amount * rates[fromCurrency];
            decimal convertedAmount = amountInSek / rates[toCurrency];

            return Math.Round(convertedAmount, 2, MidpointRounding.AwayFromZero);
        }

        public void ShowRate()
        {
            Console.WriteLine("--- Växelkurser ---");
            foreach (KeyValuePair<Currency, decimal> rate in rates)
            {
                if (rate.Key == Currency.SEK)
                {
                    continue;
                }
                Console.WriteLine($"1 {rate.Key} = {rate.Value} SEK");
            }
            Console.WriteLine($"Senast uppdaterad: {UpdatedOn:yyyy-MM-dd}");
        }
        
    }
}
