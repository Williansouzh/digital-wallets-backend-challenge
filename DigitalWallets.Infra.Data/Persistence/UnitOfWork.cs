using DigitalWallets.Domain.Interfaces.Services;
using DigitalWallets.Infra.Data.Context;

namespace DigitalWallets.Infra.Data.Persistence;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    //public IAnimalRepository AnimalRepository { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

    }
    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
