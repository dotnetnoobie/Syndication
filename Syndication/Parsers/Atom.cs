using Syndication.Models;
using System;
using System.Linq;
using System.Xml.Linq;

namespace Syndication.Parsers
{
    public class Atom
    {
        public Channel Parse(XDocument document)
        {
            var channel = new Channel();
             
            channel.Title = document.Root?.GetLocalName("title");
            channel.Description = document.Root?.GetLocalName("description")?.HtmlToPlainText();
            channel.Copyright = document.Root?.GetLocalName("rights");
            channel.Link = document.Root?.GetLocalName("link");
            channel.Image = document.Root?.GetLocalName("image");
            channel.PublishDate = document.Root?.GetLocalName("updated")?.ParseDateTimeOffset();

            var entries = document.Root?.Elements().Where(i => i.Name.LocalName == "entry");
 
            foreach (var entry in entries)
            {
                var item = new Item();
                item.Id = entry.GetLocalName("id");
                item.Title = entry.GetLocalName("title")?.HtmlDecode();

                item.Summary = entry.GetLocalName("summary")?.HtmlToPlainText();
                item.Content = entry.GetLocalName("content")?.HtmlToPlainText();

                item.Link = entry.Elements().FirstOrDefault(i => i.Name.LocalName == "link" && i.Attribute("rel")?.Value == "alternate")?.Attribute("href")?.Value;

                item.PublishDate = (entry.GetLocalName("updated") ?? entry.GetLocalName("published"))?.ParseDateTimeOffset();

                item.Enclosure = GetEnclosure(entry);
                item.Thumbnail = GetThumbnail(entry, item.Enclosure);

                channel.Items.Add(item);
            } 

            return channel;
        }

        private Enclosure GetEnclosure(XElement element)
        {
            var enclosure = element.Elements().FirstOrDefault(i => i.Name.LocalName == "link" && i.Attribute("rel")?.Value == "enclosure");
            if (enclosure == null) return null;

            Enclosure Enclosure = new Enclosure();
            Enclosure = new Enclosure();
            Enclosure.Url = enclosure.Attribute("href")?.Value;
            Enclosure.Type = enclosure.Attribute("type")?.Value;
            Enclosure.Length = enclosure.Attribute("length")?.Value != null ? Convert.ToInt64(enclosure.Attribute("length")?.Value) : 0;

            return Enclosure;
        }

        private string GetThumbnail(XElement element, Enclosure enclosure)
        {
            XNamespace media = "http://search.yahoo.com/mrss/";

            var mediaGroup = element.Elements(media + "group").ToList();

            string img = mediaGroup.Elements(media + "thumbnail").FirstOrDefault()?.Attribute("url")?.Value;

            if (img.IsNotNullOrEmpty()) return img;

            if (enclosure != null && enclosure.Type.Contains("image"))
            {
                return enclosure.Url;
            }

            return null;
        }
    }
}