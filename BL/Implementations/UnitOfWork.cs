using DAL.EF;

namespace BL.Implementations;

public class UnitOfWork
{
    private readonly PhygitalDbContext _dbContext;

    public UnitOfWork(PhygitalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void BeginTransaction()
    {
        _dbContext.Database.BeginTransaction();
    }

    public void Commit()
    {
        _dbContext.SaveChanges();
        _dbContext.Database.CommitTransaction();
    }
}