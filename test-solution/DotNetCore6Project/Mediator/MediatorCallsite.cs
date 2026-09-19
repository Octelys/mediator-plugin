using DotNetCore6_Domain.Mediator.Requests;
using MediatR;

namespace DotNetCore6_Domain.Mediator;

public class MediatorCallsite
{
    private readonly IMediator _mediator;
    
    public MediatorCallsite(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await _mediator.Send(new NewMediatorRequest(), cancellationToken);
    }
}