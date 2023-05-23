using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.IO;
using System.Windows.Input;
using Client.Model;
using System;

namespace Client
{
    

    public partial class MainWindow : Window
    {
        private TcpClient tcpClient;

        public MainWindow()
        {
            InitializeComponent();
            GlobalDataStatic.Controller = this;
            GlobalDataStatic.DispatcherMain = Dispatcher;
            ///////////////////////////////////////////////////////////////////
            //WorldElement w = new WorldElement(21,new MyPoint(10,20), SkinsEnum.PictureBlock1);
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
                    //сообщение серверу
                    tcpClient.KeyOfControlTank(Key.Up);
                    _moveKey = e.Key;
                    
                    break;
                case Key.S:
                case Key.Down:
                    tcpClient.KeyOfControlTank(Key.Down);
                    _moveKey = e.Key;
                    break;
                case Key.A:
                case Key.Left:
                    tcpClient.KeyOfControlTank(Key.Left);
                    _moveKey = e.Key;
                    break;
                case Key.D:
                case Key.Right:
                    tcpClient.KeyOfControlTank(Key.Right);
                    _moveKey = e.Key;
                    break;
                case Key.Space: //стрельба
                    if (cD == false)
                    {
                        tcpClient.KeyOfControlTank(Key.Space);
                        Task.Factory.StartNew(CooldownFire);//запускаем откат в отдельном потоке
                    }
                    break;
            }

        }
        
        private void MainWin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == _moveKey)//если кнопка движения была поднята то останавливаем танк
                tcpClient.KeyOfControlTank(Key.S);

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

        //загрузка программы ----------------
        private void MainWin_Loaded(object sender, RoutedEventArgs e)
        {
            tcpClient = new TcpClient();
        }

        //завершение программы ---------------
        private void MainWin_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        //выход - отключение от сервера дописать------------------
        private void btnOut_Click(object sender, RoutedEventArgs e)
        {
            tcpClient.MenuComand(MenuComandEnum.Out);
        }
        //новая игра
        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("контроллер клиентской программы");
            tcpClient.MenuComand(MenuComandEnum.NewGame);
            
        }
        //новый раунд
        private void btnRaundWin_Click(object sender, RoutedEventArgs e)
        {
            tcpClient.MenuComand(MenuComandEnum.NewRaund);
        }
        //переигровка раунда
        private void btnRaundReplay_Click(object sender, RoutedEventArgs e)
        {
            tcpClient.MenuComand(MenuComandEnum.Replay);
        }

        //добавление объекта на поле боя
        public void AddElement(int id, MyPoint pos, SkinsEnum skin) 
        {
            try
            {
                //MessageBox.Show("контроллер клиента\n" + Thread.CurrentThread.ManagedThreadId.ToString());
                //Action action = () =>
                //{
                WorldElement w = new WorldElement(id, pos, skin);
                //}; 
                //GlobalDataStatic.DispatcherMain.Invoke(action);
            }
            catch (Exception ex)
            {
                MessageBox.Show("попытка создать объект" + ex.Message);
            }
        }
        //удаление объекта с поля боя
        public void RemoveElement() 
        {

        }
    }
}
