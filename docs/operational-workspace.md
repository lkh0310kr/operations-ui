# Component: StatusIndicator and Inspector

These controls are domain-independent Component building blocks. Their resources are in `Themes/Component/OperationalWorkspace.xaml`, merged after the existing compatibility components in both theme entry points. They use Foundation semantic brushes; they do not reference equipment, communication or application data.

## StatusIndicator

Use a short, explicit status string in `Content`; the marker is supplemental and never the only meaning. `Tone` is presentation emphasis (`Neutral`, `Positive`, `Warning`, `Danger`), not a domain state model. A consumer can render equipment state, connection, freshness, alarm acknowledgement and command state simultaneously as separate indicators. In particular, running and stopped have no automatic color assignment. Default tone is neutral. The control has no command or independent keyboard action; place actions in separately labelled controls. Its text is exposed through the normal WPF content/automation tree. A caller must provide meaningful content, including a text fallback for missing data. Long text needs sufficient layout width; use an adjacent explanation for detailed status.

Example: `<ui:StatusIndicator Content="연결 끊김" Tone="Warning" />`. The caller supplies the actual connection result and chooses the wording and tone; the component does not infer freshness or device state.

## Inspector

Use `Header` for the selected context and `Content` for the detail or editing controls. Background, border, padding, content templates and other standard `HeaderedContentControl` properties are available. Content scrolls vertically when it exceeds the available height. The inspector itself does not capture keyboard focus or execute commands; embedded WPF controls keep their own Tab and command behavior. Provide a meaningful accessible name and an explicit empty-selection message in the content. Long text should wrap. Selection, draft values, validation and persistence belong to the consuming application.

Example: `<ui:Inspector Header="선택한 작업" AutomationProperties.Name="선택한 작업 상세">…</ui:Inspector>`.

## MES sample check

`dotnet run --project Operations.UiCatalog --no-build -- --verify-mes=artifacts/mes-check.txt` opens a real WPF window and programmatically changes its filter and search, confirms that a hidden selection is not displayed and is restored by its ID when visible, checks numeric quantity sorting, and checks the row and column UI virtualization flags. The synthetic data is entirely in memory: this is UI virtualization, not data virtualization. It does not measure speed or exercise mouse, keyboard, screen reader, network refresh or replacing objects with new instances. The current sample restores selection across its local view refresh; a live application must reconcile refreshed objects by ID without changing a pending operation or causing an unexpected row move.
