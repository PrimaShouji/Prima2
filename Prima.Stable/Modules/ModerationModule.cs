﻿using Discord;
using Discord.Commands;
using Prima.Models;
using Prima.Stable.Services;
using Prima.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Prima.Attributes;
using Prima.Stable.Handlers;

namespace Prima.Moderation.Modules
{
    /// <summary>
    /// This module includes commands that assist with moderation.
    /// </summary>
    [Name("Moderation")]
    public class ModerationModule : ModuleBase<SocketCommandContext>
    {
        public IDbService Db { get; set; }

        // Submit a report.
        [Command("modmail", RunMode = RunMode.Async)]
        [Alias("report")]
        [Description("Privately report information to the administration.")]
        public async Task ReportAsync([Remainder] string p = "")
        {
            if (Context.Guild != null)
            {
                _ = WarnOfPublicReport();
            }
            var responseMessage = await Context.Channel.SendMessageAsync(Properties.Resources.ReportThankYou);

            var guild = Context.Guild;
            if (guild == null)
            {
                foreach (var otherGuild in Context.User.MutualGuilds)
                {
                    if (Db.Guilds.Any(g => g.Id == otherGuild.Id))
                    {
                        guild = otherGuild;
                        break;
                    }
                }
            }
            var guildConfig = Db.Guilds.Single(g => g.Id == guild.Id);

            var postChannel = guild.GetTextChannel(guildConfig.ReportChannel);
            var output = $"<@&{guildConfig.Roles["Moderator"]}> {Context.User.Mention} just sent a report: {p}";
            if (output.Length > 2000) // This can only be the case once, no need for a loop.
            {
                await postChannel.SendMessageAsync(output.Substring(0, 2000));
                output = output.Substring(2000);
            }
            await postChannel.SendMessageAsync(output);
            foreach (var attachment in Context.Message.Attachments)
            {
                await postChannel.SendFileAsync(Path.Combine(Db.Config.TempDir, attachment.Filename), string.Empty);
            }
            if (Context.Guild != null)
            {
                await Context.Message.DeleteAsync();
                await Task.Delay(10000);
                await responseMessage.DeleteAsync();
            }
        }

        private async Task WarnOfPublicReport()
        {
            var warning = await ReplyAsync(Context.User.Mention + ", " + Properties.Resources.ReportInGuildWarning);
            await Task.Delay(5000);
            await warning.DeleteAsync();
        }

        // Check when a user joined Discord.
        [Command("when")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public Task WhenAsync([Remainder] string user = "")
        {
            if (!ulong.TryParse(Util.CleanDiscordMention(user), out var uid))
            {
                return ReplyAsync("Could not read user ID.");
            }

            var unixTimestamp = uid / 4194304 + 1420070400000;
            var unixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            unixTime = unixTime.AddMilliseconds(unixTimestamp);

            return ReplyAsync(unixTime.ToString(CultureInfo.InvariantCulture));
        }

        // Check when a user created their FFXIV character.
        [Command("lwhen")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public Task WhenLodestone([Remainder] string lodestoneIdOrUrl = "")
        {
            if (!ulong.TryParse(lodestoneIdOrUrl, out var uid))
            {
                var lodestonePageRegex = new Regex(@"(?<=lodestone\/character\/)\d+");
                var match = lodestonePageRegex.Match(lodestoneIdOrUrl);
                if (!match.Success)
                {
                    return ReplyAsync("Could not read Lodestone ID.");
                }

                uid = ulong.Parse(match.Value);
            }

            var creationTime = LodestoneIdTime(uid);
            return ReplyAsync(creationTime.ToString(CultureInfo.InvariantCulture));
        }

        // https://github.com/karashiiro/lodestone-id-time
        private static DateTime LodestoneIdTime(ulong id)
        {
            double excelTime;
            if (id <= 5000000)
                excelTime = 37.44 / 5000000 * id + 41539.93;
            else if (id > 28208601)
                excelTime = 305.01 / 4775200 * id + 42030.57;
            else
                excelTime = 4.10315437 * Math.Pow(10, 4)
                            + 1.00993557 * Math.Pow(10, -4) * id
                            + 31.5417054 * Math.Sin(8.57105764 * Math.Pow(10, -7) * id);
            var unixTimestamp = (ulong)Math.Floor((excelTime - 25569) * 86400);
            var unixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            unixTime = unixTime.AddSeconds(unixTimestamp);
            return unixTime;
        }

        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireContext(ContextType.Guild)]
        public async Task BanAsync(string user, [Remainder] string reason)
        {
            if (!ulong.TryParse(Util.CleanDiscordMention(user), out var uid))
            {
                await ReplyAsync("Could not read user ID.");
                return;
            }

            await Context.Guild.AddBanAsync(uid, reason: reason);
            await ReplyAsync("User banned.");
        }

        [Command("timeout")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task TimeoutAsync(IUser user)
        {
            var guildConfig = Db.Guilds.FirstOrDefault(g => g.Id == (Context.Guild?.Id ?? 0));
            if (guildConfig == null)
            {
                await ReplyAsync("This command needs to be executed in a guild!");
                return;
            }

            var timeoutRole = Context.Guild.GetRole(573340288815333386);
            var memberRole = Context.Guild.GetRole(573347763287228436);
            var bozjaRole = Context.Guild.GetRole(588913532410527754);
            var eurekaRole = Context.Guild.GetRole(588913087818498070);
            var diademRole = Context.Guild.GetRole(588913444712087564);

            var member = Context.Guild.GetUser(user.Id);
            await member.AddRoleAsync(timeoutRole);
            await member.RemoveRolesAsync(new List<IRole> { memberRole, bozjaRole, eurekaRole, diademRole });

            await ReplyAsync("User timed out. You may end their timeout at any time with `~untimeout`.");
        }

        [Command("untimeout")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task UntimeoutAsync(IUser user)
        {
            var guildConfig = Db.Guilds.FirstOrDefault(g => g.Id == (Context.Guild?.Id ?? 0));
            if (guildConfig == null)
            {
                await ReplyAsync("This command needs to be executed in a guild!");
                return;
            }

            var timeoutRole = Context.Guild.GetRole(573340288815333386);
            var memberRole = Context.Guild.GetRole(573347763287228436);
            var bozjaRole = Context.Guild.GetRole(588913532410527754);

            var member = Context.Guild.GetUser(user.Id);
            await member.RemoveRoleAsync(timeoutRole);
            await member.AddRolesAsync(new List<IRole> { memberRole, bozjaRole });

            await ReplyAsync("User timeout ended.");
        }

        // Add a regex to the blacklist.
        [Command("blocktext")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BlockTextAsync([Remainder] string regexString)
        {
            try
            {
                Regex.IsMatch("", regexString);
            }
            catch (ArgumentException)
            {
                await ReplyAsync(Properties.Resources.InvalidRegexError);
                return;
            }

            await Db.AddGuildTextBlacklistEntry(Context.Guild.Id, regexString);
            await ReplyAsync(Properties.Resources.GenericSuccess);
        }

        // Remove a regex from the blacklist.
        [Command("unblocktext")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task UnblockTextAsync([Remainder] string regexString = "")
        {
            var guildConfig = Db.Guilds.FirstOrDefault(g => g.Id == Context.Guild.Id);
            if (guildConfig == null) return;

            if (string.IsNullOrEmpty(regexString)) // Remove the last regex that was matched if none was specified.
            {
                var lastCaughtRegex = ChatCleanup.LastCaughtRegex; // TODO: Make this a guild-keyed Dictionary
                var entry = guildConfig.TextBlacklist.FirstOrDefault(rs => rs == lastCaughtRegex);
                if (entry == null)
                {
                    await ReplyAsync(Properties.Resources.RegexNotFoundError);
                    return;
                }
                await Db.RemoveGuildTextBlacklistEntry(Context.Guild.Id, entry);
            }
            else
            {
                try
                {
                    var entry = guildConfig.TextBlacklist.Single(rs => rs == regexString);
                     await Db.RemoveGuildTextBlacklistEntry(Context.Guild.Id, entry);
                }
                catch (InvalidOperationException)
                {
                    await ReplyAsync(Properties.Resources.RegexNotFoundError);
                    return;
                }
            }
            await ReplyAsync(Properties.Resources.GenericSuccess);
        }

        [Command("blockedtext")]
        [Alias("blockedtexts")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task SeeBlockedTexts([Remainder] string args = "")
        {
            var guildConfig = Db.Guilds.FirstOrDefault(g => g.Id == Context.Guild.Id);
            if (guildConfig == null) return;

            var output = $"These are the blocked expressions in the guild {Context.Guild.Name}:" +
                         "```\n" + guildConfig.TextBlacklist.Aggregate("", (agg, next) => $"{agg}{next}\n") + "```";

            await Context.User.SendMessageAsync(output);
        }
    }
}
