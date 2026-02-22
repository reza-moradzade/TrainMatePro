using System.Text.Json.Serialization;

public class ExerciseItem
{
    [JsonPropertyName("exerciseId")]  // اینو اضافه کن
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