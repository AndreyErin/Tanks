using System;
using System.Collections.Generic;
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

        public Network()
        {
            Task.Factory.StartNew(()=> StartListen()); //запускаем функцию прослушивания в отдельном потоке            
        }

        protected async Task StartListen() 
        {
            //MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());

            //сокет для прослушки входящих подключений
            using var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 7070);
            listenSocket.Bind(iPEndPoint);
            listenSocket.Listen(10);

           
            while (true)
            {
                var newClient = await listenSocket.AcceptAsync();
                Task.Run(() => new clientClass(listenSocket)); //создаем класс клиента
            }
        }

        ///////////////////////////////////клиент
        protected class clientClass 
        {
            private readonly Socket client;

            protected clientClass() { }
            //конструктор
            public clientClass(Socket clientSocket)
            {
                client = clientSocket;
               
            }

            protected async Task GetDataAsynk()
            {

            }

            protected async Task SetDataAsynk()
            {

            }

            protected void Disconnect() 
            {

            }
        }
    }
}
