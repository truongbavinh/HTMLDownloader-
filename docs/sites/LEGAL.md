# Legal & robots — Farfetch (Demo)

> **Disclaimer:** This is not legal advice. Always review Farfetch’s latest Terms and robots.txt before running any automated process.

## Robots.txt

Check Farfetch’s robots file here: https://www.farfetch.com/robots.txt

As of 2025-08-23, the file includes “Allow” rules for certain localized paths and a `Disallow` for an internal sitemap path (and a `Clean-param` directive). This may change at any time; always check the current file immediately before running your demo. :contentReference[oaicite:0]{index=0}

## Terms of Use (ToS)

Farfetch’s Terms include restrictions on automated extraction. In particular, their **Intellectual property, software and content** section states you **must not** “use any data mining, robots, or similar data gathering and extraction tools to extract (whether once or many times) for re-utilisation any substantial parts of the Website […] without our prior written consent.” :contentReference[oaicite:1]{index=1}

**Implication for this demo:** Even if `robots.txt` does not explicitly disallow a path, Farfetch’s ToS can still prohibit automated collection. Keep your demo **limited**, **non-commercial**, and **low-impact**, or obtain prior written permission.

## Demo-safe operating rules

- **Scope:** Use one category/collection page as a **seed** (e.g., women’s clothing), discover a **small subset** of product links (e.g., 10–20), and stop.
- **Depth:** `MaxDepth = 1` (only links on the seed page). Do **not** recurse through full pagination trees at scale.
- **Rate limits:** `ParallelDownloads ≤ 2`; inter-request delay **≥ 2 s**.
- **User-Agent:** Identify politely, e.g. `HtmlDownloader/1.0 (contact: youremail@example.com)`.
- **Content use:** Do **not** republish or redistribute Farfetch content; use outputs for **ephemeral demonstration** only.
- **Personal data:** Do not attempt to collect personal data or circumvent authentication, paywalls, or anti-bot protections.
- **Fallback for reproducibility:** If Farfetch changes or rate-limits, switch the demo to `books.toscrape.com` (a stable teaching site).

## Suggested config (example)

```json
{
  "UserAgent": "HtmlDownloader/1.0 (contact: youremail@example.com)",
  "NavigationTimeoutMs": 30000,
  "PostLoadDelayMs": 2000,
  "RetryCount": 1,
  "ParallelDownloads": 1,
  "MaxDepth": 1,
  "IncludeRegex": "https?://www\\.farfetch\\.com/vn/shopping/women/.+-item-.+",
  "OutputFormat": "mhtml"
}
