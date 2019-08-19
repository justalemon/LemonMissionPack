using Newtonsoft.Json;

namespace LemonMissionPack
{
    public class UserProgress
    {
        [JsonProperty("mission_01")]
        public bool Mission01 { get; set; }
    }
}
