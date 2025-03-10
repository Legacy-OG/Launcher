using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Legacy_Launcher.LaunchService;

namespace Legacy_Launcher
{
    internal class Program
    {
        static void Intro()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to Legacy launcher!");
            Console.ResetColor();
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Intro();
            Console.Write("Please enter your exchange code: "); // We should check if this is valid
            string ExchangeCode = Console.ReadLine();
            Console.Write("Please enter your game path: "); // we should also check if this is valid
            string GamePath = Console.ReadLine();
            Console.WriteLine();
            LaunchService.LaunchService.InitializeLaunching(ExchangeCode, GamePath, 0);
        }
    }
}
