﻿using Discord;
using Discord.WebSocket;
using Prima.Resources;
using Prima.Scheduler.GoogleApis.Services;
using Prima.Services;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Prima.Scheduler.Handlers
{
    public static class AnnounceEdit
    {
        public static async Task Handler(DiscordSocketClient client, CalendarApi calendar, IDbService db, SocketMessage message)
        {
            var guildConfig = db.Guilds.FirstOrDefault(g => g.Id == SpecialGuilds.CrystalExploratoryMissions);
            if (guildConfig == null)
            {
                Log.Error("No guild configuration found for the default guild!");
                return;
            }

            var guild = client.GetGuild(guildConfig.Id);

            var prefix = db.Config.Prefix;
            if (message.Content == null || !message.Content.StartsWith(prefix + "announce")) return;

            Log.Information("Announcement message being edited.");

            var outputChannel = ScheduleUtils.GetOutputChannel(guildConfig, guild, message.Channel);
            if (outputChannel == null)
            {
                Log.Information("Could not get output channel; aborting.");
                return;
            }

            var args = message.Content[(message.Content.IndexOf(' ') + 1)..];

            var splitIndex = args.IndexOf("|", StringComparison.Ordinal);
            if (splitIndex == -1)
            {
                await message.Channel.SendMessageAsync(
                    $"{message.Author.Mention}, please provide parameters with that command.\n" +
                    "A well-formed command would look something like:\n" +
                    $"`{prefix}announce 5:00PM | This is a fancy description!`");
                return;
            }

            var description = args[(splitIndex + 1)..].Trim();
            var trimmedDescription = description[..Math.Min(1700, description.Length)];
            if (trimmedDescription.Length != description.Length)
            {
                trimmedDescription += "...";
            }

            var (embedMessage, embed) = await FindAnnouncement(outputChannel, message.Id);
            var lines = embed.Description.Split('\n');
            var messageLinkLine = lines.LastOrDefault(l => l.StartsWith("Message Link: https://discordapp.com/channels/"));
            var calendarLinkLine = lines.LastOrDefault(l => l.StartsWith("[Copy to Google Calendar]"));
            await embedMessage.ModifyAsync(props =>
            {
                props.Embeds = new[] {embed
                    .ToEmbedBuilder()
                    .WithDescription(trimmedDescription + (calendarLinkLine != null
                        ? $"\n\n{calendarLinkLine}"
                        : "") + (messageLinkLine != null
                        ? $"\n{messageLinkLine}"
                        : ""))
                    .Build()};
            });

            Log.Information("Updated announcement embed.");
        }

        private static async Task<(IUserMessage, IEmbed)> FindAnnouncement(IMessageChannel channel, ulong eventId)
        {
            await foreach (var page in channel.GetMessagesAsync())
            {
                foreach (var message in page)
                {
                    var restMessage = (IUserMessage)message;

                    var embed = restMessage.Embeds.FirstOrDefault();
                    if (embed?.Footer == null) continue;

                    if (embed.Footer?.Text != eventId.ToString()) continue;

                    return (restMessage, embed);
                }
            }

            return (null, null);
        }
    }
}