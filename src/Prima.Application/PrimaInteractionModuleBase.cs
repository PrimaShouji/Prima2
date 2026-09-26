using Discord;
using Discord.Interactions;

namespace Prima.Application;

public abstract class PrimaInteractionModuleBase : InteractionModuleBase<SocketInteractionContext>
{
    public override Task BeforeExecuteAsync(ICommandInfo command)
    {
        return DeferAsync();
    }

    protected async Task<IUserMessage> ReplyAsync(string? text = null, Embed? embed = null)
    {
        var message = await FollowupAsync(text, embed: embed);
        return message;
    }
}
