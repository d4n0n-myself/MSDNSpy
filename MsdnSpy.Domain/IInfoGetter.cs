using System.Collections.Generic;

namespace MsdnSpy.Domain
{
	public interface IInfoGetter
	{
		IDictionary<string, string> GetInfoByQuery(string query);
	}
}
