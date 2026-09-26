using Discord.Interactions;
using Prima.DiscordNet;
using Prima.DiscordNet.Attributes;
using Prima.Resources;

namespace Prima.Application.Commands.PSO2;
public class GuideCommands : PrimaInteractionModuleBase
{
    private readonly HttpClient _http;

    public GuideCommands(HttpClient http)
    {
        _http = http;
    }

    [SlashCommand("aeriomats", "Run the aeriomats command.")]
    [Description("Shows the Aerio materials route.")]
    [RestrictFromGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public Task AerioMaterials()
        => DiscordUtilities.PostImage(_http, Context, "https://i.imgur.com/I8J001K.png");
}