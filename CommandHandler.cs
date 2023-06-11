using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LoadoutRandomizer
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandsList _commandsList = new CommandsList();

        public void setClient(DiscordSocketClient client)
        {
            _client = client;
            setCommands();
        }
        public async Task setCommands()
        {
            var helpCommand = new Discord.SlashCommandBuilder();
            helpCommand.WithName("show-guide");
            helpCommand.WithDescription("shows the guide to using Geoffrey");

            var rollCommand = new Discord.SlashCommandBuilder();
            rollCommand.WithName("roll-weapon");
            rollCommand.WithDescription("roll a randomize loadout for your class");
            rollCommand.AddOption("class", Discord.ApplicationCommandOptionType.String, "" +
                "classes rollable: scout, soldier, pyro, demoman, heavy, engineer, medic, sniper, spy or random", isRequired: true);

            var reskinCommand = new Discord.SlashCommandBuilder();
            reskinCommand.WithName("roll-reskin");
            reskinCommand.WithDescription("roll a reskin of an existing weapon");
            reskinCommand.AddOption("class", Discord.ApplicationCommandOptionType.String, "" +
                "classes rollable: scout, soldier, pyro, demoman, heavy, engineer, medic, sniper, spy or random", isRequired: true);

            try
            {
                await _client.CreateGlobalApplicationCommandAsync(helpCommand.Build());
                await _client.CreateGlobalApplicationCommandAsync(rollCommand.Build());
                await _client.CreateGlobalApplicationCommandAsync(reskinCommand.Build());
            }
            catch
            {
                Console.WriteLine("Error");
            }
          
        }

        public async Task HandleCommandAsync(SocketSlashCommand command)
        {

            switch (command.Data.Name)
            {

                case "show-guide":
                    await _commandsList.commandList(command);
                    break;
                case "roll-weapon":
                    await _commandsList.RollRandom(command);
                    break;
                case "roll-reskin":
                    await _commandsList.RollReskin(command);
                    break;
            }
        }
    }
}
