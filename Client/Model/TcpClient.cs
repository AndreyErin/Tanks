using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Threading;

namespace Client.Model
{
    public class TcpClient
    {
        protected Socket clientSocket { get; set; }
      
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
                await clientSocket.ConnectAsync("127.0.0.1", 7071);
                //MessageBox.Show("подключено");---------------------
                Task.Factory.StartNew( async () => await GetDataOfServer());
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
            //MessageBox.Show("нажатие отправлено на вервер\n" + command);

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
            //MessageBox.Show("нова игра - отправлено на сервер");--------
        }

        //получить данные
        private async Task GetDataOfServer()
        {
            //MessageBox.Show("запуск получения данных"); 
            List<byte> data = new List<byte>(); //весь пакет данных
            byte[] character = new byte[1];//один байт из данных
            int haveData; //проверка остались ли еще данные
            string[] command;
            while (true)
            {
                //считываем весь пакет
                while (true)
                {
                    haveData = await clientSocket.ReceiveAsync(character, SocketFlags.None);
                    // ^ - символ означающий конец  пакета
                    if (haveData == 0 || character[0] == '^') break;//если считаны все данные
                    data.Add(character[0]);
                }

                string resultString = Encoding.UTF8.GetString(data.ToArray());
                ///////////////
                


                bool isCommand = resultString.Contains('@');

                //MessageBox.Show("сообщение является командой\n" + isCommand);-------
                if (isCommand) //команда
                {
                    
                    command = resultString.Split('@');
                    ///////////////////////////////////////////////////////////////////////
//                   var subset = from UIElement s in GlobalDataStatic.Controller.cnvMain.Children
//                                where (s as WorldElement != null) && ((WorldElement)s).ID == int.Parse(command[1])
//                                select s;
                   WorldElement elementCollection = null;
//                   foreach (WorldElement worldElement in subset)
//                   {
//                       elementCollection = worldElement;
//                   }
                    
                    switch (command[0])
                    {
                        

                        case "ADD":
                            //MessageBox.Show("до вхождения в диспетчер\n" + Thread.CurrentThread.ManagedThreadId.ToString());
                            Action action = () =>
                            {
                               
                                MyPoint pos = new MyPoint(double.Parse(command[2]), double.Parse(command[3]));
                                //MessageBox.Show("команда создать элемент\n" + Thread.CurrentThread.ManagedThreadId.ToString());
                                //WorldElement w = new WorldElement(int.Parse(command[1]), pos, (SkinsEnum)(int.Parse(command[4])));
                                GlobalDataStatic.Controller.AddElement(int.Parse(command[1]), pos, (SkinsEnum)(int.Parse(command[4])));

                            };GlobalDataStatic.DispatcherMain.Invoke(action);
                            
                            break;
                        case "REMOVE":
                            if(elementCollection != null)
                                GlobalDataStatic.Controller.cnvMain.Children.Remove(elementCollection);
                            break;
                        case "SKIN":
                            if (elementCollection != null)
                                elementCollection.SkinElement((SkinsEnum)(int.Parse(command[2])));
                            break;
                        case "X":
                            if (elementCollection != null)
                                elementCollection.MoveElement(x: double.Parse(command[2]));
                            break;
                        case "Y":
                            MessageBox.Show("передвижение от сервера получено");
                            if (elementCollection != null)
                                elementCollection.MoveElement(y: double.Parse(command[2]));
                            break;
                    }
                    
                }
                else //звук
                {
                    switch (resultString)
                    {
                        case "BONUSSOUND":
                            break;
                        case "FERUMSOUND":
                            break;
                        case "FOCKSOUND":
                            break;
                        case "SHOTSOUND":
                            break;
                        case "SHOTTARGETSSOUND":
                            break;
                    }
                }


                command = null;
                data.Clear();
            }
        }
    }
}
