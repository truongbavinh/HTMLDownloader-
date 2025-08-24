
import argparse, time
from pathlib import Path
from selenium import webdriver
from selenium.webdriver.chrome.options import Options

def scroll_page(driver, n=6, delay=2.0):
    for _ in range(n):
        driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")
        time.sleep(delay)

def collect_all_anchors(url, scrolls=6, delay=2.0):
    opts = Options()
    opts.add_argument("--start-maximized")
    driver = webdriver.Chrome(options=opts)
    try:
        driver.get(url)
        scroll_page(driver, scrolls, delay)
        js = "return Array.from(new Set(Array.from(document.querySelectorAll('a[href]')).map(a => a.href)));"
        links = driver.execute_script(js)
        return sorted(links)
    finally:
        driver.quit()

def main():
    import json
    ap = argparse.ArgumentParser(description="Collect ALL anchors (rendered DOM baseline).")
    ap.add_argument("--sites", default="sites.json", help="JSON with domain,page_url,scrolls,delay_sec")
    ap.add_argument("--outdir", default="outputs/all_anchors", help="Output directory for per-domain link lists")
    ap.add_argument("--limit", type=int, default=0, help="Optional limit of links to save")
    args = ap.parse_args()

    Path(args.outdir).mkdir(parents=True, exist_ok=True)
    sites = json.loads(Path(args.sites).read_text(encoding="utf-8"))
    for s in sites:
        domain = s["domain"]
        url = s["page_url"]
        scrolls = int(s.get("scrolls", 6))
        delay   = float(s.get("delay_sec", 2.0))
        print(f"[ALL-ANCHORS] domain={domain} url={url}")
        links = collect_all_anchors(url, scrolls, delay)
        if args.limit and len(links) > args.limit:
            links = links[:args.limit]
        out = Path(args.outdir) / f"{domain}.txt"
        out.write_text("\n".join(links), encoding="utf-8")
        print(f"  -> saved {len(links)} links to {out}")

if __name__ == "__main__":
    main()
