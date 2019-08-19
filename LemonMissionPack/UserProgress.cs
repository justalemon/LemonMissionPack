using Newtonsoft.Json;

namespace LemonMissionPack
{
    public class UserProgress
    {
        [JsonProperty("mission_01")]
        public int Mission01 { get; set; }
    }
}
