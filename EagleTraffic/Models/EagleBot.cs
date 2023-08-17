using System.ComponentModel.DataAnnotations;

namespace EagleTraffic.Models
{
    public class EagleBot
    {
        [Required]
        public int? Id { get; set; }
        public decimal Lattitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime DateTimeReported { get; set; }
        public string? RoadName { get; set; }
        public bool FlowDirection { get; set; }
        public decimal FlowRate { get; set; }
        public decimal AvgVehicleSpeed { get; set; }

        public EagleBot()
        {
            RoadName = "";
        }
    }
}
