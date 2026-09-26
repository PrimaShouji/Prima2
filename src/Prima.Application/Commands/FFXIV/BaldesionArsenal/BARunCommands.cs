using Discord.Interactions;
using Prima.DiscordNet;
using Prima.DiscordNet.Attributes;
using Prima.Resources;

namespace Prima.Application.Commands.FFXIV.BaldesionArsenal;
[Prima.DiscordNet.Attributes.ModuleScope(Prima.DiscordNet.Attributes.ModuleScopeAttribute.ModuleScoping.Guild, GuildId = Prima.Resources.SpecialGuilds.CrystalExploratoryMissions)]
public class BARunCommands : PrimaInteractionModuleBase
{
    [SlashCommand("lfgcountsba", "Run the lfgcountsba command.", runMode: RunMode.Async)]
    [Description("Get the LFG role counts of all guild members for the Baldesion Arsenal.")]
    [RestrictToGuilds(SpecialGuilds.CrystalExploratoryMissions)]
    public Task GetBAProgressionCounts()
    {
        var members = Context.Guild.Users;

        var lfgFragMembers = members.Where(m => m.HasRole(551619522340323328));
        var lfgAvOzmaMembers = members.Where(m => m.HasRole(551619527008321557));
        var lfgOzmaClearMembers = members.Where(m => m.HasRole(551619529336422402));
        var lfgBaAllMembers = members.Where(m => m.HasRole(574696475976794175));

        const string outFormat = "LFG Counts (includes overlap):\n" +
                                 "LFG Frags/Fresh Prog: {0}\n" +
                                 "LFG AV/Ozma Prog: {1}\n" +
                                 "LFG Ozma Clears/Farms: {2}\n" +
                                 "LFG BA All: {3}";
        return ReplyAsync(string.Format(
            outFormat,
            lfgFragMembers.Count(),
            lfgAvOzmaMembers.Count(),
            lfgOzmaClearMembers.Count(),
            lfgBaAllMembers.Count()));
    }
}