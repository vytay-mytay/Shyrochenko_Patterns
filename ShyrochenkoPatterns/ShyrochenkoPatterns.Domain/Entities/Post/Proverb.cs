using ShyrochenkoPatterns.Models.Post.PostTypes;
using System;

namespace ShyrochenkoPatterns.Domain.Entities.Post
{
    public class Proverb : IProverb
    {
        public int Id { get; set; }

        public int ImageId { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
