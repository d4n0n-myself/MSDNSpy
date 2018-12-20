using System;
using System.Collections.Generic;
using System.Linq;
using MsdnSpy.Core;
using MsdnSpy.Infrastructure.Interfaces;

namespace MsdnSpy.Infrastructure.Repositories
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

		public IEnumerable<string> GetLastEntries(long chatId)
		{
			return _context.History.Where(he => he.ChatId == chatId)
				.OrderByDescending(he => he.DateTime)
				.Take(10)
				.Select(he => he.RequestQuery);
		}

		private readonly DatabaseContext _context;

		public int Save() => _context.SaveChanges();
	}
}