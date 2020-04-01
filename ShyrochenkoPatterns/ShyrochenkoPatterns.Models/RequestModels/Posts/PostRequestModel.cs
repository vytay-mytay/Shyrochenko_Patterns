namespace ShyrochenkoPatterns.Models.RequestModels.Posts
{
    public class PostRequestModel
    {
        public string Title { get; set; }

        public string Text { get; set; }

        // poem
        public string Synopsis { get; set; }


        // story
        public int? SeriesId { get; set; }

        public int? PartNumber { get; set; }


        // proverb
        public int? ImageId { get; set; }
    }
}
