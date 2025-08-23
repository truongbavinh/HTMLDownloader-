# Reproducibility — HTMLDownloader v1.0

This guide reproduces the demo using **two URL types** your app supports:
1) **Category URLs** (discovery flow): start from a category page, **auto‑discover product links**, then download.
   - File: `samples/urls.category.farfetch.txt`
2) **Product URLs** (direct flow): load a ready‑made list of product pages and download directly.
   - File: `samples/urls.products.farfetch.txt`

> See also: `docs/RUNTIME.md` (runtimes), `docs/LEGAL.md` (robots/ToS for Farfetch).

---

## 1) Environment

- OS: **Windows 10/11 (x86/x64)**
- App: **HtmlDownloader v1.0** (GitHub Release tag `v1.0`)
- Runtimes: **.NET Framework 4.8**, **Microsoft Edge WebView2 Runtime (Evergreen)**
- Network: normal internet access

> Pin the exact version (e.g., `v1.0`) when reporting results.

---

## 2) Inputs

- **Discovery (category)** — `samples/urls.category.farfetch.txt`  
  Contains a category URL, e.g.
  ```text
  https://www.farfetch.com/vn/shopping/women/clothing-1/items.aspx
  ```

- **Direct (products)** — `samples/urls.products.farfetch.txt`  
  Contains **50 product detail URLs** (one per line). Replace placeholders with real product URLs for your demo.

---

## 3) UI Field Reference (how the app works)

- **Link**: the main input field for a URL (category or product).  
- **CSS Class (Link)**: class of the `<a>` tag to extract product links.  
  - The app **attempts to auto‑detect** this. If missing/incorrect, open **DevTools (F12)** → inspect target `<a>` → copy its class and paste here. *(See Figure 3 in the paper.)*
- **Destination**: folder to save outputs (MHTML).  
- **Page scrolls** *(default: 5)*: how many times to scroll the page (for infinite scroll).  
- **Scroll delay** *(default: 5 s)*: delay between scrolls.  
- **Show more** *(checkbox)*: tick if the page uses a **“Show more”** button.  
  - **CSS Show more**: CSS selector for that button.  
  - **Num click**: how many times to click it.  
- **Paging** *(checkbox)*: tick if the page uses **Next/Previous pagination**.  
  - **CSS Next paging**: CSS selector for the **Next** button.  
  - **Num click**: number of pagination clicks.  
- **Get links**: extract product links from the main page (category) using the CSS above.  
- **Save links / Load links**: persist or import a list of product URLs (text file; one URL per line).  
- **Processing download**: start downloading the current list of product URLs (to MHTML).  
- **Processing Crawl**: fallback mode if the site blocks downloads (uses Google Chrome + a custom extension to perform the download).  
- **Progress / Status**: monitor the job and see per‑URL results.

---

## 4) Procedure A — Discovery from a **Category URL**

1. **Enter the category URL** into **Link** (or load from `samples/urls.category.farfetch.txt`).  
2. Wait for **CSS Class (Link)** auto‑detection; if not found/incorrect:  
   - Press **F12** to open DevTools → inspect the product **`<a>`** element → copy the class → paste into **CSS Class (Link)**.  
3. **Destination**: choose an empty folder (e.g., `results/farfetch-discovery/`).  
4. Configure dynamic loading:
   - **Page scrolls** = 5, **Scroll delay** = 5 s (defaults).  
   - If the page has **Show more**: tick **Show more**, set **CSS Show more**, and **Num click**.  
   - If the page has **pagination**: tick **Paging**, set **CSS Next paging**, and **Num click**.  
5. Click **Get links** to extract product links from the category page.  
6. *(Optional)* Click **Save links** to write them to a file for later reuse.  
7. Click **Processing download** to fetch each product page and save as **MHTML**.  
8. If the site blocks the process, switch to **Processing Crawl** (Chrome + extension).  
9. Monitor progress; when finished, **open the destination folder** to access the MHTML files.

---

## 5) Procedure B — Direct from a **Product URL list**

1. Click **Load links** and select `samples/urls.products.farfetch.txt` (or paste URLs into the app).  
2. **Destination**: choose an empty folder (e.g., `results/farfetch-direct/`).  
3. Click **Processing download** (no discovery needed).  
4. If blocked, use **Processing Crawl**.  
5. Check the output folder for generated **.mhtml** files.

---

## 6) Verify outputs (counts & SHA256)

Open **PowerShell** in your **output folder** and run:

```powershell
# Count files (expect ≈ number of links)
"Files:"; Get-ChildItem -File | Measure-Object | Select-Object -ExpandProperty Count

# List files in stable order
Get-ChildItem -File | Sort-Object Name | ForEach-Object Name

# Compute SHA256 for each output
Get-ChildItem -File | Sort-Object Name | ForEach-Object {
  $h = (Get-FileHash $_ -Algorithm SHA256).Hash.ToUpper()
  "{0}  {1}" -f $h, $_.Name
}
```

Save to a text file:
```powershell
Get-ChildItem -File | Sort-Object Name | ForEach-Object {
  (Get-FileHash $_ -Algorithm SHA256).Hash.ToUpper() + "  " + $_.Name
} | Out-File "..\checksums_outputs.txt" -Encoding ASCII
```

> Running the same list twice should produce the **same number of files**. Checksums may differ for highly dynamic pages.

---

## 7) Repeatability notes

- Keep **Page scrolls / delays / Num click** the same across runs.  
- Fix **CSS selectors** once identified (class names can change; re‑check if discovery finds zero links).  
- Use **ParallelDownloads = 1–2** and **Delay ≥ 2 s** for stability; keep the **User‑Agent** constant.  
- For camera‑ready reproducibility, provide a small, stable product list alongside the Farfetch list.

---

## 8) Exact versions (fill on release)

- App: **v1.0** (commit: `<commit-hash-here>`)  
- Dependencies: Microsoft.Web.WebView2 (NuGet), .NET Framework 4.8, WebView2 Runtime (Evergreen)

---

## 9) Troubleshooting

- **SmartScreen**: After verifying SHA256, choose **More info → Run anyway**.  
- **`WebView2Loader.dll` missing**: ensure it sits next to `HtmlDownloader.exe` (already packaged).  
- **`Access is denied (0x80070005)`**: do not run the portable build from `Program Files`; use a user‑writable folder.  
- **No links discovered**: CSS class may be wrong or markup changed; re‑inspect with **F12** and update **CSS Class (Link)** / selectors.  
- **Blocked by bot protection**: use **Processing Crawl** (Chrome + extension) and lower concurrency / add delays.
