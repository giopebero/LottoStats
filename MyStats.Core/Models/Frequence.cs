using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyStats.Core.Models
{
    public class Frequence
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Wheel Wheel { get; set; }
        public int Number { get; set; }
        public int Draws { get; set; }
    }
}