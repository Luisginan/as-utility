using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AS_Utility
{
    public class StyleUtil
    {
        public void WriteTitle(String title)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("==============================================");
            Console.WriteLine(" " + title.ToUpper());
            Console.WriteLine("==============================================");
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(3000);
        }
    }
}
