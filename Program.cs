using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;

namespace LoadoutRandomizer
{


    class Program
    {
        private DiscordSocketClient _client;

        private CommandHandler _commandHandler = new CommandHandler();
       

        static void Main(string[] args) 
        {
            new Program().RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {

            var token = ""; // Token here
            _client = new DiscordSocketClient();
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            

            _client.Ready += RegisterCommands;
            _client.SlashCommandExecuted += _commandHandler.HandleCommandAsync;
            

            await Task.Delay(-1);
               
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        
        private async Task RegisterCommands()
        {
            _commandHandler.setClient(_client);
        }

       

    }
}
