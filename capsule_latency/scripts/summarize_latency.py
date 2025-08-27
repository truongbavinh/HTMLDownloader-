#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
summarize_latency_fixed.py

Robust to BOM/whitespace/header-casing issues and occasional blank 'method' cells.
Normalizes CSV header keys to lowercase and strips BOM (\ufeff).

Usage:
  python summarize_latency_fixed.py --inputs logs/latency_logs*.csv --outdir logs --plot
"""
import argparse, glob, math, os, csv
from statistics import median
from typing import List, Dict, Any

try:
    import numpy as np
    HAVE_NUMPY = True
except Exception:
    HAVE_NUMPY = False

def quantile(values, q: float):
    if not values:
        return float('nan')
    if HAVE_NUMPY:
        import numpy as np
        return float(np.quantile(np.array(values, dtype=float), q))
    xs = sorted(values)
    if len(xs) == 1: return xs[0]
    pos = (len(xs) - 1) * q
    lo = int(math.floor(pos)); hi = int(math.ceil(pos))
    if lo == hi: return xs[lo]
    frac = pos - lo
    return xs[lo]*(1-frac) + xs[hi]*frac

def iqr(values):
    if not values: return float('nan')
    return quantile(values, 0.75) - quantile(values, 0.25)

def _norm_key(k: str) -> str:
    if k is None: return ""
    return k.replace('\\ufeff','').strip().lower()

def read_rows(paths: List[str]) -> List[Dict[str, Any]]:
    rows = []
    for p in paths:
        with open(p, 'r', encoding='utf-8', newline='') as f:
            r = csv.DictReader(f)
            for row in r:
                # normalize keys
                nrow = {}
                for k, v in row.items():
                    nk = _norm_key(k)
                    nrow[nk] = v
                # fix blank method if we can infer from notes/url
                m = (nrow.get('method') or '').strip()
                if not m:
                    note = (nrow.get('notes') or '').lower()
                    if 'wget' in note:
                        nrow['method'] = 'wget'
                    else:
                        nrow['method'] = ''  # leave blank; we'll map later
                rows.append(nrow)
    return rows

def safe_float(x, default=float('nan')) -> float:
    try: return float(x)
    except Exception: return default

def group_by(rows, keys):
    groups = {}
    for row in rows:
        k = tuple(row.get(k, "") for k in keys)
        groups.setdefault(k, []).append(row)
    return groups

def method_summary(rows: List[Dict[str, Any]]) -> Dict[str, Any]:
    total = len(rows)
    ok_rows = [r for r in rows if (str(r.get('status','')).strip().lower() == 'ok')]
    timeouts = sum(1 for r in rows if str(r.get('timeout','0')).strip().lower() in ('1','true'))
    el_all = [safe_float(r.get('elapsed_sec')) for r in rows if str(r.get('elapsed_sec','')).strip() not in ("", "nan")]
    el_ok  = [safe_float(r.get('elapsed_sec')) for r in ok_rows if str(r.get('elapsed_sec','')).strip() not in ("", "nan")]

    def stats(vals):
        if not vals:
            return dict(median=float('nan'), iqr=float('nan'), p95=float('nan'), mean=float('nan'))
        m = median(vals)
        iq = iqr(vals)
        p95 = quantile(vals, 0.95)
        mean = sum(vals)/len(vals)
        return dict(median=m, iqr=iq, p95=p95, mean=mean)

    s_all = stats(el_all)
    s_ok  = stats(el_ok)
    return dict(
        total=total,
        ok=len(ok_rows),
        success_rate=(len(ok_rows)/total if total>0 else float('nan')),
        timeout_rate=(timeouts/total if total>0 else float('nan')),
        median_all=s_all['median'], iqr_all=s_all['iqr'], p95_all=s_all['p95'], mean_all=s_all['mean'],
        median_ok=s_ok['median'],   iqr_ok=s_ok['iqr'],   p95_ok=s_ok['p95'],   mean_ok=s_ok['mean'],
    )

def write_csv(path: str, header, rows):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, 'w', encoding='utf-8', newline='') as f:
        w = csv.writer(f)
        w.writerow(header); w.writerows(rows)

def fmt(x, digits=2, na="NA"):
    import math
    if x is None or (isinstance(x, float) and (math.isnan(x) or math.isinf(x))):
        return na
    if isinstance(x, int):
        return str(x)
    return f"{x:.{digits}f}"

def main():
    import argparse, glob, os
    ap = argparse.ArgumentParser()
    ap.add_argument("--inputs", nargs="+", required=True, help="CSV files or globs (e.g., logs/latency_logs*.csv)")
    ap.add_argument("--outdir", default="logs", help="Output directory")
    ap.add_argument("--plot", action="store_true", help="Save a simple boxplot by method (requires matplotlib)")
    args = ap.parse_args()

    files = []
    for item in args.inputs:
        matches = glob.glob(item)
        files.extend(matches if matches else [item])
    files = list(dict.fromkeys(files))
    if not files:
        print("No input files found."); return 2

    rows = read_rows(files)
    if not rows:
        print("No rows found."); return 2

    # normalize method labels & fix blanks
    for r in rows:
        m = (r.get('method') or '').strip().lower()
        mapping = {
            'html_downloader':'htmldownloader','html-downloader':'htmldownloader','html downloader':'htmldownloader',
            'htmldownloader':'htmldownloader',
            'selenium':'selenium',
            'beautifulsoup':'beautifulsoup','bs4':'beautifulsoup',
            'wget':'wget',
            'scrapy':'scrapy',
        }
        if m in mapping:
            r['method'] = mapping[m]
        elif m:
            r['method'] = m
        else:
            note = (r.get('notes') or '').lower()
            if 'wget' in note: r['method'] = 'wget'
            else: r['method'] = 'wget'

    # Summaries by method
    by_method = group_by(rows, ["method"])
    overall_records = []
    for (method,), rs in sorted(by_method.items()):
        s = method_summary(rs)
        overall_records.append([
            method, s['total'], s['ok'],
            fmt(100*s['success_rate']), fmt(100*s['timeout_rate']),
            fmt(s['median_all']), fmt(s['iqr_all']), fmt(s['p95_all']),
            fmt(s['median_ok']),  fmt(s['iqr_ok']),  fmt(s['p95_ok']),
            fmt(s['mean_all']),   fmt(s['mean_ok'])
        ])

    header_overall = [
        "method","n_total","n_ok","success_rate_%","timeout_rate_%",
        "median_all_s","IQR_all_s","p95_all_s",
        "median_ok_s","IQR_ok_s","p95_ok_s",
        "mean_all_s","mean_ok_s"
    ]
    out_overall = os.path.join(args.outdir, "latency_summary_overall.csv")
    write_csv(out_overall, header_overall, overall_records)

    # Summaries by (method, domain)
    by_md = group_by(rows, ["method","domain"])
    md_records = []
    for (method, domain), rs in sorted(by_md.items()):
        s = method_summary(rs)
        md_records.append([
            method, domain, s['total'], s['ok'],
            fmt(100*s['success_rate']), fmt(100*s['timeout_rate']),
            fmt(s['median_all']), fmt(s['iqr_all']), fmt(s['p95_all']),
            fmt(s['median_ok']),  fmt(s['iqr_ok']),  fmt(s['p95_ok']),
            fmt(s['mean_all']),   fmt(s['mean_ok'])
        ])
    header_md = ["method","domain","n_total","n_ok","success_rate_%","timeout_rate_%",
                 "median_all_s","IQR_all_s","p95_all_s","median_ok_s","IQR_ok_s","p95_ok_s","mean_all_s","mean_ok_s"]
    out_md = os.path.join(args.outdir, "latency_summary_by_domain.csv")
    write_csv(out_md, header_md, md_records)

    # Markdown table
    md_path = os.path.join(args.outdir, "latency_overall_table.md")
    with open(md_path, "w", encoding="utf-8") as f:
        f.write("| Method | N | Success (%) | Timeout (%) | Median (all, s) | IQR (all, s) | p95 (all, s) |\n")
        f.write("|---|---:|---:|---:|---:|---:|---:|\n")
        for row in overall_records:
            method, n_total, n_ok, succ, tout, med, iq, p95, *_ = row
            f.write(f"| {method} | {n_total} | {succ} | {tout} | {med} | {iq} | {p95} |\n")

    # Optional plot
    if args.plot:
        try:
            import matplotlib.pyplot as plt
            data = {}
            for (method,), rs in by_method.items():
                vals = [safe_float(r.get('elapsed_sec')) for r in rs if str(r.get('elapsed_sec','')).strip() not in ("","nan")]
                data[method] = [v for v in vals if v is not None and not (isinstance(v, float) and (math.isnan(v) or math.isinf(v)))]
            labels = [k for k in sorted(data.keys()) if data[k]]
            series = [data[k] for k in labels]
            if series:
                plt.figure(figsize=(8,5))
                plt.boxplot(series, labels=labels, showfliers=False)
                plt.ylabel("Elapsed time (s)")
                plt.title("Latency by method")
                out_png = os.path.join(args.outdir, "latency_boxplot.png")
                plt.savefig(out_png, bbox_inches="tight", dpi=150)
        except Exception as e:
            print("Plotting skipped:", e)

    print("Wrote:", out_overall)
    print("Wrote:", out_md)
    print("Wrote:", md_path)
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
