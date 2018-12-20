using System;
using System.Collections.Generic;
using System.Linq;
using MsdnSpy.Core;

namespace MsdnSpy.Infrastructure
{
	public class HistoryRepository : IHistoryRepository
	{
		public HistoryRepository(DatabaseContext context)
		{
			_context = context;
		}

		public bool AddEntry(long chatId, string query)
		{
			try
			{
				_context.History.Add(new HistoryEntry(chatId, query));
				Save();
				return true;
			}
			catch (Exception)
			{
				return false;
			}	
		}

		public IEnumerable<HistoryEntry> GetLastEntries(long chatId)
		{
			return _context.History.Where(he => he.ChatId == chatId).Take(10);
		}
		
		private readonly DatabaseContext _context;

		public int Save() => _context.SaveChanges();
	}
}