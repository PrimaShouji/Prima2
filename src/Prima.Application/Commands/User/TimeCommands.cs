﻿using Discord.Commands;
using Prima.DiscordNet.Attributes;
using Prima.Services;

namespace Prima.Application.Commands.User;

[Name("User Time")]
public class TimeCommands : ModuleBase<SocketCommandContext>
{
    private readonly IDbService _db;

    public TimeCommands(IDbService db)
    {
        _db = db;
    }

    [Command("settimezone")]
    [Description("Sets your own timezone for localized DMs and personal messages.")]
    public async Task SetTimezone([Remainder] string timezone)
    {
        if (Context.Channel.Name == "welcome") // This should really be a precondition...
        {
            var r = await ReplyAsync("That command cannot be used in this channel.");
            await Task.Delay(5000);
            await r.DeleteAsync();
            return;
        }

        var dbUser = _db.Users.FirstOrDefault(u => u.DiscordId == Context.User.Id);
        if (dbUser == null)
        {
            await ReplyAsync("Your database information seems to be missing. Please use `~iam World FirstName LastName` again to regenerate it.");
            return;
        }

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            await ReplyAsync("Unable to find that timezone. Please refer to the **TZ database name** column of https://en.wikipedia.org/wiki/List_of_tz_database_time_zones for a list of valid options.");
            return;
        }
        catch (InvalidTimeZoneException)
        {
            await ReplyAsync("Registry data on that timezone has been corrupted.");
            return;
        }

        dbUser.Timezone = timezone;
        await _db.UpdateUser(dbUser);
        await ReplyAsync("Timezone updated successfully!");
    }
}