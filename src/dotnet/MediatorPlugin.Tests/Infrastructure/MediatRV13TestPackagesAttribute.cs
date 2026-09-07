using JetBrains.ReSharper.TestFramework;

namespace Octelys.MediatorPlugin.Tests.Infrastructure;

public sealed class MediatRV13TestPackagesAttribute : TestPackagesAttribute
{
    public MediatRV13TestPackagesAttribute() : base(Tests.Packages.MediatR.V13)
    {
    }
}
