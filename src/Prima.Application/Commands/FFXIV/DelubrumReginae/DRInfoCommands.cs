using Discord.Interactions;
using Prima.DiscordNet;
using Prima.DiscordNet.Attributes;
using Prima.Resources;

namespace Prima.Application.Commands.FFXIV.DelubrumReginae;
[Prima.DiscordNet.Attributes.ModuleScope(Prima.DiscordNet.Attributes.ModuleScopeAttribute.ModuleScoping.Guild, GuildId = Prima.Resources.SpecialGuilds.CrystalExploratoryMissions)]
public class DRInfoCommands : PrimaInteractionModuleBase
{
    private readonly HttpClient _http;

    public DRInfoCommands(HttpClient http)
    {
        _http = http;
    }

    [SlashCommand("drskit", "Run the drskit command.", runMode: RunMode.Async)]
    [Description("Shows the Delubrum Reginae (Savage) loadout guide.")]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public Task DrsKitAsync() =>
        ReplyAsync(
            "https://docs.google.com/spreadsheets/d/11aMSvRlvVkv9ZhDdks19ge1p0HipHOq8RGAIe-N4UxU/edit?usp=sharing");

    [SlashCommand("qg", "Run the qg command.", runMode: RunMode.Async)]
    [Description("Shows the Queen's Guard guide.")]
    [RateLimit(TimeSeconds = 30, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public async Task QgAsync()
    {
        await DiscordUtilities.PostImage(_http, Context, "https://i.imgur.com/vbpph3t.png");
        await Task.Delay(200);
        await DiscordUtilities.PostImage(_http, Context, "https://i.imgur.com/pkdrt09.png");
        await Task.Delay(200);
        await DiscordUtilities.PostImage(_http, Context,
            "https://cdn.discordapp.com/attachments/613149980114550794/891468340541919252/testbomb_20fps.gif");
        await Task.Delay(200);
        await DiscordUtilities.PostImage(_http, Context, "https://i.imgur.com/U2rTaAg.png");
        await Task.Delay(200);
        await DiscordUtilities.PostImage(_http, Context, "https://i.imgur.com/kqaMaEC.png");
        await Task.Delay(200);
        await DiscordUtilities.PostImage(_http, Context, "https://i.imgur.com/c6XyZJd.png");
    }

    [SlashCommand("qgreflect", "Run the qgreflect command.", runMode: RunMode.Async)]
    [Description("Shows Queen's Guard reflect positions.")]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public Task QueensGuardReflectAsync() => DiscordUtilities.PostImage(_http, Context,
        "https://cdn.discordapp.com/attachments/808869784195563521/809107279697150012/robotstemplate2.png");

    [SlashCommand("chess", "Run the chess command.", runMode: RunMode.Async)]
    [Description("Shows Queen Chess strat.")]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public Task QueenChessStratAsync() => DiscordUtilities.PostImage(_http, Context,
        "https://cdn.discordapp.com/attachments/808869784195563521/809107442793185310/nJ4vHiK.png");

    [SlashCommand("fatefulwords", "Run the fatefulwords command.", runMode: RunMode.Async)]
    [Description("Shows the Fateful Words guide.")]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public Task FatefulWordsAsync() => DiscordUtilities.PostImage(_http, Context,
        "https://cdn.discordapp.com/attachments/808869784195563521/813152064342589443/Fateful_Words_debuffs.png");

    [SlashCommand("brands", "Run the brands command.", runMode: RunMode.Async)]
    [Description("Shows the Trinity Avowed debuff guide.")]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public Task BrandsHotColdAsync() => DiscordUtilities.PostImage(_http, Context, "https://i.imgur.com/un5nvg4.png");

    [SlashCommand("slimes", "Run the slimes command.", runMode: RunMode.Async)]
    [Description("Shows the Delubrum Reginae slimes guide.")]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public Task SlimesAsync() => DiscordUtilities.PostImage(_http, Context, "https://i.imgur.com/wUrvKtr.gif");

    [SlashCommand("pipegame", "Run the pipegame command.", runMode: RunMode.Async)]
    [Description("Shows the Trinity Avowed ladder guide.")]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public async Task PipeGameAsync()
    {
        await DiscordUtilities.PostImage(_http, Context, "https://i.imgur.com/i2ms13x.png");
        await DiscordUtilities.PostImage(_http, Context, "https://i.imgur.com/JgiTTK9.png");
    }

    [SlashCommand("minotrap", "Run the minotrap command.", runMode: RunMode.Async)]
    [Description("Shows the Stygimoloch Lord trap handling for tanks.")]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public Task MinoTrapAsync() =>
        ReplyAsync("https://clips.twitch.tv/PoisedCovertDumplingsItsBoshyTime-Vu4V6JZqHzM9LPUf");

    [SlashCommand("specialboys", "Run the specialboys command.", runMode: RunMode.Async)]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public Task SpecialBoysAsync() => DiscordUtilities.PostImage(_http, Context,
        "https://i.imgur.com/ERMS7cw.png");

    [SlashCommand("drnspeedrun", "Run the drnspeedrun command.", runMode: RunMode.Async)]
    [Description("Shows DRN speedrun loadouts.")]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public Task DrnAsync() => DiscordUtilities.PostImage(_http, Context, "https://i.imgur.com/y0jNEno.png");

    [SlashCommand("styg", "Run the styg command.", runMode: RunMode.Async)]
    [Description("Shows Styg guide.")]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public async Task StygAsync()
    {
        await DiscordUtilities.PostImage(_http, Context,
            "https://cdn.discordapp.com/attachments/808869784195563521/813152064342589443/Fateful_Words_debuffs.png");
        await Task.Delay(200);
        await DiscordUtilities.PostImage(_http, Context,
            "https://cdn.discordapp.com/attachments/803634068092223518/1009648941408735262/unknown.png");
        await Task.Delay(200);
        await DiscordUtilities.PostImage(_http, Context,
            "https://cdn.discordapp.com/attachments/803634068092223518/1009648992080113795/unknown.png");
        await Task.Delay(200);
        await DiscordUtilities.PostImage(_http, Context,
            "https://cdn.discordapp.com/attachments/803634068092223518/1009649017069768744/unknown.png");
    }

    [SlashCommand("stygtank", "Run the stygtank command.", runMode: RunMode.Async)]
    [Description("Shows the Styg tank guide.")]
    [RateLimit(TimeSeconds = 10, Global = true)]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public async Task StygTankAsync()
    {
        await ReplyAsync("styg done right! the important bits you're looking for\n" +
                         "1) 2 ranged gcds at your start marker to establish aggro (don't use provoke, you might need it later)\n" +
                         "2) run to your next cardinal marker for entrapment while spamming ranged (dw if you run out of range, add will catch up)\n" +
                         "3) find trap, resolve trap (even if your ad dies, keep percepting till you see \"no more traps\" - this will save a tank later)\n" +
                         "4) run to the next cardinal marker at the far edge to resolve debuff (dw if you run out of range, add will catch up)\n" +
                         "5) you're set! just stay vigilant as you continue to ranged, unless things have gone truly cursed you should be fine.\n" +
                         "\n" +
                         "https://www.twitch.tv/videos/1111130884");
        await DiscordUtilities.PostImage(_http, Context,
            "https://media.discordapp.net/attachments/803634423416356904/1009166654938284152/unknown-325.png");
    }
}
