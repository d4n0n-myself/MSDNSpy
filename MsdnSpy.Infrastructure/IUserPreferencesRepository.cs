using MsdnSpy.Core;

namespace MsdnSpy.Infrastructure
{
	public interface IUserPreferencesRepository
	{
		UserPreferences GetPreferencesByChatId(long chatId);
	}
}