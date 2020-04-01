namespace ShyrochenkoPatterns.Models.Post.PostTypes
{
    public interface IStory : IPost
    {
        int SeriesId { get; set; }
    }
}
