# wget_timed_scrape_fixed.ps1
<#
Usage:
  powershell -ExecutionPolicy Bypass -File .\wget_timed_scrape_fixed.ps1 `
    -InputFile .\Linktest.txt `
    -OutDir outputs\wget `
    -Csv logs\latency_logs.csv `
    -BudgetSec 300 `
    -DelayMin 2 -DelayMax 5

Notes:
- Avoids conflict with PowerShell automatic variable $input by using -InputFile.
- Measures per-URL elapsed time and appends to CSV in standard schema.
#>

param(
  [string]$InputFile = "Linktest.txt",
  [string]$OutDir = "outputs\wget1",
  [string]$Csv = "logs\wget_latency_logs.csv",
  [double]$BudgetSec = 20.0,
  [double]$DelayMin = 2.0,
  [double]$DelayMax = 5.0,
  [string]$UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/137.0.0.0 Safari/537.36"
)

$ErrorActionPreference = "Stop"

function Ensure-Dir([string]$p) {
  $d = Split-Path -Parent $p
  if ($d -and -not (Test-Path $d)) { New-Item -ItemType Directory -Path $d | Out-Null }
}

function Append-Csv([string]$csvPath, [object[]]$row) {
  Ensure-Dir $csvPath
  $newFile = -not (Test-Path $csvPath)
  $header = "method,run_id,started_at_iso,finished_at_iso,domain,url_id,url,http_status,status,elapsed_sec,timeout,timeout_sec,retries,size_bytes,notes"
  if ($newFile) { $header | Out-File -Encoding UTF8 -FilePath $csvPath }
  ($row -join ",") | Out-File -Append -Encoding UTF8 -FilePath $csvPath
}

function CsvEscape([string]$s) {
  if ($null -eq $s) { return '""' }
  return '"' + ($s -replace '"','""') + '"'
}

function Resolve-Downloader {
  $wget = Get-Command "wget.exe" -ErrorAction SilentlyContinue
  if ($wget) { return @{ Path=$wget.Path; Kind="wget" } }
  $curl = Get-Command "curl.exe" -ErrorAction SilentlyContinue
  if ($curl) { return @{ Path=$curl.Path; Kind="curl" } }
  throw "Neither wget.exe nor curl.exe found in PATH."
}

# Validate input file
if ([string]::IsNullOrWhiteSpace($InputFile)) {
  throw "InputFile is empty. Pass -InputFile .\Linktest.txt (for example)."
}
if (-not (Test-Path -LiteralPath $InputFile)) {
  throw "Input not found: $InputFile (use a full or relative path)"
}

# Load URLs
$urls = Get-Content -LiteralPath $InputFile -Encoding UTF8 | Where-Object { $_.Trim() -and -not $_.Trim().StartsWith("#") }

# Prepare
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
Ensure-Dir $Csv
$runId = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH-mm-ssZ")
$down = Resolve-Downloader

Write-Host "Using $($down.Kind): $($down.Path)"
Write-Host "Saving HTML -> $OutDir"
Write-Host "Appending CSV -> $Csv"

$idx = 0
foreach ($url in $urls) {
  $idx++
  $domain = ""
  try { $domain = ([Uri]$url).Host } catch { $domain = "" }

  $fname = "{0:D4}.html" -f $idx
  $outPath = Join-Path $OutDir $fname

  $startedIso = (Get-Date).ToUniversalTime().ToString("o")
  $sw = [System.Diagnostics.Stopwatch]::StartNew()
  $timeoutFlag = 0
  $status = "error"
  $httpStatus = 0
  $retries = 0
  $sizeBytes = 0

  # Build args
  if ($down.Kind -eq "wget") {
    $args = @("--user-agent=""$UserAgent""", "-O", "`"$outPath`"", "`"$url`"")
  } else {
    $args = @("-L", "-A", "$UserAgent", "-o", "$outPath", "$url")
  }

  # Start process
  $psi = New-Object System.Diagnostics.ProcessStartInfo
  $psi.FileName = $down.Path
  $psi.Arguments = ($args -join " ")
  $psi.UseShellExecute = $false
  $psi.RedirectStandardOutput = $true
  $psi.RedirectStandardError = $true
  $proc = New-Object System.Diagnostics.Process
  $proc.StartInfo = $psi
  [void]$proc.Start()

  # Hard timeout
  $budgetMs = [int]([Math]::Round($BudgetSec * 1000.0))
  if (-not $proc.WaitForExit($budgetMs)) {
    try { $proc.Kill() } catch {}
    $timeoutFlag = 1
    $status = "timeout"
  } else {
    $exit = $proc.ExitCode
    if ($exit -eq 0 -and (Test-Path $outPath)) {
      $status = "ok"
      try { $sizeBytes = (Get-Item $outPath).Length } catch {}
    } else {
      $status = "error:exitcode_$exit"
    }
  }

  $sw.Stop()
  $elapsed = [Math]::Min($sw.Elapsed.TotalSeconds, $BudgetSec)
  $finishedIso = (Get-Date).ToUniversalTime().ToString("o")

  $row = @(
    "wget",
    $runId,
    $startedIso,
    $finishedIso,
    $domain,
    $idx,
    (CsvEscape $url),
    $httpStatus,
    (CsvEscape $status),
    ("{0:N2}" -f $elapsed).Replace(",", "."),
    $timeoutFlag,
    ("{0}" -f [int]$BudgetSec),
    $retries,
    $sizeBytes,
    ""
  )
  Append-Csv $Csv $row

  # polite delay
  $delayMsMin = [int]([Math]::Round($DelayMin * 1000.0))
  $delayMsMax = [int]([Math]::Round($DelayMax * 1000.0))
  if ($delayMsMax -lt $delayMsMin) { $tmp = $delayMsMin; $delayMsMin = $delayMsMax; $delayMsMax = $tmp }
  $sleepMs = if ($delayMsMax -gt $delayMsMin) { Get-Random -Minimum $delayMsMin -Maximum $delayMsMax } else { $delayMsMin }
  Start-Sleep -Milliseconds $sleepMs

  Write-Host ("[{0}/{1}] {2} -> {3}" -f $idx, $urls.Count, $status, $outPath)
}

Write-Host "Done."
Write-Host "HTML: $OutDir"
Write-Host "CSV : $Csv"
