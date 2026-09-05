using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ClassLibrary1;

public class SomeQueryHandler : IRequestHandler<SomeQuery, SomeDto>
{
    public Task<SomeDto> Handle(SomeQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SomeDto());
    }
}
