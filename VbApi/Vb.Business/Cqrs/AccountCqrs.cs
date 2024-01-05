using MediatR;
using Vb.Base.Response;
using Vb.Schema;

namespace Vb.Business.Cqrs;


public record CreateAccountCommand(AccountRequest Model) : IRequest<ApiResponse<AccountResponse>>;
public record UpdateAccountCommand(int AccountNumber,AccountRequest Model) : IRequest<ApiResponse>;
public record DeleteAccountCommand(int AccountNumber) : IRequest<ApiResponse>;

public record GetAllAccountQuery() : IRequest<ApiResponse<List<AccountResponse>>>;
public record GetAccountByIdQuery(int AccountNumber) : IRequest<ApiResponse<AccountResponse>>;
public record GetAccountByParameterQuery(string Iban) : IRequest<ApiResponse<List<AccountResponse>>>;
