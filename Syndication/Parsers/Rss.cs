using Syndication.Models;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Syndication.Parsers
{
    public class Rss
    {
        public Channel Parse(XDocument document)
        {
            //XNamespace dc = "http://purl.org/rss/1.0/modules/content/";
            //XNamespace media = "http://search.yahoo.com/mrss/";
            //XNamespace itunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";
            XNamespace content = "http://purl.org/rss/1.0/modules/content/";

            var channel = new Channel();

            var doc = document.Root?.Descendants().First(i => i.Name.LocalName == "channel");
            var entries = doc.Elements().Where(i => i.Name.LocalName == "item");

            channel.Title = doc.GetLocalName("title");
            channel.Description = doc.GetLocalName("description")?.HtmlToPlainText();
            channel.Copyright = doc.GetLocalName("copyright");
            channel.Link = doc.GetLocalName("link");
            channel.Image = doc.GetLocalName("image");
            channel.PublishDate = doc.GetLocalName("updated")?.ParseDateTimeOffset();
             
            foreach (var entry in entries)
            {
                var item = new Item();

                item.Id = entry.GetLocalName("guid");
                item.Title = entry.GetLocalName("title")?.HtmlDecode();
                item.Link = entry.GetLocalName("link");
                item.Source = entry.GetLocalName("source", "url");
                item.PublishDate = entry.GetLocalName("pubDate")?.ParseDateTimeOffset();
                item.Summary = entry.GetLocalName("description")?.HtmlToPlainText();
                item.Content = entry.Element(content + "encoded")?.Value?.HtmlToPlainText();
                item.Categories = entry.Elements("category").Select(c => c.Value).Distinct();
                item.Comments = entry.Elements("comments").Select(c => c.Value).Distinct();

                item.Enclosure = GetEnclosure(entry);
                item.Thumbnail = GetThumbnail(entry, item.Enclosure);

                channel.Items.Add(item);
            }

            return channel;
        }

        private Enclosure GetEnclosure(XElement element)
        {
            var enclosure = element.Elements().FirstOrDefault(i => i.Name.LocalName == "enclosure");

            if (enclosure == null) return null;

            Enclosure Enclosure = new Enclosure();
            Enclosure.Url = enclosure.GetAttribute("url");
            Enclosure.Type = enclosure.GetAttribute("type");

            var len = enclosure.GetAttribute("length");
            if (!string.IsNullOrEmpty(len))
            {
                try
                {
                    Enclosure.Length = Convert.ToInt64(len);
                }
                catch (Exception)
                {
                    Enclosure.Length = -1;
                }
            }

            return Enclosure;
        }

        private string GetThumbnail(XElement element, Enclosure enclosure)
        {
            XNamespace media = "http://search.yahoo.com/mrss/";

            var img = element.Elements(media + "thumbnail").FirstOrDefault()?.Attribute("url")?.Value;

            if (img.IsNotNullOrEmpty()) return img;

            if (enclosure != null && enclosure.Type.Contains("image"))
            {
                return enclosure.Url;
            }

            return null;
        }
    }
}
