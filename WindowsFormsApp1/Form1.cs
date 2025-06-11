using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using static System.Windows.Forms.LinkLabel;
using System.Diagnostics;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _httpClient; // Add HttpClient for HTTP requests
        private List<string> _skippedLinks; // To log skipped links
        private const int MinContentLength = 100;
        public Form1()
        {
            InitializeComponent();
            // Initialize HttpClient with a timeout
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10) // Set timeout to 10 seconds
            };
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");
            _skippedLinks = new List<string>(); // Initialize skipped links list
        }
        private int dem;
        private async void BTGetLinks_Click(object sender, EventArgs e)
        {
            lblinks.Items.Clear();
            if(!string.IsNullOrEmpty(txtdelay.Text))
                delay=int.Parse(txtdelay.Text);
            else
                delay=2;
            delay = delay * 1000;
            // Thiết lập URL cần tải
            string url = txtinputlink.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Please input link");
                return;
            }
            //webView21.Source = new Uri(url);
            //webView21.CoreWebView2.Settings.IsScriptEnabled = true;
            //await Task.Delay(5000);
            int numscroll =1;
            if (!string.IsNullOrEmpty(txtnumscroll.Text))
            {
                int.TryParse(txtnumscroll.Text, out numscroll);
                await ScrollToBottomAsync(numscroll, 1000);
            }
            await ScrollToBottomAsync(4, 1000);
            dem = 0;
            int num = 0;
            if(!string.IsNullOrEmpty(txtnumclick.Text))
                num=int.Parse(txtnumclick.Text);
            string mostCommonClass = "";
            if (string.IsNullOrEmpty(txtcssclass.Text))
            {
                string htmlJson = await webView21.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML;");
                string html = System.Text.Json.JsonSerializer.Deserialize<string>(htmlJson);
                AnchorClassAnalyzer analyzer = new AnchorClassAnalyzer();
                mostCommonClass = analyzer.GetMostLikelyItemAnchorClass(html);
            }
            else
            {
                mostCommonClass = txtcssclass.Text;
            }
            if (rdShowmore.Checked)
                Procsessshowmore(num, mostCommonClass);
            else if (rdpaging.Checked)
                Procsessspaging(num, mostCommonClass);
            else
                Procsesss(mostCommonClass);
        }
        private async void Procsesss(string mostCommonClass)
        {
            string script = "";
            string csslink = txtcssclass.Text.Trim();
            if (!string.IsNullOrEmpty(csslink))
            {
                script = @"
                    (function() {
                        const anchors = document.querySelectorAll('a." + csslink + @"');
                        const links = [];
                        anchors.forEach(a => {
                            if (a.href) {
                                links.push(a.href);
                            }
                        });
                        return links;
                    })();
                    ";
            }
            else
            {
                if (mostCommonClass == "")
                {
                    string parentClass = "";
                    if (string.IsNullOrEmpty(txtcssclass.Text))
                    {
                        string htmlJson = await webView21.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML;");
                        string html = System.Text.Json.JsonSerializer.Deserialize<string>(htmlJson);
                        var analyzer = new AnchorParentAnalyzer();
                        parentClass = analyzer.GetMostLikelyParentClass(html);
                    }
                    if (parentClass == "")
                    {
                        script = @"
                    (function() {
                        const anchors = document.querySelectorAll('a');
                        const links = [];
                        anchors.forEach(a => {
                            if (a.href) {
                                links.push(a.href);
                            }
                        });
                        return links;
                    })();
                    ";
                    }
                    else
                    {
                        parentClass = parentClass.Replace(' ', '.');
                        script = $@"
                        (() => {{
                            const anchors = document.querySelectorAll('.{parentClass} a');
                            const links = [];
                            anchors.forEach(a => {{
                                if (a.href && !a.href.startsWith('javascript') && a.href !== '#') {{
                                    links.push(a.href);
                                }}
                            }});
                            return links;
                        }})();
                        ";
                    }
                }
                else
                    script = @"
                    (function() {
                        const anchors = document.querySelectorAll('a." + mostCommonClass + @"');
                        const links = [];
                        anchors.forEach(a => {
                            if (a.href) {
                                links.push(a.href);
                            }
                        });
                        return links;
                    })();
                    ";

            }
            string resultJson = await webView21.ExecuteScriptAsync(script);
            resultJson = Regex.Unescape(resultJson.Trim('"'));

            var links = JsonSerializer.Deserialize<List<string>>(resultJson);

            if (links != null && links.Count > 0)
            {
                var uniqueLinks = links;
                int maxLinks = 200;
                uniqueLinks = uniqueLinks.Take(maxLinks).ToList();
                foreach (var link in uniqueLinks)
                {
                    lblinks.Items.Add(link);
                }
                lbnumber.Text = uniqueLinks.Count.ToString();
            }
        }
        private async void Procsessshowmore(int num, string mostCommonClass)
        {
            string script = "";
            if (string.IsNullOrEmpty(txtcssshowmore.Text))
            {
                MessageBox.Show("Please Input CSS Show more");
                return;
            }
            string clshowmore = txtcssshowmore.Text.Replace(' ','.').Trim();
            for (int i = 0; i < num; i++)
            {
                script = @"
    (function() {
        const btn = document.querySelector('div."+clshowmore+@"');
        if (btn) {
            btn.click();
            return 'Clicked';
        } else {
            return 'Button not found';
        }
    })();
";
        
        await webView21.CoreWebView2.ExecuteScriptAsync(script);
                await Task.Delay(5000);
                await ScrollToBottomAsync(6, 1000);
            }
            
            string csslink = txtcssclass.Text.Replace(' ','.').Trim();
            if (!string.IsNullOrEmpty(csslink))
            {
                script = @"
                    (function() {
                        const anchors = document.querySelectorAll('a." + csslink + @"');
                        const links = [];
                        anchors.forEach(a => {
                            if (a.href) {
                                links.push(a.href);
                            }
                        });
                        return links;
                    })();
                    ";
            }
            else
            {
                if(mostCommonClass=="")
                script = @"
                    (function() {
                        const anchors = document.querySelectorAll('a');
                        const links = [];
                        anchors.forEach(a => {
                            if (a.href) {
                                links.push(a.href);
                            }
                        });
                        return links;
                    })();
                    ";
                else
                    script = @"
                    (function() {
                        const anchors = document.querySelectorAll('a." + mostCommonClass + @"');
                        const links = [];
                        anchors.forEach(a => {
                            if (a.href) {
                                links.push(a.href);
                            }
                        });
                        return links;
                    })();
                    ";
            }
            string resultJson = await webView21.ExecuteScriptAsync(script);
            resultJson = Regex.Unescape(resultJson.Trim('"'));

            var links = JsonSerializer.Deserialize<List<string>>(resultJson);

            if (links != null && links.Count > 0)
            {
                var uniqueLinks = links;
                int maxLinks = 200;
                uniqueLinks = uniqueLinks.Take(maxLinks).ToList();
                foreach (var link in uniqueLinks)
                {
                    lblinks.Items.Add(link);
                }
                lbnumber.Text = uniqueLinks.Count.ToString();
            }
        }
        private async void Procsessspaging(int num, string mostCommonClass)
        {
            if (string.IsNullOrEmpty(txtnextpaging.Text))
            {
                MessageBox.Show("Please Input CSS Button Next Paging");
                return;
            }
            string clpaging = txtnextpaging.Text.Replace(' ', '.').Trim();
            for (int i = 0; i < num; i++)
            {
                string script = @"
                (function() {
                    const buttons = document.querySelectorAll('."+clpaging+ @"');
                    for (const btn of buttons) {
                        if (btn.innerText.trim().toLowerCase() === 'next') {
                            btn.click();
                            return true;
                        }
                        else
                        {
                              btn.click();
                              return true;
                        }
                    }
                    return false;
                })();
                ";
                await webView21.ExecuteScriptAsync(script);

                await ScrollToBottomAsync(20, 1000);
                await Task.Delay(10000);


                script = "";
                string csslink = txtcssclass.Text.Trim();
                if (!string.IsNullOrEmpty(csslink))
                {
                    script = @"
                    (function() {
                        const anchors = document.querySelectorAll('a." + csslink + @"');
                        const links = [];
                        anchors.forEach(a => {
                            if (a.href) {
                                links.push(a.href);
                            }
                        });
                        return links;
                    })();
                    ";
                }
                else
                {
                    if (mostCommonClass == "")
                        script = @"
                    (function() {
                        const anchors = document.querySelectorAll('a');
                        const links = [];
                        anchors.forEach(a => {
                            if (a.href) {
                                links.push(a.href);
                            }
                        });
                        return links;
                    })();
                    ";
                    else
                        script = @"
                    (function() {
                        const anchors = document.querySelectorAll('a." + mostCommonClass + @"');
                        const links = [];
                        anchors.forEach(a => {
                            if (a.href) {
                                links.push(a.href);
                            }
                        });
                        return links;
                    })();
                    ";
                }
                string resultJson = await webView21.ExecuteScriptAsync(script);
                resultJson = Regex.Unescape(resultJson.Trim('"'));

                var links = JsonSerializer.Deserialize<List<string>>(resultJson);

                if (links != null && links.Count > 0)
                {
                    var uniqueLinks = links;
                    int maxLinks = 200;
                    uniqueLinks = uniqueLinks.Take(maxLinks).ToList();
                    foreach (var link in uniqueLinks)
                    {
                        lblinks.Items.Add(link);
                    }
                    lbnumber.Text = lblinks.Items.Count.ToString();
                }
            }
        }
        private async Task ScrollToBottomAsync(int scrollTimes = 50, int delayMs = 300)
        {
            await Task.Delay(1000);
            for (int i = 0; i < scrollTimes; i++)
            {
                string scrollScript = "window.scrollBy(0, 500);"; // cuộn từng bước nhỏ
                await webView21.ExecuteScriptAsync(scrollScript);
                await Task.Delay(delayMs);
            }
        }
        private string saveFolder;
        
        private async void BTProcessing_Click(object sender, EventArgs e)
        {
            if (webView21 == null || webView21.CoreWebView2 == null)
            {
                await webView21.EnsureCoreWebView2Async();
                webView21.CoreWebView2.Settings.UserAgent =
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36";
            }

            if (!string.IsNullOrEmpty(txtpathsavefile.Text))
                saveFolder = txtpathsavefile.Text;

            _skippedLinks.Clear();
            dem = 0;
            
            for (int i = vt; i < lblinks.Items.Count; i++)
            {
                string link = lblinks.Items[i].ToString();
                if (string.IsNullOrWhiteSpace(link))
                    continue;
                lblinks.SelectedIndex = i;
                lblinks.Refresh(); // update UI
                // Step 1: Navigate to page
                var tcs = new TaskCompletionSource<bool>();
                void Handler(object s, CoreWebView2NavigationCompletedEventArgs ev)
                {
                    webView21.NavigationCompleted -= Handler;
                    if (ev.IsSuccess)
                        tcs.TrySetResult(true);
                    else
                        tcs.TrySetResult(false); // Do not throw an exception, just log it.
                }
                webView21.NavigationCompleted += Handler;

                try
                {
                    webView21.CoreWebView2.Navigate(link);

                    // Wait up to 15 seconds for the page to load.
                    bool navigationCompleted = await Task.WhenAny(tcs.Task, Task.Delay(15000)) == tcs.Task && await tcs.Task;
                    if (!navigationCompleted)
                    {
                        _skippedLinks.Add($"{link}: Skipped due to page load error or timeout.");
                        continue;
                    }

                    await ScrollToBottomAsync(5, 500);

                    // Step 2: Check for error content
                    bool hasErrorContent = await CheckForErrorContentAsync();
                    if (hasErrorContent)
                    {
                        _skippedLinks.Add($"{link}: Skipped due to detected content error (e.g: 'Page Not Found')");
                        await Task.Delay(1000);
                        continue;
                    }

                    // Step 3: Check for meaningful content
                    int contentLength = await CheckContentLengthAsync();
                    if (contentLength < MinContentLength)
                    {
                        _skippedLinks.Add($"{link}: Skipped due to insufficient content (length: {contentLength})");
                        await Task.Delay(1000);
                        continue;
                    }

                    // Step 4: Save page
                    string safeFileName = dem.ToString("D4") + ".mhtml";
                    string savePath = Path.Combine(saveFolder, safeFileName);

                    string snapshotResult = await webView21.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureSnapshot", "{}");
                    JObject jsonResult;
                    try
                    {
                        jsonResult = JObject.Parse(snapshotResult);
                    }
                    catch (JsonException jsonEx)
                    {
                        _skippedLinks.Add($"{link}: Skipped due to JSON parsing error: {jsonEx.Message}");
                        continue;
                    }

                    string mhtmlContent = jsonResult["data"]?.ToString();
                    if (string.IsNullOrEmpty(mhtmlContent))
                    {
                        _skippedLinks.Add($"{link}: Skipped due to empty MHTML content.");
                        continue;
                    }

                    File.WriteAllText(savePath, mhtmlContent);

                    dem++;
                    lbnumsave.Text = dem.ToString();
                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    _skippedLinks.Add($"{link}: Skipped error: {ex.Message}");
                    await Task.Delay(1000);
                    continue;
                }
            }

            string message = $"Saved {dem} file!";
            if (_skippedLinks.Count > 0)
            {
                message += $"\nSkipped {_skippedLinks.Count} link:\n" + string.Join("\n", _skippedLinks);
            }
            MessageBox.Show(message);
        }

        private async Task<bool> CheckForErrorContentAsync()
        {
            try
            {
                string script = @"
                    (function() {
                        const bodyText = document.body.innerText.toLowerCase();
                        const errorIndicators = [
                            'page not found',
                            '404 not found',
                            'error 404',
                            'not found',
                            'this page doesn’t exist',
                            'this page does not exist',
                            'oops! something went wrong'
                        ];
                        return errorIndicators.some(indicator => bodyText.includes(indicator));
                    })();
                ";
                string result = await webView21.ExecuteScriptAsync(script);
                return result == "true";
            }
            catch (Exception ex)
            {
                _skippedLinks.Add($"Content check failed: {ex.Message}");
                return false;
            }
        }

        private async Task<int> CheckContentLengthAsync()
        {
            try
            {
                string script = @"
                    (function() {
                        // Try to select main content, fallback to body
                        const mainContent = document.querySelector('main, article, .content, .main-content') || document.body;
                        // Exclude headers, footers, and navs
                        const excluded = mainContent.querySelectorAll('header, footer, nav, .header, .footer, .navigation');
                        let content = mainContent.innerText;
                        excluded.forEach(el => {
                            content = content.replace(el.innerText, '');
                        });
                        return content.trim().length;
                    })();
                ";
                string result = await webView21.ExecuteScriptAsync(script);
                return int.TryParse(result, out int length) ? length : 0;
            }
            catch (Exception ex)
            {
                _skippedLinks.Add($"Length check failed.: {ex.Message}");
                return 0;
            }
        }
        private int delay;
        private async void Form1_Load(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async();
            webView21.CoreWebView2.Settings.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36";

            saveFolder = "";
            string foldername=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            txtpathsavefile.Text = foldername;
            vt = 0;
        }

        private void BTBrowser_Click(object sender, EventArgs e)
        {
            // create FolderBrowserDialog
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                // setup attribute FolderBrowserDialog
                folderBrowserDialog.Description = "Choose folder to save file";
                folderBrowserDialog.ShowNewFolderButton = true; // allow create folder new
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer; // root default
                // view dialog và test
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // get path to folder chosen
                    string folderPath = folderBrowserDialog.SelectedPath;
                    txtpathsavefile.Text = folderPath;
                    
                }
            }
        }

        private void rdShowmore_CheckedChanged(object sender, EventArgs e)
        {
            if (rdShowmore.Checked)
            {
                txtcssshowmore.ReadOnly = false;
                txtnextpaging.ReadOnly = true;
                txtnumclick.ReadOnly = false;
            }
            else
                txtcssshowmore.ReadOnly = true;
        }

        private void rdpaging_CheckedChanged(object sender, EventArgs e)
        {
            if (rdpaging.Checked)
            {
                txtcssshowmore.ReadOnly = true;
                txtnextpaging.ReadOnly = false;
                txtnumclick.ReadOnly = false;
            }
            else
                txtnextpaging.ReadOnly = true;
        }
        private void SaveLinksToFile()
        {
            try
            {
                // Use SaveFileDialog to let user choose save location
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    saveFileDialog.DefaultExt = "txt";
                    saveFileDialog.Title = "Choose where to save links";
                    saveFileDialog.FileName = "links.txt"; // Default file name

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;

                        // Get all items from the ListBox
                        List<string> allItems = lblinks.Items.Cast<string>().ToList();
                        int totalItems = allItems.Count;

                        // Remove duplicates
                        List<string> uniqueItems = allItems.Distinct().ToList();
                        int duplicatesRemoved = totalItems - uniqueItems.Count;

                        // Save unique items to file
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            foreach (string item in uniqueItems)
                            {
                                writer.WriteLine(item);
                            }
                        }

                        // Show message
                        string message = $"Total items: {totalItems}\n" +
                                         $"Duplicate items removed: {duplicatesRemoved}\n" +
                                         $"Items saved: {uniqueItems.Count}";
                        MessageBox.Show(message, "Save Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file: " + ex.Message);
            }
        }

        private void LoadLinksFromFile()
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    openFileDialog.Title = "Choose a file to load links";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;
                        // Clear existing items in ListBox
                        lblinks.Items.Clear();
                        // Read links from file
                        string[] lines = File.ReadAllLines(filePath);
                        var uniqueLinks = lines
                         .Skip(1)                               
                         .Where(line => !string.IsNullOrWhiteSpace(line))  
                         .Select(line => line.Trim())         
                         .Distinct()                           
                         .ToList();
                        foreach (string link in uniqueLinks)
                        {
                            lblinks.Items.Add(link);
                        }
                        lbnumber.Text=lblinks.Items.Count.ToString();
                        MessageBox.Show($"Links loaded from {filePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading file: " + ex.Message);
            }
        }
        private void BTLoadlinks_Click(object sender, EventArgs e)
        {
            LoadLinksFromFile();
        }

        private void BTSavelinks_Click(object sender, EventArgs e)
        {
            SaveLinksToFile();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _httpClient?.Dispose();
        }

        private void txtinputlink_TextChanged(object sender, EventArgs e)
        {

        }

        private async void txtinputlink_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtinputlink.Text))
            {
                var tcs = new TaskCompletionSource<bool>();
                string link = txtinputlink.Text;
                webView21.CoreWebView2.Navigate(link);
                // Wait up to 15 seconds for the page to load.
                bool navigationCompleted = await Task.WhenAny(tcs.Task, Task.Delay(5000)) == tcs.Task && await tcs.Task;
                await Task.Delay(5000);
               
                    string mostCommonClass = "";
                    if (string.IsNullOrEmpty(txtcssclass.Text))
                    {
                        string htmlJson = await webView21.CoreWebView2.ExecuteScriptAsync("document.documentElement.outerHTML;");
                        string html = System.Text.Json.JsonSerializer.Deserialize<string>(htmlJson);
                        AnchorClassAnalyzer analyzer = new AnchorClassAnalyzer();
                        mostCommonClass = analyzer.GetMostLikelyItemAnchorClass(html);
                        txtcssclass.Text = mostCommonClass;
                    }
                
            }
        }
        private List<string> allLinks = new List<string>();
        private async void BTPreventbot_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Before crawling, please complete the following steps: 1) Install the Chrome extension 2) Start the local server.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            dem = 0;
            if (result == DialogResult.Yes)
            {
                int delay = 5;
                if(!string.IsNullOrEmpty(txtdelay.Text))
                    delay = int.Parse(txtdelay.Text);
                foreach (var item in lblinks.Items)
                {
                    allLinks.Add(item.ToString());
                }
                if (allLinks.Count == 0)
                {
                    MessageBox.Show("Please load links");
                    return;
                }
                var linksToOpen = allLinks.ToList();
                int dem = 0;
                foreach (var link in linksToOpen)
                {
                    if (dem >= 0)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(new ProcessStartInfo
                            {
                                FileName = link,
                                UseShellExecute = true
                            });
                            await Task.Delay(delay);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Can't open link: {link}\n{ex.Message}");
                        }
                    }
                    dem++;
                    lbnumsave.Text=dem.ToString();
                }

            
            }
            

        }
        private int vt;
        private void lblinks_SelectedIndexChanged(object sender, EventArgs e)
        {
            vt= lblinks.SelectedIndex;
        }
    }
    
}
    

