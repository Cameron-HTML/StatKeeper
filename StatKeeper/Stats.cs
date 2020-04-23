using Rocket.API;
using System;

namespace Batt.StatTracker {
    public class Stats : IDefaultable {
        public ulong SteamID { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public DateTime Created { get; set; } = DateTime.Now;

        public int Rank { get; set; } = 0;
        public double KDRatio { get; set; } = 0;
        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public int HeadShots { get; set; } = 0;
        public double Accuracy { get; set; } = 0;

        public void RecalculateStats() {
            if (Deaths != 0)
                KDRatio = Kills / Deaths;
            LastUpdated = DateTime.Now;
        }

        public Stats() { }

        public void LoadDefaults() {
            SteamID = 0;
            LastUpdated = DateTime.Now;
            Created = DateTime.Now;
            Rank = 1;
            Kills = 0;
            Deaths = 0;
            KDRatio = 0;
            HeadShots = 0;
            Accuracy = 0;
        }
    }
}
