using System.Text.Json.Serialization;

namespace ContreJour.Gameplay
{
    // Source-generated (reflection-free, AOT-safe) serializer for the save file. Computed get-only
    // properties such as TotalStars are derived from the saved data, so they aren't written.
    [JsonSourceGenerationOptions(WriteIndented = true, IgnoreReadOnlyProperties = true)]
    [JsonSerializable(typeof(UserData))]
    internal sealed partial class UserDataJsonContext : JsonSerializerContext
    {
    }
}
