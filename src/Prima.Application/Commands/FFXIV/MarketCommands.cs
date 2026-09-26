using Discord.Interactions;
using Newtonsoft.Json.Linq;
using Prima.DiscordNet.Attributes;
using Prima.Game.FFXIV.XIVAPI;

namespace Prima.Application.Commands.FFXIV;
public class MarketCommands : PrimaInteractionModuleBase
{
    private readonly HttpClient _http;
    private readonly XIVAPIClient _xivapi;

    public MarketCommands(HttpClient http, XIVAPIClient xivapi)
    {
        _http = http;
        _xivapi = xivapi;
    }

    [SlashCommand("market", "Run the market command.", runMode: RunMode.Async)]
    [Description("[FFXIV] Look up market data for an item and world.")]
    public async Task MarketAsync(
        [Summary("item", "Name of the FFXIV item to search for.")] string itemName,
        [Summary("world", "World or data center to query.")] string worldName)
    {
        worldName = char.ToUpper(worldName[0]) + worldName[1..];

        var searchResults = await _xivapi.SearchItem(itemName);
        if (searchResults.Count == 0)
        {
            await ReplyAsync($"No results found for \"{itemName}\", are you sure you spelled the item name correctly?");
            return;
        }

        var searchData = searchResults
            .Where(result => string.Equals(result.Name, itemName, StringComparison.CurrentCultureIgnoreCase))
            .ToList();
        var item = !searchData.Any() ? searchResults.First() : searchData.First();

        var itemId = item.Id;
        itemName = item.Name;

        var uniResponse = await _http.GetAsync(new Uri($"https://universalis.app/api/{worldName}/{itemId}"));
        var dataObjectRaw = await uniResponse.Content.ReadAsStringAsync();
        var dataObject = JObject.Parse(dataObjectRaw);

        var results = dataObject["listings"];
        if (results == null)
        {
            await ReplyAsync("Failed to fetch data from Universalis.");
            return;
        }

        var listings = results.ToObject<IList<UniversalisListing>>();
        if (listings == null)
        {
            await ReplyAsync("Failed to get listings from Universalis.");
            return;
        }

        var trimmedListings = listings.Take(Math.Min(10, listings.Count)).ToList();

        await ReplyAsync($"__{listings.Count} results for {worldName} (Showing up to 10):__\n" +
                         trimmedListings.Select(listing => listing.Quantity + " **" + itemName + "** for " +
                                                           listing.PricePerUnit + " Gil " +
                                                           (!string.IsNullOrEmpty(listing.WorldName)
                                                               ? "on " +
                                                                 listing.WorldName + " "
                                                               : "") + (listing.Quantity > 1
                                                               ? $" (For a total of {listing.Total} Gil)"
                                                               : "")));
    }

    public class UniversalisListing
    {
        public bool Hq { get; set; }
        public int PricePerUnit { get; set; }
        public int Quantity { get; set; }
        public string? RetainerName { get; set; }
        public int Total { get; set; }
        public string? WorldName { get; set; }
    }
}
