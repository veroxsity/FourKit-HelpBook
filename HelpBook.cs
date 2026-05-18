using Minecraft.Server.FourKit;
using Minecraft.Server.FourKit.Plugin;

using HelpBook.Commands;

namespace HelpBook;

public class HelpBook : ServerPlugin
{
    public override string name    => "HelpBook";
    public override string version => "0.1.0";
    public override string author  => "BanditVault";

    public override void onEnable()
    {
        var data = new HelpBookData();
        data.load();

        FourKit.getCommand("help").setExecutor(new HelpCommand(data));
        FourKit.getCommand("help").setDescription("Show help pages and command details");
        FourKit.getCommand("help").setUsage("/help [page] | /help <command>");

        Console.WriteLine("[HelpBook] enabled with " + data.Entries.Count + " entries.");
    }

    public override void onDisable()
    {
        Console.WriteLine("[HelpBook] disabled.");
    }
}
