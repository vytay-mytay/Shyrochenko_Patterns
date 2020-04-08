using ShyrochenkoPatterns.Models.Post.PostTypes;
using System;

namespace ShyrochenkoPatterns.Domain.Entities.Post
{
    public class Story : IStory
    {
        public int Id { get; set; }

        public int SeriesId { get; set; }

        public int PartNumber { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
