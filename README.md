[![Not Maintained](https://img.shields.io/badge/Maintenance%20Level-Abandoned-orange.svg)](https://gist.github.com/cheerfulstoic/d107229326a01ff0f333a1d3476e068d)

# FlaUIRecorder
UIAutomation test recorder for using with [FlaUI library](https://github.com/Roemer/FlaUI).
![Alt text](https://github.com/Roemer/FlaUI/blob/master/FlaUI.png?raw=true "FlaUI")

This application records all mouse actions and key strokes and generates (C#) code which can be used to automate the ui using [FlaUI](https://github.com/Roemer/FlaUI).

## Code provider
Supporting multiple code generator/provider increases the usability of this application. At the moment only C# code provider is implemented.

## Roadmap
* Support for key strokes
* Add verifications (Adds e.g. `Assert.Equal(uielement.Text, "myvalue")` to the generated code). Required for automated ui tests.
* Nicer project ui

## Test project restore

Test projects use NUnit 3.14 and FluentAssertions 6.12. If NuGet restore fails due to network or SSL issues, run from the repository root:

```powershell
.\restore-tests.ps1
```

A `NuGet.Config` at the solution root points to `https://api.nuget.org/v3/index.json`.

## Export options

When exporting a project (File → Export Project), you can choose:
- **FlaUI 1.2.0 (net461)** — matches the recorder runtime (default)
- **FlaUI 4.0 (net472)** — upgrade path for modern test projects

The recorder itself stays on FlaUI 1.2.0; only exported projects can target newer versions.

## Recording hotkeys

During recording, global hotkeys are available:
- `Ctrl+Shift+Alt+P` — pause/resume
- `Ctrl+Shift+Alt+C` — add comment
- `Ctrl+Shift+Alt+W` — insert wait
- `Ctrl+Shift+Alt+A` — add assertion
- `Ctrl+Shift+Alt+E` — pick element from tree
- `Ctrl+Shift+Alt+Esc` — cancel recording

## Credits
This application based on the [FlaUI](https://github.com/Roemer/FlaUI) and some ideas of [FlaUI Inspect](https://github.com/FlauTech/FlaUInspect).

### Contribution
Feel free to fork FlaUIRecorder and send pull requests of your modifications.<br />
You can also create issues if you find problems or have ideas on how to further improve this application.
