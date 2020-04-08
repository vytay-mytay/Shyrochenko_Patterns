namespace ShyrochenkoPatterns.Models
{
    public class Likes
    {
        public int UserId { get; set; }
        public int PostId { get; set; }

        public Likes(int UserId, int PostId)
        {
            this.PostId = PostId;
            this.UserId = UserId;
        }
    }
}
