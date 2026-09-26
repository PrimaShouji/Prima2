using Discord.Interactions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace Prima.DiscordNet.Attributes
{
    public class DisableInChannelsForGuildAttribute : PreconditionAttribute
    {
        public ulong GuildId { get; set; }

        private readonly ulong[] _channelIds;

        public DisableInChannelsForGuildAttribute(params ulong[] channelIds)
        {
            _channelIds = channelIds;
        }

        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo command, IServiceProvider services)
        {
            if (context.Channel is not IGuildChannel guildChannel || guildChannel.GuildId != GuildId)
            {
                return PreconditionResult.FromSuccess();
            }

            if (!_channelIds.Contains(guildChannel.Id))
            {
                return PreconditionResult.FromSuccess();
            }

            await context.Interaction.RespondAsync("That command is disabled in this channel.", ephemeral: true);

            return PreconditionResult.FromError("Command may not be executed in this channel.");

        }
    }
}
