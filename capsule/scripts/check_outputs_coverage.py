
import json
from pathlib import Path

METHODS = ["aca_apa", "all_anchors", "selenium", "static_html"]

def load_sites(path):
    sites = json.loads(Path(path).read_text(encoding="utf-8"))
    for s in sites:
        s["domain"] = s["domain"].strip().lower()
        s["page_url"] = s["page_url"].strip()
    return sites

def count_lines(p: Path) -> int:
    if not p.exists(): return -1
    return sum(1 for ln in p.read_text(encoding="utf-8").splitlines() if ln.strip() and not ln.startswith("#"))

def main():
    sites = load_sites("sites.json")
    inputs_root = Path("outputs")
    print("Checking outputs/*/{domain}.txt existence and line counts...\n")
    ok = True
    for s in sites:
        d = s["domain"]
        for m in METHODS:
            p = inputs_root / m / f"{d}.txt"
            n = count_lines(p)
            if n == -1:
                ok = False
                print(f"[MISS] {m:12s} {d:12s} -> {p} (NOT FOUND)")
            else:
                flag = "OK  " if n>0 else "ZERO"
                print(f"[{flag}] {m:12s} {d:12s} -> {p} (lines={n})")
    if not ok:
        print("\nSome files are missing. Generate them, then rerun merge.")
    else:
        print("\nAll per-domain files are present (some may still be ZERO; fill data).")

if __name__ == "__main__":
    main()
