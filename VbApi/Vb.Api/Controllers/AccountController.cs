using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Schema;

namespace VbApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IMediator mediator;
    public AccountController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet("All")]
    public async Task<ApiResponse<List<AccountResponse>>> Get()
    {
        var operation = new GetAllAccountQuery();
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ApiResponse<AccountResponse>> Get(int id)
    {
        var operation = new GetAccountByIdQuery(id);
        var result = await mediator.Send(operation);
        return result;
    }
    
    [HttpGet]
    public async Task<ApiResponse<List<AccountResponse>>> Get(string iban)
    {
        var operation = new GetAccountByParameterQuery(iban);
        var result = await mediator.Send(operation);
        return result;
    }
    
    [HttpPost]
    public async Task<ApiResponse<AccountResponse>> Post([FromBody] AccountRequest customer)
    {
        var operation = new CreateAccountCommand(customer);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiResponse> Put(int id, [FromBody] AccountRequest customer)
    {
        var operation = new UpdateAccountCommand(id,customer);
        var result = await mediator.Send(operation);
        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(int id)
    {
        var operation = new DeleteAccountCommand(id);
        var result = await mediator.Send(operation);
        return result;
    }
}