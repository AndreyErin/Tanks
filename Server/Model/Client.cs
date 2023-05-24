using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.Model
{
    public class Client
    {       
        private Socket client;

        public bool isFirstClient = false;

        protected Client() { }

        //конструктор
        public Client(Socket clientSocket, int clientNumber)
        {
            //определяем клиента как основного(либо ведомого)
            if (clientNumber == 1) isFirstClient = true;

            client = clientSocket;

            //подписываемся на события в игре
            GlobalDataStatic.Controller.GameEvent += EventOfGame;

            //получение данных в отдельном потоке
            Task.Factory.StartNew(()=> GetDataAsynk());
        }

        //получение данных
        protected async Task GetDataAsynk()
        {
                
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

                Action action = () =>
                {
                    //отправляем команды в контроллер
                    switch (command)
                    {
                        //Навигация по меню
                        case "NEWGAME":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.NewGame);
                            break;
                        case "NEWRAUND":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.NewRaund);
                            break;
                        case "OUT":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.Out);
                            break;
                        case "REPLAY":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.Replay);
                            break;
                        case "READY":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.Ready);
                            break;

                        //Движение
                        case "MOVEUP":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.MoveUp);
                            break;
                        case "MOVEDOWN":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.MoveDown);
                            break;
                        case "MOVELEFT":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.MoveLeft);
                            break;
                        case "MOVERIGHT":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.MoveRight);
                            break;
                        case "STOP":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.Stop);
                            break;

                        //Стрельба
                        case "FIRE":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.Fire);
                            break;
                    }
                    data.Clear();
                };
                GlobalDataStatic.Controller?.Dispatcher.Invoke(action);
            }
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

        //события игры(получаем событие и отправляем на сервер)
        protected void EventOfGame(GameEnum gameEvent)
        {
            int command = (int)gameEvent;
            byte[] data = Encoding.UTF8.GetBytes(command.ToString());
            SetDataAsynk(data);
        }        
    }
}
