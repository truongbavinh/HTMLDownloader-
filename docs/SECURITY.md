# Security & Trust

This document explains how we distribute binaries, how you can verify their authenticity, and what to do if Windows warns about running the app.

> TL;DR: **Download only from the official GitHub Releases page**, verify **SHA256** checksums from `checksums.txt`, then run the installer or portable ZIP.

---

## Official Distribution

We publish binaries **only** on the repository’s **GitHub Releases** page. Each release includes:
- `HTMLDownloader-<version>.msi` — MSI installer
- `HTMLDownloader-<version>-win32|win64.zip` — portable build
- `checksums.txt` — SHA256 checksums for the above files

If you obtained the files elsewhere, consider them **untrusted**.

---

## Verify Authenticity (SHA256)

Open **Windows PowerShell** in the folder containing the downloaded files and run:

```powershell
# Compute SHA256 for both MSI and ZIP
Get-FileHash .\HTMLDownloader-<version>.msi -Algorithm SHA256
Get-FileHash .\HTMLDownloader-<version>-win32.zip -Algorithm SHA256   # or -win64.zip

# Compare with the values listed in checksums.txt (shipped in the Release)
Get-Content .\checksums.txt
```

Optional helper (auto-compare both files against `checksums.txt`):

```powershell
# Save as verify.ps1 in the same folder as the downloads
$ref = Get-Content .\checksums.txt
$files = @("HTMLDownloader-<version>.msi", "HTMLDownloader-<version>-win32.zip", "HTMLDownloader-<version>-win64.zip") | Where-Object { Test-Path $_ }
foreach ($f in $files) {
  $h = (Get-FileHash $f -Algorithm SHA256).Hash.ToUpper()
  if ($ref -match $h) { Write-Host "[OK] $f matches SHA256" -ForegroundColor Green }
  else { Write-Host "[MISMATCH] $f does NOT match SHA256" -ForegroundColor Red }
}
```

> **Match = Safe to run.** Mismatch means the file is corrupted or tampered—download again from the official Releases page.

---

## Windows SmartScreen & Antivirus Notes

- New or **unsigned** binaries may trigger **Windows SmartScreen**.  
  After verifying SHA256, choose **More info → Run anyway** to proceed.
- Some antivirus tools may flag newly published executables as **“unknown publisher”** or **“rarely seen”**.  
  After checksum verification, you can safely allow/restore the file. If a false positive persists, please report it to your AV vendor and let us know via the contact below.

> Tip: If Windows adds the “**Unblock**” checkbox to the file’s **Properties**, tick it before running.

---

## Code Signing Policy

- If available, we will **code-sign** the MSI and main binaries using Authenticode from a trusted CA.
- If a specific release is **not** signed, we provide **SHA256 checksums** and host binaries **only** on GitHub Releases to ensure integrity and traceability.

To check a digital signature when present:

```powershell
Get-AuthenticodeSignature .\HTMLDownloader-<version>.msi | Format-List *
```

---

## Runtime & Permissions

- Required runtimes: **.NET Framework 4.8** and **Microsoft Edge WebView2 Runtime (Evergreen)**. See `docs/RUNTIME.md`.
- The app stores WebView2 **user data** under `%LOCALAPPDATA%\HTMLDownloader\WebView2` and logs (if enabled) under `%LOCALAPPDATA%\HTMLDownloader\logs`.
- No elevated privileges are required for the portable build. The MSI installer may request elevation to write to `Program Files`.

---

## Reporting Security Issues

If you discover a vulnerability or have a security concern, please contact:

- **Primary email:** tbvinh25nct@hutech.edu.vn  
- **Alternate:** truongbavinh@gmail.com

We appreciate responsible disclosure. Please avoid filing public issues for sensitive reports; email us first.

---

## Supply-Chain Notes

- Build artifacts are produced from this repository’s source.  
- Release assets (MSI/ZIP) are attached exclusively to the GitHub Release associated with the corresponding tag (e.g., `v1.0.0`).
