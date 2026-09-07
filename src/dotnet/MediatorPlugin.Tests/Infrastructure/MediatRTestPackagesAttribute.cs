using JetBrains.ReSharper.TestFramework;

namespace Octelys.MediatorPlugin.Tests.Infrastructure;

public sealed class MediatRTestPackagesAttribute : TestPackagesAttribute
{
    public MediatRTestPackagesAttribute() : base(
        Tests.Packages.MediatR.V12, Tests.Packages.MediatR.V13, Tests.Packages.MediatR.V14)
    {
    }
}