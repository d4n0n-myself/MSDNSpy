using System;
using System.Collections.Generic;
using System.Linq;
using MsdnSpy.Core;
using Newtonsoft.Json;

namespace MsdnSpy.Infrastructure
{
	public class UserRepository : IUserRepository
	{
		public UserRepository(DatabaseContext context)
		{
			_context = context;
		}

		public User GetUserByUserId(long userId) =>_context.Users.SingleOrDefault(up => up.Id == userId);

		public bool ChangeCategory(long userId, string categoryName)
		{
			try
			{
				var user = GetUserByUserId(userId);
				if (user == null)
				{
					user = new User(userId);
					_context.Users.Add(user);
					_context.SaveChanges();
				}

				var categoryPreferences =
					JsonConvert.DeserializeObject<IDictionary<string, bool>>(user.Preferences ?? "") ??
					new Dictionary<string, bool>();
				if (!categoryPreferences.ContainsKey(categoryName))
					categoryPreferences.Add(categoryName, true);
				else
				{
					var activeConfiguration = categoryPreferences[categoryName];
					categoryPreferences[categoryName] = !activeConfiguration;
				}
				user.Preferences = JsonConvert.SerializeObject(categoryPreferences);
				_context.SaveChanges();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public IEnumerable<string> ShowCategories(long userId) => 
			JsonConvert.DeserializeObject<IDictionary<string, bool>>(GetUserByUserId(userId).Preferences).Where(x => x.Value).Select(x => x.Key);

		public int Save() => _context.SaveChanges();

		private readonly DatabaseContext _context;
	}
}