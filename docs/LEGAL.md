# Responsible Use (Legal & Ethical Considerations)

> **Purpose.** This page provides practical guidance to help you use **HTMLDownloader** lawfully and responsibly. It is **not legal advice**. You are responsible for complying with the laws and site policies applicable to your use.

---

## 1) Scope & Intended Use
- **Research / reproducibility / archiving** of publicly accessible pages.
- Saves a **rendered snapshot (MHTML)** to local disk for inspection, reproducibility, and audit.
- **No features** for bypassing paywalls, CAPTCHAs, login flows, or other technical protections.

> If in doubt, seek permission from the website owner or your institution’s legal/ethics office.

---

## 2) Compliance Areas to Review
- **Terms of Service (ToS) & robots.txt.** Verify the site permits automated access and saving, and follow crawl-delay / disallow rules where applicable.
- **Copyright & Database Rights.** Respect content owners’ rights. Snapshots are for research and reproducibility. Do not redistribute content unless permitted.
- **Anti-circumvention / Technical Protection Measures.** Do **not** attempt to defeat access controls, CAPTCHAs, paywalls, or rate limits.
- **Privacy & Data Protection.** Avoid collecting **personal/sensitive data** (PII) unless you have a lawful basis and safeguards. Minimize and anonymize where possible.
- **Security & Load.** Use modest **concurrency** and **delays**; avoid stressing servers; identify yourself when appropriate.

---

## 3) How the Tool Supports Responsible Use
- **Rendered snapshot** only (MHTML to local disk); no bulk scraper by default.
- **Rate limiting and delays** are configurable; defaults are conservative.
- **“Dry run”**: discover links before downloading.
- **No** built-in circumvention (no CAPTCHA bypass, no paywall bypass, no forced login automation).
- **Attribution**: file naming can include source URL and timestamp for traceability.

See also: `docs/RUNTIME.md` (runtimes), `docs/SECURITY.md` (distribution, code signing), and `docs/USER_GUIDE.md` (usage).

---

## 4) Responsible Use Checklist (before you run)
1. **Read the site’s ToS** and inspect **robots.txt**; confirm your use is permitted.  
2. **Pick a small sample** first; verify results manually.  
3. **Set polite defaults**: Parallelism = **1**, Delay = **5–10 s** (or higher if site suggests).  
4. **Avoid login-gated pages** or anything protected by CAPTCHAs/paywalls.  
5. **Exclude personal/sensitive data**; if unavoidable, minimize scope and document your legal basis.  
6. **Record provenance**: keep source URLs and timestamps.  
7. **Store locally**; do not republish content unless allowed.  
8. **Stop if the site objects** (HTTP 429, robots changes, or explicit request).  
9. **Document your settings** for reproducibility (config file, version, date).

---

## 5) Example “pre-run notice” (optional UI text)
> **Responsible use.** Please verify the website’s Terms of Service and robots.txt before downloading. Use modest concurrency and delays. Do not collect personal/sensitive data or attempt to bypass access controls. Proceed only if you have a lawful basis.

---

## 6) Data Retention & Sharing
- Keep snapshots **only as long as needed** for your research or review obligations.
- If you must share data, prefer **URLs + metadata** over full page copies, unless permitted by license/policy.
- Remove content upon rights holder request where appropriate.

---

## 7) Disclaimer
This document is a **good-faith guide** and **not legal advice**. Laws and site policies vary by jurisdiction and context. Consult qualified counsel when necessary.

**Contact:** see the repository README or open an issue for questions.
