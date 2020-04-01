namespace ShyrochenkoPatterns.Models.Post.PostTypes
{
    public interface IProverb : IPost
    {
        public int ImageId { get; set; }
    }
}
