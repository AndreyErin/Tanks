using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows;

//ред
namespace Server.Model
{

    public class Map
    {
        //все открытые свойства будут сериализованны по умолчанию (простые поля не будут)
        [JsonInclude]
        public List<Point> rockBlocs { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> ironBlocs { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> woodBlocs { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> friendlyRockBlocs { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> respawnTankPlayer { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> respawnTankBots { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> LocationGun { get; set; } = new List<Point>();
        [JsonInclude]
        public bool BunkerON { get; set; } = false;
        [JsonInclude]
        public Point BunkerPos { get; set; }
        [JsonInclude]
        public bool BunkerEnamyON { get; set; } = false;
        [JsonInclude]
        public Point BunkerEnamyPos { get; set; }
        [JsonInclude]
        public int RespawnEnamyCount { get; set; } = 10;
    }
}
