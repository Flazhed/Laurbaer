using System;

namespace TranslatorBankXML
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            RabbitMQConnectionHandling conn = new RabbitMQConnectionHandling();
            conn.OpenCon();
            while (running)
            {
                conn.ReadQueue();
                Console.WriteLine("exit? {yes/[no]}: ");
                string exitkeys = Console.ReadLine();
                if (exitkeys.Equals("yes"))
                {
                    running = false;
                }
            }
            conn.CloseConn();

        }
    }
}
