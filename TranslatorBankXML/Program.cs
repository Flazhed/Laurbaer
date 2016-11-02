﻿using System;

namespace TranslatorBankXML
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            RabbitMQTranslator aTranslator = new RabbitMQTranslator();
            while(running)
            {
                aTranslator.translate();
                Console.WriteLine("exit? {yes/[no]}: ");
                string exitkeys = Console.ReadLine();
                if(exitkeys.Equals("yes"))
                {
                    running = false;
                }
            }
        }
    }
}