using MsdnSpy.Core.Common;

namespace MsdnSpy.Core
{
	public class User : Entity<long>
	{	
		public User(long id) : base(id) { }

		public string Preferences { get; set; }
		public string Username { get; set; }
	}
}