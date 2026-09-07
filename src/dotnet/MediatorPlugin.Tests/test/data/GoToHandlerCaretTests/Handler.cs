using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Application;

internal sealed class GetEntityRequestHandler : IRequestHandler<GetEntityRequest, string>
{
    public Task<string> Handle(GetEntityRequest request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}
