using Discord;
using Discord.Interactions;
using Prima.Services;

namespace Prima.Application.Commands.Random;
public class VotingCommands : PrimaInteractionModuleBase
{
    private readonly IDbService _db;

    public VotingCommands(IDbService db)
    {
        _db = db;
    }

    [SlashCommand("setvotehost", "Run the setvotehost command.")]
    [RequireOwner]
    public async Task SetVoteHost(ITextChannel channel, string messageId)
    {
        if (!ulong.TryParse(messageId, out var messageIdValue))
        {
            await ReplyAsync("Message ID must contain only digits.");
            return;
        }
        if (!await _db.AddVoteHost(messageIdValue, Context.User.Id))
        {
            await ReplyAsync("That message is already registered as a vote host.");
            return;
        }

        var message = await channel.GetMessageAsync(messageIdValue);
        foreach (var (emote, _) in message.Reactions)
        {
            await foreach (var reaction in message.GetReactionUsersAsync(emote, 100))
            {
                foreach (var user in reaction)
                {
                    if (user.Id == message.Author.Id) continue;
                    await _db.AddVote(messageIdValue, user.Id, emote.Name);
                    await message.RemoveReactionAsync(emote, user);
                }
            }
        }

        await ReplyAsync("Message registered.");
    }
}
