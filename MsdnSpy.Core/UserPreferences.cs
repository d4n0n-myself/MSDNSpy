using MsdnSpy.Core.Common;
using System.Collections.Generic;

namespace MsdnSpy.Core
{
    public class UserPreferences : ValueType<UserPreferences>
    {
        public static IDictionary<string, bool> DefaultPreferences { get; } = new Dictionary<string, bool>()
        {
            ["Properties"] = true,
            ["Methods"] = true,
            ["summary"] = true,
            ["References"] = true
        };
    }
}