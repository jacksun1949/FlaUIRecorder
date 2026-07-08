using FlaUIRecorder.CodeProvider.PowerShell;
using FlaUIRecorder.Tests.Common;
using FluentAssertions;
using NUnit.Framework;

namespace FlaUIRecorder.CodeProvider.PowerShell.Tests
{
    [TestFixture]
    public class PowerShellCodeHelperTests
    {
        [Test]
        public void Generates_Valid_VariableNames()
        {
            var name = PowerShellCodeHelper.GetVariableName(new ElementBuilder().CreateMenuItem().WithName("search").Build());
            name.Should().Be("searchMenuItem");

            name = PowerShellCodeHelper.GetVariableName(new ElementBuilder().CreateMenuItem().WithName("? .-(_search+*'´`;:<>|@#!\"§$%&/()={[]}\\").Build());
            name.Should().Be("_searchMenuItem");

            name = PowerShellCodeHelper.GetVariableName(new ElementBuilder().CreateButton().WithName("最小化").Build());
            name.Should().Be("button");
        }
    }
}
