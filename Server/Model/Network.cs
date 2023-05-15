using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

            protected clientClass() { }

            //конструктор
            public clientClass(Socket clientSocket, List<clientClass> clientList)
            {
                clientList.Add(this);//добавляемся в список клиентов
                client = clientSocket;
                //подписываемся на событие изменения  на поле боя
                GlobalDataStatic.BattleGroundCollection.CollectionChanged += ChangedBattleGround;
                tank = GlobalDataStatic.Controller?.mainTank;
            }

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
                            break;
                        case "CONTINUE":
                            GlobalDataStatic.Controller?.NewRaund();
                            break;
                        case "OUT":
                            StopClient();//отключаемся от сервера
                            break;
                        case "REPLAY":
                            GlobalDataStatic.Controller?.LostRaund();
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
                }
            }

            //отлавливаем изменения в коллекции поля боя
            protected void ChangedBattleGround(object? sender, NotifyCollectionChangedEventArgs e) 
            {
                // от сюда будем вызыват функцию SetDataAsynk()
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
        }
    }
}
