using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class ImageResponseModel
    {
        [Required]
        public int Id { get; set; }

        public string OriginalPath { get; set; }

        public string CompactPath { get; set; }
    }
}
