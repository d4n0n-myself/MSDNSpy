using System.IO;

namespace MsdnSpy.Bot.Common
{
	public static class StreamExtensions
	{
		public static string ReadToEnd(this Stream stream)
		{
			using (var reader = new StreamReader(stream))
				return reader.ReadToEnd();
		}

		public static void Write(this Stream stream, string content)
		{
			using (var writer = new StreamWriter(stream))
				writer.Write(content);
		}
	}
}
