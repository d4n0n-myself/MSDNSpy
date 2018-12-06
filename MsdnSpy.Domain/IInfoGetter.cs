using System.Collections.Generic;

namespace MsdnSpy.Domain
{
	public interface IInfoGetter
	{
		IDictionary<string, HashSet<string>> GetInfoByQuery(string query);
	}
}
