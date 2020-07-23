﻿using Discord.Commands;
using Newtonsoft.Json.Linq;
using Prima.Services;
using Prima.XIVAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Prima.Attributes;

namespace Prima.Extra.Modules
{
    /// <summary>
    /// Includes extra, unimportant commands.
    /// </summary>
    [Name("Extra")]
    public class ExtraModule : ModuleBase<SocketCommandContext>
    {
        public HttpClient Http { get; set; }
        public XIVAPIService Xivapi { get; set; }

        [Command("roll", RunMode = RunMode.Async)]
        public async Task RollAsync()
        {
            var res = (int)Math.Floor(new Random().NextDouble() * 4);
            switch (res)
            {
                case 0:
                    await ReplyAsync($"BINGO! You matched {(int)Math.Floor(new Random().NextDouble() * 11) + 1} lines! Congratulations!");
                    break;
                case 1:
                    var opt = (int)Math.Floor(new Random().NextDouble() * 2598960) + 1;
                    if (opt <= 4)
                        await ReplyAsync("JACK**P**O*T!* Roya**l flush!** You __won__ [%#*(!@] credits*!*");
                    else if (opt > 4 && opt <= 40)
                        await ReplyAsync("Straight flush! You won [TypeError: Cannot read property 'MUNZ' of undefined] credits!");
                    else if (opt > 40 && opt <= 664)
                        await ReplyAsync("Four of a kind! You won [TypeError: Cannot read property 'thinking' of undefined] credits!");
                    else if (opt > 664 && opt <= 4408)
                        await ReplyAsync("Full house! You won [TypeError: Cannot read property '<:GWgoaThinken:582982105282379797>' of undefined] credits!");
                    else if (opt > 4408 && opt <= 9516)
                        await ReplyAsync("Flush! You won -1 credits!");
                    else if (opt > 9516 && opt <= 19716)
                        await ReplyAsync("Straight! You won -20 credits!");
                    else if (opt > 19716 && opt <= 74628)
                        await ReplyAsync("Two pairs...? You won -500 credits!");
                    else if (opt > 198180 && opt <= 1296420)
                        await ReplyAsync("One pair. You won -2500 credits.");
                    else
                        await ReplyAsync("No pairs. You won -10000 credits.");
                    break;
                case 2:
                    await ReplyAsync($"Critical hit! You dealt {(int)Math.Floor(new Random().NextDouble() * 9999) + 9999} damage!");
                    break;
                case 3:
                    await ReplyAsync("You took 9999999 points of damage. You lost the game.");
                    break;
            }
        }

        [Command("market", RunMode = RunMode.Async)]
        public async Task MarketAsync(params string[] args)
        {
            if (args.Length < 2)
            {
                await ReplyAsync($"{Context.User.Mention}, please provide an item name in your command, followed by the World or DC name to query.");
                return;
            }

            var itemName = string.Join(' ', args[0..^2]);
            var worldName = args[^1];
            worldName = char.ToUpper(worldName[0]) + worldName.Substring(1);
            var worldId = 0;

            var searchResults = await Xivapi.Search<Item>(itemName);
            if (searchResults.Count == 0)
            {
                await ReplyAsync($"No results found for \"{itemName}\", are you sure you spelled the item name correctly?");
                return;
            }
            var searchData = searchResults.Where(result => result.Name.ToLower() == itemName.ToLower());
            var item = new Item();
            if (searchData.Count() == 0)
                item = searchResults.First();
            else
                item = searchData.First();

            var itemId = item.ID;
            itemName = item.Name;

            var uniResponse = await Http.GetAsync(new Uri($"https://universalis.app/api/{worldId}/{itemId}"));
            var dataObject = await uniResponse.Content.ReadAsStringAsync();
            var listings = JObject.Parse(dataObject)["Results"].ToObject<IList<UniversalisListing>>();
            var trimmedListings = listings.Take(Math.Min(10, listings.Count())).ToList();

            await ReplyAsync($"__{listings.Count} results for {worldName} (Showing up to 10):__\n" +
                trimmedListings.Select(listing => listing.Quantity + " **" + itemName + "** for " +
					listing.PricePerUnit + " Gil " + (!string.IsNullOrEmpty(listing.WorldName) ? "on " +
					listing.WorldName + " " : "") + (listing.Quantity > 1 ? $" (For a total of {listing.Total} Gil)" : "")));
        }

        public struct UniversalisListing
        {
            public bool Hq;
            public int PricePerUnit;
            public int Quantity;
            public string RetainerName;
            public int Total;
            public string WorldName;
        }

        [Command("portals", RunMode = RunMode.Async)]
        [RateLimit(TimeSeconds = 30, Global = true)]
        public async Task PortalsAsync()
        {
            using var http = new HttpClient();
            var fileStream = await http.GetStreamAsync(new Uri("https://i.imgur.com/XcXACQp.png"));
            await Context.Channel.SendFileAsync(fileStream, "XcXACQp.png");
        }

        [Command("owain", RunMode = RunMode.Async)]
        [RateLimit(TimeSeconds = 120, Global = true)]
        public async Task OwainAsync()
        {
            using var http = new HttpClient();
            var fileStream = await http.GetStreamAsync(new Uri("https://cdn.discordapp.com/attachments/715548578684207134/716812557851295815/unknown-2.png"));
            await Context.Channel.SendFileAsync(fileStream, "unknown-2.png");
        }

        [Command("art", RunMode = RunMode.Async)]
        [RateLimit(TimeSeconds = 120, Global = true)]
        public async Task ArtAsync()
        {
            using var http = new HttpClient();
            var fileStream = await http.GetStreamAsync(new Uri("https://cdn.discordapp.com/attachments/715548578684207134/716812588029182033/unknown-1.png"));
            await Context.Channel.SendFileAsync(fileStream, "unknown-1.png");
        }

        [Command("raiden", RunMode = RunMode.Async)]
        [RateLimit(TimeSeconds = 120, Global = true)]
        public async Task RaidenAsync()
        {
            using var http = new HttpClient();
            var fileStream = await http.GetStreamAsync(new Uri("https://cdn.discordapp.com/attachments/588592729609469952/721522493197779035/unknown.png"));
            await Context.Channel.SendFileAsync(fileStream, "unknown.png");
        }

        [Command("av", RunMode = RunMode.Async)]
        [RateLimit(TimeSeconds = 120, Global = true)]
        public async Task AbsoluteVirtueAsync()
        {
            using var http = new HttpClient();
            var fileStream = await http.GetStreamAsync(new Uri("https://cdn.discordapp.com/attachments/588592729609469952/721522585866731580/unknown.png"));
            await Context.Channel.SendFileAsync(fileStream, "unknown.png");
        }

        [Command("ozma", RunMode = RunMode.Async)]
        [RateLimit(TimeSeconds = 120, Global = true)]
        public async Task OzmaAsync()
        {
            using var http = new HttpClient();
            var fileStream = await http.GetStreamAsync(new Uri("https://cdn.discordapp.com/attachments/588592729609469952/721522648949063730/unknown.png"));
            await Context.Channel.SendFileAsync(fileStream, "unknown.png");
        }

        [Command("accel", RunMode = RunMode.Async)]
        [RateLimit(TimeSeconds = 120, Global = true)]
        public async Task AccelBombAsync()
        {
            using var http = new HttpClient();
            var fileStream = await http.GetStreamAsync(new Uri("https://cdn.discordapp.com/attachments/550709673104375818/721527266441560064/unknown.png"));
            await Context.Channel.SendFileAsync(fileStream, "unknown.png");
        }
    }
}