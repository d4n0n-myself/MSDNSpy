using System.Collections.Generic;
using MsdnSpy.Core;

namespace MsdnSpy.Infrastructure
{
    public class UserPreferencesRepository : IUserPreferencesRepository
    {
        private readonly DatabaseContext _context;

        public UserPreferencesRepository(DatabaseContext context)
        {
            _context = context;
        }
        
        public UserPreferences GetPreferencesByChatId(long chatId)
        {
            return new UserPreferences();
        }
    }
}