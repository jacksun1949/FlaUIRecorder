namespace FlaUIRecorder.CodeProvider.Common
{
    public static class SafeClickCodeGenerator
    {
        public const string HelperMarker = "void SafeClick(";

        private const string ElementType = "FlaUI.Core.AutomationElements.Infrastructure.AutomationElement";

        private const string GetElementClickPointSignature =
            "FlaUI.Core.Shapes.Point GetElementClickPoint(" + ElementType + " element) ";

        private const string GetElementClickPointBody =
            "{\n" +
            "    try\n" +
            "    {\n" +
            "        return element.GetClickablePoint();\n" +
            "    }\n" +
            "    catch (FlaUI.Core.Exceptions.NoClickablePointException)\n" +
            "    {\n" +
            "        var r = element.Properties.BoundingRectangle.Value;\n" +
            "        if (r.IsEmpty || r.Width <= 0 || r.Height <= 0)\n" +
            "            throw;\n" +
            "        return r.Center;\n" +
            "    }\n" +
            "}";

        private const string SafeClickBody =
            "{\n" +
            "    var point = GetElementClickPoint(element);\n" +
            "    if (doubleClick)\n" +
            "        FlaUI.Core.Input.Mouse.DoubleClick(button, point);\n" +
            "    else\n" +
            "        FlaUI.Core.Input.Mouse.Click(button, point);\n" +
            "}";

        private const string SafeClickSignature =
            "void SafeClick(" + ElementType + " element, FlaUI.Core.Input.MouseButton button = FlaUI.Core.Input.MouseButton.Left, bool doubleClick = false) ";

        private const string SafeDragBody =
            "{\n" +
            "    var start = GetElementClickPoint(fromElement);\n" +
            "    FlaUI.Core.Input.Mouse.MoveTo(start);\n" +
            "    FlaUI.Core.Input.Mouse.Down(FlaUI.Core.Input.MouseButton.Left);\n" +
            "    var end = GetElementClickPoint(toElement);\n" +
            "    FlaUI.Core.Input.Mouse.MoveTo(end);\n" +
            "    FlaUI.Core.Input.Mouse.Up(FlaUI.Core.Input.MouseButton.Left);\n" +
            "}";

        private const string SafeDragSignature =
            "void SafeDrag(" + ElementType + " fromElement, " + ElementType + " toElement) ";

        /// <summary>Local functions for inline recorder output (snippets pasted into a method body).</summary>
        public static string CSharpLocalHelperMethod =>
            GetElementClickPointSignature + GetElementClickPointBody + "\n\n" +
            SafeClickSignature + SafeClickBody + "\n\n" +
            SafeDragSignature + SafeDragBody;

        /// <summary>Static helpers for exported RecordedAutomation class (also callable from page objects).</summary>
        public static string CSharpStaticHelperMethod =>
            "public static FlaUI.Core.Shapes.Point GetElementClickPoint(" + ElementType + " element)\n" +
            GetElementClickPointBody + "\n\n" +
            "public static void SafeClick(" + ElementType + " element, FlaUI.Core.Input.MouseButton button = FlaUI.Core.Input.MouseButton.Left, bool doubleClick = false)\n" +
            SafeClickBody + "\n\n" +
            "public static void SafeDrag(" + ElementType + " fromElement, " + ElementType + " toElement)\n" +
            SafeDragBody;

        public static string BuildCSharpClick(string variableName) => $"SafeClick({variableName});";

        public static string BuildCSharpRightClick(string variableName) =>
            $"SafeClick({variableName}, FlaUI.Core.Input.MouseButton.Right);";

        public static string BuildCSharpDoubleClick(string variableName) =>
            $"SafeClick({variableName}, doubleClick: true);";

        public static string BuildCSharpDrag(string fromVariableName, string toVariableName) =>
            $"SafeDrag({fromVariableName}, {toVariableName});";
    }
}
