using System;
using System.Collections.Generic;

namespace Syndication.Models
{
    public class Item
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public string Link { get; set; }

        public string Thumbnail { get; set; }

        public string Source { get; set; }

        public DateTimeOffset? PublishDate { get; set; }

        public Enclosure Enclosure { get; set; }

        public IEnumerable<Author> Authors { get; set; }

        public IEnumerable<string> Categories { get; set; }

        public IEnumerable<string> Comments { get; set; }
    }
}