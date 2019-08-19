using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemonMissionPack.Missions
{
    public class MissionBase : Script
    {
        /// <summary>
        /// If the player has been notified about the mission.
        /// </summary>
        public static bool IsPlayerNotified { get; set; } = false;
        /// <summary>
        /// If the mission is currently in progress.
        /// </summary>
        public static bool IsInProgress { get; set; } = false;
        /// <summary>
        /// The current blip used to mark mission objectives.
        /// </summary>
        public static Blip MissionBlip { get; set; }
    }
}
