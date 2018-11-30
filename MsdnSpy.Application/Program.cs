using System;

namespace MsdnSpy.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Listener.RunNew();
            Console.WriteLine("Drop the mic");
            Console.ReadKey();
        }
    }
}