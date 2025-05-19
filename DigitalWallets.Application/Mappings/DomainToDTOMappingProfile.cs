using AutoMapper;
using DigitalWallets.Application.DTOs;
using DigitalWallets.Application.Wallets.Commands;
using DigitalWallets.Application.Wallets.Queries;
using DigitalWallets.Domain.Entities;

namespace DigitalWallets.Application.Mappings;

public class DomainToDTOMappingProfile : Profile
{
    public DomainToDTOMappingProfile()
    {
        CreateMap<Wallet, WalletDTO>().ReverseMap();
        CreateMap<Transaction, TransactionDTO>().ReverseMap();
    }
}