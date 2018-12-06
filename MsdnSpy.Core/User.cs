using MsdnSpy.Core.Common;

namespace MsdnSpy.Core
{
	public class User : Entity<int>
	{	
		public User(int id) : base(id) { }

		public string Name { get; set; }
		public int Age { get; set; }
	}
}