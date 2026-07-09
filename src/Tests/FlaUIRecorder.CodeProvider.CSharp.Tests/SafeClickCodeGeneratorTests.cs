using FlaUIRecorder.CodeProvider.Common;
using FluentAssertions;
using NUnit.Framework;

namespace FlaUIRecorder.CodeProvider.CSharp.Tests
{
    [TestFixture]
    public class SafeClickCodeGeneratorTests
    {
        [Test]
        public void BuildCSharpClick_EmitsSafeClickCall()
        {
            SafeClickCodeGenerator.BuildCSharpClick("titleBar").Should().Be("SafeClick(titleBar);");
        }

        [Test]
        public void BuildCSharpRightClick_UsesRightMouseButton()
        {
            SafeClickCodeGenerator.BuildCSharpRightClick("menuItem")
                .Should().Be("SafeClick(menuItem, FlaUI.Core.Input.MouseButton.Right);");
        }

        [Test]
        public void BuildCSharpDoubleClick_PassesDoubleClickFlag()
        {
            SafeClickCodeGenerator.BuildCSharpDoubleClick("item")
                .Should().Be("SafeClick(item, doubleClick: true);");
        }

        [Test]
        public void CSharpLocalHelperMethod_UsesBoundingRectangleCenterFallback()
        {
            SafeClickCodeGenerator.CSharpLocalHelperMethod.Should().Contain("GetClickablePoint()");
            SafeClickCodeGenerator.CSharpLocalHelperMethod.Should().Contain("NoClickablePointException");
            SafeClickCodeGenerator.CSharpLocalHelperMethod.Should().Contain("element.Properties.BoundingRectangle.Value");
            SafeClickCodeGenerator.CSharpLocalHelperMethod.Should().Contain("r.Center");
            SafeClickCodeGenerator.CSharpLocalHelperMethod.Should().Contain("FlaUI.Core.Input.Mouse.Click(button, point)");
            SafeClickCodeGenerator.CSharpLocalHelperMethod.Should().Contain("FlaUI.Core.Input.Mouse.DoubleClick(button, point)");
            SafeClickCodeGenerator.CSharpLocalHelperMethod.Should().Contain("void SafeDrag(");
            SafeClickCodeGenerator.CSharpLocalHelperMethod.Should().Contain("FlaUI.Core.Input.Mouse.Down(FlaUI.Core.Input.MouseButton.Left)");
            SafeClickCodeGenerator.CSharpLocalHelperMethod.Should().Contain("FlaUI.Core.Input.Mouse.Up(FlaUI.Core.Input.MouseButton.Left)");
        }

        [Test]
        public void BuildCSharpDrag_EmitsSafeDragCall()
        {
            SafeClickCodeGenerator.BuildCSharpDrag("source", "target")
                .Should().Be("SafeDrag(source, target);");
        }

        [Test]
        public void CSharpStaticHelperMethod_IsPublicStatic()
        {
            SafeClickCodeGenerator.CSharpStaticHelperMethod.Should().Contain("public static void SafeClick(");
            SafeClickCodeGenerator.CSharpStaticHelperMethod.Should().Contain("public static void SafeDrag(");
            SafeClickCodeGenerator.CSharpStaticHelperMethod.Should().Contain("element.Properties.BoundingRectangle.Value");
            SafeClickCodeGenerator.CSharpStaticHelperMethod.Should().Contain("FlaUI.Core.Input.Mouse.Click(button, point)");
        }
    }
}
