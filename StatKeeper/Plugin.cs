using System.Collections.Generic;
using Rocket.Core.Assets;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Rocket.Core.Plugins;
using System.IO;
using System;
using SDG.Unturned;
using Steamworks;
using System.Xml;

namespace Batt.StatKeeper {
	public class Plugin : RocketPlugin<Configuration> {
		public static Plugin Instance;
		List<string> onlinePlayers = new List<string>();

		public List<string> Aliases {
			get { return new List<string> { "stats" }; }
		}

		protected override void Load() {
			Instance = this;
			Logger.Log("StatKeeper has been loaded!", ConsoleColor.White);
			System.IO.Directory.CreateDirectory(Path.Combine(Directory, "Stats/"));

			U.Events.OnPlayerConnected += EventOnPlayerConnected;
			U.Events.OnPlayerDisconnected += EventOnPlayerDisconnected;
		}

		private void EventOnPlayerConnected(UnturnedPlayer player) {
			UpdateStats(player);

			onlinePlayers.Add(player.CSteamID.ToString());
		}

		public void UpdateStats(UnturnedPlayer player) {
			XMLFileAsset<Stats> PlayerStats = new XMLFileAsset<StatKeeper.Stats>(Path.Combine(Instance.Directory, "Stats/" + player.CSteamID + ".xml"));

			EffectManager.sendUIEffect((ushort)9879, (short)9884, player.CSteamID, true, PlayerStats.Instance.Rank.ToString());
			EffectManager.sendUIEffect((ushort)9880, (short)9885, player.CSteamID, true, PlayerStats.Instance.Kills.ToString());
			EffectManager.sendUIEffect((ushort)9881, (short)9886, player.CSteamID, true, PlayerStats.Instance.Deaths.ToString());
			EffectManager.sendUIEffect((ushort)9882, (short)9887, player.CSteamID, true, PlayerStats.Instance.KDRatio.ToString());
			EffectManager.sendUIEffect((ushort)9883, (short)9888, player.CSteamID, true, PlayerStats.Instance.HeadShots.ToString());
		}

		public void UpdateRank() {
			List<string> RankList = new List<string>();
			List<string> FilePaths = new List<string>();
			string SteamIDOne = "";
			string SteamIDTwo = "";
			int TempValOne = 0;
			int TempValTwo = 0;

			foreach (string file in System.IO.Directory.EnumerateFiles(Path.Combine(Instance.Directory, "Stats/"))) {
				FilePaths.Add(file);
			}

			while (FilePaths.Count > 0) {
				if (FilePaths.Count > 1) {
					for (int i = 0; i < (FilePaths.Count - 1); i++) {
						XmlDocument doc = new XmlDocument();
						doc.Load(FilePaths[i]);

						XmlNodeList kills = doc.GetElementsByTagName("Kills");
						XmlNodeList deaths = doc.GetElementsByTagName("Deaths");
						XmlNodeList steamID = doc.GetElementsByTagName("SteamID");

						if (kills.Count > 0 && deaths.Count > 0) {
							TempValOne = int.Parse(kills[0].InnerXml) + int.Parse(deaths[0].InnerXml);
							SteamIDOne = steamID[0].InnerXml;
						}

						doc.Load(FilePaths[i + 1]);

						kills = doc.GetElementsByTagName("Kills");
						deaths = doc.GetElementsByTagName("Deaths");
						steamID = doc.GetElementsByTagName("SteamID");

						if (kills.Count > 0 && deaths.Count > 0) {
							TempValTwo = int.Parse(kills[0].InnerXml) + int.Parse(deaths[0].InnerXml);
							SteamIDTwo = steamID[0].InnerXml;
						}

						if (TempValOne > TempValTwo) {
							RankList.Add(SteamIDTwo);
							FilePaths.RemoveAt(i + 1);
						} else {
							RankList.Add(SteamIDOne);
							FilePaths.RemoveAt(i);
						}
					}
				} else {
					XmlDocument doc = new XmlDocument();
					doc.Load(FilePaths[0]);

					XmlNodeList steamID = doc.GetElementsByTagName("SteamID");

					RankList.Add(steamID[0].InnerXml);
					FilePaths.RemoveAt(0);
				}
			}

			for (int i = 0; i < RankList.Count; i++) {
				XMLFileAsset<Stats> PlayerStats = new XMLFileAsset<StatKeeper.Stats>(Path.Combine(Instance.Directory, "Stats/" + RankList[i] + ".xml"));

				PlayerStats.Instance.Rank = i + 1;
				PlayerStats.Save();
			}

			// Find way to sort through players without adding their ID to a list
			for(int i = 0; i < onlinePlayers.Count; i++) {
				CSteamID cSteamID = (CSteamID)Convert.ToUInt64(onlinePlayers[i]);
				UnturnedPlayer player = UnturnedPlayer.FromCSteamID(cSteamID);

				UpdateStats(player);
			}
		}

		private void EventOnPlayerDisconnected(UnturnedPlayer player) {
			EffectManager.askEffectClearByID((ushort)9879, player.CSteamID);
			EffectManager.askEffectClearByID((ushort)9880, player.CSteamID);
			EffectManager.askEffectClearByID((ushort)9881, player.CSteamID);
			EffectManager.askEffectClearByID((ushort)9882, player.CSteamID);
			EffectManager.askEffectClearByID((ushort)9883, player.CSteamID);

			for(int i = 0; i < onlinePlayers.Count; i++) {
				if(player.CSteamID.ToString() == onlinePlayers[i]) {
					onlinePlayers.RemoveAt(i);
					i = onlinePlayers.Count;
				}
			}
		}

		protected override void Unload() {
			Instance = null;
			U.Events.OnPlayerConnected -= EventOnPlayerConnected;
			U.Events.OnPlayerConnected -= EventOnPlayerDisconnected;
		}
	}
}

