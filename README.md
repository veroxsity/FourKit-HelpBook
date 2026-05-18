# FourKit-HelpBook

Simple configurable help system for FourKit servers. Players run `/help` to get a list of topics; `/help <topic>` to read the details.

## Installation

```powershell
.\build.ps1 -StopServer
```

## Commands

- `/help` - list all configured help topics
- `/help <topic>` - show the lines configured for that topic

## Configuration

`plugins/HelpBook-data/entries.json` (auto-generated on first run with example entries):

```json
{
  "Entries": [
    {
      "Topic": "rules",
      "Lines": [
        "No griefing",
        "No hacks",
        "Have fun"
      ]
    },
    {
      "Topic": "commands",
      "Lines": [
        "/duel <player> <kit> - challenge someone",
        "/link - link your Discord account"
      ]
    }
  ]
}
```

Topics are matched case-insensitively. Lines are sent to the player verbatim, so Bukkit-style color codes (`&a`, `&c`, etc.) work.

## License

MIT
