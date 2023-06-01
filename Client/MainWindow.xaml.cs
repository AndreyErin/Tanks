using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Client.Model;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;

namespace Client
{
    

    public partial class MainWindow : Window
    {
        public List<WorldElement> CollectionWorldElements { get; set; } = new List<WorldElement>();
        DrawingCanvas drawingCanvas = new DrawingCanvas();
        DrawingVisual myVisual = new DrawingVisual();
        DrawingContext dc;
        Rect rect;




        private static Socket _socket;
        private Key _moveKey = Key.None;//кнопка отслеживающая пследнее движение
        private Key _lastKey = Key.None;//кнопка нажатая пользователем
        private System.Timers.Timer _timerRender = new System.Timers.Timer();

        public MainWindow()
        {
            InitializeComponent();
            GlobalDataStatic.Controller = this;
            
        }

        //загрузка программы
        private async void MainWin_Loaded(object sender, RoutedEventArgs e)
        {
            _timerRender.Elapsed += RenderingFPS;
            _timerRender.Interval = 20;
            Canvas.SetLeft(drawingCanvas, 0);
            Canvas.SetTop(drawingCanvas, 0);
            cnvMain.Children.Add(drawingCanvas);
            drawingCanvas.Visual.Add(myVisual);

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket = socket;

            try
            {
                await _socket.ConnectAsync("127.0.0.1", 7071);
                Task.Factory.StartNew(() => GetDataOfServer());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к серверу\n" + ex.Message);
            }
        }

        //рендер
        private void RenderingFPS(object sender, EventArgs e)
        {
            Action action = () =>
            {                             
                dc = myVisual.RenderOpen();
                
                foreach (WorldElement worldElement in CollectionWorldElements)
                {
                    //подготавливаем квадрат
                    rect.Width = worldElement.Width;
                    rect.Height = worldElement.Height;
                    rect.X = worldElement.ePos.Y;
                    rect.Y = worldElement.ePos.X;

                    //вынаем изображение с нужным разворотом
                    switch (worldElement.Vector)
                    {
                        case VectorEnum.Top:
                            dc.DrawImage(GlobalDataStatic.SkinDictionary[worldElement.Skin], rect);
                            break;
                        case VectorEnum.Down:
                            if ((int)worldElement.Skin >= 0 && (int)worldElement.Skin <= 18)                      
                                dc.DrawImage(GlobalDataStatic.SkinDictionary180[worldElement.Skin], rect);
                            else
                                dc.DrawImage(GlobalDataStatic.SkinDictionary[worldElement.Skin], rect);
                            break;
                        case VectorEnum.Left:
                            if ((int)worldElement.Skin >= 0 && (int)worldElement.Skin <= 18)
                                dc.DrawImage(GlobalDataStatic.SkinDictionary270[worldElement.Skin], rect);
                            else
                                dc.DrawImage(GlobalDataStatic.SkinDictionary[worldElement.Skin], rect);
                            break;
                        case VectorEnum.Right:
                            if ((int)worldElement.Skin >= 0 && (int)worldElement.Skin <= 18)
                                dc.DrawImage(GlobalDataStatic.SkinDictionary90[worldElement.Skin], rect);
                            else
                                dc.DrawImage(GlobalDataStatic.SkinDictionary[worldElement.Skin], rect);
                            break;
                    }                 
                }
                dc.Close();

            };
            Dispatcher.Invoke(action);
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
                else
                {
                    return;
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
            SetDataOfServer(data);
            MainWin.Close();
        }

        //новая игра - сообщение
        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes("NEWGAME^");
            SetDataOfServer(data);
        }
        //новый раунд  - сообщение
        private void btnRaundWin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //SearchElement = new Dictionary<int, WorldElement>();
                //SearchElement.Clear();
                CollectionWorldElements.Clear();//очищаем канвас
                byte[] data = Encoding.UTF8.GetBytes("NEWRAUND^");
                SetDataOfServer(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
        //переигровка раунда - сообщение
        private void btnRaundReplay_Click(object sender, RoutedEventArgs e)
        {
            
            CollectionWorldElements.Clear();//очищаем канвас
            byte[] data = Encoding.UTF8.GetBytes("REPLAY^");
            SetDataOfServer(data);
        }

        //готовность 2го игрока///////////не задействованно
        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes("READY^");
            SetDataOfServer(data);
        }

        //добавление объекта на поле боя
        public void AddElement(int id, MyPoint pos, SkinsEnum skin, VectorEnum vector) 
        {
            Action action = () =>
            {
                try
                {
                    WorldElement we = new WorldElement(id, pos, skin, vector);
                    
                    
                    lblElementInCanvasCount.Content = CollectionWorldElements.Count;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("добавление элемента .клиент-программа\n" + ex.Message);
                    
                }
            };
            Dispatcher.Invoke(action);


        }

        //удаление объекта с поля боя
        public void RemoveElement(int id) 
        {
            Action action = () =>
            {
                foreach (WorldElement worldElement in CollectionWorldElements) 
                {
                    if (worldElement.ID == id) 
                    {
                        CollectionWorldElements.Remove(worldElement);
                        return;
                    }
                }
            };
            Dispatcher.Invoke(action);
        }

        //изменение скина
        public void SkinUploadeElement(int id ,SkinsEnum skin)
        {
            Action action = () =>
            {
                
                try
                {
                    foreach (WorldElement worldElement in CollectionWorldElements)
                    {
                        if (worldElement.ID == id)
                        {
                            worldElement.Skin = skin;
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("смена скина .клиент-программа\n" + ex.Message);                   
                }
            };
            Dispatcher.Invoke(action);                      
        }

        //изменение положения
        public void PosElement(int id, double x = -10, double y = -10)
        {
            Action action = () =>
            {
                
                try
                {
                    foreach (WorldElement worldElement in CollectionWorldElements)
                    {
                        if (worldElement.ID == id)
                        {

                            worldElement.PosAndVectorElement(posX: x, posY: y );
                            //worldElement.ePos.Y = y;
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("движение .клиент-программа\n" + ex.Message);                   
                }
            };
            Dispatcher.Invoke(action);

            
        }

        //отправить данные
        private async Task SetDataOfServer(byte[] data)
        {

            Action action = () =>
            {
                GlobalDataStatic.Controller.lblSetPocketCount.Content = int.Parse(GlobalDataStatic.Controller.lblSetPocketCount.Content.ToString()) + 1;
            };
            GlobalDataStatic.Controller.Dispatcher.Invoke(action);

            
            try
            {
                await _socket.SendAsync(data, SocketFlags.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show("сообщение при отправке\n" + ex.Message);
              
            }
        }

        //получить данные
        private async Task GetDataOfServer()
        {
            //try
            //{

            List<byte> data = new List<byte>(); //весь пакет данных
            byte[] character = new byte[1];//один байт из данных

            while (true)
            {
                //считываем весь пакет
                while (true)
                {
                    var haveData = await _socket.ReceiveAsync(character, SocketFlags.None);
                    // ^ - символ означающий конец  пакета
                    if (haveData == 0 || character[0] == '^') break;//если считаны все данные
                    data.Add(character[0]);                  
                }

                var resultString = Encoding.UTF8.GetString(data.ToArray());
                
                bool isCommand = resultString.Contains('@');
                ///////////////////////////////////////////////////
                Action action = () =>
                {
                    GlobalDataStatic.Controller.lblGetPocketCount.Content = int.Parse(GlobalDataStatic.Controller.lblGetPocketCount.Content.ToString()) + 1;
                };
                Dispatcher.Invoke(action);


                    if (isCommand) //команда
                    {
                        var command = resultString.Split('@');

                        switch (command[0])
                        {

                        case "ADD":
                            MyPoint pos = new MyPoint(double.Parse(command[2]), double.Parse(command[3]));
                            AddElement(int.Parse(command[1]), pos, (SkinsEnum)int.Parse(command[4]), (VectorEnum)int.Parse(command[5]));
                            break;

                        case "REMOVE":
                            RemoveElement(int.Parse(command[1]));
                            break;

                        case "SKIN":
                            SkinUploadeElement(int.Parse(command[1]), (SkinsEnum)(int.Parse(command[2])));                               
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

                
                data.Clear();
            }
            //}

            //catch (Exception ex)
            //{
            //    MessageBox.Show("получение данных клиент программа\n" + ex.Message);
            //}
        }

        //события произошедшие на сервере
        private void ServerGameEvent(GameEnum gameEnum) 
        {
            Action action = () =>
            {
                //отклик от сервера. событие произошедшее на сервере
                switch (gameEnum)
            {
                case GameEnum.NewGame:
                    btnStartGame.Visibility = Visibility.Hidden;
                    lblResultOfBattleText.Visibility = Visibility.Hidden;
                    btnOut2.Visibility = Visibility.Hidden;
                        _timerRender.Start();
                    break;
                case GameEnum.NewRound:
                    btnRaundWin.Visibility = Visibility.Hidden;
                    lblResultOfBattleText.Visibility = Visibility.Hidden;
                    lblWinText.Visibility = Visibility.Hidden;
                    btnOut2.Visibility = Visibility.Hidden;
                        _timerRender.Start();
                    break;
                case GameEnum.ReplayRound:
                    btnStartGame.Visibility = Visibility.Hidden;
                    lblResultOfBattleText.Visibility = Visibility.Hidden;
                    lblWinText.Visibility = Visibility.Hidden;
                    btnRaundReplay.Visibility = Visibility.Hidden;
                    btnOut2.Visibility = Visibility.Hidden;
                        _timerRender.Start();
                    break;
                case GameEnum.DistroyEnemyTank:
                    lblResultOfBattleText.Content = "Победа";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Армия врага уничтожена!";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundWin.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                        _timerRender.Stop();
                    break;
                case GameEnum.DistroyFriendlyTank:
                    lblResultOfBattleText.Content = "Поражение";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Наша армия уничтожена.";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundReplay.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                        _timerRender.Stop();
                    break;
                case GameEnum.DestroyBunker:
                    lblResultOfBattleText.Content = "Поражение";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Наш штаб уничтожен.";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundReplay.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                        _timerRender.Stop();
                    break;
                case GameEnum.DestroyBunkerEnamy:
                    lblResultOfBattleText.Content = "Победа";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Штаб врага уничтожен!";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundWin.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                        _timerRender.Stop();
                    break;
                case GameEnum.Win:
                    lblResultOfBattleText.Content = "Игра пройдена";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Маладес!";
                    lblWinText.Visibility = Visibility.Visible;                   
                    btnOut2.Visibility = Visibility.Visible;
                        _timerRender.Stop();
                    break;
            }

            };
            Dispatcher.Invoke(action);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cnvMain.Visibility = Visibility.Hidden;
        }

        private void bbbb_Click(object sender, RoutedEventArgs e)
        {
            cnvMain.Visibility = Visibility.Visible;
        }
    }
}
