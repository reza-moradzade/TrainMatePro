using System.Text.Json.Serialization;

namespace TrainMatePro.Models.ExerciseData
{
    public class Muscle
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}