using Discord;
using Discord.Interactions;
using Prima.Services;

namespace Prima.Application.Commands.Core;
[RequireOwner]
public class AdminCommands2 : PrimaInteractionModuleBase
{
    private readonly IDbService _db;

    public AdminCommands2(IDbService db)
    {
        _db = db;
    }

    [SlashCommand("sendmessage", "Run the sendmessage command.")]
    public async Task SudoMessage(ITextChannel channel, string message)
    {
        await channel.SendMessageAsync(message);
        await ReplyAsync("Sent!");
    }

    [SlashCommand("clearbrokenusers", "Run the clearbrokenusers command.")]
    public async Task ClearBrokenUsers()
    {
        await _db.RemoveBrokenUsers();
        await ReplyAsync("Done!");
    }
}