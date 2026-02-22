using System.Text.Json.Serialization;

namespace TrainMatePro.DTOs.ExerciseData
{
    public class ExerciseDto
    {
        [JsonPropertyName("exerciseId")]
        public string ExerciseId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("gifUrl")]
        public string GifUrl { get; set; } = string.Empty;

        [JsonPropertyName("targetMuscles")]
        public string[] TargetMuscles { get; set; } = Array.Empty<string>();

        [JsonPropertyName("bodyParts")]
        public string[] BodyParts { get; set; } = Array.Empty<string>();

        [JsonPropertyName("equipments")]
        public string[] Equipments { get; set; } = Array.Empty<string>();

        [JsonPropertyName("secondaryMuscles")]
        public string[] SecondaryMuscles { get; set; } = Array.Empty<string>();

        [JsonPropertyName("instructions")]
        public string[] Instructions { get; set; } = Array.Empty<string>();
    }

    public class BodyPartDto
    {
        public string Name { get; set; } = string.Empty;
        public int ExerciseCount { get; set; }
    }

    public class EquipmentDto
    {
        public string Name { get; set; } = string.Empty;
        public int ExerciseCount { get; set; }
    }
}