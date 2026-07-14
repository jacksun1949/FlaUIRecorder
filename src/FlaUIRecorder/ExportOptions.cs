namespace FlaUIRecorder
{
    /// <summary>
    /// Options for exporting a standalone test project.
    /// The recorder runs on FlaUI 5.0.0 / net7.0-windows.
    /// </summary>
    public class ExportOptions
    {
        /// <summary>
        /// FlaUI 5.0.0 on net7.0-windows — matches the recorder runtime.
        /// </summary>
        public const string FlaUIVersion50 = "5.0.0";

        /// <summary>
        /// FlaUI 4.0 on net472 — legacy option for older targets.
        /// </summary>
        public const string FlaUIVersion40 = "4.0.0";

        public string FlaUIVersion { get; set; } = FlaUIVersion50;
        public string TargetFramework { get; set; } = "net7.0-windows";
        public bool GeneratePageObjects { get; set; } = true;
        public bool CaptureScreenshotOnFailure { get; set; } = true;
        public bool GenerateHtmlReport { get; set; } = true;
        public int PageObjectActionThreshold { get; set; } = 10;
        public bool ContinueOnError { get; set; } = true;

        public bool IsFlaUI50 => FlaUIVersion.StartsWith("5");
        public bool IsFlaUI40 => FlaUIVersion.StartsWith("4");
    }
}
