using ShyrochenkoPatterns.Models.Enums;

namespace ShyrochenkoPatterns.Models.RequestModels
{
    public class VehicleRequestModel
    {
        public string Name { get; set; }

        public VehicleType Type { get; set; }

        public EngineType Engine { get; set; }

        public Transmission Transmission { get; set; }
    }
}
