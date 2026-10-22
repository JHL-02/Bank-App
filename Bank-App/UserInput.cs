using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_App
{
    internal static class UserInput
    {
        public static string GetString(string? invalidInputMessage = null)
        {
            while (true)
            {
                string input = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }

                if (invalidInputMessage != null)
                {
                    Console.WriteLine(invalidInputMessage);
                }
            }
        }

        public static bool GetYesOrNo(string? invalidInputMessage = null)
        {
            while (true)
            {
                string input = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(input))
                {
                    if (input.ToLower() == "ja")
                    {
                        return true;
                    }
                    if (input.ToLower() == "nej")
                    {
                        return false;
                    }
                }

                if (invalidInputMessage != null)
                {
                    Console.WriteLine(invalidInputMessage);
                }
            }
        }

        public static int GetInt(string? invalidInputMessage = null)
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine() ?? "", out int number))
                {
                    return number;
                }

                if (invalidInputMessage != null)
                {
                    Console.WriteLine(invalidInputMessage);
                }
            }
        }

        public static decimal GetDecimal(string? invalidInputMessage = null)
        {
            while (true)
            {
                if (decimal.TryParse(Console.ReadLine() ?? "", out decimal number))
                {
                    return number;
                }

                if (invalidInputMessage != null)
                {
                    Console.WriteLine(invalidInputMessage);
                }
            }
        }

        public static void WaitForUserConfirm(string message = "Tryck på valfri knapp för att fortsätta.")
        {
            Console.WriteLine(message);

            Console.ReadKey(true);
        }
    }
}
