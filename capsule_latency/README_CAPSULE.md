# HTMLDownloader — Latency & Robustness Capsule

This capsule contains the raw latency logs and the summarization scripts
used to compute the distributional metrics reported in the paper.

## Environment
- Python 3.10+
- Optional: matplotlib (for boxplot)

Install:
pip install -r capsule_latency/requirements.txt

## Time budget & censoring
Per-URL time budget T = 20 s. Timeouts are **right-censored at T**.
We report Median (all), IQR (all), p95 (all), Success (%).

## Recompute tables
python capsule_latency/scripts/summarize_latency.py \
  --inputs capsule_latency/logs/latency_logs_*.csv \
  --outdir capsule_latency/outputs --plot

Outputs:
- capsule_latency/outputs/latency_summary_overall.csv
- capsule_latency/outputs/latency_summary_by_domain.csv
- capsule_latency/outputs/latency_overall_table.md
- capsule_latency/outputs/latency_boxplot.png (optional)
