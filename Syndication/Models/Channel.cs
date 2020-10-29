using System;
using System.Collections.Generic;

namespace Syndication.Models
{
    public class Channel
    {
        public string Link { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Copyright { get; set; }

        public string Image { get; set; }

        public string Content { get; set; }

        public List<Category> Categories { get; set; }

        public List<Item> Items { get; set; }

        public DateTimeOffset? PublishDate { get; set; }

        public Channel()
        {
            this.Categories = new List<Category>();
            this.Items = new List<Item>();
        }
    }
}
