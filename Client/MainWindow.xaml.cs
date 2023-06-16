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
using System.Linq;

namespace Client
{
    

    public partial class MainWindow : Window
    {
        //плеер для проигрывания музыки игры или меню
        private MediaPlayer _playerForMusic = new MediaPlayer();

        private int _numberPlayer = 0;

        public List<WorldElement> CollectionWorldElements { get; set; } = new List<WorldElement>();
        public List<WorldElement> CollectionTree { get; set; } = new List<WorldElement>();
        private DrawingVisual myVisual = new DrawingVisual();
        private DrawingContext dc;
        
        private Rect rect;
        
        private static Socket _socket;
        private Key _moveKey = Key.None;//кнопка отслеживающая пследнее движение
        private Key _lastKey = Key.None;//кнопка нажатая пользователем
        private string keyCommand = "";
        private System.Timers.Timer _timerRender = new System.Timers.Timer();

        public MainWindow()
        {
            InitializeComponent();
            GlobalDataStatic.Controller = this;
            
        }

        //загрузка программы
        private async void MainWin_Loaded(object sender, RoutedEventArgs e)
        {

            

            cnvMain.Visibility = Visibility.Hidden;


            _timerRender.Elapsed += RenderingFPS;
            _timerRender.Interval = 30;
            
            cnvMain.Visual.Add(myVisual);

            for (int i = 0; i < 300; i++)
            {
                GlobalDataStatic.StackElements.Push(new WorldElement());
            }
            //подготовка буфера для элементов
            CollectionWorldElements.Capacity = 500;



            //подгрузка изображений
                dc = myVisual.RenderOpen();
                //подготавливаем квадрат
                rect.Width = 40;
                rect.Height = 40;
                rect.X = 0;
                rect.Y = 0;

                foreach (var item in GlobalDataStatic.SkinDictionary)
                {                                       
                        dc.DrawImage(item.Value, rect);                  
                }

                foreach (var item in GlobalDataStatic.SkinDictionary90)
                {
                    dc.DrawImage(item.Value, rect);
                }

                foreach (var item in GlobalDataStatic.SkinDictionary180)
                {
                    dc.DrawImage(item.Value, rect);
                }

                foreach (var item in GlobalDataStatic.SkinDictionary270)
                {
                    dc.DrawImage(item.Value, rect);
                }
            dc.DrawImage(GlobalDataStatic.SkinDictionary[SkinsEnum.PictureBlock1], rect);

            dc.Close();
            //

            _timerRender.Start();
            //cnvMain.Visibility = Visibility.Visible;


            _playerForMusic.Open(GlobalDataStatic.SoundDictionary[SoundsEnum.menuSound]);
            _playerForMusic.MediaEnded += _playerForMusic_MediaEnded;
            _playerForMusic.Play();

           


            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket = socket;

            try
            {
                await _socket.ConnectAsync("192.168.0.34", 7071);
                Task.Factory.StartNew(() => GetDataOfServer());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось подключиться к серверу\n" + ex.Message);
            }
        }

        //зацикливаем музыку
        private void _playerForMusic_MediaEnded(object? sender, EventArgs e)
        {
            _playerForMusic.Position = TimeSpan.Zero;
            _playerForMusic.Play();
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

                    if ((int)worldElement.Skin > 18)
                    {
                        dc.DrawImage(GlobalDataStatic.SkinDictionary[worldElement.Skin], rect);
                    }
                    else
                    {
                        //вынаем изображение с нужным разворотом
                        switch (worldElement.Vector)
                        {
                            case VectorEnum.Top:
                                dc.DrawImage(GlobalDataStatic.SkinDictionary[worldElement.Skin], rect);
                                break;

                            case VectorEnum.Down:
                                dc.DrawImage(GlobalDataStatic.SkinDictionary180[worldElement.Skin], rect);
                                break;
                            case VectorEnum.Left:
                                dc.DrawImage(GlobalDataStatic.SkinDictionary270[worldElement.Skin], rect);
                                break;
                            case VectorEnum.Right:
                                dc.DrawImage(GlobalDataStatic.SkinDictionary90[worldElement.Skin], rect);
                                break;
                        }
                    }

                    //dc.DrawImage(GlobalDataStatic.SkinDictionary[SkinsEnum.PictureBlock1], rect);
                }
                foreach (WorldElement worldElement in CollectionTree) 
                {
                    rect.Width = worldElement.Width;
                    rect.Height = worldElement.Height;
                    rect.X = worldElement.ePos.Y;
                    rect.Y = worldElement.ePos.X;

                    dc.DrawImage(GlobalDataStatic.SkinDictionary[worldElement.Skin], rect);
                }

                    dc.Close();
            };
            Dispatcher.Invoke(action);
            


        }

        //двигаем танк
        private void MainWin_KeyDown(object sender, KeyEventArgs e)
        {
            //byte[] data;          
            switch (e.Key)
            {
            case Key.W:
            case Key.Up:
                //сообщение серверу
                keyCommand = "MOVEUP^";
                _moveKey = e.Key;                   
                break;
            case Key.S:
            case Key.Down:
                keyCommand = "MOVEDOWN^";
                _moveKey = e.Key;
                break;
            case Key.A:
            case Key.Left:
                keyCommand = "MOVELEFT^";
                _moveKey = e.Key;
                break;
            case Key.D:
            case Key.Right:
                keyCommand = "MOVERIGHT^";
                _moveKey = e.Key;
                break;
            case Key.Space: //стрельба
                if (cD == false)
                {
                    keyCommand = "FIRE^";
                    Task.Factory.StartNew(CooldownFire);//запускаем откат в отдельном потоке
                }
                else
                {
                    return;
                }
                break;
            }
            SetDataOfServer(Encoding.UTF8.GetBytes(keyCommand));
        }
        
        private void MainWin_KeyUp(object sender, KeyEventArgs e)
        {                      
            if (e.Key == _moveKey)//если кнопка движения была поднята то останавливаем танк
            {                              
                SetDataOfServer(Encoding.UTF8.GetBytes("STOP^"));
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
            SetDataOfServer(Encoding.UTF8.GetBytes("OUT^"));
            MainWin.Close();
        }

        //Кнопки меню - сообщение
        private void btnGameMenu_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "btnNewGameSolo":
                    SetDataOfServer(Encoding.UTF8.GetBytes("NEWGAME^"));
                    break;
                case "btnRaundWin":
                    //CollectionTree.Clear();
                    //CollectionWorldElements.Clear();//очищаем канвас
                    SetDataOfServer(Encoding.UTF8.GetBytes("NEWRAUND^"));
                    break;
                case "btnRaundReplay":
                    //CollectionTree.Clear();
                    //CollectionWorldElements.Clear();//очищаем канвас
                    SetDataOfServer(Encoding.UTF8.GetBytes("REPLAY^"));
                    break;
                case "btnOut2":
                    SetDataOfServer(Encoding.UTF8.GetBytes("OUT^"));
                    MainWin.Close();
                    break;
                case "btnMultiPlayer":
                    stkResdyCheck.Visibility = Visibility.Visible;
                    lblMultuPlayerStatus.Visibility = Visibility.Visible;
                    btnNewGameSolo.Visibility = Visibility.Hidden;
                    lblResultOfBattleText.Visibility = Visibility.Hidden;
                    btnOut2.Visibility = Visibility.Hidden;
                    btnMultiPlayer.Visibility = Visibility.Hidden;

                    btnReady.Visibility = Visibility.Visible;
                    btnOutInMenu.Visibility = Visibility.Visible;
                    //пустышка для проверки был ли игрок зашедший раньше готов
                    SetDataOfServer(Encoding.UTF8.GetBytes("NOTREADY^"));
                    break;

                case "btnOutInMenu":
                    stkResdyCheck.Visibility = Visibility.Hidden;
                    lblMultuPlayerStatus.Visibility = Visibility.Hidden;

                    btnReady.Visibility = Visibility.Hidden;
                    btnOutInMenu.Visibility = Visibility.Hidden;


                    btnNewGameSolo.Visibility = Visibility.Visible;
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                    btnMultiPlayer.Visibility = Visibility.Visible;
                    break;

                //готовность игрока///////////
                case "btnReady":
                    

                    switch (((Button)sender).Content)
                    {
                        case "Готов":
                            SetDataOfServer(Encoding.UTF8.GetBytes("READY^"));
                            ((Button)sender).Content = "Отменить";
                            btnOutInMenu.Visibility = Visibility.Hidden;
                            //lblThisPlayer.Background = Brushes.GreenYellow;
                            break;
                        case "Отменить":
                            SetDataOfServer(Encoding.UTF8.GetBytes("NOTREADY^"));
                            ((Button)sender).Content = "Готов";
                            btnOutInMenu.Visibility = Visibility.Visible;
                            //lblThisPlayer.Background = Brushes.Gray;
                            break;
                    }

                    
                    break;
            }
            
        }

        //добавление объекта на поле боя
        public void AddElement(int id, double x, double y, SkinsEnum skin, VectorEnum vector) 
        {
            Dispatcher.Invoke(() =>
            {
                    MyPoint pos = new MyPoint(x, y);
                    GlobalDataStatic.StackElements.Pop().AddMe( id, pos, skin, vector);
                    lblElementInCanvasCount.Content = CollectionWorldElements.Count;                    
            });          
        }

        //удаление объекта с поля боя
        public void RemoveElement(int id) 
        {
            Dispatcher.Invoke(() =>
            {
                //проверяем дерево ли это
                foreach (WorldElement worldElement in CollectionTree.ToList())
                {
                    if (worldElement.ID == id)
                    {
                        worldElement.DeleteMe();
                        return;
                    }
                }
                //если не дерево то ищим дальше
                foreach (WorldElement worldElement in CollectionWorldElements.ToList())
                {
                    if (worldElement.ID == id)
                    {
                        worldElement.DeleteMe();
                        return;
                    }
                }
            });           
        }

        //изменение в элементе
        public void ChangeElement(int id, double x, double y, SkinsEnum skin, VectorEnum vector)
        {
            Dispatcher.Invoke(() =>
            {               
                foreach (WorldElement worldElement in CollectionWorldElements.ToList())
                {
                    if (worldElement.ID == id)
                    {
                        worldElement.Skin = skin;
                        worldElement.Vector = vector;
                        worldElement.ePos.X = x;
                        worldElement.ePos.Y = y;
                        return;
                    }
                }
            });                                 
        }

        //отправить данные
        private async Task SetDataOfServer(byte[] data)
        {

            Dispatcher.Invoke(() =>
            {
                GlobalDataStatic.Controller.lblSetPocketCount.Content = int.Parse(GlobalDataStatic.Controller.lblSetPocketCount.Content.ToString()) + 1;
            });
                       
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

            List<byte> data = new List<byte>(); //весь пакет данных
            byte[] character = new byte[1];//один байт из данных
            int haveData = 0;
            string resultString = "";
            bool isCommand = false;
            string[] command;
            bool serverEvent = false;
            string[] bigMessage;
            int playerIndex = 0;

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

                resultString = Encoding.UTF8.GetString(data.ToArray());
                
                isCommand = resultString.Contains('@');
                ///////////////////////////////////////////////////
                Dispatcher.Invoke(() =>
                {
                    GlobalDataStatic.Controller.lblGetPocketCount.Content = int.Parse(GlobalDataStatic.Controller.lblGetPocketCount.Content.ToString()) + 1;
                    GlobalDataStatic.Controller.lblElementInDictionaryCount.Content = GlobalDataStatic.StackElements.Count;
                });
                


                if (isCommand) //команда
                {
                    command = resultString.Split('@');

                    switch (command[0])
                    {

                    case "ADD":
                        AddElement(int.Parse(command[1]),
                            double.Parse(command[2]),
                            double.Parse(command[3]),
                            (SkinsEnum)int.Parse(command[4]),
                            (VectorEnum)int.Parse(command[5]));
                        break;

                    case "REMOVE":
                        RemoveElement(int.Parse(command[1]));
                        break;

                    case "CHANGE":

                        bigMessage = resultString.Split('*');
                        

                        foreach (string s in bigMessage) 
                        {
                            if (s.Length == 0) break;

                            command = s.Split('@');
                            if (s.Contains("CHANGE"))
                            {
                                ChangeElement(int.Parse(command[1]),
                                double.Parse(command[2]),
                                double.Parse(command[3]),
                                (SkinsEnum)int.Parse(command[4]),
                                (VectorEnum)int.Parse(command[5]));
                            }
                            else 
                            {
                                ChangeElement(int.Parse(command[0]),
                                double.Parse(command[1]),
                                double.Parse(command[2]),
                                (SkinsEnum)int.Parse(command[3]),
                                (VectorEnum)int.Parse(command[4]));
                            }
                        }
                        break;

                    case "NUMBERPLAYEAR":
                        //определяем номер этого клиента
                        _numberPlayer = int.Parse(command[1]);
                        break;
                    }
                }
                else 
                {
                    //отклик от сервера. событие произошедшее на сервере
                    serverEvent = int.TryParse(resultString, out var number);
                    if (serverEvent)
                    {
                        ServerGameEvent((GameEnum)number);
                    }

                    //переключаем плеера
                    playerIndex++;
                    if(playerIndex > 4) playerIndex = 0;

                    Action action = () =>
                    {
                        //звук
                        switch (resultString)
                        {
                            case "BONUSSOUND":
                                GlobalDataStatic.MediaPlayerList[playerIndex].Open(GlobalDataStatic.SoundDictionary[SoundsEnum.bonusSound]);
                                GlobalDataStatic.MediaPlayerList[playerIndex].Play();
                                break;
                            case "FERUMSOUND":
                                GlobalDataStatic.MediaPlayerList[playerIndex].Open(GlobalDataStatic.SoundDictionary[SoundsEnum.ferumSoung]);
                                GlobalDataStatic.MediaPlayerList[playerIndex].Play();
                                break;
                            case "ROCKSOUND":
                                GlobalDataStatic.MediaPlayerList[playerIndex].Open(GlobalDataStatic.SoundDictionary[SoundsEnum.rockSound]);
                                GlobalDataStatic.MediaPlayerList[playerIndex].Play();
                                break;
                            case "SHOTSOUND":
                                GlobalDataStatic.MediaPlayerList[playerIndex].Open(GlobalDataStatic.SoundDictionary[SoundsEnum.shotSoung]);
                                GlobalDataStatic.MediaPlayerList[playerIndex].Play();
                                break;
                            case "SHOTTARGETSSOUND":
                                GlobalDataStatic.MediaPlayerList[playerIndex].Open(GlobalDataStatic.SoundDictionary[SoundsEnum.shotTargetSound]);
                                GlobalDataStatic.MediaPlayerList[playerIndex].Play();
                                break;
                        }
                    };
                    Dispatcher.Invoke(action);
                }

                
                data.Clear();
            }
        }

        //события произошедшие на сервере
        private void ServerGameEvent(GameEnum gameEnum) 
        {
            Action action = () =>
            {
                //отклик от сервера. событие произошедшее на сервере
                switch (gameEnum)
                {
                    //все готовы и игра стартовала
                    case GameEnum.NewGameMultiPlayer:
                        cnvMain.Visibility = Visibility.Visible;
                        //скрываем менюшку и запускаем рендер
                        btnOutInMenu.Visibility = Visibility.Hidden;
                        btnReady.Visibility = Visibility.Hidden;
                        lblThisPlayer.Visibility = Visibility.Hidden;
                        lblFriendlyPlayer.Visibility = Visibility.Hidden;
                        lblMultuPlayerStatus.Visibility = Visibility.Hidden;
                            //_timerRender.Start();
                        _playerForMusic.Open(GlobalDataStatic.SoundDictionary[SoundsEnum.mainSound]);                      
                        _playerForMusic.Play();

                        break;

                    case GameEnum.NewGame:
                        cnvMain.Visibility = Visibility.Visible;
                        btnNewGameSolo.Visibility = Visibility.Hidden;
                    lblResultOfBattleText.Visibility = Visibility.Hidden;
                    btnOut2.Visibility = Visibility.Hidden;
                    btnMultiPlayer.Visibility = Visibility.Hidden;
                        //_timerRender.Start();
                        _playerForMusic.Open(GlobalDataStatic.SoundDictionary[SoundsEnum.mainSound]);
                        _playerForMusic.Play();
                        break;
                case GameEnum.NewRoundMultiPlayer:
                case GameEnum.NewRound:
                    btnRaundWin.Visibility = Visibility.Hidden;
                    lblResultOfBattleText.Visibility = Visibility.Hidden;
                    lblWinText.Visibility = Visibility.Hidden;
                    btnOut2.Visibility = Visibility.Hidden;
                        //_timerRender.Start();
                        _playerForMusic.Open(GlobalDataStatic.SoundDictionary[SoundsEnum.mainSound]);
                        _playerForMusic.Play();
                        break;
                case GameEnum.ReplayRoundMultiPlayer:
                case GameEnum.ReplayRound:
                    btnNewGameSolo.Visibility = Visibility.Hidden;
                    lblResultOfBattleText.Visibility = Visibility.Hidden;
                    lblWinText.Visibility = Visibility.Hidden;
                    btnRaundReplay.Visibility = Visibility.Hidden;
                    btnOut2.Visibility = Visibility.Hidden;
                        //_timerRender.Start();
                        _playerForMusic.Open(GlobalDataStatic.SoundDictionary[SoundsEnum.mainSound]);
                        _playerForMusic.Play();
                        break;
                case GameEnum.DistroyEnemyTank:
                    lblResultOfBattleText.Content = "Победа";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Армия врага уничтожена!";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundWin.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                        //_timerRender.Stop();
                        _playerForMusic.Open(GlobalDataStatic.SoundDictionary[SoundsEnum.menuSound]);
                        _playerForMusic.Play();
                        //очищаем поле
                        //dc = myVisual.RenderOpen();
                        //dc.Close();
                        //очищаем списки элнментов
                        ClearCollectionsElements();
                        //CollectionTree.Clear();
                        //CollectionWorldElements.Clear();
                        break;
                case GameEnum.DistroyFriendlyTank:
                    lblResultOfBattleText.Content = "Поражение";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Наша армия уничтожена.";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundReplay.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                        //_timerRender.Stop();
                        _playerForMusic.Open(GlobalDataStatic.SoundDictionary[SoundsEnum.menuSound]);
                        _playerForMusic.Play();
                        //очищаем поле
                        //dc = myVisual.RenderOpen();
                        //dc.Close();
                        //очищаем списки элнментов
                        ClearCollectionsElements();
                        //CollectionTree.Clear();
                        //CollectionWorldElements.Clear();
                        break;
                case GameEnum.DestroyBunker:
                    lblResultOfBattleText.Content = "Поражение";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Наш штаб уничтожен.";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundReplay.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                        //_timerRender.Stop();
                        _playerForMusic.Open(GlobalDataStatic.SoundDictionary[SoundsEnum.menuSound]);
                        _playerForMusic.Play();
                        //очищаем поле
                        //dc = myVisual.RenderOpen();
                        //dc.Close();
                        //очищаем списки элнментов
                        ClearCollectionsElements();
                        //CollectionTree.Clear();
                        //CollectionWorldElements.Clear();
                        break;
                case GameEnum.DestroyBunkerEnamy:
                    lblResultOfBattleText.Content = "Победа";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Штаб врага уничтожен!";
                    lblWinText.Visibility = Visibility.Visible;
                    btnRaundWin.Visibility = Visibility.Visible;
                    btnOut2.Visibility = Visibility.Visible;
                        //_timerRender.Stop();
                        _playerForMusic.Open(GlobalDataStatic.SoundDictionary[SoundsEnum.menuSound]);
                        _playerForMusic.Play();
                        //очищаем поле
                        //dc = myVisual.RenderOpen();
                        //dc.Close();
                        //очищаем списки элнментов
                        ClearCollectionsElements();
                        //CollectionTree.Clear();
                        //CollectionWorldElements.Clear();
                        break;
                case GameEnum.Win:
                    lblResultOfBattleText.Content = "Игра пройдена";
                    lblResultOfBattleText.Visibility = Visibility.Visible;
                    lblWinText.Content = "Маладес!";
                    lblWinText.Visibility = Visibility.Visible;                   
                    btnOut2.Visibility = Visibility.Visible;
                        //_timerRender.Stop();
                        _playerForMusic.Open(GlobalDataStatic.SoundDictionary[SoundsEnum.menuSound]);
                        _playerForMusic.Play();
                        //очищаем поле
                        //dc = myVisual.RenderOpen();
                        //dc.Close();
                        //очищаем списки элнментов
                        ClearCollectionsElements();
                        //CollectionTree.Clear();
                        //CollectionWorldElements.Clear();
                        break;

                    case GameEnum.PlayerOneReady:
                        if (_numberPlayer == 1)                        
                            lblThisPlayer.Background = Brushes.GreenYellow;
                        else
                            lblFriendlyPlayer.Background = Brushes.GreenYellow;
                        break;
                    case GameEnum.PlayerOneNotReady:
                        if (_numberPlayer == 1)
                            lblThisPlayer.Background = Brushes.Gray;
                        else
                            lblFriendlyPlayer.Background = Brushes.Gray;
                        break;
                    case GameEnum.PlayerTwoReady:
                        if (_numberPlayer == 2)
                            lblThisPlayer.Background = Brushes.GreenYellow;
                        else
                            lblFriendlyPlayer.Background = Brushes.GreenYellow;
                        break;
                    case GameEnum.PlayerTwoNotReady:
                        if (_numberPlayer == 2)
                            lblThisPlayer.Background = Brushes.Gray;
                        else
                            lblFriendlyPlayer.Background = Brushes.Gray;
                        break;
                }
            };
            Dispatcher.Invoke(action);
        }

        //очищаем коллекции после боя
        private void ClearCollectionsElements() 
        {

            try
            {
                foreach (WorldElement item in CollectionTree.ToList())
                {
                    item.DeleteMe();
                }

                foreach (WorldElement item in CollectionWorldElements.ToList())
                {
                    item.DeleteMe();
                }

                //if (CollectionTree.Count != 0)
                //    CollectionTree = new List<WorldElement>();

                //if (CollectionWorldElements.Count != 0)
                //    CollectionWorldElements = new List<WorldElement>();


                //MessageBox.Show("Статистика клиент\nЛист деревьев: " + CollectionTree.Count +
                //    "\nЛист остальных объектов: " + CollectionWorldElements.Count +
                //    "\nСтак элементов: " + GlobalDataStatic.StackElements.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Попытка очищения списка элементов на клиенткой программе\n" + ex.Message);
            }


        }
    }
}
