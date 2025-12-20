using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AnnouncementCore.Utility
{
    public static class HtmlToTmpConverter
    {
        public static string Convert(string htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent))
                return string.Empty;

            try
            {

                string result = htmlContent;

                result = DecodeHtmlEntities(result);
                result = FixHtmlFormat(result);
                result = ProcessLineBreaks(result);
                result = ProcessParagraphs(result);
                result = ProcessHeadings(result);
                result = ProcessBlockquotes(result);
                result = ProcessInlineStyles(result);
                result = ConvertSpecialLinksDirectly(result);
                result = ProcessLists(result);
                result = RemoveRemainingHtmlTags(result);
                result = CleanupFormatting(result);
                result = EnsureLineBreaks(result);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"HTML富文本转换失败: {e.Message}\nStackTrace: {e.StackTrace}");
                return htmlContent;
            }
        }

        private static string EnsureLineBreaks(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string result = text;

            if (!result.Contains("\n"))
            {
                Debug.LogWarning("文本中未检测到换行符，尝试添加换行");

                if (result.Contains("。"))
                {
                    result = Regex.Replace(result, @"。\s*", "。\n");
                }

                if (result.Contains("；"))
                {
                    result = Regex.Replace(result, @"；\s*", "；\n");
                }

                if (result.Contains("•") || result.Contains("·"))
                {
                    result = Regex.Replace(result, @"([•·])\s*", "\n$1 ");
                }
            }

            result = Regex.Replace(result, @"\n\s*\n", "\n\n");
            result = Regex.Replace(result, @"^\n+", "");
            result = Regex.Replace(result, @"\n+$", "");

            return result;
        }
      
        private static string FixLineBreaks(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string result = text;

            result = result.Replace("\r\n", "\n");
            result = result.Replace("\r", "\n");

            result = Regex.Replace(result, @"\n{3,}", "\n\n");

            if (!result.Contains("\n") && result.Contains("。"))
            {

                result = Regex.Replace(result, @"。\s*", "。\n");
            }

            result = Regex.Replace(result, @"^\s+", "", RegexOptions.Multiline);
            result = Regex.Replace(result, @"\s+$", "", RegexOptions.Multiline);

            return result.Trim();
        }

        private static string FixHtmlFormat(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            string result = html;

            result = Regex.Replace(result, @"<a(\w+)", "<a $1");
            result = Regex.Replace(result, @"(\w+)=\""", " $1=\"");

            result = result.Replace("target=\"_blank\"rel=", "target=\"_blank\" rel=");
            result = result.Replace("rel=\"noopenernoreferrernofollow\"", "rel=\"noopener noreferrer nofollow\"");

            return result;
        }

        private static string ConvertSpecialLinksDirectly(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            Debug.Log($"开始链接转换，输入: {html}");

            string pattern = @"<a\s+[^>]*?href\s*=\s*[""']([^""']*)[""'][^>]*?>([^<]*?)\\([^<]+)</a>";

            try
            {
                string result = Regex.Replace(html, pattern, match =>
                {
                    string url = match.Groups[1].Value;
                    string urlPart = match.Groups[2].Value;
                    string displayText = match.Groups[3].Value.Trim();

                    Debug.Log($"链接匹配: URL={url}, 显示文本={displayText}");

                    if (string.IsNullOrEmpty(displayText))
                    {
                        displayText = urlPart.Trim();
                    }

                    string linkTag = $"<link=\"{url}\">{displayText}</link>";
                    Debug.Log($"转换为: {linkTag}");

                    return linkTag;
                }, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                Debug.Log($"链接转换完成，结果: {result}");
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"链接转换失败: {e.Message}");
                return html;
            }
        }
      
        public static string ConvertApiContent(string apiHtmlContent)
        {
            if (string.IsNullOrEmpty(apiHtmlContent))
                return string.Empty;

            try
            {
                string fixedHtml = apiHtmlContent;
                fixedHtml = fixedHtml.Replace("&lt;a ", "<a ")
                                    .Replace("&lt;/a&gt;", "</a>")
                                    .Replace("href=&quot;", "href=\"")
                                    .Replace("&quot;&gt;", "\">");

                if (fixedHtml.Contains("href=\"<a"))
                {
                    fixedHtml = Regex.Replace(fixedHtml, @"href=""<a[^>]*href=""([^""]*)""[^>]*>([^<]*)</a>""",
                        match => $"href=\"{match.Groups[1].Value}\"");
                }

                return Convert(fixedHtml);
            }
            catch (Exception e)
            {
                Debug.LogError($"API内容转换失败: {e.Message}");
                return apiHtmlContent;
            }
        }

        private static string DecodeHtmlEntities(string text)
        {
            try
            {
                string decoded = System.Net.WebUtility.HtmlDecode(text);
                decoded = decoded.Replace("&lt;", "<")
                                .Replace("&gt;", ">")
                                .Replace("&amp;", "&")
                                .Replace("&quot;", "\"")
                                .Replace("&#39;", "'")
                                .Replace("&nbsp;", " ");

                return decoded;
            }
            catch
            {
                return text;
            }
        }

        private static string ProcessParagraphs(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string result = text;
            result = Regex.Replace(result, @"<p[^>]*>\s*</p>", "\n", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"<p\b[^>]*>", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"</p>", "\n", RegexOptions.IgnoreCase);
            result = result.Replace("</p>", "\n");
            result = result.Replace("</P>", "\n");
            result = result.Replace("<p>", "");
            result = result.Replace("<P>", "");

            Debug.Log($"处理段落后: {result.Replace("\n", "\\n")}"); 

            return result;
        }

        private static string ProcessHeadings(string text)
        {
            string result = text;

            result = Regex.Replace(result, @"<h1[^>]*>", "<size=24><b>", RegexOptions.IgnoreCase);
            result = result.Replace("</h1>", "</b></size>\n");

            result = Regex.Replace(result, @"<h2[^>]*>", "<size=20><b>", RegexOptions.IgnoreCase);
            result = result.Replace("</h2>", "</b></size>\n");

            result = Regex.Replace(result, @"<h3[^>]*>", "<size=18><b>", RegexOptions.IgnoreCase);
            result = result.Replace("</h3>", "</b></size>\n");

            result = Regex.Replace(result, @"<h4[^>]*>", "<size=16><b>", RegexOptions.IgnoreCase);
            result = result.Replace("</h4>", "</b></size>\n");

            return result;
        }

        private static string ProcessLists(string text)
        {
            string result = text;
            result = result.Replace("<ul>", "");
            result = result.Replace("</ul>", "");
            result = result.Replace("<ol>", "");
            result = result.Replace("</ol>", "");
            result = Regex.Replace(result, @"<li[^>]*>(?!.*?<link)(.*?)</li>", "• $1\n", RegexOptions.IgnoreCase);

            return result;
        }

        private static string ProcessBlockquotes(string text)
        {
            string result = text;

            result = result.Replace("<blockquote>", "<color=#888888><i>");
            result = result.Replace("</blockquote>", "</i></color>\n");

            return result;
        }

        private static string ProcessInlineStyles(string text)
        {
            string result = text;

            result = result.Replace("<strong>", "<b>");
            result = result.Replace("</strong>", "</b>");
            result = result.Replace("<b>", "<b>");
            result = result.Replace("</b>", "</b>");

            result = result.Replace("<em>", "<i>");
            result = result.Replace("</em>", "</i>");
            result = result.Replace("<i>", "<i>");
            result = result.Replace("</i>", "</i>");

            result = result.Replace("<u>", "<u>");
            result = result.Replace("</u>", "</u>");

            result = result.Replace("<s>", "<s>");
            result = result.Replace("</s>", "</s>");
            result = result.Replace("<strike>", "<s>");
            result = result.Replace("</strike>", "</s>");

            return result;
        }

        private static string ProcessLineBreaks(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string result = text;

            Debug.Log($"处理换行前: {result.Replace("\n", "\\n")}");
            result = Regex.Replace(result, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
            result = result.Replace("\r\n", "\n");
            result = result.Replace("\r", "\n");

            Debug.Log($"处理换行后: {result.Replace("\n", "\\n")}");

            return result;
        }

        private static string RemoveRemainingHtmlTags(string text)
        {
            string pattern = @"<(?!/?(b|i|u|s|size|color|sup|sub|link=|link\b))[^>]+>";

            string result = Regex.Replace(text, pattern, string.Empty);
            result = Regex.Replace(result, @"\s+", " ");

            return result.Trim();
        }

        private static string CleanupFormatting(string text)
        {
            string result = text;
            result = FixUnclosedTags(result);
            result = Regex.Replace(result, @"\n{3,}", "\n\n");
            result = Regex.Replace(result, @"^\s+|\s+$", "", RegexOptions.Multiline);
            result = result.Trim();
            result = FixLinkTags(result);

            return result;
        }
        private static string FixLinkTags(string text)
        {
            try
            {
                int openLinks = Regex.Matches(text, @"<link=[^>]+>").Count;
                int closeLinks = Regex.Matches(text, @"</link>").Count;

                if (openLinks > closeLinks)
                {
                    for (int i = 0; i < openLinks - closeLinks; i++)
                    {
                        text += "</link>";
                    }
                }

                return text;
            }
            catch (Exception e)
            {
                Debug.LogError($"修复链接标签失败: {e.Message}");
                return text;
            }
        }

        private static string FixUnclosedTags(string text)
        {
            try
            {
                string[] tagsToCheck = { "b", "i", "u", "s", "size", "color", "link" };

                foreach (string tag in tagsToCheck)
                {
                    int openCount = Regex.Matches(text, $"<{tag}[^>]*>").Count;
                    int closeCount = Regex.Matches(text, $"</{tag}>").Count;

                    if (openCount > closeCount)
                    {
                        for (int i = 0; i < openCount - closeCount; i++)
                        {
                            text += $"</{tag}>";
                        }
                    }
                }

                return text;
            }
            catch (Exception e)
            {
                Debug.LogError($"修复未闭合标签失败: {e.Message}");
                return text;
            }
        }
        public static string ConvertApiExample(string htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent))
                return string.Empty;

            try
            {
                string result = Convert(htmlContent);
                result = Regex.Replace(result, @"^\s*$\n", "", RegexOptions.Multiline);
                result = Regex.Replace(result, @"\n{3,}", "\n\n");

                return result.Trim();
            }
            catch (Exception e)
            {
                Debug.LogError($"API示例HTML转换失败: {e.Message}");
                return htmlContent;
            }
        }

        public static string ConvertForJsonExample(string htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent))
                return string.Empty;

            try
            {
                string result = htmlContent;

                result = DecodeHtmlEntities(result);
                result = Regex.Replace(result, @"<p\b[^>]*>", "", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"</p>", "\n\n", RegexOptions.IgnoreCase);
                result = ProcessHeadings(result);
                result = ProcessLists(result);
                result = ProcessBlockquotes(result);
                result = ProcessInlineStyles(result);
                result = ProcessLineBreaks(result);
                result = ConvertSpecialLinksDirectly(result);
                result = RemoveRemainingHtmlTags(result);
                result = CleanupFormatting(result);
                result = FixLineBreaks(result);

                Debug.Log($"JSON示例转换完成: {result}");
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"JSON示例转换失败: {e.Message}");
                return htmlContent;
            }
        }
    }
}
