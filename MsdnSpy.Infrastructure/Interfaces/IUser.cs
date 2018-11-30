namespace MsdnSpy.Infrastructure.Interfaces
{
    public interface IUser
    {
        int Id { get; set; }
        string Name { get; set; }
        int Age { get; set; }
    }
}