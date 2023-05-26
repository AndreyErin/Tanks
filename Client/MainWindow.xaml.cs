using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.IO;
using System.Windows.Input;
using Client.Model;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    

    public partial class MainWindow : Window
    {
        private Socket _socket;
        private Key _moveKey = Key.None;//кнопка отслеживающая пследнее движение
        private Key _lastKey = Key.None;//кнопка нажатая пользователем

        public static Dictionary<int, WorldElement> SearchElement = new Dictionary<int, WorldElement>();

        public MainWindow()
        {
            InitializeComponent();
            GlobalDataStatic.Controller = this;
            //ФПС
            System.Windows.Media.CompositionTarget.Rendering += RenderingFPS;
        }

        //ФПС
        private void RenderingFPS(object sender, EventArgs e)
        {
            cnvMain.InvalidateVisual();
        }

        //двигаем танк
        private void MainWin_KeyDown(object sender, KeyEventArgs e)
        {
            byte[] data;
            string command = "";

            switch (e.Key)
            {
            case Key.W:
            case Key.Up:
                //сообщение серверу
                command = "MOVEUP^";
                _moveKey = e.Key;                   
                break;
            case Key.S:
            case Key.Down:
                command = "MOVEDOWN^";
                _moveKey = e.Key;
                break;
            case Key.A:
            case Key.Left:
                command = "MOVELEFT^";
                _moveKey = e.Key;
                break;
            case Key.D:
            case Key.Right:
                command = "MOVERIGHT^";
                _moveKey = e.Key;
                break;
            case Key.Space: //стрельба
                if (cD == false)
                {
                    command = "FIRE^";
                    Task.Factory.StartNew(CooldownFire);//запускаем откат в отдельном потоке
                }
                break;
            }
            data = Encoding.UTF8.GetBytes(command);
            SetDataOfServer(data);
        }
        
        private void MainWin_KeyUp(object sender, KeyEventArgs e)
        {
            byte[] data;
            
            if (e.Key == _moveKey)//если кнопка движения была поднята то останавливаем танк
            {               
                data = Encoding.UTF8.GetBytes("STOP^");
                SetDataOfServer(data);
            }

            _lastKey = Key.None;
        }
        
        private void MainWin_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_lastKey == e.Key) //проверяем нажата ли та же самая кнопка или другая
            {
                e.Handled = true;//если ктопка та же самая, то помечаем событие как обработанное
            }
            _lastKey = e.Key;
        }

        private bool cD = false;
        //откат выстрела
        private void CooldownFire()
        {
            cD = true;
            Thread.Sleep(300);
            cD = false;
        }

        //загрузка программы
        private async void MainWin_Loaded(object sender, RoutedEventArgs e)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket = socket;

            try
            {
                await _socket.ConnectAsync("127.0.0.1", 7071);                
                Task.Factory.StartNew(async () => await GetDataOfServer());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к серверу\n" + ex.Message);
            }
        }

        //завершение программы
        private void MainWin_Unloaded(object sender, RoutedEventArgs e)
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        //выход - отключение от сервера
        private void btnOut_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes("OUT^");
            Task.Run(() => SetDataOfServer(data));
            MainWin.Close();
        }

        //новая игра - сообщение
        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes("NEWGAME^");
            Task.Run(() => SetDataOfServer(data));
        }
        //новый раунд  - сообщение
        private void btnRaundWin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SearchElement = new Dictionary<int, WorldElement>();
                SearchElement.Clear();
                cnvMain.Children.Clear();//очищаем канвас
                byte[] data = Encoding.UTF8.GetBytes("NEWRAUND^");
                Task.Run(() => SetDataOfServer(data));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
        //переигровка раунда - сообщение
        private void btnRaundReplay_Click(object sender, RoutedEventArgs e)
        {
            SearchElement.Clear();
            cnvMain.Children.Clear();//очищаем канвас
            byte[] data = Encoding.UTF8.GetBytes("REPLAY^");
            Task.Run(() => SetDataOfServer(data));
        }

        //готовность 2го игрока///////////не задействованно
        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes("READY^");
            Task.Run(() => SetDataOfServer(data));
        }

        //добавление объекта на поле боя
        public void AddElement(int id, MyPoint pos, SkinsEnum skin) 
        {
            WorldElement we = new WorldElement(id, pos, skin);
            SearchElement.Add(id, we);
        }

        //удаление объекта с поля боя
        public void RemoveElement(int id) 
        {
            cnvMain.Children.Remove(SearchElement[id]);
            SearchElement.Remove(id);
        }

        //изменение скина
        public void SkinUloadeElement(int id ,SkinsEnum skin)
        {
            SearchElement[id].SkinElement(skin);            
        }

        //изменение положения
        public void PosElement(int id, double x = -10, double y = -10)
        {
            SearchElement[id].MoveElement(x, y);
        }

        //отправить данные
        private async Task SetDataOfServer(byte[] data)
        {
            Action action = () =>
            {
                GlobalDataStatic.Controller.lblSetPocketCount.Content = int.Parse(GlobalDataStatic.Controller.lblSetPocketCount.Content.ToString()) + 1;
            };
            GlobalDataStatic.Controller.Dispatcher.Invoke(action);

            await _socket.SendAsync(data, SocketFlags.None);           
        }

        //получить данные
        private async Task GetDataOfServer()
        {

           
            List<byte> data = new List<byte>(); //весь пакет данных
            byte[] character = new byte[1];//один байт из данных
            int haveData; //проверка остались ли еще данные
            string[] command;
            while (true)
            {
                //считываем весь пакет
                while (true)
                {
                    haveData = await _socket.ReceiveAsync(character, SocketFlags.None);
                    // ^ - символ означающий конец  пакета
                    if (haveData == 0 || character[0] == '^') break;//если считаны все данные
                    data.Add(character[0]);

                    
                }

                string resultString = Encoding.UTF8.GetString(data.ToArray());
                
                bool isCommand = resultString.Contains('@');

                Action action = () =>
                {
                    GlobalDataStatic.Controller.lblGetPocketCount.Content = int.Parse(GlobalDataStatic.Controller.lblGetPocketCount.Content.ToString()) + 1;



                    if (isCommand) //команда
                    {
                        command = resultString.Split('@');

                        switch (command[0])
                        {

                        case "ADD":
                            MyPoint pos = new MyPoint(double.Parse(command[2]), double.Parse(command[3]));
                            AddElement(int.Parse(command[1]), pos, (SkinsEnum)(int.Parse(command[4])));
                            break;

                        case "REMOVE":
                            RemoveElement(int.Parse(command[1]));
                            break;

                        case "SKIN":
                            SkinUloadeElement(int.Parse(command[1]), (SkinsEnum)(int.Parse(command[2])));                               
                            break;

                        case "X":
                            PosElement(int.Parse(command[1]), x: double.Parse(command[2]));                               
                            break;

                        case "Y":
                            PosElement(int.Parse(command[1]), y: double.Parse(command[2]));
                            break;
                        }

                    }
                    else 
                    {
                        //отклик от сервера. событие произошедшее на сервере
                        bool serverEvent = int.TryParse(resultString, out var number);
                        if (serverEvent)
                        {
                            ServerGameEvent((GameEnum)number);
                        }

                        //звук
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
                };
                Dispatcher.Invoke(action);

                data.Clear();
            }
        }

        //события произошедшие на сервере
        private void ServerGameEvent(GameEnum gameEnum) 
        {
            //отклик от сервера. событие произошедшее на сервере
            switch (gameEnum)
            {
                case GameEnum.NewGame:
                    btnStartGame.Visibility = Visibility.Hidden;
                    lblResultOfBattleText.Visibility = Visibility.Hidden;
                    btnOut2.Visibility = Visibility.Hidden;                    
                    break;
                case GameEnum.NewRound:
                    btnRaundWin.Visibility = Visibility.Hidden;
                    lblResultOfBattleText.Visibility = Visibility.Hidden;
                    lblWinText.Visibility = Visibility.Hidden;
                    btnOut2.Visibility = Visibility.Hidden;
                    break;
                case GameEnum.ReplayRound:
                    btnStartGame.Visibility = Visibility.Hidden;
                    lblResultOfBattleText.Visibility = Visibility.Hidden;
                    lblWinText.Visibility = Visibility.Hidden;
                    btnRaundReplay.Visibility = Visibility.Hidden;
                    btnOut2.Visibility = Visibility.Hidden;
                    break;
                case GameEnum.DistroyEnemyTank:
                    lblResultOfBattleText.Content = "Победа";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Армия врага уничтожена!";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundWin.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                    //SearchElement.Clear();//очищаем словарь
                    break;
                case GameEnum.DistroyFriendlyTank:
                    lblResultOfBattleText.Content = "Поражение";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Наша армия уничтожена.";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundReplay.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                    //SearchElement.Clear();//очищаем словарь
                    break;
                case GameEnum.DestroyBunker:
                    lblResultOfBattleText.Content = "Поражение";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Наш штаб уничтожен.";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundReplay.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                    //SearchElement.Clear();//очищаем словарь
                    break;
                case GameEnum.DestroyBunkerEnamy:
                    lblResultOfBattleText.Content = "Победа";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Штаб врага уничтожен!";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundWin.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                    //SearchElement.Clear();//очищаем словарь
                    //SearchElement = new Dictionary<int, WorldElement>();
                    break;
                case GameEnum.Win:
                    lblResultOfBattleText.Content = "Игра пройдена";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Маладес!";
                    lblWinText.Visibility = Visibility.Visible;
                    
                    btnOut2.Visibility = Visibility.Visible;
                    //SearchElement.Clear();//очищаем словарь
                    //SearchElement = new Dictionary<int, WorldElement>();
                    break;
            }
        }

    }
}
