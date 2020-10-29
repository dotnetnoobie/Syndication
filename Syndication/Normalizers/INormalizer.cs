using Syndication.Models;

namespace Syndication.Normalizers
{
    public interface INormalizer
    {
        Item Normalize(Item item);
    }
}
