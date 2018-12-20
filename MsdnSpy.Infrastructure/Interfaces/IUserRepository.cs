using System.Collections.Generic;

namespace MsdnSpy.Infrastructure.Interfaces
{
	public interface IUserRepository
	{
		bool ChangeCategory(long userId, string categoryName);
		IEnumerable<string> ShowCategories(long userId);
	}
}