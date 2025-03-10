using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Legacy_Launcher.LaunchService;
using Legacy_Launcher.Utils;

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
            Utils.Logger.good("You will be redirected to the Epic Games website to login. After logging in please get the Auth Token.");
            Process.Start(new ProcessStartInfo("https://www.epicgames.com/id/logout?redirectUrl=https%3A//www.epicgames.com/id/login%3FredirectUrl%3Dhttps%253A%252F%252Fwww.epicgames.com%252Fid%252Fapi%252Fredirect%253FclientId%253Dec684b8c687f479fadea3cb2ad83f5c6%2526responseType%253Dcode") { UseShellExecute = true });
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
