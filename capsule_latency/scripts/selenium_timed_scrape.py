from __future__ import annotations

import os
import csv
import time
import random
from time import perf_counter
from pathlib import Path
from urllib.parse import urlparse
from datetime import datetime, timezone

import psutil
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException, WebDriverException

# ===== Cấu hình đơn giản (sửa tại đây khi cần) =====
INPUT_FILE = 'Linktest.txt'    # file .txt mỗi dòng 1 URL
OUT_DIR    = 'output/Selenium'        # thư mục lưu HTML
ERROR_LOG  = 'failed_urls.txt'      # file ghi URL lỗi
HEADLESS   = True                   # chạy headless hay không
TIMEOUT    = 25                     # giây đợi tải trang (WebDriverWait)
THROTTLE   = 0.6                    # nghỉ giữa mỗi URL (giây)
USER_AGENT = (
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 "
    "(KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36"
)

# Logging (cho đánh giá)
METHOD_NAME = "selenium"
CSV_PATH    = 'logs/selenium_latency_logs.csv'
BUDGET_SEC  = 300.0  # right-censor theo ngân sách 300s
# ================================================

def iso_now():
    return datetime.now(timezone.utc).isoformat()

def ensure_dir(p: str | Path):
    p = str(p)
    d = os.path.dirname(p)
    if d and not os.path.exists(d):
        os.makedirs(d, exist_ok=True)

def append_csv_row(row: list[str]):
    ensure_dir(CSV_PATH)
    new_file = not os.path.exists(CSV_PATH)
    header = [
        "method","run_id","started_at_iso","finished_at_iso","domain","url_id","url",
        "http_status","status","elapsed_sec","timeout","timeout_sec","retries","size_bytes",
        "notes","ram_peak_mb","cpu_pct"
    ]
    with open(CSV_PATH, "a", encoding="utf-8", newline="") as f:
        w = csv.writer(f)
        if new_file:
            w.writerow(header)
        w.writerow(row)

def read_urls(path: str) -> list[str]:
    with open(path, 'r', encoding='utf-8') as f:
        return [line.strip() for line in f if line.strip() and not line.strip().startswith('#')]

def filename_from_url(url: str, idx: int) -> str:
    from urllib.parse import urlparse
    p = urlparse(url)
    path = p.path.strip('/')
    base = path.split('/')[-1] if path else (p.netloc.replace('.', '_') or 'index')
    safe = ''.join(ch if ch.isalnum() or ch in ('_', '.', '-') else '_' for ch in base)
    return f"{idx:03d}_{safe}.html"

def build_driver(headless: bool = True) -> webdriver.Chrome:
    opts = webdriver.ChromeOptions()
    if headless:
        opts.add_argument('--headless=new')
    opts.add_argument('--no-sandbox')
    opts.add_argument('--disable-dev-shm-usage')
    opts.add_argument('--disable-gpu')
    opts.add_argument('--window-size=1920,1080')
    opts.add_argument('--lang=vi-VN,vi;q=0.9,en-US;q=0.8,en;q=0.7')
    opts.add_argument(f'--user-agent={USER_AGENT}')
    opts.add_argument('--disable-blink-features=AutomationControlled')
    prefs = { 'profile.managed_default_content_settings.images': 2 }
    opts.add_experimental_option('prefs', prefs)
    return webdriver.Chrome(options=opts)

def wait_loaded(driver: webdriver.Chrome, timeout: int = TIMEOUT) -> None:
    WebDriverWait(driver, timeout).until(
        lambda d: d.execute_script('return document.readyState') == 'complete'
    )
    WebDriverWait(driver, timeout).until(EC.presence_of_element_located((By.TAG_NAME, 'body')))

def accept_cookies(driver: webdriver.Chrome) -> None:
    xpaths = [
        "//*[@id='onetrust-accept-btn-handler']",
        "//button[contains(., 'Accept all')]",
        "//button[contains(., 'Accept All')]",
        "//button[contains(., 'I agree')]",
        "//button[contains(., 'Tôi đồng ý')]",
    ]
    for xp in xpaths:
        try:
            btns = driver.find_elements(By.XPATH, xp)
            for b in btns:
                if b.is_displayed():
                    b.click()
                    time.sleep(0.2)
                    return
        except Exception:
            continue

def gentle_scroll(driver: webdriver.Chrome, steps: int = 6) -> None:
    for i in range(1, steps + 1):
        driver.execute_script(
            "window.scrollTo(0, Math.min(document.body.scrollHeight, window.innerHeight * arguments[0]));",
            i,
        )
        time.sleep(random.uniform(0.1, 0.25))

def save_html(html: str, out_path: Path) -> None:
    out_path.write_text(html, encoding='utf-8')

# ====== Đo CPU/RAM của tiến trình hiện tại + mọi tiến trình con (Chrome) ======
_proc = psutil.Process()
_cpu_cnt = psutil.cpu_count(logical=True) or 1

def cpu_time_tree() -> float:
    total = 0.0
    try:
        plist = [_proc] + _proc.children(recursive=True)
    except Exception:
        plist = [_proc]
    for p in plist:
        try:
            t = p.cpu_times()
            total += (getattr(t, 'user', 0.0) + getattr(t, 'system', 0.0))
        except (psutil.NoSuchProcess, psutil.AccessDenied):
            continue
    return total

def rss_tree_bytes() -> int:
    total = 0
    try:
        plist = [_proc] + _proc.children(recursive=True)
    except Exception:
        plist = [_proc]
    for p in plist:
        try:
            m = p.memory_info()
            total += getattr(m, 'rss', 0)
        except (psutil.NoSuchProcess, psutil.AccessDenied):
            continue
    return total
# ============================================================================

def fetch_one(driver: webdriver.Chrome, url: str, out_path: Path, sampler=None) -> int:
    """Tải một URL và lưu HTML. Trả về size_bytes nếu OK, else 0."""
    driver.get(url)
    if sampler: sampler()
    wait_loaded(driver)
    if sampler: sampler()
    accept_cookies(driver)
    if sampler: sampler()
    gentle_scroll(driver, steps=8)
    if sampler: sampler()
    time.sleep(random.uniform(0.3, 0.7))
    if sampler: sampler()
    html = driver.page_source
    save_html(html, out_path)
    try:
        return os.path.getsize(out_path)
    except Exception:
        return 0

def main() -> int:
    urls = read_urls(INPUT_FILE)
    n = len(urls)
    if n == 0:
        print("No URLs found.")
        return 2

    os.makedirs(OUT_DIR, exist_ok=True)
    ensure_dir(CSV_PATH)
    out_dir = Path(OUT_DIR)

    error_log = open(ERROR_LOG, 'w', encoding='utf-8')

    try:
        driver = build_driver(headless=HEADLESS)
    except WebDriverException as e:
        print(f"[FATAL] Cannot start Chrome driver: {e}")
        return 3

    run_id = datetime.utcnow().strftime("%Y-%m-%dT%H-%M-%SZ")
    success = 0

    try:
        for i, url in enumerate(urls, 1):
            out_file = out_dir / filename_from_url(url, i)

            # ---- Begin per-URL measurement ----
            started_at_iso = iso_now()
            t0 = perf_counter()
            cpu0 = cpu_time_tree()
            peak_rss = rss_tree_bytes()

            def sampler():
                nonlocal peak_rss
                rss = rss_tree_bytes()
                if rss > peak_rss:
                    peak_rss = rss

            status = 'ok'
            size_bytes = 0
            timeout_flag = 0
            http_status = 0  # Selenium không expose trực tiếp
            retries = 0

            try:
                size_bytes = fetch_one(driver, url, out_file, sampler=sampler)
                if size_bytes <= 0:
                    status = 'error:empty_file'
                else:
                    success += 1

            except TimeoutException:
                status = 'timeout'
                timeout_flag = 1
                # lưu dấu vết lỗi
                err_html = Path('outputs/selenium/errors')
                err_html.mkdir(parents=True, exist_ok=True)
                (err_html / f"error_page_{i:03d}.html").write_text(driver.page_source or "", encoding="utf-8")
                try:
                    driver.save_screenshot(str(err_html / f"error_page_{i:03d}.png"))
                except Exception:
                    pass

            except WebDriverException as e:
                status = f'error:{type(e).__name__}'
                err_html = Path('outputs/selenium/errors')
                err_html.mkdir(parents=True, exist_ok=True)
                (err_html / f"error_page_{i:03d}.html").write_text(driver.page_source or "", encoding="utf-8")
                try:
                    driver.save_screenshot(str(err_html / f"error_page_{i:03d}.png"))
                except Exception:
                    pass

            except Exception as e:
                status = f'error:{type(e).__name__}'
                err_html = Path('outputs/selenium/errors')
                err_html.mkdir(parents=True, exist_ok=True)
                (err_html / f"error_page_{i:03d}.html").write_text(driver.page_source or "", encoding="utf-8")
                try:
                    driver.save_screenshot(str(err_html / f"error_page_{i:03d}.png"))
                except Exception:
                    pass

            # ---- End per-URL measurement ----
            t1 = perf_counter()
            cpu1 = cpu_time_tree()

            wall = max(1e-9, t1 - t0)
            d_cpu = max(0.0, cpu1 - cpu0)
            # CPU % của toàn phiên URL này
            cpu_pct = min(100.0, (d_cpu / wall) / (_cpu_cnt or 1) * 100.0)
            ram_peak_mb = peak_rss / (1024 * 1024)

            # right-censor theo ngân sách
            if wall >= BUDGET_SEC or status == 'timeout':
                timeout_flag = 1
            elapsed_capped = min(wall, BUDGET_SEC)

            finished_at_iso = iso_now()

            # Ghi CSV
            domain = urlparse(url).netloc or ""
            row = [
                METHOD_NAME,
                run_id,
                started_at_iso,
                finished_at_iso,
                domain,
                i,
                url,
                http_status,
                status,
                f"{elapsed_capped:.2f}",
                timeout_flag,
                f"{BUDGET_SEC:.0f}",
                retries,
                size_bytes,
                "",
                f"{ram_peak_mb:.2f}",
                f"{cpu_pct:.2f}",
            ]
            append_csv_row(row)

            # In tiến độ
            if status == 'ok':
                print(f"[{i}/{n}] OK   -> {out_file}")
            else:
                msg = f"[{i}/{n}] {status} -> {url}\n"
                print(msg.strip())
                error_log.write(msg)

            # Delay lịch sự
            time.sleep(max(0.0, THROTTLE + random.uniform(-0.2, 0.2)))

    finally:
        error_log.close()
        try: driver.quit()
        except Exception: pass

    # Tổng kết nhanh (tùy chọn)
    print("✅ Done. HTML saved to:", OUT_DIR)
    print("🧾 CSV appended:", CSV_PATH)
    return 0 if success > 0 else 2

if __name__ == '__main__':
    raise SystemExit(main())
