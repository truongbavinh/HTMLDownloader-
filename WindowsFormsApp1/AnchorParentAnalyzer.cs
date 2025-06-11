using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
namespace WindowsFormsApp1
{
    public class AnchorParentAnalyzer
    {
        public string GetMostLikelyParentClass(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent)) return "";

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var anchorNodes = doc.DocumentNode.SelectNodes("//a");
            if (anchorNodes == null || anchorNodes.Count == 0) return "";

            var parentClassCount = new Dictionary<string, int>();

            foreach (var a in anchorNodes)
            {
                var innerText = a.InnerText.Trim();
                var href = a.GetAttributeValue("href", "").Trim();

                if (string.IsNullOrEmpty(href) || href == "#" || href.StartsWith("javascript")) continue;
                if (innerText.Length < 3) continue;
                if (IsInExcludedSection(a)) continue;

                // 🔁 Dò tối đa 2 cấp, nhưng nếu cấp 1 có class thì không dò tiếp
                string parentClass = "";

                var parent1 = a.ParentNode;
                if (parent1 != null)
                {
                    parentClass = parent1.GetAttributeValue("class", "").Trim().ToLower();

                    if (string.IsNullOrEmpty(parentClass))
                    {
                        var parent2 = parent1.ParentNode;
                        if (parent2 != null)
                        {
                            parentClass = parent2.GetAttributeValue("class", "").Trim().ToLower();
                        }
                    }
                }

                if (string.IsNullOrEmpty(parentClass)) continue;

                if (parentClassCount.ContainsKey(parentClass))
                    parentClassCount[parentClass]++;
                else
                    parentClassCount[parentClass] = 1;
            }


            //Chỉ chọn class có ít nhất 3 thẻ <a>
            var filtered = parentClassCount
                .Where(kv => kv.Value >= 3)
                .OrderByDescending(kv => kv.Value)
                .ToList();

            return filtered.FirstOrDefault().Key ?? "";
        }

        public List<string> GetFlightUrls(string htmlContent)
        {
            var results = new List<string>();
            if (string.IsNullOrWhiteSpace(htmlContent)) return results;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var flightRows = doc.DocumentNode.SelectNodes("//tr[@url]");
            if (flightRows != null)
            {
                foreach (var tr in flightRows)
                {
                    var url = tr.GetAttributeValue("url", "").Trim();
                    if (!string.IsNullOrEmpty(url))
                    {
                        results.Add(url);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Loại bỏ thẻ <a> nếu tổ tiên thuộc các vùng không mong muốn (footer, header, menu, ...)
        /// </summary>
        private bool IsInExcludedSection(HtmlNode node)
        {
            var current = node;
            while (current != null)
            {
                var tag = current.Name.ToLower();
                if (tag == "footer" || tag == "nav" || tag == "header") return true;

                var cls = current.GetAttributeValue("class", "").ToLower();
                if (cls.Contains("footer") || cls.Contains("menu") || cls.Contains("disclaimer") ||
                    cls.Contains("locale") || cls.Contains("dropdown") || cls.Contains("language") ||
                    cls.Contains("link-list") || cls.Contains("link-group") || cls.Contains("bottom"))
                    return true;

                current = current.ParentNode;
            }
            return false;
        }
    }
}

