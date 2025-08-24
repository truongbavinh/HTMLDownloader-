# HTMLDownloader — Reproducible Capsule (Data & Scripts)

**DOI:** 10.5281/zenodo.16935169  
**Version:** v1.0.0

This capsule contains **labeled data** and **scripts** to recompute the precision results reported in the paper and to optionally re-run the URL collection pipeline.

---

## Contents

```
capsule/
├─ data/
│  ├─ annotations_filled.csv        # ground-truth labels (0/1) used for metrics
│  ├─ sites.json                    # 10 evaluation domains (page_url, selector, etc.)
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
│  ├─ static_html_collect.py        # static HTML collector
│  
├─ env/
│  └─ requirements.txt              # Python dependencies
├─ README_CAPSULE.md                # this file
└─ LICENSE                          # MIT (scripts) + license for labels (e.g., CC BY 4.0)
```

> **Note.** You only need `data/annotations_filled.csv`, `data/sites.json`, and `scripts/compute_metrics_plus.py` to recompute the headline numbers. The collectors are provided so reviewers can re-run the pipeline if desired.

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
   - `data/sites.json` (the 10 domains used in the paper)

2) Compute precision (per-domain) and macro-averages (available/fixed/matched):
   ```bash
   python scripts/compute_metrics_plus.py data/annotations_filled.csv --sites data/sites.json --min_n 50 -o data/precision_summary.csv
   ```

Outputs produced:
- `data/precision_summary.csv` — precision per (domain, method)  
- `data/macro_summary.md` — macro-averages (available / fixed / matched)  
- `data/coverage_matrix.csv` — coverage (#rows per cell)

**Definitions.**
- **Fixed macro**: arithmetic mean over the **predefined set of domains** (missing cells treated as precision = 0).  
- **Matched macro**: mean over the **intersection of domains** where **all methods** have at least `min_n` rows (default 50).

---

## (Optional) Re-run the end-to-end URL collection

1) **Collect URL lists** into `outputs/<method>/<domain>.txt` (one URL per line):

   ```bash
   # All anchors (baseline)
   python scripts/all_anchors_collect.py --sites data/sites.json --outdir outputs/all_anchors

   # Selenium (hand-crafted selector from sites.json)
   python scripts/selenium_collect.py --sites data/sites.json --outdir outputs/selenium

   # Static HTML (no rendering)
   python scripts/static_html_collect.py --sites data/sites.json --outdir outputs/static_html
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

---

## Ethics & usage notes

- This capsule is intended for **reproducible benchmarking** on **small samples** (≈100 links per domain × method).  
- Respect websites’ **robots.txt**, **Terms of Service**, and applicable laws.  
- Site layouts and anti-bot measures evolve; results reflect the collection window stated in the manuscript.

---

## License

- **Scripts:** MIT License  
- **Labels/data:** MIT or CC BY 4.0 (choose one and state it explicitly in `LICENSE`).

If you have questions, please contact the authors (see manuscript or repository README).
