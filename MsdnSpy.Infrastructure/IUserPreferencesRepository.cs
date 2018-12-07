using MsdnSpy.Core;

namespace MsdnSpy.Infrastructure
{
	public interface IUserPreferencesRepository
	{
		UserPreferences GetPreferencesByChatId(long chatId);
		bool AddCategory(long chatId, string categoryName);
		bool DeleteCategory(long chatId, string categoryName);
		int Save();
	}
}