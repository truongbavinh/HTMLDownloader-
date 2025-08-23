
# Reproducibility — Two URL Flows (Farfetch)

This guide shows how to reproduce the demo with **two inputs**:
1) **Category URLs** → the app **discovers product links** and downloads them.  
   File: `samples/urls.category.farfetch.txt`
2) **Product URLs** → the app **loads a list of product pages** and downloads them directly.  
   File: `samples/urls.products.farfetch.txt`

> **Legal note:** This demo uses Farfetch pages. Keep your runs **limited, non‑commercial, and polite** (rate limits, MaxDepth=1) and review `docs/LEGAL.md` (robots/ToS).

---

## 1) Environment

- OS: **Windows 10/11 (x86/x64)**
- App: **HtmlDownloader v1.0** (GitHub Releases tag `v1.0`)
- Runtimes: **.NET Framework 4.8** and **Microsoft Edge WebView2 Runtime (Evergreen)**  
  → See `docs/RUNTIME.md` for official Microsoft links.
- Network: normal internet access

> If you regenerate for camera‑ready results later, note the exact app tag (e.g., `v1.0.1`) and repeat with that tag.

---

## 2) Inputs

- **Discovery flow (category):** `samples/urls.category.farfetch.txt`  
  Contains one seed URL (women’s clothing category):
  ```text
  https://www.farfetch.com/vn/shopping/women/clothing-1/items.aspx
  ```
  *(Optional) Selectors for discovery:* `samples/selectors.farfetch.json`  
  These CSS/regex rules extract product links (`-item-`) at **MaxDepth = 1**.

- **Direct flow (products):** `samples/urls.products.farfetch.txt`  
  A list of **50 product detail URLs** (you should replace the placeholders with real links before release).

---

## 3) Steps — Discovery from category (auto-find product links)

1. **Install or extract** the app  
   - MSI: run `HtmlDownloader.msi` → Next → Finish.  
   - Portable: extract `HtmlDownloader-v1.0-win32.zip` to a user‑writable folder.
2. **Launch** `HtmlDownloader.exe` (adjust the name if your build differs).
3. **Load seed**  
   - *Option A:* **File → Open Project…** → `samples/demo.discovery.farfetch.project` (if provided); **or**  
   - *Option B:* In the UI, set the seed to the first line of `samples/urls.category.farfetch.txt`.
4. **Load discovery settings**  
   - Click **Load selectors** (if your UI supports it) → pick `samples/selectors.farfetch.json`; otherwise ensure your built‑in rules match `a[href*='-item-']` with **MaxDepth=1** and **same‑domain**.
5. **Discover**  
   Click **Discover** to extract product links from the category page. You should see URLs containing `-item-`.
6. **Review & Download**  
   Review the discovered list. Click **Download** to save each product page as **MHTML** into your chosen output folder (e.g., `results/farfetch-discovery/`).

---

## 4) Steps — Direct from product URLs (load list → download)

1. **Launch** `HtmlDownloader.exe`.
2. **Import URLs** → select `samples/urls.products.farfetch.txt`.
3. **Choose an output folder** (e.g., `results/farfetch-direct/`).
4. **Download** → the app fetches each product URL and saves **MHTML** files.

---

## 5) Verify outputs (counts & SHA256)

Open **PowerShell** in your **output folder** and run:

```powershell
# Count files (expect ≈ number of discovered/imported URLs)
"Files:"; Get-ChildItem -File | Measure-Object | Select-Object -ExpandProperty Count

# List files in a stable order
Get-ChildItem -File | Sort-Object Name | ForEach-Object Name

# Compute SHA256 for each output
Get-ChildItem -File | Sort-Object Name | ForEach-Object {
  $h = (Get-FileHash $_ -Algorithm SHA256).Hash.ToUpper()
  "{0}  {1}" -f $h, $_.Name
}
```

Save the checksums for your run:
```powershell
Get-ChildItem -File | Sort-Object Name | ForEach-Object {
  (Get-FileHash $_ -Algorithm SHA256).Hash.ToUpper() + "  " + $_.Name
} | Out-File "..\checksums_outputs.txt" -Encoding ASCII
```

> Running the same inputs twice should produce the **same file counts**. For dynamic sites, checksums may differ; this is expected.

---

## 6) Repeatability guidance

- Keep **MaxDepth = 1**, **Parallel ≤ 1–2**, **Delay ≥ 2000 ms**.  
- Fix the same **User‑Agent** and **timeouts** across runs. If your app allows, store these in a config JSON.  
- For camera‑ready reproducibility, consider including a small **static** fallback list (Books to Scrape).

Example minimal config (optional):
```json
{
  "UserAgent": "HtmlDownloader/1.0 (contact: you@example.com)",
  "NavigationTimeoutMs": 30000,
  "PostLoadDelayMs": 2000,
  "ParallelDownloads": 1,
  "MaxDepth": 1,
  "OutputFormat": "mhtml"
}
```

---

## 7) Exact versions (fill on release)

- App: **v1.0** (commit: `<commit-hash-here>`)  
- Dependencies:
  - Microsoft.Web.WebView2 (NuGet) — version at build time
  - .NET Framework 4.8
  - Microsoft Edge WebView2 Runtime (Evergreen)

Record these in the camera‑ready submission for traceability.

---

## 8) Troubleshooting

- **SmartScreen**: After verifying SHA256, choose **More info → Run anyway**.  
- **`WebView2Loader.dll` missing**: Ensure it sits **next to** `HtmlDownloader.exe` (already packaged).  
- **`Access is denied (0x80070005)`**: Don’t run the portable build from `Program Files`; use a user‑writable folder.  
- **No links discovered**: Check that selectors are loaded; ensure **MaxDepth=1** and **same‑domain**; the site may have changed markup.

---

## 9) Citation

When reproducing or building upon this tool, please cite the software and paper per the Journal guidelines. Include the exact version tag (e.g., `v1.0`) and a link to the GitHub Release.
