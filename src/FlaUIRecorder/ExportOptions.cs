namespace FlaUIRecorder
{
    /// <summary>
    /// Options for exporting a standalone test project.
    /// The recorder itself stays on FlaUI 1.2.0 / net461; only exported projects can target newer FlaUI.
    /// </summary>
    public class ExportOptions
    {
        /// <summary>
        /// FlaUI 1.2.0 on net461 — matches the recorder runtime and avoids API surprises.
        /// </summary>
        public const string FlaUIVersion12 = "1.2.0";

        /// <summary>
        /// FlaUI 4.x on net472 — upgrade path for modern exported test projects.
        /// See ProjectExporter for migration notes (API changes between 1.x and 4.x).
        /// </summary>
        public const string FlaUIVersion40 = "4.0.0";

        public string FlaUIVersion { get; set; } = FlaUIVersion12;
        public string TargetFramework { get; set; } = "net461";
        public bool GeneratePageObjects { get; set; } = true;
        public bool CaptureScreenshotOnFailure { get; set; } = true;
        public int PageObjectActionThreshold { get; set; } = 10;

        public bool IsFlaUI40 => FlaUIVersion.StartsWith("4");
    }
}
