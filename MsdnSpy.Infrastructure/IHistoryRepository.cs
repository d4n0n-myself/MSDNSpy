using System.Collections.Generic;
using MsdnSpy.Core;

namespace MsdnSpy.Infrastructure
{
	public interface IHistoryRepository
	{
		bool AddEntry(long chatId, string query);
		IEnumerable<string> GetLastEntries(long chatId);
	}
}