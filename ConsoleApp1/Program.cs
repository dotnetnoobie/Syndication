using System;
using System.Net.Http;
using Syndication;

namespace ConsoleApp1
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var client = new SyndicationClient(new HttpClient());

            var data = await client.Load("https://infowars.com/feed/custom_feed_rss");

            foreach(var item in data.Items)
            {
                Console.WriteLine(item.Title.GenerateSlug(50));
            }

            Console.ReadLine( );
        }
    }
}
