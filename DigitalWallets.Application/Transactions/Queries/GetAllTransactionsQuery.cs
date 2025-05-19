using DigitalWallets.Domain.Entities;
using MediatR;
using System.Collections.Generic;

namespace DigitalWallets.Application.Transactions.Queries
{
    public class GetAllTransactionsQuery : IRequest<IReadOnlyCollection<Transaction>>
    {
    }
}
