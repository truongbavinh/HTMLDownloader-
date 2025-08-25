# HTMLDownloader — Quick Start (End-User)

## Download
Choose one from the Releases page:
- **Installer (MSI, recommended)**: `HTMLDownloader-v1.0.msi`
- **Portable ZIP**: `HTMLDownloader-v1.0-win32.zip` (no installation)

## Runtime Requirements
- .NET Framework 4.8
- Microsoft Edge WebView2 Runtime (Evergreen)  
See `docs/RUNTIME.md` for links and steps.
> **Responsible use:** Please review [LEGAL.md](LEGAL.md) before downloading at scale.
> Follow website ToS/robots.txt, avoid personal/sensitive data, and use polite delays.

## Download samples
Bạn có sẵn 4 file mẫu để chạy nhanh hai luồng (Discovery & Direct):
- `samples/urls.category.farfetch.txt`
- `samples/urls.products.farfetch.txt`
- `samples/urls.category.rightmove.txt`
- `samples/urls.products.rightmove.txt`

**Vị trí:**
- **MSI install:** `C:\Program Files (x86)\<Publisher>\<Product>\samples\`
- **Portable ZIP:** `<extracted>\samples\`

## First Run (Quick Check)

1. Open **HtmlDownloader**.
2. Open the bundled **samples** folder:
   - **MSI install:** `C:\Program Files (x86)\<Publisher>\<Product>\samples\`
   - **Portable ZIP:** `<extracted>\samples\`

### Option A — Direct (fastest)
1) Click **Load links** → chọn `samples/urls.products.farfetch.txt` *(hoặc `...rightmove.txt`)*  
2) Set **Destination** → chọn thư mục người dùng (Documents/Desktop).  
3) Click **Processing download** → các file **.mhtml** sẽ xuất hiện trong thư mục đích.

### Option B — Discovery (from category)
1) Mở `samples/urls.category.farfetch.txt` và **copy URL** dán vào ô **Link**.  
2) Đợi **CSS Class (Link)** tự nhận; nếu sai/thiếu, nhấn **F12** (DevTools) → inspect thẻ `<a>` sản phẩm → copy class vào **CSS Class (Link)**.  
3) Nếu trang có cuộn vô hạn/phân trang:
   - **Page scrolls** (mặc định 5) & **Scroll delay** (mặc định 5s)  
   - Nếu có **Show more**: tick **Show more** + điền **CSS Show more** + **Num click**  
   - Nếu có **Pagination**: tick **Paging** + điền **CSS Next paging** + **Num click**  
4) Click **Get links** → rà soát danh sách link → **Processing download**.

> Nếu website chặn bot, chuyển sang **Processing Crawl** (Chrome + extension) và giảm tốc độ (Parallel thấp, tăng Delay).

**Hoàn tất:** Thư mục đích xuất hiện các file **.mhtml** ⇒ xác nhận môi trường (.NET 4.8 + WebView2) hoạt động bình thường.

## Troubleshooting
- SmartScreen: *More info → Run anyway* (after verifying SHA256).
- If `WebView2Loader.dll` error appears, ensure this DLL sits next to `HTMLDownloader.exe` (already included).

## Install (MSI)
![Install demo](img/install.gif)

## Portable (ZIP)
![Portable demo](img/portable.gif)

## First Run
![First run](img/play.gif)
