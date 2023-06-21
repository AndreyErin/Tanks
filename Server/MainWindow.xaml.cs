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
        public bool? _lastGameIsWin = null;

        //проверяем у нас мультиплеерная игра или нет
        public bool IsMultiPlayer = false;

        //таймер очищения очереди
        public System.Timers.Timer TimerQueueCler = new System.Timers.Timer(25);
        //общий таймер для все движущихся объектов
        public System.Timers.Timer GlobalTimerMove = new System.Timers.Timer(15);

        public delegate void gEvent(GameEnum gameEvent);
        public event gEvent? GameEvent;
        public delegate void eEvent(ElementEventEnum elementEvent, int id, double x = -10, double y = -10, SkinsEnum skin = SkinsEnum.None, VectorEnum vector = VectorEnum.Top, string bigStringMessage = "");
        public event eEvent? ElementEvent;
        public delegate void cdMessage();
        public delegate void sEvent(SoundsEnum sound);
        public event sEvent? SoundEvent;
        
        public Map? map;
        int lvlMap = 0;
        string[] mapPool;
        
        private System.Timers.Timer tTimer_RespawnBotTank = new System.Timers.Timer();
        private int countTimerRespawn = 0;

        public MainWindow()
        {
            InitializeComponent();         
        }

        //загрузка программы
        private void MainWin_Loaded(object sender, RoutedEventArgs e)
        {

            GlobalDataStatic.Controller = this;

            //заполняем подготовленные объекты
            for (int i = 0; i < 300; i++)
            {
                GlobalDataStatic.StackBlocksFerum.Push(new BlockFerum());
                
                GlobalDataStatic.StackBlocksRock.Push(new BlockRock());
                //GlobalDataStatic.StackTankBot.Push(new TankBot());
                GlobalDataStatic.StackTree.Push(new Tree());                                               
            }

            //заполняем подготовленные танки-боты
            for (int i = 0; i < 32; i++)
            {              
                GlobalDataStatic.StackTankBot.Push(new TankBot());               
            }

            for (int i = 0; i < 50; i++)
            {
                GlobalDataStatic.StackLoot.Push(new Loot());
                GlobalDataStatic.StackTankOfDistroy.Push(new TankOfDistroy());
                GlobalDataStatic.StackBullet.Push(new Bullet());
                GlobalDataStatic.StackLocationGun.Push(new LocationGun());
            }


            //загружаем все имена карт из папки Maps
            mapPool = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Maps", "*.json");

            mapPool.OrderBy(x => x.ToString());

            //настраеваем таймер респавна ботов-танков
            tTimer_RespawnBotTank.Elapsed += TTimer_RespawnBotTank_Elapsed;
            
            //таймер отправки сообщений клиенту
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

                    //если оба слота занято то игнорим новые подключения
                    if (PartyPlayers.One != null && PartyPlayers.Two != null) break;

                    new Client(newClient); //создаем класс клиента
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сервер не был запущен.\n" + ex.Message);
            }
        }

        //получаем команды от клиента
        public void GetCommandsOfClient(ComandEnum comandEnum, TankPlayer? tank = null ) 
        {
            switch (comandEnum)
            {
                case ComandEnum.NewGame:
                    
                        NewGame();
                    break;                    
                case ComandEnum.NewRaund:
                    if (IsMultiPlayer)
                        NewRaundMultyPlayer();
                    else
                        NewRaund();
                    break;
                case ComandEnum.Replay:
                    if (IsMultiPlayer)
                        ReplayRaundMultyPlayer();
                    else
                        ReplayRaund();
                    break;
                case ComandEnum.Out:
                    MainWin.Close();    
                    break;
                    
                case ComandEnum.Ready:
                    if (PartyPlayers.One.Ready)                    
                        GameEvent?.Invoke(GameEnum.PlayerOneReady);
                    else
                        GameEvent?.Invoke(GameEnum.PlayerOneNotReady);

                    //если второй игрок существует
                    if (PartyPlayers.Two != null)
                    {
                        if (PartyPlayers.Two.Ready)
                            GameEvent?.Invoke(GameEnum.PlayerTwoReady);
                        else
                            GameEvent?.Invoke(GameEnum.PlayerTwoNotReady);

                        //если оба готовы то запускаем мультиплеерную игру
                        if (PartyPlayers.One.Ready == true && PartyPlayers.Two.Ready == true)
                        {
                            switch (_lastGameIsWin)
                            {
                                case null:
                                    NewGameMultyPlayer();
                                    break;
                                case true:
                                    NewRaundMultyPlayer();
                                    break;
                                case false:
                                    ReplayRaundMultyPlayer();
                                    break;
                            }
                            
                        }
                    }
                                                           
                    break;
                case ComandEnum.MoveUp:
                    tank.Move(VectorEnum.Top);
                    break;
                case ComandEnum.MoveDown:
                    tank.Move(VectorEnum.Down);
                    break;
                case ComandEnum.MoveLeft:
                    tank.Move(VectorEnum.Left);
                    break;
                case ComandEnum.MoveRight:
                    tank.Move(VectorEnum.Right);
                    break;
                case ComandEnum.Stop:
                    tank.Stop();
                    break;
                case ComandEnum.Fire:
                    tank.ToFire();
                    break;
            }
        }

        //таймер респавна танков-ботов
        private void TTimer_RespawnBotTank_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Action action = () =>
            {
            

            //ограничение по числу танков на поле одновременно
            if (GlobalDataStatic.PartyTankBots.Count >= 30) { return; }

            tTimer_RespawnBotTank.Interval = 10000;
            GlobalDataStatic.RespawnBotON = true;

            //загрузка танков-ботов
            foreach (MyPoint point in map.respawnTankBots)
            {
                TankBot tankBot = GlobalDataStatic.StackTankBot.Pop();
                tankBot.InitElement(point);
                tankBot.DistroyEvent += DistroyEnemyTank;
                GlobalDataStatic.PartyTankBots.Add(tankBot);
            }

            if (++countTimerRespawn >= map.RespawnEnamyCount)
            {
                tTimer_RespawnBotTank.Stop();
                GlobalDataStatic.RespawnBotON = false;
            }
            };
            GlobalDataStatic.Controller?.Dispatcher.Invoke(action);
        }

        //загрузка элементов окружения 
        private void CreateWorldElements(string fileMapName)
        {
            GlobalDataStatic.BattleGroundCollection.Clear();

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
                    GlobalDataStatic.StackBlocksRock.Pop().InitElement(pos);
                }
                //железные блоки
                foreach (MyPoint pos in map.ironBlocs)
                {
                    GlobalDataStatic.StackBlocksFerum.Pop().InitElement(pos);
                }
                //деревья
                foreach (MyPoint pos in map.woodBlocs)
                {
                    GlobalDataStatic.StackTree.Pop().InitElement(pos);
                }
                //дружеские каменные блоки
                foreach (MyPoint pos in map.friendlyRockBlocs)
                {
                    GlobalDataStatic.StackBlocksRock.Pop().InitElement(pos, true);                   
                }
                //локальные пушки
                foreach (MyPoint pos in map.LocationGun)
                {
                    GlobalDataStatic.StackLocationGun.Pop().InitElement(pos, 3);
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
                PartyPlayers.One.tank = GlobalDataStatic.PartyTanksOfPlayers[0];

                //подписываемся
                foreach (TankPlayer tanksPlayer in GlobalDataStatic.PartyTanksOfPlayers)
                {
                    tanksPlayer.DestroyPayerTank += DistroyFriendlyTank;
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Новая игра" + ex.Message);
            }

            PartyPlayers.One.UploadHpPlayers();
            //передача состояния объектов
            TimerQueueCler.Start();

            GlobalTimerMove = new System.Timers.Timer(15);
            GlobalTimerMove.Start();

            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();
        }

        //следующий раунд
        public void NewRaund()
        {
            //TimerQueueCler.Start();

            GameEvent?.Invoke(GameEnum.NewRound);

            //заполняем карту элементами мира следующего уровня
            CreateWorldElements(mapPool[lvlMap]);
                      
            GlobalDataStatic.PartyTanksOfPlayers.Clear();
            if (PartyPlayers.One.tank.HP > 0)
                PartyPlayers.One.tank.UploadTank(map.respawnTankPlayer[0]);
            else
                PartyPlayers.One.tank = new TankPlayer(map.respawnTankPlayer[0]);

            GlobalDataStatic.PartyTanksOfPlayers.Add(PartyPlayers.One.tank);

            //подписываемся
            foreach (TankPlayer tanksPlayer in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tanksPlayer.DestroyPayerTank += DistroyFriendlyTank;
            }

            PartyPlayers.One.UploadHpPlayers();

            //передача состояния объектов
            TimerQueueCler.Start();

            GlobalTimerMove = new System.Timers.Timer(15);
            GlobalTimerMove.Start();

            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();
        }

        //повторяем раунд при проигрыше 
        public void ReplayRaund()
        {
            GameEvent?.Invoke(GameEnum.ReplayRound);

            //заполняем карту элементами мира следующего уровня
            CreateWorldElements(mapPool[lvlMap]);

            GlobalDataStatic.PartyTanksOfPlayers.Clear();
            PartyPlayers.One.tank = new TankPlayer(map.respawnTankPlayer[0]);           
            GlobalDataStatic.PartyTanksOfPlayers.Add(PartyPlayers.One.tank);

            //подписываемся
            foreach (TankPlayer tanksPlayer in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tanksPlayer.DestroyPayerTank += DistroyFriendlyTank;
            }

            PartyPlayers.One.UploadHpPlayers();

            //передача состояния объектов
            TimerQueueCler.Start();

            GlobalTimerMove = new System.Timers.Timer(15);
            GlobalTimerMove.Start();

            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();
        }

        //мультиплеер начальное меню - новая игра
        public void NewGameMultyPlayer()
        {
            //делаем отметку, что игра наша мультиплеерная
            IsMultiPlayer = true;

            //сбрасываем флаги готовности
            PartyPlayers.One.Ready = false;
            PartyPlayers.Two.Ready = false;

            

            try
            {
                GameEvent?.Invoke(GameEnum.NewGameMultiPlayer);

                //создаем элементы окружения
                CreateWorldElements(mapPool[lvlMap]);

                GlobalDataStatic.PartyTanksOfPlayers.Clear();
                //GlobalDataStatic.PartyTanksOfPlayers.Add(new TankPlayer(map.respawnTankPlayer[0]));                                
                //GlobalDataStatic.PartyTanksOfPlayers.Add(new TankPlayer(map.respawnTankPlayer[1]));

                //PartyPlayers.One.tank = GlobalDataStatic.PartyTanksOfPlayers[0];
                //PartyPlayers.Two.tank = GlobalDataStatic.PartyTanksOfPlayers[1];

                PartyPlayers.One.tank = new TankPlayer(map.respawnTankPlayer[0]);
                PartyPlayers.Two.tank = new TankPlayer(map.respawnTankPlayer[1], false);
                GlobalDataStatic.PartyTanksOfPlayers.Add(PartyPlayers.One.tank);
                GlobalDataStatic.PartyTanksOfPlayers.Add(PartyPlayers.Two.tank);

                //подписываемся
                foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
                {
                    tank.DestroyPayerTank += DistroyFriendlyTank;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Новая игра Мультиплеер" + ex.Message);
            }

            PartyPlayers.One.UploadHpPlayers();
            PartyPlayers.Two.UploadHpPlayers();

            //передача состояния объектов
            TimerQueueCler.Start();

            GlobalTimerMove = new System.Timers.Timer(15);
            GlobalTimerMove.Start();

            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();
        }

        //мультиплеер следующий раунд
        public void NewRaundMultyPlayer()
        {
            //сбрасываем флаги готовности
            PartyPlayers.One.Ready = false;
            PartyPlayers.Two.Ready = false;

            GameEvent?.Invoke(GameEnum.NewRoundMultiPlayer);

            //заполняем карту элементами мира следующего уровня
            CreateWorldElements(mapPool[lvlMap]);

            
            //обновляем танки и добавляем в пати
            if (PartyPlayers.One.tank.HP > 0)
                PartyPlayers.One.tank.UploadTank(map.respawnTankPlayer[0]);
            else
                PartyPlayers.One.tank = new TankPlayer(map.respawnTankPlayer[0]);

            if (PartyPlayers.Two.tank.HP > 0)
                PartyPlayers.Two.tank.UploadTank(map.respawnTankPlayer[1]);
            else
                PartyPlayers.Two.tank = new TankPlayer(map.respawnTankPlayer[1], false);

            GlobalDataStatic.PartyTanksOfPlayers.Clear();
            GlobalDataStatic.PartyTanksOfPlayers.Add(PartyPlayers.One.tank);
            GlobalDataStatic.PartyTanksOfPlayers.Add(PartyPlayers.Two.tank);

            //подписываемся
            foreach (TankPlayer tanksPlayer in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tanksPlayer.DestroyPayerTank += DistroyFriendlyTank;
            }


            PartyPlayers.One.UploadHpPlayers();
            PartyPlayers.Two.UploadHpPlayers();

            //передача состояния объектов
            TimerQueueCler.Start();

            GlobalTimerMove = new System.Timers.Timer(15);
            GlobalTimerMove.Start();

            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();
        }

        //повторяем раунд при проигрыше 
        public void ReplayRaundMultyPlayer()
        {
            //сбрасываем флаги готовности
            PartyPlayers.One.Ready = false;
            PartyPlayers.Two.Ready = false;

            GameEvent?.Invoke(GameEnum.ReplayRoundMultiPlayer);

            //заполняем карту элементами мира следующего уровня
            CreateWorldElements(mapPool[lvlMap]);
           
            GlobalDataStatic.PartyTanksOfPlayers.Clear();

            PartyPlayers.One.tank = new TankPlayer(map.respawnTankPlayer[0]);
            PartyPlayers.Two.tank = new TankPlayer(map.respawnTankPlayer[1], false);
            GlobalDataStatic.PartyTanksOfPlayers.Add(PartyPlayers.One.tank);
            GlobalDataStatic.PartyTanksOfPlayers.Add(PartyPlayers.Two.tank);


            //подписываемся
            foreach (TankPlayer tanksPlayer in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tanksPlayer.DestroyPayerTank += DistroyFriendlyTank;
            }

            PartyPlayers.One.UploadHpPlayers();
            PartyPlayers.Two.UploadHpPlayers();

            //передача состояния объектов
            TimerQueueCler.Start();

            GlobalTimerMove = new System.Timers.Timer(15);
            GlobalTimerMove.Start();

            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();
        }

        //уничтожение танков-ботов
        private void DistroyEnemyTank(TankBot tankBot)
        {
            GlobalDataStatic.PartyTankBots.Remove(tankBot);//удаляем танк из пачки вражеский танков
            //если вражеские танки уничтожены все и новых не будет, то -
            //ПОБЕДА
            if ((GlobalDataStatic.PartyTankBots.Count == 0) && (GlobalDataStatic.RespawnBotON == false))
            {              
                tTimer_RespawnBotTank.Stop();
                GlobalDataStatic.IdNumberElement = 0;
                //отписываемся
                foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
                {
                    tank.DestroyPayerTank -= DistroyFriendlyTank;
                }

                //передача состояния объектов
                TimerQueueCler.Stop();
                GlobalTimerMove.Stop();

                _lastGameIsWin = true;

                //очищаем поле
                RemoteAllElement();
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
                
                tTimer_RespawnBotTank.Stop();
                GlobalDataStatic.IdNumberElement = 0;

                _lastGameIsWin = false;

                //передача состояния объектов
                TimerQueueCler.Stop();
                GlobalTimerMove.Stop();
                //очищаем поле
                RemoteAllElement();
                GlobalDataStatic.BattleGroundCollection.Clear();

                GameEvent?.Invoke(GameEnum.DistroyFriendlyTank);
            }
        }

        //уничтожен бункер
        private void DestroyBunker()
        {
            
            tTimer_RespawnBotTank.Stop();
            GlobalDataStatic.IdNumberElement = 0;
            //отписываемся
            foreach (TankPlayer tank in GlobalDataStatic.PartyTanksOfPlayers)
            {
                tank.DestroyPayerTank -= DistroyFriendlyTank;
            }

            _lastGameIsWin = false;

            //передача состояния объектов
            TimerQueueCler.Stop();
            GlobalTimerMove.Stop();
            //очищаем поле
            RemoteAllElement();           
            GlobalDataStatic.BattleGroundCollection.Clear();

            GameEvent?.Invoke(GameEnum.DestroyBunker);
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

            //передача состояния объектов
            TimerQueueCler.Stop();
            GlobalTimerMove.Stop();

            _lastGameIsWin = true;

            //очищаем поле
            RemoteAllElement();         
            GlobalDataStatic.BattleGroundCollection.Clear();

            if (mapPool.Length > (++lvlMap))//если раунды еще будут
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
            foreach (var worldElement in GlobalDataStatic.BattleGroundCollection.ToList()) 
            {
                //останавливаем отсылку сообщений в объекте и удаляем его из коллекции
                worldElement.Value.MessageStopAndRemoveMe();
            }
        }

        //передача звуков клиенту
        public void SoundOfElement(SoundsEnum sound) 
        {
            SoundEvent?.Invoke(sound);
        }

        //отлавливаем изменения в конкретных элементах        
        public void ChangedElement(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName?.ToUpper())
            {
                case "FRAGS":
                    string data;

                    if (PartyPlayers.One.tank == ((TankPlayer)sender))
                        data = $"1@{((TankPlayer)sender)._myFrags.lvl1}@{((TankPlayer)sender)._myFrags.lvl2}@{((TankPlayer)sender)._myFrags.lvl3}@{((TankPlayer)sender)._myFrags.lvl4}@{((TankPlayer)sender)._myFrags.lvlSpeed1}@{((TankPlayer)sender)._myFrags.lvlSpeed2}@{((TankPlayer)sender)._myFrags.LocationGan}";
                    else
                        data = $"2@{((TankPlayer)sender)._myFrags.lvl1}@{((TankPlayer)sender)._myFrags.lvl2}@{((TankPlayer)sender)._myFrags.lvl3}@{((TankPlayer)sender)._myFrags.lvl4}@{((TankPlayer)sender)._myFrags.lvlSpeed1}@{((TankPlayer)sender)._myFrags.lvlSpeed2}@{((TankPlayer)sender)._myFrags.LocationGan}";

                    ElementEvent?.Invoke(ElementEventEnum.Frags, 0, bigStringMessage: data);

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

        //таймер отправки сообщений клиенту
        private void TimerCooldownMessage_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //все изменненые элементы добавляем в сообщение
            foreach (var worldElement in GlobalDataStatic.BattleGroundCollection.Where(w => w.Value.ElementIsChanget == true))
            {
                GlobalDataStatic.BigMessage.Append($"{worldElement.Key}@{worldElement.Value.X}@{worldElement.Value.Y}@{(int)worldElement.Value.Skin}@{(int)worldElement.Value.VectorElement}*");
            }

            string bigString = GlobalDataStatic.BigMessage.ToString();
            ElementEvent?.Invoke(ElementEventEnum.Change, 0, bigStringMessage: bigString);
            GlobalDataStatic.BigMessage.Clear();
        }
    }
}
