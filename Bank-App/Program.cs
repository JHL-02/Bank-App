namespace Bank_App
{
    // Starts the banking application.
    internal class Program
    {
        
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Bank.StartProgram();
        }
    }
}
