using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Booter
{
	[ApiVersion(2, 1)]
    public class Booter : TerrariaPlugin
    {
        public override string Author
            => "Rozen4334";

        public override string Description
            => "A plugin that upgrades the regular kick command with autoreasons & success messages!";

        public override string Name
            => "Booter";

        public override Version Version
            => new Version(1, 0);

        public Booter(Main game) : base(game)
            => Order = 1;

        public override void Initialize()
        {
            var kickCmd = new Command(Permissions.kick, Kick, "kick");

            Commands.ChatCommands.RemoveAll(cmd => cmd.Names.Exists(alias => kickCmd.Names.Contains(alias)));
            Commands.ChatCommands.Add(kickCmd);
        }

        private void Kick(CommandArgs args)
        {
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /kick <player> [reason]");
				args.Player.SendInfoMessage("Use 'lang', 'hack', 'toxic' or 'chat' as reason to automatically add a reason matching this keyword.");
				return;
			}
			if (args.Parameters[0].Length == 0)
			{
				args.Player.SendErrorMessage("Missing player name.");
				return;
			}

			string plStr = args.Parameters[0];
			var players = TSPlayer.FindByNameOrID(plStr);
			if (players.Count == 0)
				args.Player.SendErrorMessage("Invalid player!");
			else if (players.Count > 1)
				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
			else
			{
				string reason = "";
				if (args.Parameters.Count > 1)
				{
					switch (args.Parameters[1])
					{
						case "lang":
							reason = "This is an english only server. Please speak english.";
							break;
						case "hack":
							reason = "Hacking is not allowed on this server.";
							break;
						case "toxic":
							reason = "Toxicity is not allowed on this server!";
							break;
						case "chat":
							reason = "NSFW, Toxic or respectless chat is not allowed. Please communicate with respect.";
							break;
						default:
							reason = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
							break;

					}
				}
				else reason = "Misbehaviour.";

				if (players[0].Kick(reason, !args.Player.RealPlayer, false, args.Player.Name))
					args.Player.SendSuccessMessage($"Kicked {players[0].Name} for: {reason}");
				else args.Player.SendErrorMessage("You can't kick another admin!");
			}
		}
    }
}
