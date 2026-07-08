using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUIRecorder.CodeProvider.Common;
using FlaUIRecorder.CodeProvider.CSharp;
using FlaUIRecorder.Tests.Common;
using FluentAssertions;
using NUnit.Framework;

namespace FlaUIRecorder.CodeProvider.CSharp.Tests
{
    [TestFixture]
    public class CSharpCodeHelperTests
    {
        [Test]
        public void Generates_Valid_VariableNames()
        {
            var name = CSharpCodeHelper.GetVariableName(new ElementBuilder().CreateMenuItem().WithName("search").Build());
            name.Should().Be("searchMenuItem");

            name = CSharpCodeHelper.GetVariableName(new ElementBuilder().CreateMenuItem().WithName("? .-(_search+*'´`;:<>|@#!\"§$%&/()={[]}\\").Build());
            name.Should().Be("_searchMenuItem");

            name = CSharpCodeHelper.GetVariableName(new ElementBuilder().CreateButton().WithName("最小化").Build());
            name.Should().Be("button");
        }

        [Test]
        public void GetVariableName_CombinesParentAndAutomationId()
        {
            var parent = new ElementBuilder().CreateGroup().WithName("Login").Build();
            var button = new ElementBuilder().CreateButton().WithName("Submit").WithAutomationId("btnSubmit").Build();

            var name = CSharpCodeHelper.GetVariableName(button, parent);
            name.Should().Be("loginSubmitBtnSubmitButton");
        }

        [Test]
        public void SanitizeIdentifier_RemovesInvalidCharacters()
        {
            CSharpCodeHelper.SanitizeIdentifier("btn-Save_1").Should().Be("btnSave_1");
            CSharpCodeHelper.SanitizeIdentifier("").Should().BeEmpty();
        }

        [Test]
        public void BuildSelector_CombinesAutomationIdControlTypeAndName()
        {
            var element = new ElementBuilder()
                .CreateButton()
                .WithAutomationId("saveBtn")
                .WithName("Save")
                .Build();

            var selector = SelectorBuilder.Build(element);
            var condition = SelectorBuilder.BuildCSharpCondition(selector);

            condition.Should().Contain("ByAutomationId(\"saveBtn\")");
            condition.Should().Contain("ByControlType(FlaUI.Core.Definitions.ControlType.Button)");
            condition.Should().Contain("ByName(\"Save\")");
            condition.Should().Contain("IsEnabled");
        }

        [Test]
        public void BuildSelector_UsesFindAllChildrenFallback()
        {
            var element = new ElementBuilder().CreateButton().Build();
            var selector = SelectorBuilder.Build(element);

            selector.FindMethod.Should().Be(SelectorFindMethod.FindAllChildren);
            selector.ControlType.Should().Be(ControlType.Button);
        }
    }
}
