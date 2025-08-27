#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
bs4_timed_scrape.py

Download raw HTML with requests + BeautifulSoup (for basic parsing if needed),
measure per-URL latency with a stopwatch, and append results to a CSV that
is compatible with your summarizer.

Usage:
  python bs4_timed_scrape.py \
      --input Linktest.txt \
      --outdir outputs/beautifulsoup \
      --csv logs/latency_logs.csv \
      --budget 300 \
      --delay-min 2 --delay-max 5

Columns appended to CSV (if file doesn't exist, header is created):
  method,run_id,started_at_iso,finished_at_iso,domain,url_id,url,http_status,
  status,elapsed_sec,timeout,timeout_sec,retries,size_bytes,notes

Notes:
- method is set to "beautifulsoup"
- elapsed_sec is right-censored at --budget seconds for consistency
- timeout=1 when a requests timeout occurs or elapsed >= budget
- HTML saved is the raw response.text (UTF-8), not the prettified soup
"""

import argparse
import csv
import os
import random
import sys
import time
from datetime import datetime, timezone
from urllib.parse import urlparse

import requests
from bs4 import BeautifulSoup  # not strictly required for saving raw HTML

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

def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--input", "-i", default="Linktest.txt", help="Path to URL list (one per line). Default: Linktest.txt")
    ap.add_argument("--outdir", "-o", default="outputs/beautifulsoup", help="Folder to save HTML files")
    ap.add_argument("--csv", "-c", default="logs/beautifulsoup_latency_logs.csv", help="CSV path to append logs")
    ap.add_argument("--budget", type=float, default=1000.0, help="Timeout right-censor budget in seconds (default 300)")
    ap.add_argument("--delay-min", type=float, default=2.0, help="Min delay between requests (s)")
    ap.add_argument("--delay-max", type=float, default=5.0, help="Max delay between requests (s)")
    ap.add_argument("--req-timeout", type=float, default=20.0, help="requests.get timeout in seconds (socket timeout)")
    args = ap.parse_args()

    # Prepare
    ensure_dir(args.csv)
    os.makedirs(args.outdir, exist_ok=True)
    run_id = datetime.utcnow().strftime("%Y-%m-%dT%H-%M-%SZ")

    # Load URLs
    if not os.path.exists(args.input):
        print(f"[ERROR] Input not found: {args.input}")
        sys.exit(1)
    with open(args.input, "r", encoding="utf-8") as f:
        urls = [line.strip() for line in f if line.strip()]

    header = [
        "method","run_id","started_at_iso","finished_at_iso","domain","url_id","url",
        "http_status","status","elapsed_sec","timeout","timeout_sec","retries","size_bytes","notes"
    ]

    ua = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36"
    session = requests.Session()
    session.headers.update({"User-Agent": ua})

    for idx, url in enumerate(urls, start=1):
        started_at = iso_now()
        t0 = time.monotonic()
        domain = urlparse(url).netloc or ""
        http_status = 0
        status = "error"
        timeout_flag = 0
        size_bytes = 0
        retries = 0

        try:
            resp = session.get(url, timeout=args.req_timeout)  # socket/connect timeout
            http_status = resp.status_code

            if resp.status_code == 200:
                # Optionally parse with BeautifulSoup (not needed to save raw HTML)
                # soup = BeautifulSoup(resp.text, "html.parser")
                html = resp.text

                # save raw HTML
                fname = f"{idx:04d}.html"
                fpath = os.path.join(args.outdir, fname)
                with open(fpath, "w", encoding="utf-8") as fp:
                    fp.write(html)
                size_bytes = os.path.getsize(fpath)

                status = "ok"
            else:
                status = f"http_{resp.status_code}"

        except requests.exceptions.Timeout:
            status = "timeout"
            timeout_flag = 1
        except Exception as ex:
            status = f"error:{type(ex).__name__}"

        # timing & right-censor
        elapsed = time.monotonic() - t0
        if elapsed >= args.budget or status == "timeout":
            timeout_flag = 1
        elapsed_capped = min(elapsed, args.budget)

        finished_at = iso_now()

        row = [
            "beautifulsoup",
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

    print(f"Done. Saved HTML to: {args.outdir}")
    print(f"Appended logs to: {args.csv}")

if __name__ == "__main__":
    main()
