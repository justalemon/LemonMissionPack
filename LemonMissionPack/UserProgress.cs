using Newtonsoft.Json;

namespace LemonMissionPack
{
    public class UserProgress
    {
        [JsonProperty("mission_01")]
        public double Mission01 { get; set; }
        [JsonProperty("mission_02")]
        public double Mission02 { get; set; }
    }
}
