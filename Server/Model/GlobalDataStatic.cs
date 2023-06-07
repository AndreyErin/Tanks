
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;


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

        public static StringBuilder BigMessage = new StringBuilder();

        public static Stack<BlockRock> StackBlocksRock = new Stack<BlockRock>();
        public static Stack<BlockFerum> StackBlocksFerum = new Stack<BlockFerum>();
        public static Stack<Bullet> StackBullet = new Stack<Bullet>();
        public static Stack<Loot> StackLoot = new Stack<Loot>();
        public static Stack<TankBot> StackTankBot = new Stack<TankBot>();
        public static Stack<TankOfDistroy> StackTankOfDistroy = new Stack<TankOfDistroy>();
        public static Stack<Tree> StackTree = new Stack<Tree>();
        public static Stack<LocationGun> StackLocationGun = new Stack<LocationGun>();
    }
}
