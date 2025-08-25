import json
import sys
import time
from pathlib import Path
from urllib.parse import urljoin, urlparse

import requests
from requests.adapters import HTTPAdapter
from urllib3.util.retry import Retry
from bs4 import BeautifulSoup

def make_session():
    headers = {
        "User-Agent": (
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) "
            "AppleWebKit/537.36 (KHTML, like Gecko) "
            "Chrome/124.0.0.0 Safari/537.36"
        ),
        "Accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8",
        "Accept-Language": "en-US,en;q=0.9",
        "Accept-Encoding": "gzip, deflate, br",
        "Connection": "keep-alive",
        "Upgrade-Insecure-Requests": "1",
    }
    sess = requests.Session()
    retry = Retry(
        total=3, connect=3, read=3, backoff_factor=0.7,
        status_forcelist=[429, 500, 502, 503, 504],
        allowed_methods=["GET", "HEAD"]
    )
    adapter = HTTPAdapter(max_retries=retry, pool_maxsize=64, pool_connections=32)
    sess.mount("http://", adapter)
    sess.mount("https://", adapter)
    sess.headers.update(headers)
    return sess

def collect_static(session, url, css_selector, deadline_ts):
    """Fetch a single page and extract links with the given CSS selector.
    Enforces a per-domain deadline via 'deadline_ts' (monotonic seconds).
    Returns a list of absolute URLs (deduplicated).
    """
    # Compute remaining time budget for the request
    remain = max(0.0, deadline_ts - time.monotonic())
    if remain <= 0:
        raise TimeoutError("Per-domain time budget exhausted before request")

    # per-request timeout based on remaining budget, clamp to [5, 30] seconds
    per_req_timeout = min(30.0, max(5.0, remain))

    r = session.get(url, timeout=per_req_timeout)
    r.raise_for_status()

    soup = BeautifulSoup(r.text, "lxml")
    links = set()

    # Try CSS selector if given; else fallback to all anchors
    elems = soup.select(css_selector) if css_selector else soup.select("a[href]")
    for a in elems:
        href = a.get("href")
        if not href:
            continue
        abs_url = urljoin(r.url, href)
        links.add(abs_url)

    return sorted(links)

def main():
    import argparse
    ap = argparse.ArgumentParser(description="Static HTML collector with retries, CSS selector, and per-domain timeout")
    ap.add_argument("--sites", required=True, help="sites.json")
    ap.add_argument("--outdir", required=True, help="output directory (created if missing)")
    ap.add_argument("--sleep", type=float, default=0.5, help="sleep seconds between domains")
    ap.add_argument("--domain-timeout", type=float, default=200.0, help="max seconds to spend per domain before skipping")
    args = ap.parse_args()

    sites_path = Path(args.sites)
    outdir = Path(args.outdir); outdir.mkdir(parents=True, exist_ok=True)

    with sites_path.open("r", encoding="utf-8") as f:
        sites = json.load(f)

    session = make_session()

    for site in sites:
        domain = site.get("domain", "").strip().lower()
        url = site.get("page_url", "").strip()
        selector = site.get("selector", "").strip()

        if not domain or not url:
            print(f"[STATIC] skip invalid site entry: {site!r}")
            continue

        out_file = outdir / f"{domain}.txt"
        domain_start = time.monotonic()
        deadline_ts = domain_start + float(args.domain_timeout)

        try:
            print(f"[STATIC] domain={domain} url={url}")
            links = collect_static(session, url, selector, deadline_ts)
            with out_file.open("w", encoding="utf-8") as g:
                for u in links:
                    g.write(u + "\n")
            print(f"  -> saved {len(links)} links to {out_file}")
        except requests.exceptions.Timeout:
            print(f"[STATIC] {domain} TIMEOUT: per-request timeout reached")
            out_file.touch()
        except requests.exceptions.RequestException as e:
            print(f"[STATIC] {domain} ERROR: {e}")
            out_file.touch()
        except TimeoutError as e:
            print(f"[STATIC] {domain} SKIPPED: {e}")
            out_file.touch()
        except Exception as e:
            print(f"[STATIC] {domain} UNEXPECTED ERROR: {e}")
            out_file.touch()

        # Enforce the domain timeout before sleeping
        elapsed = time.monotonic() - domain_start
        if elapsed > args.domain_timeout:
            print(f"[STATIC] {domain} reached time budget ({elapsed:.1f}s > {args.domain_timeout}s), moving on.")
        time.sleep(args.sleep)

if __name__ == "__main__":
    main()
