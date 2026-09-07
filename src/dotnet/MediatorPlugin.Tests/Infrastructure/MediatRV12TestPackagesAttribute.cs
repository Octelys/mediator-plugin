using JetBrains.ReSharper.TestFramework;

namespace Octelys.MediatorPlugin.Tests.Infrastructure;

public sealed class MediatRV12TestPackagesAttribute : TestPackagesAttribute
{
    public MediatRV12TestPackagesAttribute() : base(Tests.Packages.MediatR.V12)
    {
    }
}
