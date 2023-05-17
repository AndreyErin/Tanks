using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace Server.Model
{

    public class Map
    {
        //все открытые свойства будут сериализованны по умолчанию (простые поля не будут)
        [JsonInclude]
        public List<MyPoint> rockBlocs { get; set; } = new List<MyPoint>();
        [JsonInclude]
        public List<MyPoint> ironBlocs { get; set; } = new List<MyPoint>();
        [JsonInclude]
        public List<MyPoint> woodBlocs { get; set; } = new List<MyPoint>();
        [JsonInclude]
        public List<MyPoint> friendlyRockBlocs { get; set; } = new List<MyPoint>();
        [JsonInclude]
        public List<MyPoint>? respawnTankPlayer { get; set; } = new List<MyPoint>();
        [JsonInclude]
        public List<MyPoint> respawnTankBots { get; set; } = new List<MyPoint>();
        [JsonInclude]
        public List<MyPoint> LocationGun { get; set; } = new List<MyPoint>();
        [JsonInclude]
        public bool BunkerON { get; set; } = false;
        [JsonInclude]
        public MyPoint BunkerPos { get; set; }
        [JsonInclude]
        public bool BunkerEnamyON { get; set; } = false;
        [JsonInclude]
        public MyPoint BunkerEnamyPos { get; set; }
        [JsonInclude]
        public int RespawnEnamyCount { get; set; } = 10;
    }
}
