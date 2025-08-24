
import argparse, json, time, sys
from pathlib import Path
from urllib.parse import urlparse
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.chrome.options import Options

def load_sites(path):
    import json
    with open(path, "r", encoding="utf-8") as f:
        return json.load(f)

def scroll_page(driver, n=20, delay=10.0):
    for _ in range(n):
        driver.execute_script("window.scrollTo(0, 400);")
        time.sleep(delay)

def collect_by_selector(url, selector, scrolls=20, delay=10.0):
    opts = Options()
    opts.add_argument("--start-maximized")
    driver = webdriver.Chrome(options=opts)
    try:
        driver.get(url)
        scroll_page(driver, scrolls, delay)
        els = driver.find_elements(By.CSS_SELECTOR, selector) if selector else []
        links = set()
        for el in els:
            try:
                href = el.get_attribute("href")
                if href:
                    links.add(href)
            except Exception:
                pass
        return sorted(links)
    finally:
        driver.quit()

def main():
    ap = argparse.ArgumentParser(description="Collect product links using Selenium with per-site CSS selectors.")
    ap.add_argument("--sites", default="sites.json", help="JSON with domain,page_url,selector,scrolls,delay_sec")
    ap.add_argument("--outdir", default="outputs/selenium", help="Output directory for per-domain link lists")
    ap.add_argument("--limit", type=int, default=0, help="Optional limit of links to save")
    args = ap.parse_args()

    Path(args.outdir).mkdir(parents=True, exist_ok=True)
    sites = load_sites(args.sites)
    for s in sites:
        domain = s["domain"]
        url = s["page_url"]
        selector = s.get("selector","").strip()
        if not selector:
            print(f"[WARN] No selector set for domain={domain}; skipping.", file=sys.stderr)
            continue
        scrolls = int(s.get("scrolls", 20))
        delay   = float(s.get("delay_sec", 10.0))
        print(f"[SELENIUM] domain={domain} url={url} selector={selector!r}")
        links = collect_by_selector(url, selector, scrolls, delay)
        if args.limit and len(links) > args.limit:
            links = links[:args.limit]
        out = Path(args.outdir) / f"{domain}.txt"
        out.write_text("\n".join(links), encoding="utf-8")
        print(f"  -> saved {len(links)} links to {out}")

if __name__ == "__main__":
    main()
