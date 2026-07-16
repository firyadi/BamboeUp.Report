# BamboeUp Report Engines (§9B)

Multi-renderer plug-in architecture for RPT/DOC reports. This folder is a **standalone git repository**; clone it as a sibling of `BamboeUp.Api`:

```
d:\Project\BamboeUp\
├── BamboeUp.Api\
├── BamboeUp.Report\          ← this repo
└── ReportEngines.Local.props   ← optional parent-level overrides (Api solution builds)
```

## Repository layout

```
BamboeUp.Report/
├── BamboeUp.Report.csproj      ← core abstractions, handlers, services
├── Abstractions/, Handlers/, Services/, Doc/, Rpt/, Pvt/
├── engines/
│   ├── BamboeUp.Report.QuestPdf/
│   ├── BamboeUp.Report.FastReport/
│   ├── BamboeUp.Report.Telerik/
│   ├── BamboeUp.Report.DevExpress/
│   └── BamboeUp.Report.Engines/   ← DI bootstrap (references all engines)
├── BamboeUp.Report.sln
├── Directory.Build.props
└── ReportEngines.Local.props.example
```

## Projects

| Project | Engine | Template | NuGet / License |
|---------|--------|----------|-----------------|
| `BamboeUp.Report` | Core abstractions | — | — |
| `engines/BamboeUp.Report.QuestPdf` | QuestPDF | C# fluent (`DocPdfRenderer`) | nuget.org (Community) |
| `engines/BamboeUp.Report.FastReport` | FastReport Open Source | `.frx` | nuget.org |
| `engines/BamboeUp.Report.Telerik` | Telerik Reporting 2024 Q4 | `.trdp` / `.trdx` | Local installer DLLs or Telerik NuGet feed |
| `engines/BamboeUp.Report.DevExpress` | DevExpress 25.2 | `.repx` | DevExpress feed/local components (license) |
| `engines/BamboeUp.Report.Engines` | DI bootstrap | — | references all engines |

## Runtime flow

```
ReportService → ReportHandlerSet (RPT / DOC / PVT)
                    → SP via IReportDataProvider
                    → IReportRenderer (by ReportDefinition.RendererType)
                    → PDF bytes
```

## Setup licensed engines

### Telerik (local installer — no NuGet)

If Telerik Reporting is installed (e.g. `C:\Program Files (x86)\Progress\Telerik Reporting 2024 Q4\`):

1. Copy `ReportEngines.Local.props.example` → `ReportEngines.Local.props` (in this repo root, or parent `BamboeUp/` for Api solution builds)
2. Set `EnableTelerikReport=true` and confirm `TelerikReportingPath` points to the `Bin` folder
3. `dotnet build` — project references `Telerik.Reporting.dll` directly from installer

### Telerik / DevExpress (NuGet alternative)

1. Copy `ReportEngines.Local.props.example` → `ReportEngines.Local.props` (in this repo root, or parent `BamboeUp/` for Api solution builds)
2. Set `EnableTelerikReport=true` and/or `EnableDevExpressReport=true`
3. Set `UseTelerikLocalInstall=false` for Telerik NuGet mode
4. Add vendor NuGet sources in Visual Studio:
   - **Telerik:** `Telerik_Reporting_2024_Q4_18_3_24_1112_DEV`
   - **DevExpress:** `DevExpress 25.2 Local` or feed
5. `dotnet restore` and rebuild

Without licensed packages or local install, Telerik/DevExpress renderers return a clear configuration error at runtime.

## Database

Run `BamboeUp.Api/Table/ReportDefinition_RendererType.sql` to add `[core].[ReportDefinition].RendererType`.

## Template folders

Place designer files under `BamboeUp.Report/`:

```
BamboeUp.Report/
  Rpt/Standard/*.frx | *.trdp | *.repx
  Doc/Standard/*.frx
  Pvt/Standard/*.trdp
```

`ReportTemplateOptions.TemplateRoot` defaults to `Reports/` under the API output directory, or the `BamboeUp.Report` project folder in development.

## ReportDefinition fields

| Column | Example |
|--------|---------|
| `RendererType` | `QuestPDF`, `FastReport`, `Telerik`, `DevExpress` |
| `FilePath` | `Rpt/Standard/PayrollSummary.frx` |

If `RendererType` is null, runtime infers from `FilePath` extension; DOC defaults to `QuestPDF`.

Admin UI (`96.00.04` Report Definition) exposes **Renderer Engine** dropdown — no SQL required for engine selection.

## E2E verification

See `BamboeUP.App/Document/ReportEngine-E2E-Checklist.md` for Bank toolbar print sign-off (QuestPDF, FastReport, Telerik).
