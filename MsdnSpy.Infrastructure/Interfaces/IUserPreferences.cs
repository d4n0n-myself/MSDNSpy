using System.Collections;
using System.Collections.Generic;

namespace MsdnSpy.Infrastructure.Interfaces
{
    public interface IUserPreferences
    {
        IDictionary<string,bool> Preferences { get; }
    }
}