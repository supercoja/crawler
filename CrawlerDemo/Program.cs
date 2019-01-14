using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace CrawlerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var _urlCollection = new List<string>();
            var _result = new ResultScrap();
            var _timeStarted = DateTime.Now;
            _urlCollection.Add("https://tecnoblog.net/212622/fim-conta-corrente-digital-gratis/");
            _urlCollection.Add("https://br.investing.com/");
            _urlCollection.Add("https://economia.estadao.com.br/noticias/geral,banrisul-e-novo-alvo-do-banco-do-brasil,180755");

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
                     _siteContentLink.Append(ReadURLToFile(_linkedLink.Trim()));
                    var _resultPhones = ExtractData.ExtractValidPhones(_siteContentLink.ToString());

                    if (_resultPhones.Count > 0)
                        _result.SetPhoneNumber(_resultPhones[0]);
                    var _resultEmails = ExtractData.ExtractEmails(_siteContentLink.ToString());
                    if (_resultEmails.Count > 0)
                    {
                        _result.SetEmail(_resultEmails[0]);
                    }
                    var _resultImages = ExtractData.ExtractImages(_siteContentLink.ToString());
                    if (_resultImages.Count > 0)
                    {
                        _result.SetImage(_resultImages[0]);
                    }

                    if (_result.IsValidResult())
                    {
                        break;
                    }
                }
            }
            var _timeFinished = DateTime.Now;
            var _timeElapsed = _timeFinished.Subtract(_timeStarted);
            Console.WriteLine($"Tempo de Execução {_timeElapsed.TotalSeconds} Segundos");
            Console.WriteLine($"Resultados Encontrados: Telefone: {_result.PhoneNumber} Email: {_result.Email } Imagem: {_result.Image } ");
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

    }
}
