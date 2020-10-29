using Syndication.Models;
using Syndication.Normalizers;
using Syndication.Parsers;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Syndication
{
    public class SyndicationClient
    {
        private readonly HttpClient http;
         
        public SyndicationClient(HttpClient http)
        {
            this.http = http;
        }

        public async Task<Channel> Load(string url) => await Load(url, new DefaultNormalizer());

        public async Task<Channel> Load(string url, INormalizer normalizer)
        {
            var xml = await GetXml(url);
            var document = XDocument.Parse(xml, LoadOptions.None);
            var element = document.Root?.Name.LocalName.ToLower();

            switch (element)
            {
                case "rss":
                    {
                        var doc = new Rss();
                        return doc.Parse(document);
                    }
                case "feed":
                    {
                        var doc = new Atom();
                        return doc.Parse(document);
                    }
                default:
                    return null;
            }
        }

        private async Task<string> GetXml(string uri)
        {
            http.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36");

            var response = await http.GetAsync(uri);

            while (response.StatusCode == HttpStatusCode.MovedPermanently)
            {
                response = await http.GetAsync(response.Headers.Location.AbsoluteUri);
            }

            var contentType = response.Content.Headers.First(h => h.Key.Equals("Content-Type"));
            var rawEncoding = contentType.Value.First();

            string xml;
            if (rawEncoding.Contains("utf8") || rawEncoding.Contains("UTF-8"))
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                xml = Encoding.UTF8.GetString(bytes);
            }
            else
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                xml = Encoding.Default.GetString(bytes);
            }

            xml = xml.TrimStart('\r', '\n')
                     .Replace("&", "&amp;")
                     .Replace("&amp;amp;", "&amp;");

            return xml;
        }
    }
}