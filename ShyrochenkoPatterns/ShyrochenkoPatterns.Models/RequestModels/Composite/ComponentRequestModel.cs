using ShyrochenkoPatterns.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Models.RequestModels.Composite
{
    public class ComponentRequestModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int? Size { get; set; }

        [Required]
        public ComponentType Type { get; set; }
    }
}
