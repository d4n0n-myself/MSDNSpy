using System.Collections;
using System.Collections.Generic;
using MsdnSpy.Core;

namespace MsdnSpy.Infrastructure
{
	public interface IUserRepository
	{
		User GetUserByUserId(long userId);
		bool ChangeCategory(long userId, string categoryName);
		IEnumerable<string> ShowCategories(long userId);
		int Save();
	}
}