using System;

namespace MsdnSpy.Application
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			Listener.RunNew("http://127.0.0.1:1234/", new Server());
			Console.WriteLine("Drop the mic");
			Console.ReadKey();
		}
	}
}