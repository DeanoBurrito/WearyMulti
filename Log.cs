using System;
using System.IO;

namespace Weary
{
    public static class Log
    {
        public static bool breakOnError = true;
        
        public static void WriteLine(string message)
        {
            Console.WriteLine("[LOG] " + message);
        }

        public static void WriteError(string message)
        {
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR] " + message);
            Console.ResetColor();

            if (breakOnError)
                System.Diagnostics.Debugger.Break();
        }

        public static void FatalError(string message)
        {
            string[] messageArray = new string[] 
            {
                DateTime.Now.ToLongTimeString(),
                "---- Weary Encountered a fatal error and crashed, message below ----",
                message,
            };
            File.WriteAllLines("./CrashMessage.txt", messageArray);
            
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0 ; i < messageArray.Length; i++)
                Console.WriteLine(messageArray[i]);
            Console.ResetColor();

            Environment.Exit(-1);
        }
    }
}