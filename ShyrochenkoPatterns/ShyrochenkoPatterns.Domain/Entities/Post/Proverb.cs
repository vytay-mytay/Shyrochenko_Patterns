using ShyrochenkoPatterns.Models.Post;
using ShyrochenkoPatterns.Models.Post.PostTypes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Domain.Entities.Post
{
    public class Proverb : IProverb, IEntity
    {
        [Key]
        public int Id { get; set; }

        public int ImageId { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime CreationDate { get; set; }

        public Proverb()
        { }

        public Proverb(int imageId, string title, string text, DateTime creationDate)
        {
            ImageId = imageId;
            Title = title;
            Text = text;
            CreationDate = creationDate;
        }


        public IPost Clone()
        {
            return new Proverb(ImageId, Title, Text, CreationDate);
        }
    }
}
