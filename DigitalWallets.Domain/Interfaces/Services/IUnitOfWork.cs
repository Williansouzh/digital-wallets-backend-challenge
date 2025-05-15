using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalWallets.Domain.Interfaces.Services;

public interface IUnitOfWork
{
    //IAnimalRepository AnimalRepository { get; }
    Task<int> CommitAsync();
}