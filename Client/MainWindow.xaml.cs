using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.IO;
using System.Windows.Input;
using Client.Model;

namespace Client
{
    

    public partial class MainWindow : Window
    {
        private TcpClient tcpClient;

        public MainWindow()
        {
            InitializeComponent();
        }

        private Key _moveKey = Key.None;//кнопка отслеживающая пследнее движение
        private Key _lastKey = Key.None;//кнопка нажатая пользователем
        //двигаем танк ------править
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
        //править
        private void MainWin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == _moveKey)//если кнопка движения была поднята то останавливаем танк
                tcpClient.KeyOfControlTank(Key.S);

            _lastKey = Key.None;
        }
        //описан
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

        private void MainWin_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MainWin_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnOut_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRaundWin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRaundLose_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
