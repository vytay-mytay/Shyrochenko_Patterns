using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Models.Post;
using ShyrochenkoPatterns.Models.Post.PostTypes;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShyrochenkoPatterns.Domain.Entities.Post
{
    public class Story : IStory, IEntity
    {
        [Key]
        public int Id { get; set; }

        public int SeriesId { get; set; }

        public int PartNumber { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime CreationDate { get; set; }

        public int AuthorId { get; set; }

        [DefaultValue(0)]
        public int LikeCount { get; set; }

        [ForeignKey("AuthorId")]
        [InverseProperty("Stories")]
        public virtual ApplicationUser Author { get; set; }

        public Story()
        { }

        public Story(int seriesId, int partNumber, string title, string text, DateTime creationDate)
        {
            SeriesId = seriesId;
            PartNumber = partNumber;
            Title = title;
            Text = text;
            CreationDate = creationDate;
        }


        public IPost Clone()
        {
            return new Story(SeriesId, PartNumber, Title, Text, CreationDate);
        }
    }
}
