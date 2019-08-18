using Newtonsoft.Json;

namespace LemonMissionPack
{
    /// <summary>
    /// The mission identifiers.
    /// </summary>
    public enum Mission
    {
        None = -1
    }

    public class UserProgress
    {
        /// <summary>
        /// The last completed mission by the player.
        /// </summary>
        [JsonProperty("last_mission")]
        public Mission LastMission { get; set; }
    }
}
