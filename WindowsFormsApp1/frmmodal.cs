using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApp1
{
    public partial class frmmodal : Form
    {
        public frmmodal()
        {
            InitializeComponent();
        }

        private string saveFolder;
        private int dem;
        private List<string> _skippedLinks; // To log skipped links
        private async void frmmodal_Load(object sender, EventArgs e)
        {
            await webView21.EnsureCoreWebView2Async();
            webView21.CoreWebView2.Settings.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36";

            saveFolder = "";
            string foldername = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            txtpathsavefile.Text = foldername;
            dem = 0;
            _skippedLinks = new List<string>(); // Initialize skipped links list
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

            }
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

        private void BTProcessing_Click(object sender, EventArgs e)
        {
            int num = 0;
            if (!string.IsNullOrEmpty(txtnumclick.Text))
                num = int.Parse(txtnumclick.Text);
            if (rdShowmore.Checked)
                Procsessshowmore(num);
            else if (rdpaging.Checked)
                Procsessspaging(num);
            else
                ProcessPage();
        }
        public async Task ProcessPage()
        {
            string url = txtinputlink.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Please input link");
                return;
            }
            string clss = txtcssclass.Text.Trim();
            if (string.IsNullOrEmpty(clss))
            {
                MessageBox.Show("Please input css class item");
                return;
            }
            try
            {
                string script = @"
                    (function() {
                        const buttons = document.querySelectorAll('." + clss + @"');
                        const results = Array.from(buttons).map((btn, index) => {
                            const itemContainer = btn.closest('.flight-result__item, .Fxw9-result-item-container');
                            const resultDiv = itemContainer ? itemContainer.querySelector('div[data-resultid]') : null;
                            const resultId = resultDiv ? resultDiv.getAttribute('data-resultid') : null;
                            const destination = btn.querySelector('.PlaceCard_nameContent__NDM5M h2 span')?.textContent || null;
                            const price = btn.querySelector('.PriceDescription_priceContainer__YjgxZ span')?.textContent || null;
                            const flightType = btn.querySelector('.PlaceCard_additionalInfoContainer__NjFkM span')?.textContent || null;
                            return { 
                                index: index, 
                                element: btn.outerHTML, 
                                resultId: resultId,
                                destination: destination,
                                price: price,
                                flightType: flightType
                            };
                        });
                        return results;
                    })();
                ";

                string resultJson = await webView21.ExecuteScriptAsync(script);
                await Task.Delay(2000);
                var buttons = JsonSerializer.Deserialize<List<ButtonInfo>>(resultJson.Replace("resultId", "ResultId"));
                Console.WriteLine($"Found {buttons?.Count ?? 0} buttons");

                if (buttons != null && buttons.Count > 0)
                {
                    var uniqueButtons = buttons.Take(300).ToList();
                    Console.WriteLine($"Processing {uniqueButtons.Count} unique buttons");

                    try
                    {
                        for (int i = 0; i < uniqueButtons.Count; i++)
                        {
                            var button = uniqueButtons[i];
                            string clickScript = @"
                                (function() {
                                    const buttons = document.querySelectorAll('." + clss + @"');
                                    if (buttons[" + i + @"]) {
                                        buttons[" + i + @"].click();
                                        return true;
                                    }
                                    return false;
                                })();
                            ";
                            string clickResult = await webView21.ExecuteScriptAsync(clickScript);
                            Console.WriteLine($"Click result: {clickResult}");

                            await Task.Delay(8000); // Tăng thời gian chờ
                            string htmlContent = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML");
                            // htmlContent = JsonSerializer.Deserialize<string>(htmlContent);

                            if (!string.IsNullOrWhiteSpace(htmlContent))
                            {
                                saveFolder = txtpathsavefile.Text.Trim();
                                string safeFileName = file_name() + ".mhtml";
                                string savePath = Path.Combine(saveFolder, safeFileName);

                                string snapshotResult = await webView21.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureSnapshot", "{}");
                                JObject jsonResult;
                                try
                                {
                                    jsonResult = JObject.Parse(snapshotResult);
                                }
                                catch (JsonException jsonEx)
                                {
                                    _skippedLinks.Add($"Skipped due to JSON parsing error: {jsonEx.Message}");
                                    continue;
                                }

                                string mhtmlContent = jsonResult["data"]?.ToString();
                                if (string.IsNullOrEmpty(mhtmlContent))
                                {
                                    _skippedLinks.Add($"Skipped due to empty MHTML content.");
                                    continue;
                                }

                                File.WriteAllText(savePath, mhtmlContent);

                                dem++;
                                lbnumsave.Text = dem.ToString();
                                await Task.Delay(2000);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể xử lý nút: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    }


                    Console.WriteLine($"Đã lưu {dem} links và HTML");
                }
                else
                {
                    Console.WriteLine($"Không tìm thấy nút chi tiết");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing page: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        public async Task Procsessshowmore(int num)
        {
            string url = txtinputlink.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Please input link");
                return;
            }
            string clss = txtcssclass.Text.Trim();
            if (string.IsNullOrEmpty(clss))
            {
                MessageBox.Show("Please input css class item");
                return;
            }
            string script = "";
            string clshowmore = txtcssshowmore.Text.Replace(' ', '.').Trim();
            for (int i = 0; i < num; i++)
            {
                script = @"
                    (function() {
                        const btn = document.querySelector('." + clshowmore + @"');
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
            try
            {
                script = @"
                    (function() {
                        const buttons = document.querySelectorAll('." + clss + @"');
                        const results = Array.from(buttons).map((btn, index) => {
                            const itemContainer = btn.closest('.flight-result__item, .Fxw9-result-item-container');
                            const resultDiv = itemContainer ? itemContainer.querySelector('div[data-resultid]') : null;
                            const resultId = resultDiv ? resultDiv.getAttribute('data-resultid') : null;
                            const destination = btn.querySelector('.PlaceCard_nameContent__NDM5M h2 span')?.textContent || null;
                            const price = btn.querySelector('.PriceDescription_priceContainer__YjgxZ span')?.textContent || null;
                            const flightType = btn.querySelector('.PlaceCard_additionalInfoContainer__NjFkM span')?.textContent || null;
                            return { 
                                index: index, 
                                element: btn.outerHTML, 
                                resultId: resultId,
                                destination: destination,
                                price: price,
                                flightType: flightType
                            };
                        });
                        return results;
                    })();
                ";

                string resultJson = await webView21.ExecuteScriptAsync(script);
                await Task.Delay(2000);
                var buttons = JsonSerializer.Deserialize<List<ButtonInfo>>(resultJson.Replace("resultId", "ResultId"));
                Console.WriteLine($"Found {buttons?.Count ?? 0} buttons");

                if (buttons != null && buttons.Count > 0)
                {
                    var uniqueButtons = buttons.Take(300).ToList();
                    Console.WriteLine($"Processing {uniqueButtons.Count} unique buttons");

                    try
                    {
                        for (int i = 0; i < uniqueButtons.Count; i++)
                        {
                            var button = uniqueButtons[i];
                            string clickScript = @"
                                (function() {
                                    const buttons = document.querySelectorAll('." + clss + @"');
                                    if (buttons[" + i + @"]) {
                                        buttons[" + i + @"].click();
                                        return true;
                                    }
                                    return false;
                                })();
                            ";
                            string clickResult = await webView21.ExecuteScriptAsync(clickScript);
                            Console.WriteLine($"Click result: {clickResult}");

                            await Task.Delay(8000); // Tăng thời gian chờ
                            string htmlContent = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML");
                            // htmlContent = JsonSerializer.Deserialize<string>(htmlContent);

                            if (!string.IsNullOrWhiteSpace(htmlContent))
                            {
                                saveFolder = txtpathsavefile.Text.Trim();
                                string safeFileName = file_name() + ".mhtml";
                                string savePath = Path.Combine(saveFolder, safeFileName);

                                string snapshotResult = await webView21.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureSnapshot", "{}");
                                JObject jsonResult;
                                try
                                {
                                    jsonResult = JObject.Parse(snapshotResult);
                                }
                                catch (JsonException jsonEx)
                                {
                                    _skippedLinks.Add($"Skipped due to JSON parsing error: {jsonEx.Message}");
                                    continue;
                                }

                                string mhtmlContent = jsonResult["data"]?.ToString();
                                if (string.IsNullOrEmpty(mhtmlContent))
                                {
                                    _skippedLinks.Add($"Skipped due to empty MHTML content.");
                                    continue;
                                }

                                File.WriteAllText(savePath, mhtmlContent);

                                dem++;
                                lbnumsave.Text = dem.ToString();
                                await Task.Delay(2000);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Không thể xử lý nút: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    }


                    Console.WriteLine($"Đã lưu {dem} links và HTML");
                }
                else
                {
                    Console.WriteLine($"Không tìm thấy nút chi tiết");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing page: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        public async Task Procsessspaging(int num)
        {
            string url = txtinputlink.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                MessageBox.Show("Please input link");
                return;
            }
            string clss = txtcssclass.Text.Trim();
            if (string.IsNullOrEmpty(clss))
            {
                MessageBox.Show("Please input css class item");
                return;
            }
            string clpaging = txtnextpaging.Text.Replace(' ', '.').Trim();
            for (int u = 0; u < num; u++)
            {
                string script = @"
                (function() {
                    const buttons = document.querySelectorAll('." + clpaging + @"');
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
                try
                {
                    script = @"
                    (function() {
                        const buttons = document.querySelectorAll('." + clss + @"');
                        const results = Array.from(buttons).map((btn, index) => {
                            const itemContainer = btn.closest('.flight-result__item, .Fxw9-result-item-container');
                            const resultDiv = itemContainer ? itemContainer.querySelector('div[data-resultid]') : null;
                            const resultId = resultDiv ? resultDiv.getAttribute('data-resultid') : null;
                            const destination = btn.querySelector('.PlaceCard_nameContent__NDM5M h2 span')?.textContent || null;
                            const price = btn.querySelector('.PriceDescription_priceContainer__YjgxZ span')?.textContent || null;
                            const flightType = btn.querySelector('.PlaceCard_additionalInfoContainer__NjFkM span')?.textContent || null;
                            return { 
                                index: index, 
                                element: btn.outerHTML, 
                                resultId: resultId,
                                destination: destination,
                                price: price,
                                flightType: flightType
                            };
                        });
                        return results;
                    })();
                ";

                    string resultJson = await webView21.ExecuteScriptAsync(script);
                    await Task.Delay(2000);
                    var buttons = JsonSerializer.Deserialize<List<ButtonInfo>>(resultJson.Replace("resultId", "ResultId"));
                    Console.WriteLine($"Found {buttons?.Count ?? 0} buttons");

                    if (buttons != null && buttons.Count > 0)
                    {
                        var uniqueButtons = buttons.Take(300).ToList();
                        Console.WriteLine($"Processing {uniqueButtons.Count} unique buttons");

                        try
                        {
                            for (int i = 0; i < uniqueButtons.Count; i++)
                            {
                                var button = uniqueButtons[i];
                                string clickScript = @"
                                (function() {
                                    const buttons = document.querySelectorAll('." + clss + @"');
                                    if (buttons[" + i + @"]) {
                                        buttons[" + i + @"].click();
                                        return true;
                                    }
                                    return false;
                                })();
                            ";
                                string clickResult = await webView21.ExecuteScriptAsync(clickScript);
                                Console.WriteLine($"Click result: {clickResult}");

                                await Task.Delay(8000); // Tăng thời gian chờ
                                string htmlContent = await webView21.ExecuteScriptAsync("document.documentElement.outerHTML");
                                // htmlContent = JsonSerializer.Deserialize<string>(htmlContent);

                                if (!string.IsNullOrWhiteSpace(htmlContent))
                                {
                                    saveFolder = txtpathsavefile.Text.Trim();
                                    string safeFileName = file_name() + ".mhtml";
                                    string savePath = Path.Combine(saveFolder, safeFileName);

                                    string snapshotResult = await webView21.CoreWebView2.CallDevToolsProtocolMethodAsync("Page.captureSnapshot", "{}");
                                    JObject jsonResult;
                                    try
                                    {
                                        jsonResult = JObject.Parse(snapshotResult);
                                    }
                                    catch (JsonException jsonEx)
                                    {
                                        _skippedLinks.Add($"Skipped due to JSON parsing error: {jsonEx.Message}");
                                        continue;
                                    }

                                    string mhtmlContent = jsonResult["data"]?.ToString();
                                    if (string.IsNullOrEmpty(mhtmlContent))
                                    {
                                        _skippedLinks.Add($"Skipped due to empty MHTML content.");
                                        continue;
                                    }

                                    File.WriteAllText(savePath, mhtmlContent);

                                    dem++;
                                    lbnumsave.Text = dem.ToString();
                                    await Task.Delay(2000);
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Không thể xử lý nút: {ex.Message}");
                            Console.WriteLine($"Stack trace: {ex.StackTrace}");
                        }

                        Console.WriteLine($"Đã lưu {dem} links và HTML");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing page: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
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
        private string file_name()
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string randomPart = new Random().Next(1000, 9999).ToString();
            string fileName = $"{timeStamp}_{randomPart}";
            return fileName;
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

        private void txtinputlink_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
