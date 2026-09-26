using Discord.Interactions;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Prima.DiscordNet.Services;

namespace Prima.DiscordNet.Attributes
{
    public class RateLimitAttribute : PreconditionAttribute
    {
        public int TimeSeconds { get; set; }

        public bool Global { get; set; }

        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo command, IServiceProvider services)
        {
            var rateLimits = services.GetRequiredService<RateLimitService>();
            if (!rateLimits.IsReady(command))
            {
                await context.Interaction.RespondAsync(
                    $"That command cannot be used for another {rateLimits.TimeUntilReady(command)} seconds.", ephemeral: true);
                return PreconditionResult.FromError("Command rate limit has not yet expired.");
            }

            rateLimits.ResetTime(command, TimeSeconds);
            return PreconditionResult.FromSuccess();
        }
    }
}
