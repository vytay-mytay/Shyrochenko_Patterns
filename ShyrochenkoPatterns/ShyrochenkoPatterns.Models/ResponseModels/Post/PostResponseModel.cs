using ShyrochenkoPatterns.Models.Enums;
using System;

namespace ShyrochenkoPatterns.Models.ResponseModels.Post
{
    public class PostResponseModel
    {
        public int Id { get; set; }

        public PostType Type { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        // poem
        public string Synopsis { get; set; }


        // story
        public int? SeriesId { get; set; }

        public int? PartNumber { get; set; }


        // proverb
        public int? ImageId { get; set; }


        public DateTime CreationDate { get; set; }
    }
}
