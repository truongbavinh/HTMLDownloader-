# HTMLDownloader — Reproducible Capsule (Data & Scripts)

This capsule contains **labeled data** and **evaluation scripts** used to compute the precision results reported in the paper and to optionally re-run the URL collection pipeline.

> **Scope.** The capsule is intended for **small, reproducible samples** (≈100 links per domain × method), not for bulk crawling.
> Please respect websites’ **robots.txt**, **Terms of Service**, and applicable laws.

---

## Contents

```
capsule/
├─ data/
│  ├─ annotations_filled.csv        # ground-truth labels (0/1) used for metrics
│  ├─ sites.json                    # evaluation domains (page_url, selector, etc.)
│  ├─ precision_summary.csv         # (generated) per-domain × method precision
│  ├─ macro_summary.md              # (generated) available/fixed/matched macro
│  ├─ coverage_matrix.csv           # (generated) #rows per (domain, method)
│  └─ annotations_skeleton.csv      # (generated) merged sample to label
├─ scripts/
│  ├─ compute_metrics_plus.py       # compute metrics + macro (available/fixed/matched)
│  ├─ merge_to_annotation_verbose.py# merge + coverage diagnostics
│  ├─ check_outputs_coverage.py     # verify outputs/*/<domain>.txt presence
│  ├─ all_anchors_collect.py        # baseline collector (all <a>)
│  ├─ selenium_collect.py           # Selenium collector (uses CSS selector)
│  ├─ static_html_collect.py        # static HTML collector (with retries/timeout)
│  └─ label_assist.py               # optional: auto-label heuristics
├─ env/
│  └─ requirements.txt              # Python dependencies
└─ README_CAPSULE.md                # this file
```

> **Minimal set to recompute the reported numbers:** `data/annotations_filled.csv`, `data/sites.json`, and `scripts/compute_metrics_plus.py`.

---

## Environment

- **Python:** 3.9–3.11  
- Install dependencies:
  ```bash
  pip install -r env/requirements.txt
  ```
- **Selenium:** requires Chrome or Edge installed. Selenium 4 manages drivers automatically in most setups.

---

## Reproduce key metrics from labels (no crawling)

1) Ensure these files exist:
   - `data/annotations_filled.csv` (ground truth labels, 0/1)
   - `data/sites.json` (the predefined domain set)

2) Compute precision (per-domain) and macro-averages (available / fixed / matched):
   ```bash
   python scripts/compute_metrics_plus.py data/annotations_filled.csv --sites data/sites.json --min_n 50 -o data/precision_summary.csv
   ```

**Outputs produced**
- `data/precision_summary.csv` — precision per (domain, method)  
- `data/macro_summary.md` — macro-averages (available / fixed / matched)  
- `data/coverage_matrix.csv` — coverage (#rows per cell)

**Macro definitions**
- **Available macro:** mean over domains where a method has ≥ `min_n` rows.  
- **Fixed macro (used in paper):** arithmetic mean over the **predefined domain set** (missing cells treated as precision = 0).  
- **Matched macro:** mean over the **intersection** of domains where **all methods** have ≥ `min_n` rows.

---

## (Optional) Re-run the end-to-end URL collection

**Layout for outputs (one URL per line):**
```
outputs/
  aca_apa/<domain>.txt
  all_anchors/<domain>.txt
  selenium/<domain>.txt
  static_html/<domain>.txt
```

1) **Collect URL lists** into `outputs/<method>/<domain>.txt`:

```bash
# All anchors (baseline)
python scripts/all_anchors_collect.py --sites data/sites.json --outdir outputs/all_anchors

# Selenium (hand-crafted selector from sites.json)
python scripts/selenium_collect.py --sites data/sites.json --outdir outputs/selenium

# Static HTML (no rendering; with retries & per-domain timeout)
python scripts/static_html_collect.py --sites data/sites.json --outdir outputs/static_html --domain-timeout 300
```

For **ACA+APA** (our method), export URLs from the HTMLDownloader app to:  
`outputs/aca_apa/<domain>.txt`

2) **Check coverage:**
```bash
python scripts/check_outputs_coverage.py
```

3) **Merge & sample** (e.g., 100 links per cell):
```bash
python scripts/merge_to_annotation_verbose.py --sites data/sites.json --inputs_root outputs --out_csv data/annotations_skeleton.csv --sample_n 100
```

4) **Label** (or reuse our labels):
```bash
# Option A: use our ground-truth directly
#   data/annotations_filled.csv

# Option B: quick heuristics then manual review
python scripts/label_assist.py data/annotations_skeleton.csv
# -> data/annotations_auto.csv and data/annotations_filled.csv
```

5) **Compute metrics** (as above):
```bash
python scripts/compute_metrics_plus.py data/annotations_filled.csv --sites data/sites.json --min_n 50 -o data/precision_summary.csv
```

**Notes**
- Static HTML often returns 0 links on **JS-rendered** catalogs (expected). This is part of the baseline comparison.  
- Some sites employ **anti-bot** measures; consider smaller samples, longer delays, or use the app’s **Crawl** mode when appropriate.

---

## Quick reference — headline results (20 domains, fixed macro)

| Method                  | Precision (%) | #Domains |
|-------------------------|--------------:|---------:|
| ACA+APA                 | 92.70         | 20       |
| Collect-all-anchors     | 31.40         | 20       |
| Selenium (hand-crafted) | 87.60         | 20       |
| Static HTML             | 29.10         | 20       |

*Note.* Fixed macro = mean over the predefined domain set; missing cells counted as 0.

---

## License

- **Scripts:** MIT License  
- **Labels/data:** CC BY 4.0 (or MIT) — choose one and state it in `LICENSE` inside this folder.

**Contact.** For questions, please see the repository README or contact the authors.
