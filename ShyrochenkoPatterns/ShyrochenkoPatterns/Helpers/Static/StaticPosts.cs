using ShyrochenkoPatterns.Domain.Entities.Post;
using ShyrochenkoPatterns.Models.Post;

namespace ShyrochenkoPatterns.Services.Static
{
    public static class StaticPosts
    {
        public static IPost Poem = new Poem();

        public static IPost Story = new Story();

        public static IPost Proverb = new Proverb();
    }
}
