using JetBrains.ReSharper.TestFramework;

namespace Octelys.MediatorPlugin.Tests.Infrastructure;

public sealed class MediatorTestPackagesAttribute : TestPackagesAttribute
{
    public MediatorTestPackagesAttribute() : base(
        Tests.Packages.Mediator.V3)
    {
    }
}