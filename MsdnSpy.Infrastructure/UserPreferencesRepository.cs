using MsdnSpy.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MsdnSpy.Infrastructure
{
	public class UserPreferencesRepository : IUserPreferencesRepository
	{
		public UserPreferencesRepository(DatabaseContext context)
		{
			_context = context;
		}

		public UserPreferences GetPreferencesByChatId(long chatId) =>
			_context.UserPreferences.SingleOrDefault(up => up.ChatId == chatId);
		
		public bool AddCategory(long chatId, string categoryName) => ChangePreferencesFlag(chatId, categoryName, true);

		public bool DeleteCategory(long chatId, string categoryName) => ChangePreferencesFlag(chatId, categoryName, false);

		public int Save()
		{
			return 0;
			//return _context.SaveChanges();
		}

		private readonly DatabaseContext _context;

		private bool ChangePreferencesFlag(long chatId, string categoryName, bool flag)
		{
			try
			{
				var userPreferences = GetPreferencesByChatId(chatId).Preferences;
				var categoryPreferences = JsonConvert.DeserializeObject<IDictionary<string, bool>>(userPreferences);
				
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}