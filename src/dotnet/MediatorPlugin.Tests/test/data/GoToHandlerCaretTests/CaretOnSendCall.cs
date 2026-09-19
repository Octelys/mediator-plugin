using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Domain;

public class GetEntityRequest : IRequest<string>
{
}

public class GetEntityRequestSender
{
    private readonly IMediator _mediator;

    public GetEntityRequestSender(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<string> SendAsync(CancellationToken cancellationToken)
    {
        return _mediator.{caret}Send(new GetEntityRequest(), cancellationToken);
    }
}
