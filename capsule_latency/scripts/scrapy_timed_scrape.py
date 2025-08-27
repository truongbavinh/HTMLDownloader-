#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Scrapy_timed_scrape.py

Headless Chrome via Selenium.
Measures per-URL latency (navigate -> body present -> save page_source),
right-censors at --budget seconds, and appends rows to latency CSV.

Usage:
  python selenium_timed_scrape.py \
    --input fashion-hm.txt \
    --outdir outputs/selenium/html_files \
    --errdir outputs/selenium/errors \
    --csv logs/latency_logs.csv \
    --budget 300 \
    --wait-timeout 15 \
    --delay-min 20 --delay-max 30 \
    --user-agent "Mozilla/5.0 ..."

CSV columns (created if missing):
  method,run_id,started_at_iso,finished_at_iso,domain,url_id,url,http_status,
  status,elapsed_sec,timeout,timeout_sec,retries,size_bytes,notes

Notes:
- method is "selenium"
- http_status not available -> 0
- On errors we also dump page_source + screenshot into --errdir
"""

import argparse
import csv
import os
import random
import sys
import time
from datetime import datetime, timezone
from urllib.parse import urlparse

from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.common.exceptions import TimeoutException, WebDriverException

def iso_now():
    return datetime.now(timezone.utc).isoformat()

def ensure_dir(p):
    d = os.path.dirname(p)
    if d and not os.path.exists(d):
        os.makedirs(d, exist_ok=True)

def append_csv(csv_path, row, header):
    ensure_dir(csv_path)
    new_file = not os.path.exists(csv_path)
    with open(csv_path, "a", encoding="utf-8", newline="") as f:
        w = csv.writer(f)
        if new_file:
            w.writerow(header)
        w.writerow(row)

def build_driver(user_agent: str):
    options = Options()
    options.add_argument("--headless=new")  # Chrome 109+
    options.add_argument("--disable-gpu")
    options.add_argument("--no-sandbox")
    options.add_argument("--window-size=1920,1080")
    options.add_argument("--lang=en-US")
    if user_agent:
        options.add_argument(f'--user-agent={user_agent}')
    # You can add pageLoadStrategy if needed:
    # options.set_capability("pageLoadStrategy", "normal")
    driver = webdriver.Chrome(options=options)
    return driver

def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--input", "-i", default="Linktest.txt", help="URL list file (one per line)")
    ap.add_argument("--outdir", "-o", default="outputs/Scrapy/html_files", help="Where to save HTML files")
    ap.add_argument("--errdir", default="outputs/Scrapy/errors", help="Where to save error HTML/screenshots")
    ap.add_argument("--csv", "-c", default="logs/Scrapy_latency_logs.csv", help="CSV path to append logs")
    ap.add_argument("--budget", type=float, default=300.0, help="Right-censor time budget in seconds (default 300)")
    ap.add_argument("--wait-timeout", type=float, default=15.0, help="WebDriverWait timeout (s) for body presence")
    ap.add_argument("--delay-min", type=float, default=20.0, help="Min delay between requests (s)")
    ap.add_argument("--delay-max", type=float, default=30.0, help="Max delay between requests (s)")
    ap.add_argument("--user-agent", default="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/137.0.0.0 Safari/537.36")
    args = ap.parse_args()

    # Prepare
    ensure_dir(args.csv)
    os.makedirs(args.outdir, exist_ok=True)
    os.makedirs(args.errdir, exist_ok=True)

    run_id = datetime.utcnow().strftime("%Y-%m-%dT%H-%M-%SZ")

    if not os.path.exists(args.input):
        print(f"[ERROR] Input not found: {args.input}")
        sys.exit(1)

    with open(args.input, "r", encoding="utf-8") as f:
        urls = [line.strip() for line in f if line.strip()]

    header = [
        "method","run_id","started_at_iso","finished_at_iso","domain","url_id","url",
        "http_status","status","elapsed_sec","timeout","timeout_sec","retries","size_bytes","notes"
    ]

    # Build driver
    try:
        driver = build_driver(args.user_agent)
    except WebDriverException as e:
        print(f"[FATAL] Cannot start Chrome driver: {e}")
        sys.exit(2)

    try:
        for idx, url in enumerate(urls, start=1):
            started_at = iso_now()
            t0 = time.monotonic()
            http_status = 0  # not available
            status = "error"
            timeout_flag = 0
            size_bytes = 0
            retries = 0
            domain = urlparse(url).netloc or ""

            try:
                driver.get(url)
                WebDriverWait(driver, args.wait-timeout).until(
                    EC.presence_of_element_located((By.TAG_NAME, "body"))
                )

                html = driver.page_source

                # simple error heuristics similar to your code
                if ("chrome-error" in html or "ERR_" in html or
                    "This site can’t be reached" in html or
                    "This site can't be reached" in html):
                    raise Exception("Chrome error page detected")

                # save html
                fname = f"{idx:04d}.html"
                fpath = os.path.join(args.outdir, fname)
                with open(fpath, "w", encoding="utf-8") as fp:
                    fp.write(html)
                size_bytes = os.path.getsize(fpath)

                status = "ok"

            except TimeoutException:
                status = "timeout"
                timeout_flag = 1
                # dump error artifacts
                err_html = os.path.join(args.errdir, f"error_page_{idx:04d}.html")
                with open(err_html, "w", encoding="utf-8") as fp:
                    fp.write(driver.page_source or "")
                driver.save_screenshot(os.path.join(args.errdir, f"error_page_{idx:04d}.png"))
            except Exception as ex:
                status = f"error:{type(ex).__name__}"
                # dump error artifacts
                err_html = os.path.join(args.errdir, f"error_page_{idx:04d}.html")
                with open(err_html, "w", encoding="utf-8") as fp:
                    fp.write(driver.page_source or "")
                driver.save_screenshot(os.path.join(args.errdir, f"error_page_{idx:04d}.png"))

            # timing & right-censor
            elapsed = time.monotonic() - t0
            if elapsed >= args.budget or status == "timeout":
                timeout_flag = 1
            elapsed_capped = min(elapsed, args.budget)

            finished_at = iso_now()

            row = [
                "scrapy",
                run_id,
                started_at,
                finished_at,
                domain,
                idx,
                url,
                http_status,
                status,
                f"{elapsed_capped:.2f}",
                timeout_flag,
                f"{args.budget:.0f}",
                retries,
                size_bytes,
                ""
            ]
            append_csv(args.csv, row, header)

            # polite delay
            time.sleep(random.uniform(args.delay_min, args.delay_max))

    finally:
        try:
            driver.quit()
        except Exception:
            pass

    print(f"Done. Saved HTML to: {args.outdir}")
    print(f"Appended logs to: {args.csv}")
    print(f"Errors (if any) at: {args.errdir}")

if __name__ == "__main__":
    main()
