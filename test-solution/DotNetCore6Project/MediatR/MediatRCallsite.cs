using DotNetCore6_Domain.MediatR.Requests;
using MediatR;

namespace DotNetCore6_Domain.MediatR;

public class MediatRCallsite
{
    private readonly IMediator _mediator;
    
    public MediatRCallsite(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await _mediator.Send(new CustomClassRequest(), cancellationToken);
    }
}