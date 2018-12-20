using System.Collections.Generic;

namespace MsdnSpy.Infrastructure.Interfaces
{
	public interface IHistoryRepository
	{
		bool AddEntry(long chatId, string query);
		IEnumerable<string> GetLastEntries(long chatId);
	}
}