using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command;

public class AddressCommandHandler :
    IRequestHandler<CreateAddressCommand, ApiResponse<AddressResponse>>,
    IRequestHandler<UpdateAddressCommand, ApiResponse>,
    IRequestHandler<DeleteAddressCommand, ApiResponse>

{
    private readonly VbDbContext dbContext;
    private readonly IMapper mapper;

    public AddressCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<AddressResponse>> Handle(CreateAddressCommand request,
        CancellationToken cancellationToken)
    {
        // # get active address 
        var activeAddress = await dbContext.Set<Address>()
            .Where(x => x.CustomerId == request.Model.CustomerId && x.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
        // if there is no active address and request.IsDefault is false then return error
        // if there is active address and request.IsDefault is true then set active address to false and save new address as active
        // if there is active address and request.IsDefault is false then save new address as not active
        if (activeAddress == null && !request.Model.IsDefault)
        {
            return new ApiResponse<AddressResponse>("There is no active address. Please set IsDefault to true");
        }

        if (activeAddress != null && request.Model.IsDefault)
        {
            activeAddress.IsDefault = false;
            var entity = mapper.Map<AddressRequest, Address>(request.Model);

            var entityResult = await dbContext.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var mapped = mapper.Map<Address, AddressResponse>(entityResult.Entity);
            return new ApiResponse<AddressResponse>(mapped);
        }
        else
        {
            var entity = mapper.Map<AddressRequest, Address>(request.Model);

            var entityResult = await dbContext.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var mapped = mapper.Map<Address, AddressResponse>(entityResult.Entity);
            return new ApiResponse<AddressResponse>(mapped);
        }
    }

    public async Task<ApiResponse> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var fromdb = await dbContext.Set<Address>().Where(x => x.Id == request.Model.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (fromdb == null)
        {
            return new ApiResponse("Record not found");
        }
        
        fromdb.CustomerId = request.Model.CustomerId;
        fromdb.Address1 = request.Model.Address1;
        fromdb.Address2 = request.Model.Address2;
        fromdb.Country = request.Model.Country;
        fromdb.City = request.Model.City;
        fromdb.County = request.Model.County;
        fromdb.PostalCode = request.Model.PostalCode;
        fromdb.IsDefault = request.Model.IsDefault;
        

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var fromdb = await dbContext.Set<Address>().Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (fromdb == null)
        {
            return new ApiResponse("Record not found");
        }
        //dbContext.Set<Address>().Remove(fromdb);

        fromdb.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }
}