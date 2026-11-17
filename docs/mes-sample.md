# MES sample: current scope and verification

The catalog opens a separate MES sample from its toolbar. It uses the existing shared WPF resources, button, text box and DataGrid. It has 10,000 synthetic rows, text filtering, built-in column sorting, and a selection-linked inspector. IDs are strings so leading zeroes remain intact. No equipment, production service or command is connected.

Baseline: Windows 10.0.26200, .NET SDK 11.0.100, Windows Desktop runtime 10.0.15; `dotnet build VisionApp.Dev.slnx --no-restore` succeeded with zero warnings and errors. The sample was rendered by the running WPF application with `dotnet run --project Operations.UiCatalog -- --mes-snapshot=artifacts/mes.png`; the captured image is `mes-sample.png`. This is a rendering check, not an interaction, DPI, accessibility or performance measurement.

This is an initial sample, not the requested complete design system. Existing resources still use the old palette, sizing and navigation. Theme/density independence, high contrast, full Foundation/Component resource architecture, HMI and ERP, status contracts, stable selection across refresh, keyboard/IME testing, popup/dialog checks and performance observations remain to be implemented and validated. Filtering out the selected item clears the inspector. The quantity column sorts by the numeric produced count.
