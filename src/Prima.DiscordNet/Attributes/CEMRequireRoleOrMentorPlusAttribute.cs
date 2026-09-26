using System;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace Prima.DiscordNet.Attributes
{
    public class CEMRequireRoleOrMentorPlusAttribute : PreconditionAttribute
    {
        private const ulong CEMMentorRoleId = 579916868035411968;

        private readonly ulong _roleId;

        public CEMRequireRoleOrMentorPlusAttribute(ulong roleId)
        {
            _roleId = roleId;
        }

        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo command, IServiceProvider services)
        {
            if (context.User is not IGuildUser member)
            {
                return PreconditionResult.FromError("Command cannot be executed outside of a guild.");
            }

            var role = member.Guild.GetRole(_roleId);

            if (member.MemberHasRole(role, context) || member.MemberHasRole(CEMMentorRoleId, context) || member.GuildPermissions.KickMembers)
                return PreconditionResult.FromSuccess();

            await context.Interaction.RespondAsync(
                $"{member.Mention}, you don't have the {role.Name} role!", ephemeral: true);

            return PreconditionResult.FromError($"User does not have required role \"{role.Name}\" or Mentor+.");
        }
    }
}
