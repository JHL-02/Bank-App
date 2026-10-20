namespace Bank_App
{
    public class InputHelper
    {


        public static void ErrorMessages(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
