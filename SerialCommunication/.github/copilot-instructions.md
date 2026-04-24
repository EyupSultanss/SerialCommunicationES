# Copilot instructions for SerialCommunication (WinForms)

Summary
- Single-project WinForms desktop app (C# targeting .NET Framework 4.7.2).
- Entry: Program.Main -> Form1 (Form1.cs + Form1.Designer.cs). UI and serial logic live primarily in Form1.

Build / run
- Build with MSBuild (recommended for .NET Framework projects):
  msbuild SerialCommunication.csproj /p:Configuration=Debug
- Alternatively (if SDK supports):
  dotnet build SerialCommunication.csproj
- Run from Visual Studio (open the folder or project) or execute the built binary: bin\Debug\SerialCommunication.exe

Tests / lint
- No test projects detected. No automated linters configured.
- To add tests: create an MSTest / NUnit / xUnit project and run via Visual Studio Test Explorer or dotnet test.

High-level architecture
- Single executable WinForms application.
- Program.cs: application entry and startup settings.
- Form1.cs: main UI logic and serial-port interaction (uses System.IO.Ports).
- Form1.Designer.cs: generated UI layout (controls, event wiring). Keep manual edits minimal — use designer for layout.
- Resources (Resources\*.png) are embedded and referenced from Designer/Resources.Designer.cs.
- Project file: SerialCommunication.csproj targets .NET Framework v4.7.2 and defines Compile/EmbeddedResource items.

Key conventions and patterns
- UI + behavior colocated: event handlers and serial code live in Form1.cs (typical for small WinForms apps). When expanding, consider extracting serial-port logic to a dedicated class to enable headless testing.
- Port enumeration: Form1_Load and comboBox DropDown handlers repopulate available serial ports (SerialPort.GetPortNames()).
- Event handler naming follows controlName_event (e.g., buttonConnect_Click). Designer wires Click events in Form1.Designer.cs; when renaming handlers, update Designer or refactor via the Forms designer.
- Resource usage: Images are stored in Resources and referenced via designer-generated code. Keep image filenames stable to avoid breaking the Designer.
- Language: UI labels and identifiers use Dutch (e.g., "Poort", "Instellingen"). Keep naming consistent when adding new controls.

Files to check when making changes
- Form1.cs / Form1.Designer.cs — primary UI and behavior.
- SerialCommunication.csproj — target framework, references and embedded resources.

If you want, can add guidance to:
- Extract serial logic to a testable class and add unit tests
- Configure a CI job (GitHub Actions) to build the project automatically

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>
