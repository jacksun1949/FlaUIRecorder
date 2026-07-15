using FlaUI.Core.Definitions;
using CC_FlaUIAutomationRecoder.CodeProvider.Common;
using CC_FlaUIAutomationRecoder.Tests.Common;
using FluentAssertions;
using NUnit.Framework;

namespace CC_FlaUIAutomationRecoder.CodeProvider.CSharp.Tests
{
    [TestFixture]
    public class ShellElementFilterTests
    {
        [Test]
        public void IsDesktopPane_MatchesChineseDesktopName()
        {
            var pane = new ElementBuilder().CreatePane().WithName("桌面 1").Build();

            ShellElementFilter.IsDesktopPane(pane).Should().BeTrue();
            ShellElementFilter.IsShellOrDesktopElement(pane, null, null).Should().BeTrue();
        }

        [Test]
        public void IsDesktopPane_MatchesEnglishDesktopName()
        {
            var pane = new ElementBuilder().CreatePane().WithName("Desktop 2").Build();

            ShellElementFilter.IsDesktopPane(pane).Should().BeTrue();
            ShellElementFilter.IsShellOrDesktopElement(pane, null, null).Should().BeTrue();
        }

        [Test]
        public void IsDesktopPane_DoesNotMatchApplicationPane()
        {
            var pane = new ElementBuilder().CreatePane().WithName("TitleBar").Build();

            ShellElementFilter.IsDesktopPane(pane).Should().BeFalse();
            ShellElementFilter.IsShellOrDesktopElement(pane, null, null).Should().BeFalse();
        }

        [Test]
        public void IsDesktopPane_DoesNotMatchButton()
        {
            var button = new ElementBuilder().CreateButton().WithName("最小化").Build();

            ShellElementFilter.IsDesktopPane(button).Should().BeFalse();
            ShellElementFilter.IsShellOrDesktopElement(button, null, null).Should().BeFalse();
        }
    }
}
