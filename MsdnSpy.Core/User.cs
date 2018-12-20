using System.Collections.Generic;
using MsdnSpy.Core.Common;
using Newtonsoft.Json;

namespace MsdnSpy.Core
{
	public class User : Entity<long>
	{
		public User(long id) : base(id)
		{
			Preferences = JsonConvert.SerializeObject(new Dictionary<string, bool>
			{
				{
					"Name", true
				},
				{
					"MsdnUrl", true
				},
				{
					"Description", true
				}
			});
		}

		public string Preferences { get; set; }
	}
}