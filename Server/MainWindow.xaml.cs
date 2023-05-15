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
        public TankPlayer mainTank;
        Map? map;
        int lvlMap = 0;
        string[] mapPool;

        private System.Timers.Timer tTimer_RespawnBotTank = new System.Timers.Timer();
        private int countTimerRespawn = 0;

        public MainWindow()
        {
            InitializeComponent();

            GlobalDataStatic.Controller = this;
            //MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());
            //Network network = new Network();
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
        }

        //таймер респавна танков-ботов
        private void TTimer_RespawnBotTank_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tTimer_RespawnBotTank.Interval = 5000;
            GlobalDataStatic.RespawnBotON = true;

                //загрузка танков-ботов
                foreach (Point point in map.respawnTankBots)
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

            //заполняем поле
            try
            {
                //вынаем данные из файла
                string jsonText = File.ReadAllText(fileMapName);
                //запихиваем ети данные в объект
                map = JsonSerializer.Deserialize<Map>(jsonText);


                //каменные блоки
                foreach (System.Windows.Point pos in map.rockBlocs)
                {
                    Block b = new Block(pos);
                }
                //железные блоки
                foreach (System.Windows.Point pos in map.ironBlocs)
                {
                    BlockFerum b = new BlockFerum(pos);
                }
                //деревья
                foreach (System.Windows.Point pos in map.woodBlocs)
                {
                    Tree b = new Tree(pos);
                }
                //дружеские каменные блоки
                foreach (System.Windows.Point pos in map.friendlyRockBlocs)
                {
                    Block b = new Block(pos) { IsPlayer = true };
                }
                //локальные пушки
                foreach (System.Windows.Point pos in map.LocationGun)
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
                MessageBox.Show("Чёйта карта не загрузилась");
                return;
            }
        }

        //двигаем танк из клиента


        private bool cD = false;
        //откат выстрела
        private void CooldownFire()
        {
            cD = true;
            Thread.Sleep(300);
            cD = false;
        }

        //начальное меню - новая игра
        public void NewGame()
        {
            //создаем элементы окружения
            //должны загружаться до респавнов
            CreateWorldElements(mapPool[lvlMap]);

            System.Windows.Point tPos = map.respawnTankPlayer[0];
            //создаем танк игрока
            mainTank = new TankPlayer(tPos);

            mainTank.DestroyPayerTank += DistroyFriendlyTank;
            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();
        }

        //уничтожение танков-ботов ----------------
        private void DistroyEnemyTank(TankBot tankBot)
        {
            GlobalDataStatic.PartyTankBots.Remove(tankBot);//удаляем танк из пачки вражеский танков
        }
        //уничтожены танки игроков----------------
        private void DistroyFriendlyTank(TankPlayer tank)
        {
            if (GlobalDataStatic.PartyTanksOfPlayers.Count == 0)
            {
            /////////конец раунда
            }
        }
        //уничтожен бункер -----------------
        private void DestroyBunker()
        {
 
        }

        //уничтожен вражеский бункер ----------------------------
        private void DestroyBunkerEnamy()
        {
            //
        }

        //следующий раунд-------------------------
        public void NewRaund()
        {
            if (mapPool.Length > (++lvlMap))
            {
                //заполняем карту элементами мира следующего уговня
                CreateWorldElements(mapPool[lvlMap]);

                //запускаем респавн  ботов-танков
                tTimer_RespawnBotTank.Start();


                //добавляем игрока на карту следующего раунда               
                System.Windows.Point tPos = map.respawnTankPlayer[0];
                mainTank.ePos = tPos;
                mainTank.UpdateHpForTeer(); //выравниваем HP по тиру
                GlobalDataStatic.BattleGroundCollection.Add(mainTank);
            }
        }
        //повторяем раунд при проигрыше ----------------------------
        public void LostRaund()
        {
            //заполняем карту элементами мира следующего уровня
            CreateWorldElements(mapPool[lvlMap]);

            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();

            //создаем танк игрока
            mainTank = new TankPlayer(map.respawnTankPlayer[0]);
            mainTank.DestroyPayerTank += DistroyFriendlyTank;
        }

    }
}
