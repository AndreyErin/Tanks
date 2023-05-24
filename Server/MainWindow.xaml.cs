using Server.Model;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System;
using System.Windows;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Text;
using System.ComponentModel;

namespace Server
{
    public partial class MainWindow : Window
    {
        public delegate void gEvent(GameEnum gameEvent);
        public event gEvent? GameEvent;

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
            //загружаем все имена карт из папки Maps
            mapPool = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Maps", "*.json");

            mapPool.OrderBy(x => x.ToString());

            //настраеваем таймер респавна ботов-танков
            tTimer_RespawnBotTank.Elapsed += TTimer_RespawnBotTank_Elapsed;
            tTimer_RespawnBotTank.EndInit();

            GlobalDataStatic.PartyTanksOfPlayers.Add(new TankPlayer(new MyPoint(0, 0)));
            GlobalDataStatic.PartyTanksOfPlayers.Add(new TankPlayer(new MyPoint(0, 0)));

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
                    Task.Run(() => new Client(newClient, ++clientNumber)); //создаем класс клиента
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
                    break;
                case ComandEnum.NewRaund:
                    break;
                case ComandEnum.Replay:
                    break;
                case ComandEnum.Out:
                    break;
                case ComandEnum.Ready:
                    break;
                case ComandEnum.MoveUp:
                    break;
                case ComandEnum.MoveDown:
                    break;
                case ComandEnum.MoveLeft:
                    break;
                case ComandEnum.MoveRight:
                    break;
                case ComandEnum.Stop:
                    break;
                case ComandEnum.Fire:
                    break;

            }

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


                GlobalDataStatic.PartyTanksOfPlayers[0] = new TankPlayer(map.respawnTankPlayer[0]);
                //GlobalDataStatic.PartyTanksOfPlayers[1] = new TankPlayer(map.respawnTankPlayer[1]);
                GlobalDataStatic.BattleGroundCollection.Add(GlobalDataStatic.PartyTanksOfPlayers[0]);//добавляемся на поле боя
                //GlobalDataStatic.BattleGroundCollection.Add(GlobalDataStatic.PartyTanksOfPlayers[1]);//добавляемся на поле боя

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

        public void CreatePlayerTank()
        {
            
        }


        /////////////////////////////////////////////////////////////////
        //отлавливаем изменения в коллекции поля боя
        protected void ChangedBattleGround(object? sender, NotifyCollectionChangedEventArgs e)
        {
            string commandString = "";
            switch (e.Action)
            {
                //при добавление объкта будем подписываться
                case NotifyCollectionChangedAction.Add:
                    //MessageBox.Show("добавление новых объектов в коллекцию");
                    ((WorldElement)e.NewItems[0]).PropertyChanged += ChangedElement;
                    //если элемент может издавать звуки
                    if (e.NewItems[0] is ISoundsObjects)
                    {
                        ((ISoundsObjects)e.NewItems[0]).SoundEvent += Sounds;
                    }
                    commandString = $"ADD@{((WorldElement)e.NewItems[0]).ID}@{((WorldElement)e.NewItems[0]).ePos.X}@{((WorldElement)e.NewItems[0]).ePos.Y}@{(int)(((WorldElement)e.NewItems[0]).Skin)}^";
                    //MessageBox.Show("добавление новых объектов в коллекцию\n" + commandString);
                    break;

                //при удаление объекта будем отписываться
                case NotifyCollectionChangedAction.Remove:
                    ((WorldElement)e.NewItems[0]).PropertyChanged -= ChangedElement;
                    //если элемент может издавать звуки
                    if (e.NewItems[0] is ISoundsObjects)
                    {
                        ((ISoundsObjects)e.NewItems[0]).SoundEvent -= Sounds;
                    }
                    commandString = $"REMOVE@{((WorldElement)e.OldItems[0]).ID}^";
                    break;
            }
            byte[] data = Encoding.UTF8.GetBytes(commandString);
            SetDataAsynk(data);
        }

        //отлавливаем изменения в конкретных элементах
        protected void ChangedElement(object? sender, PropertyChangedEventArgs e)
        {
            string commandString = "";
            switch (e.PropertyName?.ToUpper())
            {
                case "SKIN":
                    commandString = $"SKIN@{((WorldElement)sender).ID}@{((WorldElement)sender).Skin}^";
                    break;
                case "X":
                    commandString = $"X@{((WorldElement)sender).ID}@{((WorldElement)sender).ePos.X}^";
                    break;
                case "Y":
                    commandString = $"Y@{((WorldElement)sender).ID}@{((WorldElement)sender).ePos.Y}^";
                    break;
            }

            byte[] data = Encoding.UTF8.GetBytes(commandString);
            SetDataAsynk(data);

        }
    }
}
