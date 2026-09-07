using JetBrains.ReSharper.TestFramework;

namespace Octelys.MediatorPlugin.Tests.Infrastructure;

public sealed class MediatRV14TestPackagesAttribute : TestPackagesAttribute
{
    public MediatRV14TestPackagesAttribute() : base(Tests.Packages.MediatR.V14)
    {
    }
}
