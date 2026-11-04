namespace Bank_App
{
    internal static class UserInput
    {
        public static string? ReadLine()
        {
            ConsoleColor oldColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;

            string? input = Console.ReadLine();

            Console.ForegroundColor = oldColor;
            return input;
        }

        public static string GetString(string? invalidInputMessage = "Du måste skriva in något.")
        {
            while (true)
            {
                string input = UserInput.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }

                if (invalidInputMessage != null)
                {
                    Message.WriteLine(new Message(invalidInputMessage));
                }
            }
        }

        public static bool GetYesOrNo(string? invalidInputMessage = "Du kan bara skriv in (ja/nej)")
        {
            while (true)
            {
                string input = UserInput.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(input))
                {
                    if (input.Trim().ToLower() == "ja")
                    {
                        return true;
                    }
                    if (input.Trim().ToLower() == "nej")
                    {
                        return false;
                    }
                }

                if (invalidInputMessage != null)
                {
                    Message.WriteLine(new Message(invalidInputMessage));
                }
            }
        }

        public static int GetInt(string? invalidInputMessage = "Du måste skriva in ett heltal.")
        {
            while (true)
            {
                if (int.TryParse(UserInput.ReadLine() ?? "", out int number))
                {
                    return number;
                }

                if (invalidInputMessage != null)
                {
                    Message.WriteLine(new Message(invalidInputMessage));
                }
            }
        }

        public static decimal GetDecimal(string? invalidInputMessage = "Du måste skriva in en siffra.")
        {
            while (true)
            {
                if (decimal.TryParse(UserInput.ReadLine() ?? "", out decimal number))
                {
                    return number;
                }

                if (invalidInputMessage != null)
                {
                    Message.WriteLine(new Message(invalidInputMessage));
                }
            }
        }

        public static void WaitForUserConfirm(string message = "Tryck på valfri knapp för att fortsätta.")
        {
            Message.WriteLine(new Message(message));

            Console.ReadKey(true);
        }

        public static T? UserGetObjectFromList<T>(List<T> list, Func<T, string> getName)
        {
            if (list.Count < 1)
            {
                return default;
            }

            for (int i = 0; i < list.Count; i++)
            {
                Message.WriteLine(new Message($"{i + 1}. {getName(list[i])}"));
            }

            int input = 0;
            while (true)
            {
                input = GetInt();
                if (input >= 1 && input <= list.Count)
                {
                    return list[input - 1];
                }
                Message.WriteLine(new Message("Felaktig inmatning."));
            }
        }
    }
}
