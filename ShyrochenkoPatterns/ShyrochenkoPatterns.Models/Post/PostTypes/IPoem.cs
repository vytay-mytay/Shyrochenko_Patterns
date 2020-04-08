namespace ShyrochenkoPatterns.Models.Post.PostTypes
{
    public interface IPoem : IPost
    {
        string Synopsis { get; set; }
    }
}
