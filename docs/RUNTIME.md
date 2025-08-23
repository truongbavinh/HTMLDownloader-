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
