using ShyrochenkoPatterns.Models.Post;
using ShyrochenkoPatterns.Models.Post.PostTypes;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Domain.Entities.Post
{
    public class Poem : IPoem, IEntity
    {
        [Key]
        public int Id { get; set; }

        public string Synopsis { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime CreationDate { get; set; }


        public Poem()
        { }

        public Poem(string synopsis, string title, string text, DateTime creationDate)
        {
            Synopsis = synopsis;
            Title = title;
            Text = text;
            CreationDate = creationDate;
        }


        public IPost Clone()
        {
            return new Poem(Synopsis, Title, Text, CreationDate);
        }
    }
}
