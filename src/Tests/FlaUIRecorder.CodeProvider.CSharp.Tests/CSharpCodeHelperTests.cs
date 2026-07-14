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
        public void GetVariableName_UsesAutomationIdWithoutParentPrefix()
        {
            var parent = new ElementBuilder().CreateGroup().WithName("Login").Build();
            var button = new ElementBuilder().CreateButton().WithName("Submit").WithAutomationId("btnSubmit").Build();

            var name = CSharpCodeHelper.GetVariableName(button, parent);
            name.Should().Be("btnSubmitButton");
        }

        [Test]
        public void GetVariableName_ProducesConciseNamesForCommonControls()
        {
            var titleBar = new ElementBuilder()
                .CreateGroup()
                .WithAutomationId("TitleBar")
                .WithName("TitleBar")
                .Build();

            var maximizeButton = new ElementBuilder()
                .CreateButton()
                .WithAutomationId("Maximize-Restore")
                .WithName("Maximize")
                .Build();

            CSharpCodeHelper.GetVariableName(titleBar).Should().Be("titleBar");
            CSharpCodeHelper.GetVariableName(maximizeButton).Should().Be("maximizeRestoreButton");
        }

        [Test]
        public void SanitizeIdentifier_RemovesInvalidCharacters()
        {
            CSharpCodeHelper.SanitizeIdentifier("btn-Save_1").Should().Be("btnSave_1");
            CSharpCodeHelper.SanitizeIdentifier("").Should().BeEmpty();
            CSharpCodeHelper.SanitizeIdentifier("123").Should().Be("_123");
            CSharpCodeHelper.SanitizeIdentifier("1abc").Should().Be("_1abc");
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
            condition.Should().NotContain("IsEnabled");
        }

        [Test]
        public void BuildSelector_ClickableButton_DoesNotEmitIsEnabledPredicate()
        {
            var selector = new SelectorInfo
            {
                AutomationId = "Maximize-Restore",
                ControlType = ControlType.Button,
                Name = "还原",
                RequireEnabled = true,
                FindMethod = SelectorFindMethod.FindFirstDescendant
            };

            var condition = SelectorBuilder.BuildCSharpCondition(selector);

            condition.Should().Be("e.ByAutomationId(\"Maximize-Restore\").And(e.ByControlType(FlaUI.Core.Definitions.ControlType.Button)).And(e.ByName(\"还原\"))");
            condition.Should().NotContain("IsEnabled");
        }

        [Test]
        public void BuildSelector_ControlTypeAndName_StartsWithByCall()
        {
            var selector = new SelectorInfo
            {
                ControlType = ControlType.Pane,
                Name = "桌面 1",
                FindMethod = SelectorFindMethod.FindFirstChild
            };

            var condition = SelectorBuilder.BuildCSharpCondition(selector);

            condition.Should().Be("e.ByControlType(FlaUI.Core.Definitions.ControlType.Pane).And(e.ByName(\"桌面 1\"))");
            condition.Should().NotContain("e.And(");
        }

        [Test]
        public void BuildSelector_SingleControlType_HasNoAnd()
        {
            var selector = new SelectorInfo
            {
                ControlType = ControlType.Button,
                FindMethod = SelectorFindMethod.FindAllChildren
            };

            var condition = SelectorBuilder.BuildCSharpCondition(selector);

            condition.Should().Be("e.ByControlType(FlaUI.Core.Definitions.ControlType.Button)");
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
