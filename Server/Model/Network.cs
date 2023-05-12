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

            protected clientClass() { }

            //конструктор
            public clientClass(Socket clientSocket, List<clientClass> clientList)
            {
                clientList.Add(this);//добавляемся в список клиентов
                client = clientSocket;
                //подписываемся на событие изменения  на поле боя
                GlobalDataStatic.BattleGroundCollection.CollectionChanged += ChangedBattleGround;
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
                            break;
                        case "CONTINUE":
                            break;
                        case "OUT":
                            break;
                        case "REPLAY":
                            break;

                        //Движение
                        case "MOVEUP":
                            break;
                        case "MOVEDOWN":
                            break;
                        case "MOVELEFT":
                            break;
                        case "MOVERIGHT":
                            break;
                        //Стрельба
                        case "FIRE":
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
            protected async Task SetDataAsynk()
            {

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
