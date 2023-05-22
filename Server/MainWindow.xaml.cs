using Server.Model;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System;
using System.Windows;


namespace Server
{
    public partial class MainWindow : Window
    {
        public Network network;

        public delegate void gEvent(GameEnum gameEvent);
        public event gEvent? GameEvent;

        public TankPlayer mainTank;
        public Map? map;
        int lvlMap = 0;
        string[] mapPool;

        private System.Timers.Timer tTimer_RespawnBotTank = new System.Timers.Timer();
        private int countTimerRespawn = 0;

        public MainWindow()
        {
            InitializeComponent();

            GlobalDataStatic.Controller = this;
        }

        //загрузка программы
        private void MainWin_Loaded(object sender, RoutedEventArgs e)
        {
            //загружаем все имена карт из папки Maps
            mapPool = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Maps", "*.json");

            mapPool.OrderBy(x => x.ToString());

            //настраеваем таймер респавна ботов-танков
            tTimer_RespawnBotTank.Elapsed += TTimer_RespawnBotTank_Elapsed;
            tTimer_RespawnBotTank.EndInit();

            //MessageBox.Show("количество загруженных карт: " + mapPool.Count().ToString() );

            network = new Network();
        }

        //таймер респавна танков-ботов
        private void TTimer_RespawnBotTank_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tTimer_RespawnBotTank.Interval = 5000;
            GlobalDataStatic.RespawnBotON = true;

                //загрузка танков-ботов
                foreach (MyPoint point in map.respawnTankBots)
                {
                    TankBot tankBot = new TankBot(point);
                    tankBot.DistroyEvent += DistroyEnemyTank;
                    GlobalDataStatic.PartyTankBots.Add(tankBot);
                }

            if (++countTimerRespawn >= map.RespawnEnamyCount)
            {
                tTimer_RespawnBotTank.Stop();
                GlobalDataStatic.RespawnBotON = false;
            }
        }

        //загрузка элементов окружения 
        private void CreateWorldElements(string fileMapName)
        {
            tTimer_RespawnBotTank.Interval = 1;          
            countTimerRespawn = 0;
            //очищаем поле
            GlobalDataStatic.BattleGroundCollection.Clear();
            GlobalDataStatic.PartyTankBots.Clear();
            GlobalDataStatic.IdNumberElement = 0;

            //заполняем поле
            try
            {
                //вынаем данные из файла
                string jsonText = File.ReadAllText(fileMapName);
                //запихиваем ети данные в объект
                map = JsonSerializer.Deserialize<Map>(jsonText);

                //каменные блоки
                foreach (MyPoint pos in map.rockBlocs)
                {
                    BlockIron b = new BlockIron(pos);
                }
                //железные блоки
                foreach (MyPoint pos in map.ironBlocs)
                {
                    BlockFerum b = new BlockFerum(pos);
                }
                //деревья
                foreach (MyPoint pos in map.woodBlocs)
                {
                    Tree b = new Tree(pos);
                }
                //дружеские каменные блоки
                foreach (MyPoint pos in map.friendlyRockBlocs)
                {
                    BlockIron b = new BlockIron(pos) { IsPlayer = true };
                }
                //локальные пушки
                foreach (MyPoint pos in map.LocationGun)
                {
                    LocationGun b = new LocationGun(pos, 3);
                }
                //бункер игрока
                if (map.BunkerON == true)
                {
                    Bunker b = new Bunker(map.BunkerPos);
                    b.BunkerDestroy += DestroyBunker;
                }
                //бункер врага
                if (map.BunkerEnamyON == true)
                {
                    BunkerEnamy b = new BunkerEnamy(map.BunkerEnamyPos);
                    b.BunkerDestroy += DestroyBunkerEnamy;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Чёйта карта не загрузилась\n" + ex.Message);
                return;
            }
        }

        //начальное меню - новая игра
        public void NewGame()
        {                        
            //создаем элементы окружения
            //должны загружаться до респавнов
            CreateWorldElements(mapPool[lvlMap]);

            GameEvent?.Invoke(GameEnum.NewGame);
            //подписываемся
            foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tank.DestroyPayerTank += DistroyFriendlyTank;
            }
            
            //запускаем респавн  ботов-танков
            //tTimer_RespawnBotTank.Start();
        }

        //уничтожение танков-ботов
        private void DistroyEnemyTank(TankBot tankBot)
        {
            GlobalDataStatic.PartyTankBots.Remove(tankBot);//удаляем танк из пачки вражеский танков
            //если вражеские танки уничтожены все и новых не будет, то -
            //ПОБЕДА
            if ((GlobalDataStatic.PartyTankBots.Count == 0) && (countTimerRespawn == 0) && (GameEvent != null))
            {
                GameEvent(GameEnum.DistroyEnemyTank);
                tTimer_RespawnBotTank.Stop();
                GlobalDataStatic.IdNumberElement = 0;
                //отписываемся
                foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
                {
                    tank.DestroyPayerTank -= DistroyFriendlyTank;
                }
            }
        }

        //уничтожены танки игроков
        private void DistroyFriendlyTank(TankPlayer tank)
        {
            GlobalDataStatic.PartyTanksOfPlayers.Remove(tank);
            //ПОРАЖЕНИЕ
            if (GlobalDataStatic.PartyTanksOfPlayers.Count == 0 && (GameEvent != null))
            {
                GameEvent(GameEnum.DistroyFriendlyTank);
                tTimer_RespawnBotTank.Stop();
                GlobalDataStatic.IdNumberElement = 0;
            }
        }

        //уничтожен бункер
        private void DestroyBunker()
        {
            if(GameEvent !=null) GameEvent.Invoke(GameEnum.DestroyBunker);
            tTimer_RespawnBotTank.Stop();
            GlobalDataStatic.IdNumberElement = 0;
            //отписываемся
            foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tank.DestroyPayerTank -= DistroyFriendlyTank;
            }
        }

        //уничтожен вражеский бункер
        private void DestroyBunkerEnamy()
        {
            if (GameEvent != null) GameEvent.Invoke(GameEnum.DestroyBunkerEnamy);
            tTimer_RespawnBotTank.Stop();
            GlobalDataStatic.IdNumberElement = 0;
            //отписываемся
            foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tank.DestroyPayerTank -= DistroyFriendlyTank;
            }
        }

        //следующий раунд
        public void NewRaund()
        {
            if (mapPool.Length > (++lvlMap))
            {
                //заполняем карту элементами мира следующего уговня
                CreateWorldElements(mapPool[lvlMap]);

                if (GameEvent != null) GameEvent.Invoke(GameEnum.NewRound);

                //подписываемся
                foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
                {
                    tank.DestroyPayerTank += DistroyFriendlyTank;
                }

                //запускаем респавн  ботов-танков
                tTimer_RespawnBotTank.Start();
            }
        }

        //повторяем раунд при проигрыше 
        public void ReplayRaund()
        {
            //заполняем карту элементами мира следующего уровня
            CreateWorldElements(mapPool[lvlMap]);

            if (GameEvent != null) GameEvent.Invoke(GameEnum.ReplayRound);
            //подписываемся
            foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tank.DestroyPayerTank += DistroyFriendlyTank;
            }

            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();
        }

    }
}
