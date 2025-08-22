using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WindowsFormsApp1
{
    public class AnchorClassAnalyzer
    {
        /// <summary>
        /// Trả về class "có khả năng là item anchor" nhất dựa trên thống kê & scoring.
        /// Nếu không tìm được theo score, fallback: class xuất hiện nhiều nhất.
        /// </summary>
        public string GetMostLikelyItemAnchorClass(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent)) return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // Lấy tất cả <a> có class và href hợp lệ (không rỗng, không "#")
            var anchorNodes = doc.DocumentNode.SelectNodes(
                "//a[@class and string-length(normalize-space(@href))>0 and @href!='#']");

            if (anchorNodes == null || anchorNodes.Count == 0)
                return string.Empty;

            var classGroups = new Dictionary<string, List<HtmlNode>>(StringComparer.Ordinal);

            foreach (var a in anchorNodes)
            {
                if (IsInExcludedSection(a, 5))
                    continue;

                var classAttr = a.GetAttributeValue("class", "").Trim();
                if (string.IsNullOrWhiteSpace(classAttr)) continue;

                foreach (var cls in SafeSplitClasses(classAttr))
                {
                    List<HtmlNode> list;
                    if (!classGroups.TryGetValue(cls, out list))
                    {
                        list = new List<HtmlNode>();
                        classGroups[cls] = list;
                    }
                    list.Add(a);
                }
            }

            if (classGroups.Count == 0)
                return string.Empty;

            string bestClass = string.Empty;
            double bestScore = double.MinValue;

            foreach (var kv in classGroups)
            {
                var className = kv.Key;
                var nodes = kv.Value;

                // Hạ ngưỡng: cho qua if < 2
                if (nodes.Count < 2) continue;

                var texts = nodes.Select(n => NormalizeText(n.InnerText)).ToList();
                double avgTextLength = texts.Count > 0 ? texts.Average(t => t != null ? t.Length : 0) : 0;
                int distinctTextCount = texts.Where(t => !string.IsNullOrEmpty(t)).Distinct().Count();

                var hrefs = nodes.Select(n => n.GetAttributeValue("href", "")).ToList();
                int uniqueHrefs = hrefs.Where(h => !string.IsNullOrWhiteSpace(h)).Distinct().Count();

                int likelyItemCount = hrefs.Count(IsLikelyItemHref);
                double itemRatio = nodes.Count > 0 ? (double)likelyItemCount / nodes.Count : 0.0;

                double score =
                      nodes.Count
                    + uniqueHrefs * 0.7
                    + avgTextLength * 0.2
                    + distinctTextCount * 0.3
                    + itemRatio * 2.0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestClass = className;
                }
            }

            if (string.IsNullOrEmpty(bestClass))
            {
                bestClass = classGroups
                    .OrderByDescending(pair => pair.Value.Count)
                    .Select(pair => pair.Key)
                    .FirstOrDefault() ?? string.Empty;
            }

            return bestClass ?? string.Empty;
        }

        /// <summary>
        /// Lấy danh sách tất cả class của thẻ &lt;a&gt; kèm tần suất (phục vụ debug).
        /// </summary>
        public List<(string Class, int Count)> GetAllAnchorClasses(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent)) return new List<(string Class, int Count)>();

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var anchors = doc.DocumentNode.SelectNodes(
                "//a[@class and string-length(normalize-space(@href))>0 and @href!='#']");

            if (anchors == null || anchors.Count == 0) return new List<(string Class, int Count)>();

            var classCounts = anchors
                .Where(a => !IsInExcludedSection(a, 5))
                .SelectMany(a => SafeSplitClasses(a.GetAttributeValue("class", "")))
                .GroupBy(c => c, StringComparer.Ordinal)
                .Select(g => (Class: g.Key, Count: g.Count()))
                .OrderByDescending(x => x.Count)
                .ToList();

            return classCounts;
        }

        /// <summary>
        /// Giảm false-positive:
        /// - Chỉ xét tối đa 'maxDepth' tổ tiên
        /// - Tag header/nav/footer, hoặc role="navigation"
        /// - Class chứa menu/nav/dropdown/breadcrumb/footer/header
        /// </summary>
        private bool IsInExcludedSection(HtmlNode node, int maxDepth)
        {
            int depth = 0;
            var current = node.ParentNode;
            while (current != null && depth < maxDepth)
            {
                var tag = (current.Name ?? string.Empty).ToLowerInvariant();
                if (tag == "header" || tag == "footer" || tag == "nav")
                    return true;

                var role = current.GetAttributeValue("role", "").ToLowerInvariant();
                if (role.Contains("navigation"))
                    return true;

                var cls = current.GetAttributeValue("class", "").ToLowerInvariant();
                if (ContainsAny(cls, new[] { "menu", "nav", "dropdown", "breadcrumb", "footer", "header" }))
                    return true;

                current = current.ParentNode;
                depth++;
            }
            return false;
        }

        private static IEnumerable<string> SafeSplitClasses(string classAttr)
        {
            if (string.IsNullOrWhiteSpace(classAttr))
                yield break;

            foreach (var token in Regex.Split(classAttr.Trim(), "\\s+"))
            {
                if (!string.IsNullOrWhiteSpace(token))
                    yield return token;
            }
        }

        private static string NormalizeText(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            var t = raw.Trim();
            t = Regex.Replace(t, "\\s+", " ");
            return t;
        }

        private static bool ContainsAny(string haystack, IEnumerable<string> needles)
        {
            if (string.IsNullOrEmpty(haystack)) return false;
            foreach (var n in needles)
            {
                if (haystack.Contains(n)) return true;
            }
            return false;
        }

        /// <summary>
        /// Heuristic: href giống link "chi tiết item".
        /// </summary>
        private static bool IsLikelyItemHref(string href)
        {
            if (string.IsNullOrWhiteSpace(href)) return false;
            var needles = new[]
            {
                "/hotel", "/product", "/item", "/detail", "/details", "/dp/",
                "/p/", "/d/", "productId=", "itemId="
            };
            return needles.Any(n => href.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
