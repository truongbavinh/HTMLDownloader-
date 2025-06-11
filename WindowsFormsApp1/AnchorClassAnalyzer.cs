using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
namespace WindowsFormsApp1
{
    public class AnchorClassAnalyzer
    {
        public string GetMostLikelyItemAnchorClass(string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(htmlContent)) return "";

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var anchorNodes = doc.DocumentNode.SelectNodes("//a[@class]");
            if (anchorNodes == null) return "";

            var classGroups = new Dictionary<string, List<HtmlNode>>();

            foreach (var a in anchorNodes)
            {
                if (IsInExcludedSection(a)) continue;

                var classAttr = a.GetAttributeValue("class", "").Trim();
                if (string.IsNullOrWhiteSpace(classAttr)) continue;

                var href = a.GetAttributeValue("href", "");
                if (string.IsNullOrWhiteSpace(href) || href == "#") continue;

                var innerText = a.InnerText.Trim();
                if (innerText.Length < 15) continue; // bỏ nếu nội dung quá ngắn

                var classList = classAttr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var cls in classList)
                {
                    if (!classGroups.ContainsKey(cls))
                        classGroups[cls] = new List<HtmlNode>();

                    classGroups[cls].Add(a);
                }
            }

            // Tính điểm cho từng nhóm class
            string bestClass = "";
            double bestScore = 0;

            foreach (var kv in classGroups)
            {
                var className = kv.Key;
                var nodes = kv.Value;

                if (nodes.Count < 3) continue; // bỏ qua nhóm quá nhỏ

                var avgTextLength = nodes.Average(n => n.InnerText.Trim().Length);
                var uniqueHrefs = nodes.Select(n => n.GetAttributeValue("href", "")).Distinct().Count();

                double score = nodes.Count + avgTextLength * 0.3 + uniqueHrefs * 0.5;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestClass = className;
                }
            }

            return bestClass;
        }

        private bool IsInExcludedSection(HtmlNode node)
        {
            var current = node.ParentNode;
            while (current != null)
            {
                var tag = current.Name.ToLower();
                if (tag == "header" || tag == "footer" || tag == "nav" || tag == "ul" || tag == "li")
                    return true;

                var classAttr = current.GetAttributeValue("class", "").ToLower();
                if (classAttr.Contains("menu") || classAttr.Contains("dropdown") || classAttr.Contains("nav") || classAttr.Contains("footer") || classAttr.Contains("header"))
                    return true;

                current = current.ParentNode;
            }
            return false;
        }
    }

}
