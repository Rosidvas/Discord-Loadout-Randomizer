using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadoutRandomizer
{
    public class CommandsList : ModuleBase
    {
        private DataCaller _DataCaller = new DataCaller();
        
        //Shows the bot command list on request
        public async Task commandList(SocketSlashCommand command)
        {
            var builder = new EmbedBuilder()

                     .WithFooter(footer => footer.Text = "Roll your Loadout?!")
                     .WithColor(new Color(255, 140, 0))
                     .WithTitle("Commands List")
                     .WithDescription("/show-guide: shows possible commands that the bot can perform.\n"+
                     "/roll-weapon [class]: roll a random primary, secondary and melee weapon for the specified or random class \n" +
                     "/roll-reskin [class]: roll a reskin of a weapon based from a class or with random")
                     .AddField("Available Options", "All the available options are each of the 9 classes in Tf2: \n" +
                     "Scout, Soldier, Pyro, Demoman, Heavy, Engineer, Medic, Sniper and Spy. You can also use Random to get a random class aswell.")
                     .AddField("Suggestions", "Maybe with your ideas, you can help Geoffrey achieve his dreams to become the best bot of all time!" +
                     " no guarantees tho.")

                     .WithFooter(footer => footer.Text = "Randomize now!");

            var response = builder.Build();

            await command.RespondAsync(embed: response);
        }

        // Roll for a random loadout per request
        public async Task RollRandom(SocketSlashCommand command)
        {
            // Uses DataCaller to get the primary, secondary and melee weapons
            string classrequest = (string)command.Data.Options.First().Value;
            string tfClass = _DataCaller.collectionRandomizer(classrequest);

            //Special case: Spy has a different loadout set than the other class
            string watch = "Invis Watch";
            string watchImg = "Image not found";
            string sapper = "Sapper";
            string sapperImg = "Image not found";
            string primary;
            string primaryImg = "Image not found";
            string secondary = "";
            string secondaryImg = "Image not found";
            string melee = "";
            string meleeImg = "Image not found";

            if (tfClass == "spy")
            {
                sapper = _DataCaller.callSapper();
                watch = _DataCaller.callWatch();
                primary = _DataCaller.callPrimary(tfClass);
                melee = _DataCaller.callMelee(tfClass);
                
                //calls the Images of each spy weapon
                sapperImg = _DataCaller.callImages(sapper, tfClass, false);
                watchImg = _DataCaller.callImages(watch, tfClass, false);
                meleeImg = _DataCaller.callImages(melee, tfClass, false);
                primaryImg = _DataCaller.callImages(primary, tfClass, false);
            }
            else
            {
                primary = _DataCaller.callPrimary(tfClass);
                secondary = _DataCaller.callSecondary(tfClass);
                melee = _DataCaller.callMelee(tfClass);

                //calls the Images of each weapon
                primaryImg = _DataCaller.callImages(primary, tfClass, false);
                secondaryImg = _DataCaller.callImages(secondary, tfClass, false);
                meleeImg = _DataCaller.callImages(melee, tfClass, false);
            }
            
            if (melee == "invalid Request")
            {
                InvalidCommand(command);
                return;
            }

            var builder = new EmbedBuilder();

            if (tfClass == "spy")
            {
                builder = new EmbedBuilder()
                .WithColor(new Color(255, 140, 0))
                .WithTitle($"Your loadout for the {tfClass} is..")
                .AddField($"Primary: {primary}", $"[image]({primaryImg})")
                .AddField($"Sapper: {sapper}", $"[image]({sapperImg})")
                .AddField($"Melee: {melee}", $"[image]({meleeImg})")
                .AddField($"Watch: {watch}", $"[image]({watchImg})")
                .AddField("Looking for the Reskin?", "Weapon reskins aren't guaranteed to be owned freely by everyone. \n" +
                "If you want roll reskins, check out the /roll-reskin command.")
                .WithFooter(footer => footer.Text = "enhance your Tf2 experience!");
            }
            else
            {
                builder = new EmbedBuilder()
                .WithColor(new Color(255, 140, 0))
                .WithTitle($"Your loadout for the {tfClass} is..")
                .AddField($"Primary: {primary}", $"[image]({primaryImg})")
                .AddField($"Secondary: {secondary}", $"[image]({secondaryImg})")
                .AddField($"Melee: {melee}", $"[image]({meleeImg})")
                .AddField("Looking for the Reskin?", "Weapon reskins aren't guaranteed to be owned freely by everyone. \n" +
                "If you want roll reskins, check out the /roll-reskin command.")
                .WithFooter(footer => footer.Text = "enhance your Tf2 experience!");
            }
            

            var response = builder.Build();

            await command.RespondAsync(embed: response);
        }

        //Roll a Reskin of a weapon
        public async Task RollReskin(SocketSlashCommand command)
        {
            string classrequest = (string)command.Data.Options.First().Value;
            string tfClass = _DataCaller.collectionRandomizer(classrequest);
            string[] reskin = _DataCaller.callReskin(tfClass);

            if (reskin.Contains("invalid Request"))
            {

                InvalidCommand(command);
                return;
            }

            string Image = _DataCaller.callImages(reskin[0], tfClass, true);

            var builder = new EmbedBuilder()
                .WithColor(new Color(255, 140, 0))
                .WithTitle($"Your rolled reskin is..")
                .WithDescription($"Weapon: {reskin[0]} \n This replaces the {reskin[1]} used by the {tfClass}")
                .WithImageUrl(Image)
                .AddField("Missing a Reskin?", "Some reskins such as festived, botkillers and australium are not part of this command." +
                "And others aren't included since they can be acquired freely through the game itself."+
                "The reskins rolled here are ones that can't be acquired normally by just playing the game.")
                .WithFooter(footer => footer.Text = "enhance your Tf2 experience!");

            var response = builder.Build();

            await command.RespondAsync(embed: response);
        }

        //Will be called if commands are invalid
        public async Task InvalidCommand(SocketSlashCommand command)
        {
            var builder = new EmbedBuilder()
                .WithColor(new Color(139, 0, 0))
                .WithTitle($"Dagnabbit! another impractical request...")
                .WithDescription($"Oops! looks like you gave an invalid option. Please use the options listed" +
                $" in /show-guide ") // ***
                .WithFooter(footer => footer.Text = "enhance your Tf2 experience!");

            var response = builder.Build();

            await command.RespondAsync(embed: response);
        }
    }
}
