
import argparse, urllib.parse, requests, bs4
from pathlib import Path

def collect_static(url):
    html = requests.get(url, headers={"User-Agent": "Mozilla/5.0"}).text
    soup = bs4.BeautifulSoup(html, "html.parser")
    base = "{uri.scheme}://{uri.netloc}".format(uri=urllib.parse.urlparse(url))
    links = set()
    for a in soup.select("a[href]"):
        href = urllib.parse.urljoin(base, a["href"])
        links.add(href)
    return sorted(links)

def main():
    import json
    ap = argparse.ArgumentParser(description="Collect anchors from static HTML (no JS).")
    ap.add_argument("--sites", default="sites.json", help="JSON with domain,page_url")
    ap.add_argument("--outdir", default="outputs/static_html", help="Output directory for per-domain link lists")
    ap.add_argument("--limit", type=int, default=0, help="Optional limit of links to save")
    args = ap.parse_args()

    Path(args.outdir).mkdir(parents=True, exist_ok=True)
    sites = json.loads(Path(args.sites).read_text(encoding="utf-8"))
    for s in sites:
        domain = s["domain"]
        url = s["page_url"]
        print(f"[STATIC] domain={domain} url={url}")
        links = collect_static(url)
        if args.limit and len(links) > args.limit:
            links = links[:args.limit]
        out = Path(args.outdir) / f"{domain}.txt"
        out.write_text("\n".join(links), encoding="utf-8")
        print(f"  -> saved {len(links)} links to {out}")

if __name__ == "__main__":
    main()
