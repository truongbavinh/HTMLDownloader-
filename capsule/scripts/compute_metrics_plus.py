
import csv
import json
import argparse
from collections import defaultdict

def load_expected_domains(sites_json):
    if not sites_json:
        return []
    try:
        data = json.load(open(sites_json, "r", encoding="utf-8"))
        doms = []
        for s in data:
            d = (s.get("domain","") or "").strip().lower()
            if d:
                doms.append(d)
        # preserve order, unique
        seen = set()
        out = []
        for d in doms:
            if d not in seen:
                seen.add(d)
                out.append(d)
        return out
    except Exception:
        return []

def load_rows(path):
    rows = []
    with open(path, "r", encoding="utf-8") as f:
        reader = csv.DictReader((line for line in f if not line.lstrip().startswith("#")))
        required = ["domain","page_url","method","link_url","is_product"]
        for col in required:
            if col not in reader.fieldnames:
                raise SystemExit(f"Missing required column: {col}")
        for row in reader:
            r = {k: (row.get(k, "") or "").strip() for k in reader.fieldnames}
            r["domain"] = r["domain"].lower()
            r["method"] = r["method"].lower()
            try:
                r["is_product"] = 1 if int(r["is_product"]) == 1 else 0
            except Exception:
                r["is_product"] = 0
            rows.append(r)
    return rows

def compute_cell_precision(rows):
    # rows per (domain, method)
    by_dm = defaultdict(list)
    for r in rows:
        by_dm[(r["domain"], r["method"])].append(r)

    # precision per (domain, method)
    per_dm = {}
    counts = {}
    for (dom, m), group in by_dm.items():
        tp = sum(1 for g in group if g["is_product"] == 1)
        fp = sum(1 for g in group if g["is_product"] == 0)
        n = tp + fp
        p = (tp / n) if n else 0.0
        per_dm[(dom, m)] = (p, tp, fp, n)
        counts[(dom, m)] = n
    return per_dm, counts

def macro_available(per_dm, methods, domains):
    # average over domains where (dom,method) exists
    out = {}
    for m in methods:
        plist = [per_dm[(d,m)][0] for d in domains if (d,m) in per_dm]
        out[m] = (sum(plist)/len(plist), len(plist)) if plist else (0.0, 0)
    return out

def macro_fixed(per_dm, methods, expected_domains):
    # average over expected_domains; missing cells contribute 0
    out = {}
    for m in methods:
        vals = []
        for d in expected_domains:
            if (d,m) in per_dm:
                vals.append(per_dm[(d,m)][0])
            else:
                vals.append(0.0)  # treat missing as 0 precision
        out[m] = (sum(vals)/len(vals), len(expected_domains)) if expected_domains else (0.0, 0)
    return out

def macro_matched(per_dm, methods, domains, counts, min_n):
    # average over domains where ALL methods have at least min_n rows
    eligible = []
    for d in domains:
        ok = True
        for m in methods:
            if counts.get((d,m), 0) < min_n:
                ok = False
                break
        if ok:
            eligible.append(d)
    out = {}
    for m in methods:
        plist = [per_dm[(d,m)][0] for d in eligible] if eligible else []
        out[m] = (sum(plist)/len(plist), len(eligible)) if plist else (0.0, 0)
    return out, eligible

def main():
    ap = argparse.ArgumentParser(description="Compute precision tables + macro-averages (available/fixed/matched).")
    ap.add_argument("csv", help="annotations CSV (filled with is_product 0/1)")
    ap.add_argument("-o", "--out", default="precision_summary.csv", help="output CSV filename (per (domain,method))")
    ap.add_argument("--sites", default="", help="sites.json to define expected domains for fixed-macro averaging")
    ap.add_argument("--min_n", type=int, default=50, help="min rows per (domain,method) for matched macro")
    args = ap.parse_args()

    rows = load_rows(args.csv)
    methods = sorted({r["method"] for r in rows})
    domains = sorted({r["domain"] for r in rows})
    expected = load_expected_domains(args.sites)
    if not expected:
        expected = domains[:]  # fallback: whatever present in CSV

    per_dm, counts = compute_cell_precision(rows)

    # Write per-(domain,method) table
    lines = []
    header = ["domain","method","n","tp","fp","precision"]
    lines.append(",".join(header))
    for d in sorted(set(expected) | set(domains)):
        for m in methods:
            p, tp, fp, n = per_dm.get((d,m), (0.0, 0, 0, 0))
            lines.append(f"{d},{m},{n},{tp},{fp},{p:.3f}")
    open(args.out, "w", encoding="utf-8").write("\n".join(lines) + "\n")

    # Macro-averages
    available = macro_available(per_dm, methods, expected)  # domains with data
    fixed = macro_fixed(per_dm, methods, expected)          # average over expected domains; missing=0
    matched, eligible = macro_matched(per_dm, methods, expected, counts, args.min_n)

    # Print console report
    print(f"Wrote per-(domain,method) table to {args.out}\n")
    print("== Macro-averages ==")
    print("Available (avg over domains where method has data):")
    for m in methods:
        v, k = available[m]
        print(f"  {m:12s}  {v:.3f}  (domains={k})")
    print("\nFixed (avg over expected domains; missing cells -> 0):")
    for m in methods:
        v, k = fixed[m]
        print(f"  {m:12s}  {v:.3f}  (domains={k})")
    print(f"\nMatched (min_n={args.min_n}; domains where ALL methods have >= min_n rows):")
    print("Eligible domains:", ", ".join(eligible) if eligible else "(none)")
    for m in methods:
        v, k = matched[m]
        print(f"  {m:12s}  {v:.3f}  (domains={k})")

    # Also write a small markdown summary
    md = []
    md.append("| Macro type | Method | Value | #domains |")
    md.append("|------------|--------|------:|---------:|")
    for name, obj in [("available", available), ("fixed", fixed), ("matched", matched)]:
        for m in methods:
            v, k = obj[m]
            md.append(f"| {name} | {m} | {v:.3f} | {k} |")
    open("macro_summary.md","w", encoding="utf-8").write("\n".join(md) + "\n")

    # Coverage diagnostics
    cov_lines = []
    cov_lines.append("domain,method,rows_present")
    exp_set = sorted(set(expected))
    for d in exp_set:
        for m in methods:
            cov_lines.append(f"{d},{m},{counts.get((d,m),0)}")
    open("coverage_matrix.csv","w", encoding="utf-8").write("\n".join(cov_lines) + "\n")

if __name__ == "__main__":
    main()
