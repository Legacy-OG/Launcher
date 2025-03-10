using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legacy_Launcher.Utils
{
    public class Logger
    {
        public static void good(string Message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] " +  Message);
            Console.ResetColor();
        }

        public static void warn(string Message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[*] " + Message);
            Console.ResetColor();
        }

        public static void error(string Message) 
        { 
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[-] " + Message);
            Console.ResetColor();
        }
    }
}
