using Microsoft.Extensions.Configuration;

namespace MsdnSpy.Infrastructure.Interfaces
{
	public interface IConfigurationProvider
	{
		IConfiguration Config { get; }
	}
}
