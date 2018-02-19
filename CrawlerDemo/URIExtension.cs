using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerDemo
{
    public static class URIExtension
    {
        // Scrapes html pages from given Uri
        // Usage: this.Literal1.Text = new System.Uri("https://www.amazon.com/gp/goldbox/ref=nav_cs_gb").Scrape();
        public static string Scrape(this System.Uri uri)
        {
            using (var sr = new System.IO.StreamReader(System.Net.HttpWebRequest.Create(uri.GetLeftPart(System.UriPartial.Query)).GetResponse().GetResponseStream()))
            {
                return sr.ReadToEnd();

            }

        }
    }
}
