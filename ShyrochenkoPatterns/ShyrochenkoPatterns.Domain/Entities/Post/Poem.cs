using ShyrochenkoPatterns.Models.Post;
using ShyrochenkoPatterns.Models.Post.PostTypes;
using System;

namespace ShyrochenkoPatterns.Domain.Entities.Post
{
    public class Poem : IPoem
    {
        public int Id { get; set; }

        public string Synopsis { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime CreationDate { get; set; }

        public IPost Clone()
        {
            return new Poem();
        }
    }
}
