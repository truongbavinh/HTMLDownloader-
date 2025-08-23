# Runtime Requirements

This app targets **Windows** and relies on the following Microsoft runtimes:

- **.NET Framework 4.8**  
  Official download: https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48
- **Microsoft Edge WebView2 Runtime (Evergreen)**  
  Official download: https://developer.microsoft.com/en-us/microsoft-edge/webview2/

> End-users typically need to install these **once** per machine.  
> The MSI installer checks for them; the portable ZIP requires that they are present.

---

## Quick Install (Windows)

1. Install **.NET Framework 4.8** from the official page above.  
2. Install **Microsoft Edge WebView2 Runtime (Evergreen)** from the official page above.  
3. Launch **HTMLDownloader** again.

If your organization uses an offline environment or needs a specific architecture, use the **Standalone** installers linked from those pages.

---

## How to Verify Installation

### .NET Framework 4.8
- **Control Panel → Programs and Features** should list “Microsoft .NET Framework 4.8”.
- Or check the **Release** registry value (>= **528040** indicates .NET 4.8 or later):

```powershell
$rel = (Get-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full' -Name Release -ErrorAction SilentlyContinue).Release
"Release = $rel (>= 528040 means .NET 4.8+)"
```

### WebView2 Runtime (Evergreen)
- **Settings → Apps → Installed apps** should list “Microsoft Edge WebView2 Runtime”.
- Or check via registry (present under any of these keys):

```powershell
$paths = @(
  'HKLM:\SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}',
  'HKLM:\SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}',
  'HKCU:\SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}'
)
$found = $false
foreach ($p in $paths) {
  if (Test-Path $p) {
    $pv = (Get-ItemProperty -Path $p -ErrorAction SilentlyContinue).pv
    if ($pv) { "WebView2 Runtime found, version: $pv"; $found = $true }
  }
}
if (-not $found) { "WebView2 Runtime not found." }
```

---

## Common Issues & Fixes

- **SmartScreen warning (unsigned binaries)**  
  After verifying SHA256 checksums from `checksums.txt`, choose **More info → Run anyway**.

- **`WebView2Loader.dll` missing**  
  Ensure `WebView2Loader.dll` sits **next to** `HTMLDownloader.exe` (already packaged).  
  Portable builds should be extracted to a **user-writable** folder (e.g., `Documents`/`Desktop`), not `Program Files`.

- **`Access is denied (0x80070005)` when initializing WebView2**  
  The WebView2 user data folder must be under a user-writable path (the app uses `%LOCALAPPDATA%\HTMLDownloader\WebView2`).

---

## Notes for IT / Enterprise

- Both runtimes support **per-machine** deployment.  
- WebView2 **Evergreen** is recommended for security and compatibility. If you need a fixed version, use the **Fixed Version** distributions linked from the official WebView2 page.
