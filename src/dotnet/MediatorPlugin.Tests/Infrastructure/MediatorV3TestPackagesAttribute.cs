using JetBrains.ReSharper.TestFramework;

namespace Octelys.MediatorPlugin.Tests.Infrastructure;

public sealed class MediatorV3TestPackagesAttribute : TestPackagesAttribute
{
    public MediatorV3TestPackagesAttribute() : base(Tests.Packages.Mediator.V3)
    {
    }
}
