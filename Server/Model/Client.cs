using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Server.Model
{
    public class Client
    {   
        public TankPlayer tank;
        
        private Socket client;
        
        public bool Ready { get; set; } = false;

        protected Client() { }

        //конструктор
        public Client(Socket clientSocket)
        {
            //записываемся в пати как первый либо 2й игрок
            if (PartyPlayers.One == null)
            {
                //tank = 
                PartyPlayers.One = this;
                //отправляем данные о том какй это по номеру игрок 1й или 2й
                Task.Run(() => SetDataAsynk(Encoding.UTF8.GetBytes("NUMBERPLAYEAR@1^")));
            }
            else
            {
                PartyPlayers.Two = this;
                //отправляем данные о том какй это по номеру игрок 1й или 2й
                Task.Run(() => SetDataAsynk(Encoding.UTF8.GetBytes("NUMBERPLAYEAR@2^")));
            }







            client = clientSocket;

            //подписываемся на события в игре
            GlobalDataStatic.Controller.GameEvent += EventOfGame;
            GlobalDataStatic.Controller.ElementEvent += EventOfElement;
            GlobalDataStatic.Controller.SoundEvent += Sounds;



            //получение данных в отдельном потоке
            Task.Factory.StartNew(()=> GetDataAsynk());
        }

        //получение данных
        protected async Task GetDataAsynk()
        {
                
            List<byte> data = new List<byte>(); //весь пакет данных
            byte[] character = new byte[1];//один байт из данных
            int haveData; //проверка остались ли еще данные
            string command;
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
                command = Encoding.UTF8.GetString(data.ToArray());

                Action action = () =>
                {
                    
                    GlobalDataStatic.Controller.lblGetPocketCount.Content = int.Parse(GlobalDataStatic.Controller.lblGetPocketCount.Content.ToString()) +1;

                    //отправляем команды в контроллер
                    switch (command)
                    {
                        //Навигация по меню
                        case "NEWGAME":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.NewGame, tank);
                            break;
                        case "NEWRAUND":

                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.NewRaund, tank);
                            break;
                        case "OUT":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.Out, tank);
                            break;
                        case "REPLAY":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.Replay, tank);
                            break;

                        case "READY":
                            Ready = true;
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.Ready);
                            break;
                        case "NOTREADY":
                            Ready = false;
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.Ready);
                            break;

                        //Движение
                        case "MOVEUP":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.MoveUp, tank);
                            break;
                        case "MOVEDOWN":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.MoveDown, tank);
                            break;
                        case "MOVELEFT":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.MoveLeft, tank);
                            break;
                        case "MOVERIGHT":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.MoveRight, tank);
                            break;
                        case "STOP":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.Stop, tank);
                            break;

                        //Стрельба
                        case "FIRE":
                            GlobalDataStatic.Controller?.GetCommandsOfClient(ComandEnum.Fire, tank);
                            break;
                    }
                    
                };
                GlobalDataStatic.Controller?.Dispatcher.Invoke(action);

                data.Clear();
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
                        commandString = "ROCKSOUND^";
                        break;
                    case SoundsEnum.shotSoung:
                        commandString = "SHOTSOUND^";
                        break;
                    case SoundsEnum.shotTargetSound:                   
                        commandString = "SHOTTARGETSSOUND^";
                        break;

                }
                byte[] data = Encoding.UTF8.GetBytes(commandString);
                Task.Run(() => SetDataAsynk(data));
            }

        //отправка данных
        protected async Task SetDataAsynk(byte[] data)
        {
            Action action = () =>
            {
                GlobalDataStatic.Controller.lblSetPocketCount.Content = int.Parse(GlobalDataStatic.Controller.lblSetPocketCount.Content.ToString()) + 1;
                GlobalDataStatic.Controller.lblIndex.Content = GlobalDataStatic.IdNumberElement;
            };
            GlobalDataStatic.Controller.Dispatcher.Invoke(action);

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
            byte[] data = Encoding.UTF8.GetBytes(command.ToString() + '^');
            Task.Run(() => SetDataAsynk(data));
        } 
        
        protected void EventOfElement(ElementEventEnum elementEvent, int id, double x = -10, double y = -10, SkinsEnum skin = SkinsEnum.None, VectorEnum vector = VectorEnum.Top, string bigStringMessage = "") 
        {
            string commandString = "";
            switch (elementEvent)
            {
                case ElementEventEnum.Add:
                    commandString = $"ADD@{id}@{x}@{y}@{(int)skin}@{(int)vector}^";
                    break;
                case ElementEventEnum.Remove:
                    commandString = $"REMOVE@{id}^";
                    break;
                case ElementEventEnum.Change:                   
                    commandString = $"CHANGE@{bigStringMessage}^";
                    
                    break;
                
            }            
            byte[] data = Encoding.UTF8.GetBytes(commandString);
            Task.Run(()=> SetDataAsynk(data));
        }
    }
}
