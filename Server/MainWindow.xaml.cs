using Server.Model;
using System.IO;
using System.Linq;
using System.Text.Json;
using System;
using System.Windows;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.ComponentModel;


namespace Server
{
    public partial class MainWindow : Window
    {
        //таймер очищения очереди
        public System.Timers.Timer TimerQueueCler = new System.Timers.Timer(50);
        //общий таймер для все движущихся объектов
        public System.Timers.Timer GlobalTimerMove = new System.Timers.Timer(15);

        public delegate void gEvent(GameEnum gameEvent);
        public event gEvent? GameEvent;
        public delegate void eEvent(ElementEventEnum elementEvent, int id, double x = -10, double y = -10, SkinsEnum skin = SkinsEnum.None, VectorEnum vector = VectorEnum.Top);
        public event eEvent? ElementEvent;
        public delegate void cdMessage();
        public event Action CooldownMessage;

        public TankPlayer mainTank;
        public Map? map;
        int lvlMap = 0;
        string[] mapPool;

        public int clientNumber = 0;

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
            GlobalTimerMove.Start();
            TimerQueueCler.Start();

            //загружаем все имена карт из папки Maps
            mapPool = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Maps", "*.json");

            mapPool.OrderBy(x => x.ToString());

            //настраеваем таймер респавна ботов-танков
            tTimer_RespawnBotTank.Elapsed += TTimer_RespawnBotTank_Elapsed;
            tTimer_RespawnBotTank.EndInit();

            TimerQueueCler.Elapsed += TimerCooldownMessage_Elapsed;

            //запускаем функцию прослушивания в отдельном потоке
            Task.Factory.StartNew(() => StartListen());  
        }

        //слушаем входящие подключения
        protected async Task StartListen()
        {
            //сокет для прослушки входящих подключений
            using var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 7071);
            try
            {
                listenSocket.Bind(iPEndPoint);
                listenSocket.Listen();
               
                while (true)
                {
                    var newClient = await listenSocket.AcceptAsync();
                    //Task.Run(() => 
                    new Client(newClient, ++clientNumber);//); //создаем класс клиента
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сервер не был запущен.\n" + ex.Message);
            }
        }

        //получаем команды от клиента
        public void GetCommandsOfClient(ComandEnum comandEnum) 
        {
            switch (comandEnum)
            {
                case ComandEnum.NewGame:
                    if(GlobalDataStatic.readyCheck)
                        NewGame();
                    break;                    
                case ComandEnum.NewRaund:
                    if (GlobalDataStatic.readyCheck)
                        NewRaund();
                    break;
                case ComandEnum.Replay:
                    if (GlobalDataStatic.readyCheck)
                        ReplayRaund();
                    break;
                case ComandEnum.Out:
                    MainWin.Close();    
                    break;
                case ComandEnum.Ready:
                    GlobalDataStatic.readyCheck = true;
                    break;
                case ComandEnum.MoveUp:
                    mainTank.Move(VectorEnum.Top);
                    break;
                case ComandEnum.MoveDown:
                    mainTank.Move(VectorEnum.Down);
                    break;
                case ComandEnum.MoveLeft:
                    mainTank.Move(VectorEnum.Left);
                    break;
                case ComandEnum.MoveRight:
                    mainTank.Move(VectorEnum.Right);
                    break;
                case ComandEnum.Stop:
                    mainTank.Stop();
                    break;
                case ComandEnum.Fire:
                    mainTank.ToFire();
                    break;
            }
        }

        //таймер респавна танков-ботов
        private void TTimer_RespawnBotTank_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //ограничение по числу танков на поле одновременно
            if(GlobalDataStatic.PartyTankBots.Count >= 30) { return; }

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
            try
            {
                GameEvent?.Invoke(GameEnum.NewGame);

                //создаем элементы окружения
                CreateWorldElements(mapPool[lvlMap]);

                GlobalDataStatic.PartyTanksOfPlayers.Add(new TankPlayer(map.respawnTankPlayer[0]));
                //потом убрать-------------------------------------
                mainTank = GlobalDataStatic.PartyTanksOfPlayers[0];

                //подписываемся
                foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
                {
                    tank.DestroyPayerTank += DistroyFriendlyTank;
                }

                //запускаем респавн  ботов-танков
                //tTimer_RespawnBotTank.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Новая игра" + ex.Message);
            }


        }

        //следующий раунд
        public void NewRaund()
        {

                GameEvent?.Invoke(GameEnum.NewRound);

                //заполняем карту элементами мира следующего уровня
                CreateWorldElements(mapPool[lvlMap]);

                //mainTank.ID = GlobalDataStatic.IdNumberElement++;
                //GlobalDataStatic.BattleGroundCollection.Add(mainTank);

                mainTank = new TankPlayer(map.respawnTankPlayer[0]);
                GlobalDataStatic.PartyTanksOfPlayers.Clear();
                GlobalDataStatic.PartyTanksOfPlayers.Add(mainTank);

                //подписываемся
                foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
                {
                    tank.DestroyPayerTank += DistroyFriendlyTank;
                }

                //запускаем респавн  ботов-танков
                tTimer_RespawnBotTank.Start();

        }

        //повторяем раунд при проигрыше 
        public void ReplayRaund()
        {
            GameEvent?.Invoke(GameEnum.ReplayRound);

            //заполняем карту элементами мира следующего уровня
            CreateWorldElements(mapPool[lvlMap]);

            mainTank = new TankPlayer(map.respawnTankPlayer[0]);
            GlobalDataStatic.PartyTanksOfPlayers.Clear();
            GlobalDataStatic.PartyTanksOfPlayers.Add(mainTank);

            //подписываемся
            foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tank.DestroyPayerTank += DistroyFriendlyTank;
            }

            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();
        }

        //уничтожение танков-ботов
        private void DistroyEnemyTank(TankBot tankBot)
        {
            GlobalDataStatic.PartyTankBots.Remove(tankBot);//удаляем танк из пачки вражеский танков
            //если вражеские танки уничтожены все и новых не будет, то -
            //ПОБЕДА
            if ((GlobalDataStatic.PartyTankBots.Count == 0) && (countTimerRespawn == 0))
            {
                
                tTimer_RespawnBotTank.Stop();
                GlobalDataStatic.IdNumberElement = 0;
                //отписываемся
                foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
                {
                    tank.DestroyPayerTank -= DistroyFriendlyTank;
                }

                //очищаем поле
                RemoteAllElement();//////////////////////////////////////////////////////////////////
                GlobalDataStatic.BattleGroundCollection.Clear();

                
                if (mapPool.Length > (++lvlMap))
                {
                    GameEvent?.Invoke(GameEnum.DistroyEnemyTank);
                }
                else
                {
                    GameEvent?.Invoke(GameEnum.Win);
                }
            }
        }

        //уничтожены танки игроков
        private void DistroyFriendlyTank(TankPlayer tank)
        {
            GlobalDataStatic.PartyTanksOfPlayers.Remove(tank);
            //ПОРАЖЕНИЕ
            if (GlobalDataStatic.PartyTanksOfPlayers.Count == 0)
            {
                GameEvent?.Invoke(GameEnum.DistroyFriendlyTank);
                tTimer_RespawnBotTank.Stop();
                GlobalDataStatic.IdNumberElement = 0;

                //очищаем поле
                RemoteAllElement();//////////////////////////////////////////////////////////////////
                GlobalDataStatic.BattleGroundCollection.Clear();
            }
        }

        //уничтожен бункер
        private void DestroyBunker()
        {
            GameEvent?.Invoke(GameEnum.DestroyBunker);
            tTimer_RespawnBotTank.Stop();
            GlobalDataStatic.IdNumberElement = 0;
            //отписываемся
            foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tank.DestroyPayerTank -= DistroyFriendlyTank;
            }

            //очищаем поле
            RemoteAllElement();//////////////////////////////////////////////////////////////////            
            GlobalDataStatic.BattleGroundCollection.Clear();
        }

        //уничтожен вражеский бункер
        private void DestroyBunkerEnamy()
        {
            
            tTimer_RespawnBotTank.Stop();
            GlobalDataStatic.IdNumberElement = 0;
            //отписываемся
            foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tank.DestroyPayerTank -= DistroyFriendlyTank;
            }

            //очищаем поле
            RemoteAllElement();//////////////////////////////////////////////////////////////////            
            GlobalDataStatic.BattleGroundCollection.Clear();

            if (mapPool.Length > (++lvlMap))//если арунды еще будут
            {
                GameEvent?.Invoke(GameEnum.DestroyBunkerEnamy);
            }
            else
            {
                GameEvent?.Invoke(GameEnum.Win);
            }
        }

        //удаляем оставшиеся на карте элементы
        protected void RemoteAllElement() 
        {
            foreach (var worldElement in GlobalDataStatic.BattleGroundCollection) 
            {
                //останавливаем отсылку сообщений в объекте и удаляем его из коллекции
                worldElement.Value.RemoveMe();
            }
        }

            
        //отлавливаем изменения в конкретных элементах
        public void ChangedElement(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName?.ToUpper())
            {
                case "CHANGE":
                    ElementEvent?.Invoke(ElementEventEnum.Change,
                        ((WorldElement)sender).ID,
                        ((WorldElement)sender).X,
                        ((WorldElement)sender).Y,
                        ((WorldElement)sender).Skin,
                        ((WorldElement)sender).VectorElement);
                    //((WorldElement)sender).MessageSetON = false;
                    break;

                case "ADD":                
                    ElementEvent?.Invoke(ElementEventEnum.Add,
                        ((WorldElement)sender).ID,
                        ((WorldElement)sender).X,
                        ((WorldElement)sender).Y,
                        ((WorldElement)sender).Skin,
                        ((WorldElement)sender).VectorElement);
                    break;

                case "REMOVE":
                    int IdOld = ((WorldElement)sender).ID;

                    ElementEvent?.Invoke(ElementEventEnum.Remove, IdOld);
                    break;
            }
        }

        //ограничение на отправку сообщений элементом
        private void TimerCooldownMessage_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CooldownMessage?.Invoke();

        }
    }
}
