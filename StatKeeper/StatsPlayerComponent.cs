using Rocket.Core.Assets;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.IO;

namespace Batt.StatTracker {
    public class StatsPlayerComponent : UnturnedPlayerComponent {
        public XMLFileAsset<Stats> Stats;
        public void Start() {
            Stats = new XMLFileAsset<StatTracker.Stats>(Path.Combine(Plugin.Instance.Directory,"Stats/"+Player.Id+".xml"));
            Stats.Instance.SteamID = (ulong)Player.CSteamID;

            //TODO: Add tracking of acc when player is in combat

            Player.Events.OnDeath += (UnturnedPlayer player, SDG.Unturned.EDeathCause cause, SDG.Unturned.ELimb limb, Steamworks.CSteamID m) => {
                UnturnedPlayer murderer = UnturnedPlayer.FromCSteamID(m);

                if (murderer != null && PlayerTool.getSteamPlayer(m) != null && cause != SDG.Unturned.EDeathCause.SUICIDE) {
                    XMLFileAsset<Stats> killerStats = murderer.GetComponent<StatsPlayerComponent>().Stats;

                    if (limb == SDG.Unturned.ELimb.SKULL) {
                        killerStats.Instance.HeadShots += 1;
                    }

                    killerStats.Instance.Kills += 1;
                    killerStats.Instance.RecalculateStats();
                    killerStats.Save();
                }

                Stats.Instance.Deaths += 1;
                Stats.Instance.RecalculateStats();
                Stats.Save();
                Plugin.Instance.UpdateRank();
            };
            Stats.Instance.RecalculateStats();
            Plugin.Instance.UpdateRank();
        }

        public void OnDisable() {
            Stats.Instance.RecalculateStats();
            Stats.Save();
        }
    }
}
