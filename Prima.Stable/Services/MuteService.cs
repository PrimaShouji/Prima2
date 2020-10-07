﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Prima.Extensions;
using Prima.Services;

namespace Prima.Stable.Services
{
    public class MuteService
    {
        private readonly DbService _db;

        private readonly IList<ulong> _usersToUnmute;

        public MuteService(DbService db)
        {
            _db = db;
            _usersToUnmute = new List<ulong>();
        }

        public async Task OnVoiceJoin(SocketUser user, SocketVoiceState prev, SocketVoiceState cur)
        {
            var guild = cur.VoiceChannel.Guild;
            var member = guild.GetUser(user.Id);
            if (_usersToUnmute.Contains(member.Id))
            {
                try
                {
                    await member.ModifyAsync(props => props.Mute = false);
                }
                catch
                {
                    var config = _db.Guilds.Single(g => g.Id == guild.Id);
                    var deletedMessageChannel = guild.GetChannel(config.DeletedMessageChannel) as SocketTextChannel ?? throw new NullReferenceException();
                    await deletedMessageChannel.SendMessageAsync($"<@{_db.Config.BotMaster}>, user {user} failed to be server unmuted! Please unmute them manually.");
                }
                _usersToUnmute.RemoveAll(id => id == member.Id);
            }
        }

        public void DeferUnmute(IGuildUser member)
        {
            _usersToUnmute.Add(member.Id);
        }
    }
}