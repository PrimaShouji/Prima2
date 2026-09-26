using Discord;
using Discord.Interactions;
using Prima.DiscordNet.Attributes;
using Prima.DiscordNet.Extensions;
using Prima.Services;
using Color = Discord.Color;

namespace Prima.Application.Commands.Core;
public class HelpCommands : PrimaInteractionModuleBase
{
    private readonly InteractionService _commandManager;
    private readonly IServiceProvider _services;
    private readonly ITemplateProvider _templates;

    public HelpCommands(InteractionService commandManager, IServiceProvider services, ITemplateProvider templates)
    {
        _commandManager = commandManager;
        _services = services;
        _templates = templates;
    }

    [SlashCommand("help", "Run the help command.")]
    [Description("<:LappDumb:736310777463439422>")]
    public async Task HelpAsync()
    {
        var commands = _commandManager.SlashCommands
            .Where(command => command.Attributes.Any(attr => attr is DescriptionAttribute));

        var fields = new List<EmbedFieldBuilder>();
        foreach (var command in commands)
        {
            var restrictedToAttr = (RestrictToGuildsAttribute?)command.Attributes.FirstOrDefault(attr => attr is RestrictToGuildsAttribute);
            if (restrictedToAttr != null && (Context.Guild == null || !restrictedToAttr.GuildIds.Contains(Context.Guild.Id)))
                continue;

            var restrictedFromAttr = (RestrictFromGuildsAttribute?)command.Attributes.FirstOrDefault(attr => attr is RestrictFromGuildsAttribute);
            if (restrictedFromAttr != null && (Context.Guild != null && restrictedFromAttr.GuildIds.Contains(Context.Guild.Id)))
                continue;

            var descAttr = (DescriptionAttribute)command.Attributes.First(attr => attr is DescriptionAttribute);

            var fieldBuilder = new EmbedFieldBuilder()
                .WithIsInline(true)
                .WithName("/" + command.Name)
                .WithValue((restrictedToAttr != null ? $"({Context.Guild?.Name}) " : "") + descAttr.Description);
            fields.Add(fieldBuilder);
        }

        var fieldsArr = fields.ToArray();
        for (var i = 0; i < Math.Ceiling(fields.Count / 25.0); i++)
        {
            var embed = new EmbedBuilder()
                .WithTitle("These are the commands you can use with Prima in that server:")
                .WithColor(Color.DarkGreen)
                .WithFields(fieldsArr[(i * 25)..Math.Min(fields.Count, (i + 1) * 25)])
                .Build();
            await Context.User.SendMessageAsync(embed: embed);
        }

        await ReplyAsync($"{Context.User.Mention}, a list of commands you can use in this server was sent to you via DM.");
    }

    [SlashCommand("privacy", "Run the privacy command.")]
    [Description("See information about data this bot collects.")]
    public async Task PrivacyPolicy()
    {
        var embed = _templates.Execute("privacy.md", new{})
            .ToEmbedBuilder()
            .WithColor(Color.DarkGreen)
            .Build();
        await Context.User.SendMessageAsync(embed: embed);
    }
}
