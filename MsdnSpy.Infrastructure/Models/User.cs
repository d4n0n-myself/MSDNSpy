using MsdnSpy.Infrastructure.Interfaces;

namespace MsdnSpy.Infrastructure.Models
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}