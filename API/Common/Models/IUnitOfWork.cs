namespace API.Common.Models;

public interface IUnitOfWork
{
    Task CommitChangesAsync();
}