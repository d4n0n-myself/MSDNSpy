using System;
using MsdnSpy.Core.Common;

namespace MsdnSpy.Core
{
	public class HistoryEntry : ValueType<HistoryEntry>
	{
		public HistoryEntry(long chatId, string requestQuery)
		{
			ChatId = chatId;
			RequestQuery = requestQuery;
			DateTime = DateTime.Now;
		}

		public Guid Id { get; set; }
		public long ChatId { get; set; }
		public string RequestQuery { get; set; }
		public DateTime DateTime { get; set; }
	}
}