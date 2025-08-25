# HTMLDownloader — Quick Start (End-User)

## Download
Choose one from the Releases page:
- **Installer (MSI, recommended)**: `HTMLDownloader-v1.0.msi`
- **Portable ZIP**: `HTMLDownloader-v1.0-win32.zip` (no installation)

## Runtime Requirements
- .NET Framework 4.8  
- Microsoft Edge WebView2 Runtime (Evergreen)  
See `docs/RUNTIME.md` for official links and steps.

> **Responsible use:** Please review [LEGAL.md](LEGAL.md) before large-scale runs.  
> Follow website ToS/robots.txt, avoid personal/sensitive data, and use polite delays.

## Download samples
Four sample files are included to quickly try both flows (Discovery & Direct):
- `samples/urls.category.farfetch.txt`
- `samples/urls.products.farfetch.txt`
- `samples/urls.category.rightmove.txt`
- `samples/urls.products.rightmove.txt`

**Locations:**
- **MSI install:** `C:\Program Files (x86)\<Publisher>\<Product>\samples\`
- **Portable ZIP:** `<extracted>\samples\`

## First Run (Quick Check)

1. Open **HTMLDownloader**.
2. Open the bundled **samples** folder:
   - **MSI install:** `C:\Program Files (x86)\<Publisher>\<Product>\samples\`
   - **Portable ZIP:** `<extracted>\samples\`

### Option A — Direct (fastest)
1) Click **Load links** → select `samples/urls.products.farfetch.txt` *(or `...rightmove.txt`)*.  
2) Set **Destination** → choose a user-writable folder (e.g., Documents/Desktop).  
3) Click **Processing download** → `.mhtml` files will appear in the destination folder.

### Option B — Discovery (from a category page)
1) Open `samples/urls.category.farfetch.txt` and **copy the URL** into the **Link** field.  
2) Wait for **CSS Class (Link)** to auto-detect; if wrong/missing, press **F12** (DevTools), inspect a product `<a>`, and paste its class into **CSS Class (Link)**.  
3) If the page uses infinite scroll or pagination:
   - **Page scrolls** (default: 5) & **Scroll delay** (default: 5 s)  
   - If there is **Show more**: check **Show more** and fill **CSS Show more** + **Num click**  
   - If there is **Pagination**: check **Paging** and fill **CSS Next paging** + **Num click**  
4) Click **Get links** → review the link list → **Processing download**.

> If the website employs bot protection, switch to **Processing Crawl** (Chrome + extension) and slow down (lower **Parallel**, increase **Delay**).

**Done:** The destination folder now contains the `.mhtml` files — confirming the environment (.NET 4.8 + WebView2) is working.

## Troubleshooting
- **SmartScreen:** *More info → Run anyway* (after verifying SHA256).  
- **`WebView2Loader.dll` error:** ensure this DLL sits next to `HTMLDownloader.exe` (already included).

## Install (MSI)
![Install demo](img/install.gif)

## Portable (ZIP)
![Portable demo](img/portable.gif)

## First Run
![First run](img/play.gif)
