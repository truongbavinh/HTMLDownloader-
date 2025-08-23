# Target Framework Rationale

## Current choice
We target **.NET Framework 4.8 (Windows)** for this review release to:
- Maximize compatibility with existing Windows environments.
- Avoid UI regressions in WinForms/WebView2 during the review timeline.
- Keep packaging simple and reproducible for end-users (MSI + portable ZIP).

## Trade-offs
- .NET Framework 4.8 is Windows-only and in maintenance mode.
- Newer features (single-file, trimming, cross-platform) are available in .NET 8 LTS.

## Roadmap (post-review)
We will evaluate migration to **.NET 8 LTS**:
- WinForms/WebView2 on .NET 8.
- Consider **self-contained** and **single-file** publishing to remove external runtime dependencies.
- Provide side-by-side “Preview” builds and a migration note in the release where applicable.

## What users need today
- Windows 10/11
- .NET Framework 4.8 and Microsoft Edge WebView2 Runtime (see `docs/RUNTIME.md`)
