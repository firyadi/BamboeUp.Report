# Engine Demo — Bank Master Slip

Contoh cetakan toolbar Bank (`02.09.04`) dengan **4 engine berbeda** via `RendererType` + Bootstrap DI.

| Toolbar label | ProgramCode | RendererType | DefinitionKey | Template |
|---------------|-------------|--------------|---------------|----------|
| Bank Master Slip (QuestPDF) | `02.09.04.01` | `QuestPDF` | `BankMasterSlip_QuestPDF` | C# fluent |
| Bank Master Slip (FastReport) | `02.09.04.03` | `FastReport` | `BankMasterSlip_FastReport` | `BankMasterSlip_FastReport.frx` |
| Bank Master Slip (Telerik) | `02.09.04.04` | `Telerik` | `BankMasterSlip_Telerik` | `BankMasterSlip_Telerik.trdx` (2024/4.0) |
| Bank Master Slip (DevExpress) | `02.09.04.05` | `DevExpress` | `BankMasterSlip_DevExpress` | `BankMasterSlip_DevExpress.repx` |

**E2E checklist:** `BamboeUP.App/Document/ReportEngine-E2E-Checklist.md`

Semua memakai SP yang sama: `app.doc_BankMasterSlip`.

## Seed database

```sql
-- Jalankan setelah Bank program & SP ada:
:r BamboeUp.Api\Table\BankPrintEngineDemoSeed.sql
```

## Uji cetak

1. Buka Bank Detail → simpan record bank
2. Toolbar **Print** → pilih slip (nama menampilkan engine)
3. QuestPDF & FastReport berjalan tanpa lisensi tambahan
4. Telerik: aktifkan `ReportEngines.Local.props` — pakai DLL installer lokal (tanpa NuGet) atau feed vendor
5. DevExpress: aktifkan `ReportEngines.Local.props` + NuGet feed vendor
