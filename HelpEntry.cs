namespace HelpBook;

/// <summary>
/// One node in the help tree. Each entry may carry nested
/// <see cref="Subcommands"/>; the lookup path /help duel accept walks
/// from the top-level "duel" entry into its "accept" subcommand.
///
/// JSON property names match case-insensitively at load time, and
/// missing <c>subcommands</c> deserialises to an empty list so leaf
/// entries don't need to include the field.
/// </summary>
public sealed class HelpEntry
{
    public string Command     { get; set; } = "";
    public string Description { get; set; } = "";
    public string Usage       { get; set; } = "";
    public List<HelpEntry> Subcommands { get; set; } = new();

    public HelpEntry? findSubcommand(string name)
    {
        foreach (var s in Subcommands)
            if (string.Equals(s.Command, name, StringComparison.OrdinalIgnoreCase))
                return s;
        return null;
    }
}
