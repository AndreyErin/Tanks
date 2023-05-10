using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Tanks.Model;

namespace Tanks
{

    public partial class MainWindow : Window
    {

        protected MediaPlayer _player = new MediaPlayer();
        

        TankPlayer mainTank;
        Map? map;
        int lvlMap = 0;
        string[] mapPool;

        private System.Timers.Timer tTimer_RespawnBotTank = new System.Timers.Timer();
        private int countTimerRespawn = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        //закрыть программу
        private void btnOut_Click(object sender, RoutedEventArgs e)
        {
            _player.Stop();
            MainWin.Close();
        }

        //
        private void RenderingFPS(object sender, EventArgs e)
        {
            //lblFPS.Content = (int.Parse(lblFPS.Content.ToString()) + 1).ToString();

            cnvMain.InvalidateVisual();
            
        }

        //загрузка программы
        private void MainWin_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Media.CompositionTarget.Rendering += RenderingFPS;



            GlobalDataStatic.cnvMap1 = cnvMain;
            GlobalDataStatic.MainDispatcher = Dispatcher;
            GlobalDataStatic.lblStatisticTank = lblStatisticTank;
            cnvMain.Width = 1320;
            cnvMain.Height = 720;
            btnStartGame.Visibility = Visibility.Visible;
            lblResultOfBattleText.Content = "Танчики";


            //зацикливаем мелодию
            _player.MediaEnded += _player_MediaEnded;
            _player.Open(GlobalDataStatic.menuSound);
            //_player.Volume = 1;
            _player.Play();

            //загружаем все имена карт из папки Maps
            mapPool = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Maps", "*.json");

            mapPool.OrderBy(x => x.ToString());


            //показываем меню при запуске программы
            //imgLogo.Source = GlobalDataStatic.PictureLogo;
            gridMenu.Visibility = Visibility.Visible;

            //настраеваем таймер респавна ботов-танков
            
            tTimer_RespawnBotTank.Elapsed += TTimer_RespawnBotTank_Elapsed;
            tTimer_RespawnBotTank.EndInit();
            

        }



        //зацикливаем мелодию
        private void _player_MediaEnded(object? sender, EventArgs e)
        {
            _player.Position = TimeSpan.Zero;
            _player.Play();
        }

        //таймер респавна танков-ботов
        private void TTimer_RespawnBotTank_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tTimer_RespawnBotTank.Interval = 5000;
            GlobalDataStatic.RespawnBotON = true;

            Action action = () => {

                //загрузка танков-ботов
                foreach (Point point in map.respawnTankBots)
                {
                    TankBot tankBot = new TankBot(point);
                    tankBot.DistroyEvent += DistroyEnemyTank;
                    GlobalDataStatic.PartyTankBots.Add(tankBot);
                }

            };
            Dispatcher.Invoke(action);

            if (++countTimerRespawn >= map.RespawnEnamyCount)
            {
                tTimer_RespawnBotTank.Stop();
                GlobalDataStatic.RespawnBotON = false;
            }
        }

        //загрузка элементов окружения 
        private void CreateViewWorldElements(string fileMapName)
        {
            tTimer_RespawnBotTank.Interval = 1;
            _player.Stop();
            countTimerRespawn = 0;
            //очищаем поле
            GlobalDataStatic.cnvMap1.Children.Clear();
            GlobalDataStatic.PartyTankBots.Clear();

            //заполняем поле
            try
            {                
                //вынаем данные из файла
                string jsonText = File.ReadAllText(fileMapName);
                //запихиваем ети данные в объект
                map = JsonSerializer.Deserialize<Map>(jsonText);

                
                //каменные блоки
                foreach(System.Windows.Point pos in map.rockBlocs)
                {
                        Block b = new Block(pos);                   
                }
                //железные блоки
                foreach (System.Windows.Point pos in map.ironBlocs)
                {
                    BlockFerum b = new BlockFerum(pos);
                }
                //деревья
                foreach (System.Windows.Point pos in map.woodBlocs)
                {
                    Tree b = new Tree(pos);
                }
                //дружеские каменные блоки
                foreach (System.Windows.Point pos in map.friendlyRockBlocs)
                {
                    Block b = new Block(pos) {  IsPlayer = true};
                }
                //локальные пушки
                foreach (System.Windows.Point pos in map.LocationGun)
                {
                    LocationGun b = new LocationGun(pos, 3);
                }
                //бункер игрока
                if (map.BunkerON == true)
                {
                    Bunker b = new Bunker(map.BunkerPos);
                    b.BunkerDestroy += DestroyBunker;
                }
                //бункер врага
                if (map.BunkerEnamyON == true)
                {
                    BunkerEnamy b = new BunkerEnamy(map.BunkerEnamyPos);
                    b.BunkerDestroy += DestroyBunkerEnamy;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Чёйта карта не загрузилась");
                return;
            }

            _player.Open(GlobalDataStatic.mainSound);
            _player.Play();
        }

        private Key _moveKey = Key.None;//кнопка отслеживающая пследнее движение
        private Key _lastKey = Key.None;//кнопка нажатая пользователем
        //двигаем танк 
        private void MainWin_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    mainTank.Move(VectorEnum.Top);
                    _moveKey = e.Key;
                    break;
                case Key.S:
                case Key.Down:
                    mainTank.Move(VectorEnum.Down);
                    _moveKey = e.Key;
                    break;
                case Key.A:
                case Key.Left:
                    mainTank.Move(VectorEnum.Left);
                    _moveKey = e.Key;
                    break;
                case Key.D:
                case Key.Right:
                    mainTank.Move(VectorEnum.Right);
                    _moveKey = e.Key;
                    break;
                case Key.Space: //стрельба
                    if (cD == false)
                    { 
                        mainTank.ToFire();
                        Task.Factory.StartNew(CooldownFire);//запускаем откат в отдельном потоке
                    }
                    break;
                }
              
        }

        private void MainWin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == _moveKey)//если кнопка движения была поднята то останавливаем танк
                mainTank.Stop();

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
        //начальное меню - новая игра
        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            //создаем элементы окружения
            //должны загружаться до респавнов
            CreateViewWorldElements(mapPool[lvlMap]);

            btnStartGame.Visibility = Visibility.Hidden;
            gridMenu.Visibility = Visibility.Hidden;


            System.Windows.Point tPos = map.respawnTankPlayer[0];
            //создаем танк игрока
            mainTank = new TankPlayer(tPos);
            mainTank.DestroyPayerTank += DistroyFriendlyTank;
            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();
        }

        //уничтожение танков-ботов
        private void DistroyEnemyTank(TankBot tankBot) 
        {
            GlobalDataStatic.PartyTankBots.Remove(tankBot);//удаляем танк из пачки вражеский танков

            //если это был последний танк на карте
            if ((GlobalDataStatic.RespawnBotON == false) && (GlobalDataStatic.PartyTankBots.Count == 0))
            {
                _player.Stop();
                GlobalDataStatic.cnvMap1.Background = Brushes.LightYellow;
                //победа в раунде
                btnRaundLose.Visibility = Visibility.Hidden;
                btnRaundWin.Visibility = Visibility.Visible;
                lblResultOfBattleText.Content = "Победа!";
                lblWinText.Content = "Армия врага уничтожена!";
                lblWinText.Visibility = Visibility.Visible;
                //lblResultOfBattleText.Foreground = btnRaundWin.Background;
                //конец раунда
                gridMenu.Visibility = Visibility.Visible;
                GlobalDataStatic.cnvMap1.Children.Clear();

                //ИГРА ПРОЙДЕНА
                if (mapPool.Length == lvlMap + 1)
                {
                    btnRaundLose.Visibility = Visibility.Hidden;
                    btnRaundWin.Visibility = Visibility.Hidden;
                    lblWinText.Content = "Игра пройдена! Маладес тебе!";
                    //lblWinText.Visibility = Visibility.Visible;
                }

                _player.Open(GlobalDataStatic.menuSound);
                _player.Play();
            }
        }
        //уничтожены танки игроков
        private void DistroyFriendlyTank(Tank tank) 
        {
            if (GlobalDataStatic.PartyTanksOfPlayers.Count == 0)
            {
                _player.Stop();
                GlobalDataStatic.cnvMap1.Background = Brushes.Gray;
                //поражение в раунде
                btnRaundLose.Visibility = Visibility.Visible;
                btnRaundWin.Visibility = Visibility.Hidden;              
                lblResultOfBattleText.Content = "Поражение.";
                lblWinText.Content = "Наша армия уничтожена.";
                lblWinText.Visibility = Visibility.Visible;
                //конец раунда
                gridMenu.Visibility = Visibility.Visible;
                GlobalDataStatic.cnvMap1.Children.Clear();

                _player.Open(GlobalDataStatic.menuSound);
                _player.Play();
            }
        }
        //уничтожен бункер
        private void DestroyBunker() 
        {
            _player.Stop();

            GlobalDataStatic.cnvMap1.Background = Brushes.Gray;
            //поражение в раунде
            btnRaundLose.Visibility = Visibility.Visible;
            btnRaundWin.Visibility = Visibility.Hidden;
            lblResultOfBattleText.Foreground = btnRaundWin.Background;
            lblResultOfBattleText.Content = "Поражение.";
            lblWinText.Content = "Штаб был уничтожен врагом.";
            lblWinText.Visibility = Visibility.Visible;
            //конец раунда
            gridMenu.Visibility = Visibility.Visible;
            GlobalDataStatic.cnvMap1.Children.Clear();

            _player.Open(GlobalDataStatic.menuSound);            
            _player.Play();
        }

        //уничтожен вражеский бункер
        private void DestroyBunkerEnamy()
        {
            _player.Stop();

            GlobalDataStatic.cnvMap1.Background = Brushes.LightYellow;
            //победа в раунде
            btnRaundLose.Visibility = Visibility.Hidden;
            btnRaundWin.Visibility = Visibility.Visible;
            lblResultOfBattleText.Content = "Победа!";
            lblWinText.Content = "Штаб врага уничтожен!";
            lblWinText.Visibility = Visibility.Visible;
            //конец раунда
            gridMenu.Visibility = Visibility.Visible;
            GlobalDataStatic.cnvMap1.Children.Clear();


            //ИГРА ПРОЙДЕНА
            if (mapPool.Length == lvlMap + 1)
            {
                btnRaundLose.Visibility = Visibility.Hidden;
                btnRaundWin.Visibility = Visibility.Hidden;
                lblWinText.Content = "Игра пройдена! Маладес тебе!";
                //lblWinText.Visibility = Visibility.Visible;
            }

            _player.Open(GlobalDataStatic.menuSound);
            _player.Play();
        }

        //следующий раунд
        private void btnRaundWin_Click(object sender, RoutedEventArgs e)
        {
            if (mapPool.Length > (++lvlMap) )
            {
                //заполняем карту элементами мира следующего уговня
                CreateViewWorldElements(mapPool[lvlMap]);

                //запускаем респавн  ботов-танков
                tTimer_RespawnBotTank.Start();


                //добавляем игрока на карту следующего раунда               
                System.Windows.Point tPos = map.respawnTankPlayer[0];
                mainTank._ePos = tPos;
                mainTank.UpdateHpForTeer(); //выравниваем HP по тиру
                Canvas.SetTop(mainTank, tPos.X);
                Canvas.SetLeft(mainTank, tPos.Y);
                cnvMain.Children.Add(mainTank);
                
            }
            cnvMain.Background = Brushes.LightYellow;
            gridMenu.Visibility = Visibility.Hidden;

        }
        //повторяем раунд при проигрыше
        private void btnRaundLose_Click(object sender, RoutedEventArgs e)
        {
            //заполняем карту элементами мира следующего уровня
            CreateViewWorldElements(mapPool[lvlMap]);

            //запускаем респавн  ботов-танков
            tTimer_RespawnBotTank.Start();

            //создаем танк игрока
            mainTank = new TankPlayer(map.respawnTankPlayer[0]);
            mainTank.DestroyPayerTank += DistroyFriendlyTank;

            cnvMain.Background = Brushes.LightYellow;
            gridMenu.Visibility = Visibility.Hidden;
        }
        //выгрузка при закрытие программы
        private void MainWin_Unloaded(object sender, RoutedEventArgs e)
        {
            //выключаем плеер
           _player.Close();
        }
    }
}
