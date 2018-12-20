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

		public User GetUserByUserId(long userId)
		{
			var user = new User(userId);
			if (_context.Users.Contains(user))
				return _context.Users.SingleOrDefault(up => up.Id == userId);
			_context.Users.Add(new User(userId));
			_context.SaveChanges();
			return user;
		}

		public bool ChangeCategory(long userId, string categoryName)
		{
			try
			{
				var user = GetUserByUserId(userId);

				var categoryPreferences =
					JsonConvert.DeserializeObject<IDictionary<string, bool>>(user.Preferences);
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

		public IEnumerable<string> ShowCategories(long userId)
		{
			try
			{
				var pref = GetUserByUserId(userId).Preferences;
				var res = JsonConvert.DeserializeObject<IDictionary<string, bool>>(pref) ??
				          new Dictionary<string, bool>();
				return res
					.Where(x => x.Value)
					.Select(x => x.Key);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public int Save() => _context.SaveChanges();

		private readonly DatabaseContext _context;
	}
}