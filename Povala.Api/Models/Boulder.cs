using System.Text.Json.Serialization;
using Povala.Api.Models.Enums;

namespace Povala.Api.Models
{
    public class Boulder
    {
        public string Id { get; set; } = string.Empty;
        public string GymId { get; set; } = string.Empty;
        
        [JsonIgnore] // To avoid circular reference
        public Gym? Gym { get; set; }

        public BoulderDifficulty Difficulty { get; set; } = BoulderDifficulty.V0;
        public BoulderColor Color { get; set; } = BoulderColor.Blue;

        public double CoordX { get; set; }
        public double CoordY { get; set; }
        public bool IsActive { get; set; }
        public DateTime Added { get; set; } = DateTime.UtcNow;
    }
}
