using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.Model
{
    public class TcpClient
    {
        protected Socket clientSocket { get; set; }

        //protected TcpClient() { }
        public TcpClient()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket = socket;

            Connect();
        }

        //подключение к серверу
        public async Task Connect()
        {
            try
            {
                await clientSocket.ConnectAsync("192.168.0.34", 7070);
                Task.Factory.StartNew(() => GetDataOfServer());
            }
            catch (System.Exception e)
            {
                MessageBox.Show("Не удалось подключиться к серверу\n" + e.Message);
            }
        }

        //управление танком
        public void KeyOfControlTank(Key key) 
        {
            byte[] data;
            string command = "";

            switch (key)
            {
                case Key.Up:
                    command = "MOVEUP^";
                    break;
                case Key.Down:
                    command = "MOVEDOWN^";
                    break;
                case Key.Left:
                    command = "MOVELEFT^";
                    break;
                case Key.Right:
                    command = "MOVERIGHT^";
                    break;
                case Key.S:
                    command = "STOP^";
                    break;

                case Key.Space:
                    command = "FIRE^";
                    break;                    
            }

            data = Encoding.UTF8.GetBytes(command);
            SetDataOfServer(data);
        }

        //навигация по меню
        public void MenuComand(MenuComandEnum comandEnum)
        {
            byte[] data;
            string command = "";

            switch (comandEnum)
            {
                case MenuComandEnum.NewGame:
                    command = "NEWGAME^";
                    break;
                case MenuComandEnum.NewRaund:
                    command = "NEWRAUND^";
                    break;
                case MenuComandEnum.Replay:
                    command = "REPLAY^";
                    break;
                case MenuComandEnum.Out:
                    command = "OUT^";
                    break;
                case MenuComandEnum.Ready:
                    command = "READY^";
                    break;
            }

            data = Encoding.UTF8.GetBytes(command);
            SetDataOfServer(data);
        }

        //отправить данные
        private async Task SetDataOfServer(byte[] data)
        {
            await clientSocket.SendAsync(data, SocketFlags.None);
        }

        //получить данные
        private async Task GetDataOfServer()
        {
            
        }
    }
}
