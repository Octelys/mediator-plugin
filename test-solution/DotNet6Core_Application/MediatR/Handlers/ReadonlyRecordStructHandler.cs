using DotNetCore6_Domain.MediatR.Requests;
using MediatR;

namespace DotNet6Core_Application.MediatR.Handlers;

public class ReadonlyRecordStructHandler : IRequestHandler<ReadonlyRecordStructRequest, MyResponse>
{
    public Task<MyResponse> Handle(ReadonlyRecordStructRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}