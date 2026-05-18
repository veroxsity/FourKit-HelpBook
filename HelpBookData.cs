using System.Text.Json;

namespace HelpBook;

/// <summary>
/// Loads and queries the help entries from
/// <c>plugins/HelpBook-data/help.json</c>. The file is the single source
/// of truth for what /help shows; no entries are baked into the plugin.
///
/// On first run, if the file doesn't exist, an empty <c>[]</c> stub is
/// written so the path is discoverable. Add entries by editing that file
/// and either restarting the server or running <c>/help reload</c> from
/// the server console.
///
/// File schema is a JSON array of objects, each with command / description
/// / usage strings:
/// <code>
/// [
///   { "command": "duel", "description": "...", "usage": "..." },
///   { "command": "we",   "description": "...", "usage": "..." }
/// ]
/// </code>
/// </summary>
public sealed class HelpBookData
{
    public const string DataFolder = "plugins/HelpBook-data";
    public const string FileName   = "help.json";

    public List<HelpEntry> Entries { get; private set; } = new();

    public void load()
    {
        var path = Path.Combine(DataFolder, FileName);
        if (!File.Exists(path))
            writeEmptyStub(path);

        try
        {
            using var stream = File.OpenRead(path);
            var list = JsonSerializer.Deserialize<List<HelpEntry>>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            Entries = list ?? new List<HelpEntry>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("[HelpBook] failed to load help.json: " + ex.Message);
            Entries = new List<HelpEntry>();
        }
    }

    public HelpEntry? findByCommand(string name)
    {
        foreach (var e in Entries)
            if (string.Equals(e.Command, name, StringComparison.OrdinalIgnoreCase))
                return e;
        return null;
    }

    /// <summary>
    /// Writes a minimal empty-array stub so the file exists on disk and
    /// users can find the path to edit. Deliberately NOT pre-populated -
    /// the JSON file is the source of truth for entries, not this code.
    /// </summary>
    private static void writeEmptyStub(string path)
    {
        try
        {
            Directory.CreateDirectory(DataFolder);
            File.WriteAllText(path, "[]\n");
            Console.WriteLine("[HelpBook] created empty help.json at " + path
                + " - add entries to populate /help.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[HelpBook] failed to create help.json stub: " + ex.Message);
        }
    }
}
