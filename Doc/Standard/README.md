# Document print templates (DOC)

Bank toolbar slips (Phase 3.9) use **QuestPDF** (`RendererType = QuestPDF`) with C# fluent layouts in `BamboeUp.Report.QuestPdf`.

## §9B engine plug-in

| RendererType | FilePath example | Engine project |
|--------------|------------------|----------------|
| `QuestPDF` | (optional — layout in C#) | `BamboeUp.Report.QuestPdf` |
| `FastReport` | `Doc/Standard/BankMasterSlip.frx` | `BamboeUp.Report.FastReport` |
| `Telerik` | `Doc/Standard/BankMasterSlip.trdp` | `BamboeUp.Report.Telerik` |
| `DevExpress` | `Doc/Standard/BankMasterSlip.repx` | `BamboeUp.Report.DevExpress` |

Place `.frx` / `.trdp` / `.repx` files in this folder tree when switching from QuestPDF to a designer template.

See `BamboeUp.Report/README.md` for setup and `ReportEngines.Local.props.example` for licensed engines.
