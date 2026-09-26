using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Prima.DiscordNet.Attributes;

namespace Prima.DiscordNet.Services
{
    public class InteractionHandlingService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private readonly ILogger<InteractionHandlingService> _logger;
        private readonly IServiceProvider _services;

        public InteractionHandlingService(DiscordSocketClient client, InteractionService handler,
            ILogger<InteractionHandlingService> logger, IServiceProvider services)
        {
            _client = client;
            _handler = handler;
            _logger = logger;
            _services = services;
        }

        public async Task InitializeAsync(Assembly assembly = null)
        {
            _client.InteractionCreated += HandleInteraction;
            _handler.SlashCommandExecuted += async (_, context, result) =>
            {
                if (result.IsSuccess)
                {
                    return;
                }

                if (result.Error == InteractionCommandError.UnmetPrecondition && context.Interaction.HasResponded)
                {
                    return;
                }

                _logger.LogError("Slash command failed: {ErrorReason}", result.ErrorReason);
                if (!context.Interaction.HasResponded)
                {
                    await context.Interaction.RespondAsync("Failed to process interaction.", ephemeral: true);
                }
                else
                {
                    await context.Interaction.FollowupAsync("Failed to process interaction.", ephemeral: true);
                }
            };
            var modules = await _handler.AddModulesAsync(assembly ?? Assembly.GetEntryAssembly(), _services);
            var scopedModules = modules
                .Select(m => new
                {
                    Module = m,
                    Scope = m.Attributes.OfType<ModuleScopeAttribute>().FirstOrDefault(),
                })
                .ToList();

            var guildModules = scopedModules
                .Where(pair => pair.Scope?.Scope == ModuleScopeAttribute.ModuleScoping.Guild)
                .GroupBy(pair => pair.Scope.GuildId)
                .ToDictionary(group => group.Key, group => group.ToArray());
            foreach (var (guildId, moduleGroup) in guildModules)
            {
                await _handler.AddModulesToGuildAsync(guildId, true, moduleGroup.Select(pair => pair.Module).ToArray());
            }

            var globalModules = scopedModules
                .Where(pair => pair.Scope?.Scope != ModuleScopeAttribute.ModuleScoping.Guild);
            await _handler.AddModulesGloballyAsync(true, globalModules.Select(pair => pair.Module).ToArray());
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
                var context = new SocketInteractionContext(_client, interaction);

                // Execute the incoming command.
                var result = await _handler.ExecuteCommandAsync(context, _services);

                if (result.IsSuccess)
                {
                    return;
                }

                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        _logger.LogWarning("Unmet precondition: {ErrorReason}", result.ErrorReason);
                        break;
                    case InteractionCommandError.UnknownCommand:
                        _logger.LogWarning("Unknown command: {ErrorReason}", result.ErrorReason);
                        break;
                    case InteractionCommandError.BadArgs:
                        _logger.LogWarning("Invalid number of arguments: {ErrorReason}", result.ErrorReason);
                        break;
                    case InteractionCommandError.Exception:
                        _logger.LogError("Interaction error: {ErrorReason}", result.ErrorReason);
                        break;
                    case InteractionCommandError.Unsuccessful:
                        _logger.LogWarning("Command could not be executed: {ErrorReason}", result.ErrorReason);
                        break;
                    case InteractionCommandError.ConvertFailed:
                        _logger.LogWarning("Failed to convert object: {ErrorReason}", result.ErrorReason);
                        break;
                    case InteractionCommandError.ParseFailed:
                        _logger.LogWarning("Failed to parse object: {ErrorReason}", result.ErrorReason);
                        break;
                    case null:
                        _logger.LogWarning("Null error type in unsuccessful result: {ErrorReason}", result.ErrorReason);
                        break;
                    default:
                        _logger.LogWarning("Unknown error type from unsuccessful result: {ErrorReason}",
                            result.ErrorReason);
                        break;
                }

                if (!context.Interaction.HasResponded)
                {
                    await context.Interaction.RespondAsync("Failed to process interaction.", ephemeral: true);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to process interaction");
                if (!interaction.HasResponded)
                {
                    await interaction.RespondAsync("Failed to process interaction.", ephemeral: true);
                }
                else
                {
                    try
                    {
                        await interaction.ModifyOriginalResponseAsync(properties =>
                            properties.Content = "Failed to process interaction.");
                    }
                    catch
                    {
                        await interaction.FollowupAsync("Failed to process interaction.", ephemeral: true);
                    }
                }
            }
        }
    }
}
