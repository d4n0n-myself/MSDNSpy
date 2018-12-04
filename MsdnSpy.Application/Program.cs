using System;

namespace MsdnSpy.Application
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			Listener.RunNew("http://localhost:1234/");
			Console.WriteLine("Drop the mic");
			Console.ReadKey();
		}
	}
}