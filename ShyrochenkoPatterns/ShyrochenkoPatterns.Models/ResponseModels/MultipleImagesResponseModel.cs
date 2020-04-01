using Newtonsoft.Json;
using ShyrochenkoPatterns.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class MultipleImagesResponseModel
    {
        public ImageResponseModel Image { get; set; }
                
        public ImageSaveStatus? Status { get; set; }

        public string Name { get; set; }
    }
}
