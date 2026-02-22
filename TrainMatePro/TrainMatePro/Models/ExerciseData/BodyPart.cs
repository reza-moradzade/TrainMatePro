using System.Text.Json.Serialization;

namespace TrainMatePro.Models.ExerciseData
{
    public class BodyPart
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}