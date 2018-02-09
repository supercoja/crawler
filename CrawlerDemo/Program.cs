using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            var _urlCollection = new List<string>();
            _urlCollection.Add("http://www.byebyepaper.com.br");
//            _urlCollection.Add("http://atl.clicrbs.com.br/#!/home");


            foreach (string _urlToRead in _urlCollection)
            {
                var _fileURL = ReadURLToFile(_urlToRead);
                Console.WriteLine($"URL Pesquisada: { _urlToRead }");
                var _saveFileWithoutHTMLTags = HtmlRemoval.StripTagsRegexCompiled(_fileURL);
                _saveFileWithoutHTMLTags.Trim();
                var _folderURL = _urlToRead.Trim().Replace("http://www.", "");
                var _destinationFolder = $@"{Environment.CurrentDirectory}\scrapping\{_folderURL}";
                if (!Directory.Exists(_destinationFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(_destinationFolder);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($@"Não Foi Possível Criar o Diretório de Destino: {e.Message}");
                    }
                }
                try
                {
                    SaveToFile(_saveFileWithoutHTMLTags, _destinationFolder, "site.txt");
                    Console.WriteLine($@"Arquivo Salvo em: {_destinationFolder}\site.txt");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Não Foi Possível Salvar o Arquivo: {e.Message}");
                }
                var _siteContentLink = new StringBuilder();
                var _resultLinks = ExtractData.ExtractLinks(_fileURL, _urlToRead);
                foreach (string _linkedLink in _resultLinks)
                {
                        Console.WriteLine($"Lendo SubSite: {_linkedLink}");
                        _siteContentLink.Append(ReadURLToFile(_linkedLink.Trim()));
                }
                startCrawlerasync(_siteContentLink.ToString(), _folderURL);
            }
            Console.WriteLine("Fim do Scrapping....");
            Console.WriteLine("Pressione Enter para sair do programa...");
            ConsoleKeyInfo keyinfor = Console.ReadKey(true);
            if (keyinfor.Key == ConsoleKey.Enter)
            {
                System.Environment.Exit(0);
            }
        }

        private static string ReadURLToFile(string Url)
        {
            WebRequest request = WebRequest.Create(Url.Trim());
            request.Credentials = CredentialCache.DefaultCredentials;
            string htmlData = string.Empty;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                htmlData = reader.ReadToEnd();
            }
            catch (Exception)
            {
                Console.WriteLine($"Não Foi Possível Localizar a URL: {Url.Trim()}");
            }
            return htmlData;
        }

        private static void SaveToFile(string ContentToSave, string FilePathSave, string FileName)
        {
            string _file = $@"{FilePathSave}\{FileName}";
            File.WriteAllText($@"{FilePathSave}\{FileName}", ContentToSave);
        }

        private static async Task startCrawlerasync(string FileUrl, string BaseUrl)
        {
            var _resultPhones =  ExtractData.ExtractValidPhones(FileUrl);
            Console.WriteLine($"Telefones Encontrados: {_resultPhones.Count}");
            foreach (string _item in _resultPhones)
            {
                Console.WriteLine(_item);

            }
            var _resultEmails = ExtractData.ExtractEmails(FileUrl);
            Console.WriteLine($"Emails Encontrados: {_resultEmails.Count}");

            foreach (string _item in _resultEmails)
            {
                Console.WriteLine(_item);
            }
            var _resultImages = ExtractData.ExtractImages(FileUrl);
            Console.WriteLine($"Imagens Encontradas: {_resultImages.Count}");

            foreach (string _item in _resultImages)
            {
                Console.WriteLine(_item);
            }
        }
    }
}
