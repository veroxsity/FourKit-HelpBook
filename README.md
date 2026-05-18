# FourKit-HelpBook

Lightweight in-game help-text plugin for FourKit servers running Minecraft Legacy Console Edition. Add entries to a JSON file, players list them with `/help` and read individual pages by name.

## Installation

1. Build (see below) or grab the latest `HelpBook.dll` from Releases
2. Drop it into `<server>/plugins/`
3. Restart the server

## Configuration

`plugins/HelpBook-data/entries.json` is generated on first run. Each entry has a name (the lookup key) and a list of body lines. Edit the file in place and run `/help reload` (admin) or restart to pick up changes.

## Commands

| Command | Description |
|---|---|
| `/help` | List all available help entries |
| `/help <name>` | Show the body of a specific entry |
## Building from source

Requires .NET 10 SDK.

```powershell
.\build.ps1 -StopServer
```

The script auto-stops a running `Minecraft.Server.exe`, builds in Release mode, and copies the DLL to `..\..\Server\plugins\`. Or build manually:

```powershell
dotnet build -c Release
```

## License

MIT
