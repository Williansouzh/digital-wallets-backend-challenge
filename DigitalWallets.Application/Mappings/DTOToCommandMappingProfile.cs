using AutoMapper;
using DigitalWallets.Application.DTOs;
using DigitalWallets.Application.Transactions.Commands;
using DigitalWallets.Application.Wallets.Commands;

namespace DigitalWallets.Application.Mappings;

public class DTOToCommandMappingProfile : Profile
{
    public DTOToCommandMappingProfile()
    {
        CreateMap<WalletDTO, CreateWalletCommand>();
        CreateMap<WalletDTO, DebitWalletCommand>();
        CreateMap<WalletDTO, CreditWalletCommand>();
        CreateMap<WalletDTO, TransferCommand>();

        CreateMap<TransactionDTO, CreateTransactionCommand>();
        CreateMap<TransactionDTO, DeleteTransactionCommand>();
        CreateMap<TransactionDTO, UpdateTransactionCommand>();
        CreateMap<TransactionDTO, CreateCreditTransactionCommand>();
        CreateMap<TransactionDTO, CreateDebitTransactionCommand>();
        CreateMap<TransactionDTO, CreateTransferTransactionCommand>();


    }
}
