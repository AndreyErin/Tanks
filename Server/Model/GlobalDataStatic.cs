
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

//ред
namespace Server.Model
{
    public static class GlobalDataStatic
    {
        //поле боя
        public static ConcurrentDictionary<int ,WorldElement> BattleGroundCollection { get; set; } = new ConcurrentDictionary<int ,WorldElement>();

        public static bool RespawnBotON { get; set; } //будут ли еще появляться танки врага
        public static List<TankPlayer> PartyTanksOfPlayers { get; set; } = new List<TankPlayer>(); //коллекция играков танков
        public static List<TankBot> PartyTankBots { get; set; } = new List<TankBot>(); //коллекция вражеский танков
        public static MainWindow? Controller { get; set; }
        public static bool readyCheck { get; set; } = true; //-----------изменить при добавление 2го игрока
        public static int IdNumberElement { get; set; } = 0;

        
    }
}
