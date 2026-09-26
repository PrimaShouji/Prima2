using Discord;
using Discord.Interactions;
using Prima.Models;
using Prima.Services;

namespace Prima.Application.Commands.Core;
public class ConfigCommands : PrimaInteractionModuleBase
{
    private readonly IDbService _db;

    public ConfigCommands(IDbService db)
    {
        _db = db;
    }
    
    [SlashCommand("configglobal", "Run the configglobal command.", runMode: RunMode.Async)]
    [RequireOwner]
    public async Task ConfigureGlobalAsync(string key, string value)
    {
        try
        {
            await _db.SetGlobalConfigurationProperty(key, value);
            await ReplyAsync("Property updated. Please verify your global configuration change.");
        }
        catch (ArgumentException e)
        {
            await ReplyAsync($"Error: {e.Message}");
        }
    }

    [SlashCommand("configure", "Run the configure command.", runMode: RunMode.Async)]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    public async Task ConfigureAsync(string key, string value)
    {
        try
        {
            await _db.SetGuildConfigurationProperty(Context.Guild.Id, key, value);
            await ReplyAsync("Property updated. Please verify your guild configuration change.");
        }
        catch (ArgumentException e)
        {
            await ReplyAsync($"Error: {e.Message}");
        }
    }

    [SlashCommand("setupguild", "Run the setupguild command.", runMode: RunMode.Async)]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    public async Task SetupGuildAsync()
    {
        await _db.AddGuild(new DiscordGuildConfiguration(Context.Guild.Id));
        await ReplyAsync("Guild configuration created.");
    }

    [SlashCommand("configurerole", "Run the configurerole command.", runMode: RunMode.Async)]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    public async Task ConfigureRoleAsync(string roleName, IRole role)
    {
        await _db.ConfigureRole(Context.Guild.Id, roleName, role.Id);
        await ReplyAsync("Role registered.");
    }

    [SlashCommand("deconfigurerole", "Run the deconfigurerole command.", runMode: RunMode.Async)]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    public async Task DeconfigureRoleAsync(string roleName)
    {
        await _db.DeconfigureRole(Context.Guild.Id, roleName);
        await ReplyAsync("Role deregistered.");
    }

    [SlashCommand("configureroleemote", "Run the configureroleemote command.", runMode: RunMode.Async)]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    public async Task ConfigureRoleEmoteAsync(IRole role, string emote)
    {
        await _db.ConfigureRoleEmote(Context.Guild.Id, role.Id, emote);
        await ReplyAsync("Emote registered.");
    }

    [SlashCommand("deconfigureroleemote", "Run the deconfigureroleemote command.", runMode: RunMode.Async)]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    public async Task DeconfigureRoleEmoteAsync(string emoteId)
    {
        await _db.DeconfigureRoleEmote(Context.Guild.Id, emoteId);
        await ReplyAsync("Emote deregistered.");
    }
}
