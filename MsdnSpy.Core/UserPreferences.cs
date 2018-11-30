using MsdnSpy.Core.Common;
using System.Collections.Generic;

namespace MsdnSpy.Core
{
    public class UserPreferences : ValueType<UserPreferences>
    {
        public IDictionary<string, bool> Preferences { get; } = new Dictionary<string, bool>()
        {
            ["Properties"] = true,
            ["Methods"] = true,
            ["summary"] = true,
            ["References"] = true
        };
    }
}