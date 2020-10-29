using Syndication.Models;

namespace Syndication.Normalizers
{
    public class DefaultNormalizer : INormalizer
    {
        public Item Normalize(Item item) => item;
    }
}