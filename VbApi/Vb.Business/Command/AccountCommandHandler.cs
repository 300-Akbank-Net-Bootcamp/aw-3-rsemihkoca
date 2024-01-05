using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command;

public class AccountCommandHandler :
    IRequestHandler<CreateAccountCommand, ApiResponse<AccountResponse>>,
    IRequestHandler<UpdateAccountCommand,ApiResponse>,
    IRequestHandler<DeleteAccountCommand,ApiResponse>
{
    private readonly VbDbContext dbContext;
    private readonly IMapper mapper;

    public AccountCommandHandler(VbDbContext dbContext,IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<AccountResponse>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var checkIdentity = await dbContext.Set<Account>().Where(x => x.AccountNumber == request.Model.AccountNumber)
            .FirstOrDefaultAsync(cancellationToken);
        if (checkIdentity != null)
        {
            return new ApiResponse<AccountResponse>($"{request.Model.AccountNumber} is used by another customer.");
        }
        
        
        int newAccountNumber;
        do
        {
            newAccountNumber = new Random().Next(1000000, 9999999);
        } while (await dbContext.Set<Account>().AnyAsync(x => x.AccountNumber == newAccountNumber, cancellationToken));

        
        var entity = mapper.Map<AccountRequest, Account>(request.Model);
        entity.AccountNumber = newAccountNumber;
        
        var entityResult = await dbContext.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var mapped = mapper.Map<Account, AccountResponse>(entityResult.Entity);
        return new ApiResponse<AccountResponse>(mapped);
    }

    public async Task<ApiResponse> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var fromdb = await dbContext.Set<Account>().Where(x => x.AccountNumber == request.AccountNumber)
            .FirstOrDefaultAsync(cancellationToken);
        if (fromdb == null)
        {
            return new ApiResponse("Record not found");
        }

        fromdb.CustomerId = request.Model.CustomerId;
        fromdb.IBAN = request.Model.IBAN;
        fromdb.Balance = request.Model.Balance;
        fromdb.CurrencyType = request.Model.CurrencyType;
        fromdb.Name = request.Model.Name;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var fromdb = await dbContext.Set<Account>().Where(x => x.AccountNumber == request.AccountNumber)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (fromdb == null)
        {
            return new ApiResponse("Record not found");
        }
        //dbContext.Set<Account>().Remove(fromdb);
        
        fromdb.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }
}