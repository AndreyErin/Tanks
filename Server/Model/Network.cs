using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Server.Model
{
    public class Network
    {
        protected int clientCount { get; set; } = 0;
        protected List<clientClass> clientList = new List<clientClass>();

        public Network()
        {
            
            Task.Factory.StartNew(()=> StartListen()); //запускаем функцию прослушивания в отдельном потоке            
        }

        protected async Task StartListen() 
        {
            //сокет для прослушки входящих подключений
            using var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 7071);
            try
            {
                
                listenSocket.Bind(iPEndPoint);
                listenSocket.Listen();

                //MessageBox.Show("Сервер запущен");

                while (true)
                {
                    var newClient = await listenSocket.AcceptAsync();
                    Task.Run(() => new clientClass(newClient, clientList)); //создаем класс клиента
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сервер не был запущен.\n" + ex.Message);
            }
            
        }

        protected void StopServer() 
        {
            //отключаем всех клиентов
            foreach(clientClass cl in clientList) 
            {
                cl.StopClient();
            }
        }


        ///////////////////////////////////клиент///////////////////////////////////////////////////////
        protected class clientClass 
        {
            private Socket client;

            public TankPlayer? tank;

            public bool isFirstClient = false;

            protected clientClass() { }

            //конструктор
            public clientClass(Socket clientSocket, List<clientClass> clientList)
            {
                //MessageBox.Show("на сервере создан класс клиента");

                //определяем клияента как основного(либо ведомого)
                if (clientList.Count == 0) isFirstClient = true;

                clientList.Add(this);//добавляемся в список клиентов
                client = clientSocket;                

                //подписываемся на события в игре
                GlobalDataStatic.Controller.GameEvent += EventOfGame;

                GetDataAsynk();
            }

            //получение данных
            protected async Task GetDataAsynk()
            {
                //MessageBox.Show("на сервере запущено получение данных");-------------------
                List<byte> data = new List<byte>(); //весь пакет данных
                byte[] character = new byte[1];//один байт из данных
                int haveData; //проверка остались ли еще данные
                while (true) 
                {
                    
                    //считываем весь пакет
                    while (true)
                    {
                        haveData = await client.ReceiveAsync(character, SocketFlags.None);
                        // ^ - символ означающий конец  пакета
                        if (haveData == 0 || character[0] == '^') break;//если считаны все данные
                        data.Add(character[0]);
                        
                    }

                    //перевод массива байт в команды от клиента
                    string command = Encoding.UTF8.GetString(data.ToArray());

                    

                    switch (command) 
                    {
                        //Навигация по меню
                        case "NEWGAME":

                            SubscribeForEventsElements(); //подписываемся на события коллекции
                            GlobalDataStatic.Controller?.NewGame();//запускаем новую игру в контроллере


                            break;
                        case "NEWRAUND":
                            GlobalDataStatic.Controller?.NewRaund();
                            break;
                        case "OUT":
                            StopClient();//отключаемся от сервера
                            break;
                        case "REPLAY":
                            GlobalDataStatic.Controller?.ReplayRaund();
                            break;
                        case "READY":
                            GlobalDataStatic.readyCheck = true;
                            break;

                        //Движение
                        case "MOVEUP":
                            tank?.Move(VectorEnum.Top);
                            break;
                        case "MOVEDOWN":
                            tank?.Move(VectorEnum.Down);
                            break;
                        case "MOVELEFT":
                            tank?.Move(VectorEnum.Left);
                            break;
                        case "MOVERIGHT":
                            tank?.Move(VectorEnum.Right);
                            break;
                        case "STOP":
                            tank?.Stop();
                            break;
                                
                        //Стрельба
                        case "FIRE":
                            tank?.ToFire();
                            break;

                    }
                    data.Clear();
                }
            }

            //подписаться на события коллекции
            protected void SubscribeForEventsElements() 
            {
                //подписываемся на событие изменения  на поле боя
                GlobalDataStatic.BattleGroundCollection.CollectionChanged += ChangedBattleGround;

//                //подписываемся на события конкретных элементов
//                var subset = from e in GlobalDataStatic.BattleGroundCollection
//                             where (e as Loot == null) && (e as Tree == null)
//                             select e;
//                foreach (WorldElement worldElement in subset)
//                {
//                    worldElement.PropertyChanged += ChangedElement;
//                    //если элемент может издавать звуки
//                    if (worldElement is ISoundsObjects)
//                    {
//                        ((ISoundsObjects)worldElement).SoundEvent += Sounds;
//                    }
//                }
            }

            //отписаться от оставшихся
            protected void UnSubscribeForEventsElements()
            {
                GlobalDataStatic.BattleGroundCollection.CollectionChanged -= ChangedBattleGround;

                var subset = from e in GlobalDataStatic.BattleGroundCollection
                             where (e as Loot == null) && (e as Tree == null)
                             select e;
                foreach (WorldElement worldElement in subset)
                {
                    worldElement.PropertyChanged -= ChangedElement;
                    //если элемент может издавать звуки
                    if (worldElement is ISoundsObjects)
                    {
                        ((ISoundsObjects)worldElement).SoundEvent -= Sounds;
                    }
                }
            }

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

            //звуки
            protected void Sounds(SoundsEnum sound) 
            {
                string commandString = "";
                switch (sound)
                {

                    case SoundsEnum.bonusSound:
                        commandString = "BONUSSOUND^";
                        break;
                    case SoundsEnum.ferumSoung:
                        commandString = "FERUMSOUND^";
                        break;
                    case SoundsEnum.rockSound:
                        commandString = "FOCKSOUND^";
                        break;
                    case SoundsEnum.shotSoung:
                        commandString = "SHOTSOUND^";
                        break;
                    case SoundsEnum.shotTargetSound:
                        commandString = "SHOTTARGETSSOUND^";
                        break;

                }
                byte[] data = Encoding.UTF8.GetBytes(commandString);
                SetDataAsynk(data);
            }

            //отправка данных
            protected async Task SetDataAsynk(byte[] data)
            {
                await client.SendAsync(data, SocketFlags.None);
            }

            //останавливаем клиент (сокет этого клиента)
            public void StopClient() 
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }

            //события игры
            protected void EventOfGame(GameEnum gameEvent) 
            {
                string commandString = "";
                byte[] data;

                switch (gameEvent)
                {
                    case GameEnum.NewGame:
                        Action action = () =>
                        {
                            tank = new TankPlayer(isFirstClient ? GlobalDataStatic.Controller.map.respawnTankPlayer[0] : GlobalDataStatic.Controller.map.respawnTankPlayer[1]);
                            
                            //GlobalDataStatic.BattleGroundCollection.Add(tank);//добавляемся на поле боя   
                        };
                        GlobalDataStatic.Controller.Dispatcher.Invoke(action);

                        GlobalDataStatic.PartyTanksOfPlayers.Add(tank);//добавляемся в армию
                        

                        break;

                    case GameEnum.NewRound:
                        if (tank.HP <= 0) 
                        {
                            tank = new TankPlayer(isFirstClient ? GlobalDataStatic.Controller.map.respawnTankPlayer[0] : GlobalDataStatic.Controller.map.respawnTankPlayer[1]);
                            GlobalDataStatic.PartyTanksOfPlayers.Add(tank);//добавляемся в армию
                        }
                        else 
                        {
                            tank.UpdateHpForTeer();
                            tank.ID = GlobalDataStatic.IdNumberElement++;
                            tank.ePos = (isFirstClient ? GlobalDataStatic.Controller.map.respawnTankPlayer[0] : GlobalDataStatic.Controller.map.respawnTankPlayer[1]);
                        }
                        GlobalDataStatic.BattleGroundCollection.Add(tank);//добавляемся на поле боя
                        //подписываемся на события коллекции
                        SubscribeForEventsElements();
                        break;

                    case GameEnum.ReplayRound:
                        if (tank.HP <= 0)
                        {
                            tank = new TankPlayer(isFirstClient ? GlobalDataStatic.Controller.map.respawnTankPlayer[0] : GlobalDataStatic.Controller.map.respawnTankPlayer[1]);
                            GlobalDataStatic.PartyTanksOfPlayers.Add(tank);//добавляемся в армию
                        }
                        else
                        {
                            tank.UpdateHpForTeer();
                            tank.ID = GlobalDataStatic.IdNumberElement++;
                            tank.ePos = (isFirstClient ? GlobalDataStatic.Controller.map.respawnTankPlayer[0] : GlobalDataStatic.Controller.map.respawnTankPlayer[1]);
                        }
                        GlobalDataStatic.BattleGroundCollection.Add(tank);//добавляемся на поле боя
                        //подписываемся на события коллекции
                        SubscribeForEventsElements();
                        break;

                    //результат сражения
                    case GameEnum.DistroyEnemyTank:
                        //ПОБЕДА
                        commandString = "WIN@DESTROYENEMYTANK^";
                        data = Encoding.UTF8.GetBytes(commandString);
                        SetDataAsynk(data);
                        //отписываемся от событий элементов и коллекции
                        UnSubscribeForEventsElements();
                        GlobalDataStatic.readyCheck = false;
                        break;

                    case GameEnum.DistroyFriendlyTank:
                        //ПОРАЖЕНИЕ
                        commandString = "LOSE@DESTROYFRIENDLYTANK^";
                        data = Encoding.UTF8.GetBytes(commandString);
                        SetDataAsynk(data);
                        //отписываемся от событий элементов и коллекции
                        UnSubscribeForEventsElements();
                        GlobalDataStatic.readyCheck = false;
                        break;

                    case GameEnum.DestroyBunker:
                        //ПОРАЖЕНИЕ
                        commandString = "LOSE@DESTROYBUNKER^";
                        data = Encoding.UTF8.GetBytes(commandString);
                        SetDataAsynk(data);
                        //отписываемся от событий элементов и коллекции
                        UnSubscribeForEventsElements();
                        GlobalDataStatic.readyCheck = false;
                        break;

                    case GameEnum.DestroyBunkerEnamy:
                        //ПОБЕДА
                        commandString = "WIN@DESTROYBUNKERENEMY^";
                        data = Encoding.UTF8.GetBytes(commandString);
                        SetDataAsynk(data);
                        //отписываемся от событий элементов и коллекции
                        UnSubscribeForEventsElements();
                        GlobalDataStatic.readyCheck = false;
                        break;

                }
            }
        }
    }
}
