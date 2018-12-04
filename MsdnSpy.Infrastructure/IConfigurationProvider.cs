using Microsoft.Extensions.Configuration;

namespace MsdnSpy.Infrastructure
{
	public interface IConfigurationProvider
	{
		IConfiguration Config { get; }
	}
}
