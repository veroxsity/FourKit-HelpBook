using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Command;
using Minecraft.Server.FourKit.Entity;

namespace HelpBook.Commands;

public sealed class HelpCommand : CommandExecutor
{
    /// <summary>Top-level entries shown per page. User asked for 5.</summary>
    private const int PerPage = 5;

    private readonly HelpBookData _data;

    public HelpCommand(HelpBookData data) { _data = data; }

    public bool onCommand(CommandSender sender, Command command, string label, string[] args)
    {
        if (args.Length == 0)
        {
            showPage(sender, 1);
            return true;
        }

        var arg = args[0];

        // Console-only reload. Bonus utility so editing help.json doesn't
        // require a full server restart. Players hit a hard no.
        if (string.Equals(arg, "reload", StringComparison.OrdinalIgnoreCase))
        {
            if (sender is Player)
            {
                sender.sendMessage("§c/help reload can only be run from the server console.");
                return true;
            }
            _data.load();
            sender.sendMessage("Reloaded help.json: " + _data.Entries.Count + " top-level entries.");
            return true;
        }

        // Bare page number: /help 2
        if (args.Length == 1 && int.TryParse(arg, out var page))
        {
            showPage(sender, page);
            return true;
        }

        // Otherwise resolve as a command path: /help duel  or  /help duel accept
        var top = _data.findByCommand(arg);
        if (top == null)
        {
            sender.sendMessage("§cCommand '" + arg + "' not found. Type §f/help§c for the list.");
            return true;
        }

        // Walk into subcommands one arg at a time so /help duel accept etc.
        // resolves to the leaf entry. Any miss in the middle reports the
        // partial path plus the available siblings at that level.
        var entry = top;
        var path  = new List<string> { entry.Command };

        for (int i = 1; i < args.Length; i++)
        {
            var next = entry.findSubcommand(args[i]);
            if (next == null)
            {
                string pathStr = "/" + string.Join(" ", path);
                sender.sendMessage("§cSubcommand '" + args[i] + "' not found under §f" + pathStr + ".");
                if (entry.Subcommands.Count > 0)
                {
                    var names = new List<string>();
                    foreach (var s in entry.Subcommands) names.Add(s.Command);
                    sender.sendMessage("§7Available: §f" + string.Join(", ", names));
                }
                return true;
            }
            entry = next;
            path.Add(entry.Command);
        }

        showEntry(sender, entry, path);
        return true;
    }

    private void showPage(CommandSender sender, int page)
    {
        var entries = _data.Entries;
        if (entries.Count == 0)
        {
            sender.sendMessage("§7No help entries configured yet. Edit §f"
                + Path.Combine(HelpBookData.DataFolder, HelpBookData.FileName)
                + "§7 to add some.");
            return;
        }

        int totalPages = Math.Max(1, (int)Math.Ceiling(entries.Count / (double)PerPage));
        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        int start = (page - 1) * PerPage;
        int end   = Math.Min(start + PerPage, entries.Count);

        sender.sendMessage("§6[Help] §7Page §f" + page + " of " + totalPages + ":");
        for (int i = start; i < end; i++)
        {
            var e = entries[i];
            sender.sendMessage("/" + e.Command + " - " + e.Description);
        }

        if (totalPages > 1)
            sender.sendMessage("§8Type §f/help <page>§8 for more, or §f/help <command>§8 for details.");
        else
            sender.sendMessage("§8Type §f/help <command>§8 for details.");
    }

    private void showEntry(CommandSender sender, HelpEntry e, List<string> path)
    {
        string fullPath = "/" + string.Join(" ", path);

        sender.sendMessage("[Help] " + fullPath);
        if (!string.IsNullOrEmpty(e.Description))
            sender.sendMessage("§7  " + e.Description);
        if (!string.IsNullOrEmpty(e.Usage))
            sender.sendMessage("§7Usage: §f" + e.Usage);

        if (e.Subcommands.Count > 0)
        {
            sender.sendMessage("§7Subcommands:");
            foreach (var s in e.Subcommands)
                sender.sendMessage("- " + fullPath + " " + s.Command + " - " + s.Description);

            // Drill-down hint. fullPath already starts with '/' so strip it
            // for the type-this-next string.
            string hint = fullPath.Length > 1 ? fullPath.Substring(1) : "";
            sender.sendMessage("Type /help " + hint + " <subcommand>§8 for details.");
        }
    }
}
