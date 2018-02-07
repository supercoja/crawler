using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrawlerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            startCrawlerasync();
            Console.WriteLine("Fim do Crawler/Scrap");
            Console.ReadLine();
        }

        private static async Task startCrawlerasync()
        {
            var _url = "http://www.byebyepaper.com.br";
            Console.WriteLine($"URL Pesquisada: { _url }");
            Console.WriteLine();
            Console.WriteLine();
//            if (!_url.StartsWith("http://") && !_url.StartsWith("https://"))
//              _url = "http://" + _url;

            string responseUrl = string.Empty;
            //            string htmlData = ASCIIEncoding.ASCII.GetString(DownloadData(_url, out responseUrl));
            WebRequest request = WebRequest.Create(_url.Trim());
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string htmlData = reader.ReadToEnd();

            if (responseUrl != string.Empty)
                _url = responseUrl;

            var _resultPhones = ExtractPhones(htmlData);

            Console.WriteLine($"Encontrou? { _resultPhones.Count } Telefone(s)");
            foreach (var _item in _resultPhones)
            {
                Console.WriteLine($"Telefone encontrado: {_item.ToString()}");
            }

            var _resultEmails = ExtractEmails(htmlData);

            Console.WriteLine($"Encontrou? { _resultEmails.Count } Email(s)");
            foreach (var _item in _resultEmails)
            {
                Console.WriteLine($"Email encontrado: {_item.ToString()}");
            }

            var _resultImages = ExtractImages(htmlData);

            Console.WriteLine($"Encontrou? { _resultImages.Count } Imagem(ns)");
            foreach (var _item in _resultImages)
            {
                Console.WriteLine($"Imagem encontrada: {_item.ToString()}");
            }

            var _resultLinks = ExtractLinksII(htmlData,_url);

            Console.WriteLine($"Encontrou? { _resultLinks.Count } Imagem(ns)");
            foreach (var _item in _resultLinks)
            {
                Console.WriteLine($"Link encontrado: {_item.ToString()}");
            }

            Console.WriteLine();
            Console.WriteLine();

            var _saveFileWithoutHTMLTags = HtmlRemoval.StripTagsRegexCompiled(htmlData);

            Console.WriteLine($@"Arquivo Salvo em: {Environment.CurrentDirectory}\WriteText.txt");

            File.WriteAllText($@"{Environment.CurrentDirectory}\WriteText.txt", _saveFileWithoutHTMLTags.Trim());
            //_saveFileWithoutHTMLTags
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Fim do Scrapping....");
            Console.WriteLine("Pressione Enter para sair do programa...");
            ConsoleKeyInfo keyinfor = Console.ReadKey(true);
            if(keyinfor.Key == ConsoleKey.Enter)
            {
                System.Environment.Exit(0);
            }
        }

        private static byte[] DownloadData(string Url)
        {
            string empty = string.Empty;
            return DownloadData(Url, out empty);
        }

        private static byte[] DownloadData(string Url, out string responseUrl)
        {
            byte[] downloadedData = new byte[0];
            try
            {
                WebRequest req = WebRequest.Create(Url);
                WebResponse response = req.GetResponse();
                Stream stream = response.GetResponseStream();

                responseUrl = response.ResponseUri.ToString();

                byte[] buffer = new byte[1024];

                MemoryStream memStream = new MemoryStream();
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        break;
                    }
                    else
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }

                downloadedData = memStream.ToArray();

                stream.Close();
                memStream.Close();
            }
            catch (Exception)
            {
                responseUrl = string.Empty;
                return new byte[0];
            }

            return downloadedData;
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

        private static List<string> ExtractImages(string Url)
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
                        imageList.Add(loc);
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

        private static List<string> ExtractLinksII(string Url, string BaseUrl)
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
                        if ((!linkList.Contains(loc.Trim())) && (!linkList.ToString().ToUpper().Contains("FACEBOOK")) && (!linkList.ToString().Trim().Equals(BaseUrl.Trim())))
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

        public static List<string> ExtractPhones(string textToScrape)
        {
            Regex regExpPhoneWithTrace = new Regex(@"\((\d{2})\)\s*(\d{4,5}\-\d{4})", RegexOptions.IgnoreCase);
            Regex regExpPhoneWithoutSpaceAndTrace = new Regex(@"\((\d{2})\)\s*(\d{4,5}\d{4})", RegexOptions.IgnoreCase);
            Regex regExpWorldWidePhone = new Regex(@"\+\d{0,2}\((\d{2})\)\s*(\d{4,5}\d{4})", RegexOptions.IgnoreCase);

            Match match;

            List<string> results = new List<string>();
            for (match = regExpPhoneWithTrace.Match(textToScrape); match.Success; match = match.NextMatch())
            {
                if (!(results.Contains(match.Value)))
                    results.Add(match.Value);
            }
            for (match = regExpPhoneWithoutSpaceAndTrace.Match(textToScrape); match.Success; match = match.NextMatch())
            {
                if (!(results.Contains(match.Value)))
                    results.Add(match.Value);
            }
            for (match = regExpWorldWidePhone.Match(textToScrape); match.Success; match = match.NextMatch())
            {
                if (!(results.Contains(match.Value)))
                    results.Add(match.Value);
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
                    results.Add(match.Value);
            }

            return results;
        }

        private static List<string> ExtractLinks(string input, string baseUrl)
        {
            var links = new List<string>();
            Regex regex = new Regex("a href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
            Match match;
            for (match = regex.Match(input); match.Success; match = match.NextMatch())
            {
                foreach (Group group in match.Groups)
                {
                    if (!(group.Value.Contains(baseUrl.Trim())) && (!(links.Contains(match.Value))))
                        links.Add(group.Value);
                }
            }
            return links;
        }

    }
}
