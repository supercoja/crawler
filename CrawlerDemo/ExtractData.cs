using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CrawlerDemo
{
    public static class ParseURL
    {
        public static bool isValidURL(this string _urlToValidate)
        {
            if ((_urlToValidate.ToUpper().StartsWith("WWW") || _urlToValidate.ToUpper().StartsWith("HTTP")) && ((!_urlToValidate.ToUpper().EndsWith(".PNG")) && (!_urlToValidate.ToUpper().EndsWith(".JPG"))))
                return true;
            else
                return false;
        }
        public static bool isValidImage(this string _urlToValidate)
        {
            if (((_urlToValidate.ToUpper().EndsWith(".PNG")) || (_urlToValidate.ToUpper().EndsWith(".JPG")) || (_urlToValidate.ToUpper().EndsWith(".GIF"))) && ((_urlToValidate.ToUpper().StartsWith("WWW") || _urlToValidate.ToUpper().StartsWith("HTTP"))))
                return true;
            else
                return false;
        }

    }

    public static class ExtractData
    {
        public static List<String> ExtractValidPhones(string textToScrape)
        {
            Regex regExpPhoneWithTrace = new Regex(@"\((\d{2})\)\s*(\d{4,5}\-\d{4})", RegexOptions.IgnoreCase);
            Regex regExpPhoneWithoutSpaceAndTrace = new Regex(@"\((\d{2})\)\s*(\d{4,5}\d{4})", RegexOptions.IgnoreCase);
            Regex regExpPhoneWithPoint = new Regex(@"\((\d{2})\)\s*(\d{4,5}\.\d{4})", RegexOptions.IgnoreCase);

            Match match;

            List<string> results = new List<string>();
            for (match = regExpPhoneWithTrace.Match(textToScrape); match.Success; match = match.NextMatch())
            {
                if (!(results.Contains(match.Value)))
                {
                    results.Add(match.Value);
                    break;
                }
            }
            for (match = regExpPhoneWithoutSpaceAndTrace.Match(textToScrape); match.Success; match = match.NextMatch())
            {
                if (!(results.Contains(match.Value)))
                {
                    results.Add(match.Value);
                    break;
                }

            }
            for (match = regExpPhoneWithPoint.Match(textToScrape); match.Success; match = match.NextMatch())
            {
                if (!(results.Contains(match.Value)))
                {
                    results.Add(match.Value);
                    break;
                }
            }

            return results;
        }

        public static List<string> ExtractEmails(string textToScrape)
        {
            Regex reg = new Regex(@"[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}", RegexOptions.IgnoreCase);
            Match match;

            List<string> results = new List<string>();
            for (match = reg.Match(textToScrape); match.Success; match = match.NextMatch())
            {
                if (!(results.Contains(match.Value)))
                {
                    results.Add(match.Value);
                    break;
                }
            }

            return results;
        }

        public static List<string> ExtractImages(string Url)
        {
            List<string> imageList = new List<string>();
            string htmlData = Url;

            if (htmlData != string.Empty)
            {
                string imageHtmlCode = "<img";
                string imageSrcCode = @"src=""";

                int index = htmlData.IndexOf(imageHtmlCode);
                while (index != -1)
                {
                    htmlData = htmlData.Substring(index);
                    int brackedEnd = htmlData.IndexOf('>');
                    int start = htmlData.IndexOf(imageSrcCode) + imageSrcCode.Length;
                    int end = htmlData.IndexOf('"', start + 1);
                    if (end > start && start < brackedEnd)
                    {
                        string loc = htmlData.Substring(start, end - start);
                        if (loc.isValidImage())
                        {
                            imageList.Add(loc);
                            break;
                        }
                    }
                    if (imageHtmlCode.Length < htmlData.Length)
                        index = htmlData.IndexOf(imageHtmlCode, imageHtmlCode.Length);
                    else
                        index = -1;
                }
                for (int i = 0; i < imageList.Count; i++)
                {
                    string img = imageList[i];

                    string baseUrl = GetBaseURL(Url);

                    if ((!img.StartsWith("http://") && !img.StartsWith("https://"))
                        && baseUrl != string.Empty)
                        img = baseUrl + "/" + img.TrimStart('/');

                    imageList[i] = img;
                }
            }
            return imageList;
        }

        public static List<string> ExtractLinks(string Url, string BaseUrl)
        {
            List<string> linkList = new List<string>();
            string htmlData = Url;
            string _baseURL = BaseUrl.Trim();
            if (!_baseURL.Trim().EndsWith(@"/"))
                _baseURL = _baseURL + "@/";

            if (htmlData != string.Empty)
            {
                string linkHtmlCode = "<a";
                string linkHrefCode = @"href=""";

                int index = htmlData.IndexOf(linkHtmlCode);
                while (index != -1)
                {
                    htmlData = htmlData.Substring(index);
                    int brackedEnd = htmlData.IndexOf('>');
                    int start = htmlData.IndexOf(linkHrefCode) + linkHrefCode.Length;
                    int end = htmlData.IndexOf('"', start + 1);
                    if (end > start && start < brackedEnd)
                    {
                        string loc = htmlData.Substring(start, end - start);
                        if ((!linkList.Contains(loc.Trim())) && (!loc.ToUpper().Contains("FACEBOOK")) && (!loc.Trim().Equals(BaseUrl.Trim())) && (loc.Trim().isValidURL()))
                            linkList.Add(loc.Trim());
                    }
                    if (linkHtmlCode.Length < htmlData.Length)
                        index = htmlData.IndexOf(linkHtmlCode, linkHtmlCode.Length);
                    else
                        index = -1;
                }
                for (int i = 0; i < linkList.Count; i++)
                {
                    string img = linkList[i];

                    string baseUrl = GetBaseURL(Url);

                    if ((!img.StartsWith("http://") && !img.StartsWith("https://"))
                        && baseUrl != string.Empty)
                        img = baseUrl + "/" + img.TrimStart('/');

                    linkList[i] = img;
                }
            }
            return linkList;
        }

        private static string GetBaseURL(string Url)
        {
            int inx = Url.IndexOf("://") + "://".Length;
            int end = Url.IndexOf('/', inx);

            string baseUrl = string.Empty;
            if (end != -1)
                return Url.Substring(0, end);
            else
                return string.Empty;
        }

    }
}
