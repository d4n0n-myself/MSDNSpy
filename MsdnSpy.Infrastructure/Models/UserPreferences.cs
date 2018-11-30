using System.Collections.Generic;
using MsdnSpy.Infrastructure.Interfaces;

namespace MsdnSpy.Infrastructure.Models
{
    public class UserPreferences : IUserPreferences
    {
        public IDictionary<string, bool> Preferences { get; } = new Dictionary<string, bool>()
        {
            {"Properties", true},
            {"Methods", true},
            {"summary", true},
            {"References", true}
        };
    }
}