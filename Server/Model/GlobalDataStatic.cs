using System.Collections.Generic;
using System.Collections.ObjectModel;

//ред
namespace Server.Model
{
    public static class GlobalDataStatic
    {
        //поле боя
        public static ObservableCollection<object> BattleGroundCollection = new ObservableCollection<object>();

        public static bool RespawnBotON { get; set; } //будут ли еще появляться танки врага
        public static List<Tank> PartyTanksOfPlayers { get; set; } = new List<Tank>(); //коллекция играков танков
        public static List<TankBot> PartyTankBots { get; set; } = new List<TankBot>(); //коллекция вражеский танков        
    }
}
