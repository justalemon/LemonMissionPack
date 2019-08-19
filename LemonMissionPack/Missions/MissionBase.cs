using GTA;

namespace LemonMissionPack.Missions
{
    public class MissionBase : Script
    {
        /// <summary>
        /// If the player has been notified about the mission.
        /// </summary>
        public bool IsPlayerNotified { get; set; } = false;
        /// <summary>
        /// If the mission is currently in progress.
        /// </summary>
        public bool IsInProgress { get; set; } = false;
        /// <summary>
        /// The current blip used to mark mission objectives.
        /// </summary>
        public Blip MissionBlip { get; set; }
    }
}
