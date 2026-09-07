using System.Text.Json;
using Sc2Timer.Core.Models;

namespace Sc2Timer.Core;

public static class BuildOrderLoader
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static BuildOrderFile LoadFromFile(string path) => LoadFromJson(File.ReadAllText(path));

    public static BuildOrderFile LoadFromJson(string json)
    {
        BuildOrderFile? file;
        try
        {
            file = JsonSerializer.Deserialize<BuildOrderFile>(json, Options);
        }
        catch (JsonException ex)
        {
            throw new InvalidBuildOrderException($"Build order is not valid JSON: {ex.Message}");
        }

        if (file is null)
        {
            throw new InvalidBuildOrderException("Build order file is empty.");
        }

        if (string.IsNullOrWhiteSpace(file.Name))
        {
            throw new InvalidBuildOrderException("Build order is missing a 'name'.");
        }

        if (file.Events.Count == 0)
        {
            throw new InvalidBuildOrderException("Build order has no events.");
        }

        return file;
    }
}
