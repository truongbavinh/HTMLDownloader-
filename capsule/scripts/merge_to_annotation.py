
import argparse, csv
from pathlib import Path

def load_links(path):
    p = Path(path)
    if not p.exists():
        return []
    lines = [line.strip() for line in p.read_text(encoding="utf-8").splitlines() if line.strip() and not line.startswith("#")]
    return list(dict.fromkeys(lines))  # de-dupe, keep order

def main():
    ap = argparse.ArgumentParser(description="Merge per-domain link lists into an annotation CSV skeleton.")
    ap.add_argument("--sites", default="sites.json")
    ap.add_argument("--inputs_root", default="outputs", help="Root dir containing subdirs: aca_apa, all_anchors, selenium, static_html")
    ap.add_argument("--out_csv", default="annotations_skeleton.csv")
    ap.add_argument("--sample_n", type=int, default=100, help="Optionally cap rows per (domain, method)")
    args = ap.parse_args()

    methods = ["aca_apa", "all_anchors", "selenium", "static_html"]
    rows = []
    for s in __import__("json").loads(Path(args.sites).read_text(encoding="utf-8")):
        domain = s["domain"]
        page_url = s["page_url"]
        for m in methods:
            f = Path(args.inputs_root) / m / f"{domain}.txt"
            links = load_links(f)
            if args.sample_n and len(links) > args.sample_n:
                links = links[:args.sample_n]
            for link in links:
                rows.append([domain, page_url, m, link, ""])  # is_product left blank for human labeling

    with open(args.out_csv, "w", encoding="utf-8", newline="") as f:
        w = csv.writer(f)
        w.writerow(["domain","page_url","method","link_url","is_product"])
        w.writerows(rows)
    print(f"Wrote {args.out_csv} with {len(rows)} rows. Fill is_product with 1/0 then run metrics.")
if __name__ == "__main__":
    main()
