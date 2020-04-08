using System;

namespace ShyrochenkoPatterns.Models.Post
{
    public interface IPost
    {
        string Title { get; set; }

        string Text { get; set; }

        DateTime CreationDate { get; set; }
    }
}
