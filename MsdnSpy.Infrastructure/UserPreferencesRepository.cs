using MsdnSpy.Core;

namespace MsdnSpy.Infrastructure
{
	public class UserPreferencesRepository : IUserPreferencesRepository
	{
		public UserPreferencesRepository(DatabaseContext context)
		{
			_context = context;
		}

		public UserPreferences GetPreferencesByChatId(long chatId)
		{
			return new UserPreferences();
		}

		private readonly DatabaseContext _context;
	}
}