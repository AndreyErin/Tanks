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

            try
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 7070);
                listenSocket.Bind(iPEndPoint);
                listenSocket.Listen(10);

                while (true)
                {
                    var newClient = await listenSocket.AcceptAsync();
                    Task.Run(() => new clientClass(listenSocket, clientList)); //создаем класс клиента
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

            private bool readyCheck = false;

            protected clientClass() { }

            //конструктор
            public clientClass(Socket clientSocket, List<clientClass> clientList)
            {
                clientList.Add(this);//добавляемся в список клиентов
                client = clientSocket;
                tank = GlobalDataStatic.Controller?.mainTank;

                //подписываемся на события в игре
                GlobalDataStatic.Controller.GameEvent += EventOfGame;
            }

            //получение данных
            protected async Task GetDataAsynk()
            {
                List<byte> data = new List<byte>(); //весь пакет данных
                byte[] character = new byte[1];//один байт из данных
                int havaData; //проверка остались ли еще данные
                while (true) 
                {
                    //считываем весь пакет
                    while (true)
                    {
                        havaData = await client.ReceiveAsync(character, SocketFlags.None);
                        // ^ - символ означающий конец  пакета
                        if (havaData == 0 || havaData == '^') break;//если считаны все данные
                        data.Add(character[0]);
                    }

                    //перевод массива байт в команды от клиента
                    var command = Encoding.UTF8.GetString(data.ToArray());

                    switch (command) 
                    {
                        //Навигация по меню
                        case "NEWGAME":
                            GlobalDataStatic.Controller?.NewGame();
                            SubscribeForEventsElements();
                            break;
                        case "CONTINUE":
                            GlobalDataStatic.Controller?.NewRaund();
                            SubscribeForEventsElements();
                            break;
                        case "OUT":
                            StopClient();//отключаемся от сервера
                            break;
                        case "REPLAY":
                            GlobalDataStatic.Controller?.LostRaund();
                            SubscribeForEventsElements();
                            break;
                        case "READY":
                            //проверка на готовность 2го игрока
                            readyCheck = true;
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

                        //Итог сражения
                        //странно канешно получать его от клиента, но пока так
                        case "ENDROUND":
                            UnSubscribeForEventsElements();
                            break;
                    }                   
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
                        ((WorldElement)e.NewItems[0]).PropertyChanged += ChangedElement;
                        //если элемент может издавать звуки
                        if (e.NewItems[0] is ISoundsObjects)
                        {
                            ((ISoundsObjects)e.NewItems[0]).SoundEvent += Sounds;
                        }
                        commandString = $"ADD@{((WorldElement)e.NewItems[0]).ID}@{((WorldElement)e.NewItems[0]).ePos}@{((WorldElement)e.NewItems[0]).Skin}^";
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

            //результат сражения
            protected void EventOfGame(GameEnum gameEvent) 
            {
                switch (gameEvent)
                {
                    case GameEnum.NewGame:
                        break;
                    case GameEnum.NewRound:
                        break;
                    case GameEnum.ReplayRound:
                        break;
                    case GameEnum.Win:
                        break;
                    case GameEnum.Lose:
                        break;

                }
            }
        }
    }
}
