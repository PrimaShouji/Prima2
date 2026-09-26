using System.Diagnostics;
using Discord.Interactions;

namespace Prima.Application.Commands.Core;
[RequireOwner]
public class DiagnosticsCommands : PrimaInteractionModuleBase
{
    private readonly InteractionService _commands;

    public DiagnosticsCommands(InteractionService commands)
    {
        _commands = commands;
    }

    [SlashCommand("ping", "Run the ping command.", runMode: RunMode.Async)]
    public async Task PingAsync()
    {
        await ReplyAsync($"`{Process.GetCurrentProcess().ProcessName} online, heartbeat {Context.Client.Latency}ms`");
    }

    [SlashCommand("modules", "Run the modules command.")]
    public Task Modules()
    {
        return ReplyAsync(_commands.Modules
            .Select(module => module.Name)
            .Aggregate("```\n", (acc, next) => acc + next + "\n") + "```");
    }
}
