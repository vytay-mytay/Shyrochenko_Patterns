using ShyrochenkoPatterns.Models.Enums;
using System.Collections.Generic;

namespace ShyrochenkoPatterns.Models.ResponseModels.Composite
{
    public class ComponentResponseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Size { get; set; }

        public ComponentType Type { get; set; }

        public List<ComponentResponseModel> Children { get; set; }

        public ComponentResponseModel()
        {
            Children = new List<ComponentResponseModel>();
        }
    }
}
