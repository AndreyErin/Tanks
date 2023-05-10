using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Server.Model
{
    public class Bullet :  Image
    {
        protected System.Windows.Point _ePos;
        protected VectorEnum _vector;
        protected int _damage;       
        protected System.Timers.Timer tTimerToFire = new System.Timers.Timer();
        protected MediaPlayer _player = GlobalDataStatic.SoundPlayer;
        protected bool _isPlayer = false;

        protected Bullet() { }
        public Bullet(VectorEnum vector, System.Windows.Point tpos, int damage)
        {
            
            _damage = damage;
            _vector = vector;
            _ePos = tpos;

            //_player.Volume = 1;

            //задаем начальное положение пули
            switch (vector)
            {
                case VectorEnum.Top:
                    _ePos.X -= 10;
                    _ePos.Y += 10;
                    break;
                case VectorEnum.Down:
                    _ePos.X += 30;
                    _ePos.Y += 10;
                    break;
                case VectorEnum.Left:
                    _ePos.X += 10;
                    _ePos.Y -= 10;
                    break;
                case VectorEnum.Right:
                    _ePos.X += 10;
                    _ePos.Y += 30;
                    break;
            }

            //настройка таймера выстрела
            tTimerToFire.Interval = 10;
            tTimerToFire.Elapsed += tTimerToFire_Elapsed;
            tTimerToFire.EndInit();

            //отрисовка снаряда            
            Width = 10;
            Height = 10;
            Canvas.SetTop(this, _ePos.X);
            Canvas.SetLeft(this, _ePos.Y);
            

            switch (damage)
            { 

                case 1:
                    Source = Map.PictureBullet1;
                    break;
                case 2:
                    Source = Map.PictureBullet2;
                    break;
                case 3:
                    Source = Map.PictureBullet3;
                    break;
                case > 3:
                    Source = Map.PictureBullet4;                   
                    break;
            }

            GlobalDataStatic.cnvMap1.Children.Add(this);

            //выстрел
           // _player?.Open(GlobalDataStatic.shotSoung);
           // _player.Play();

            tTimerToFire.Start();

        }

        protected void tTimerToFire_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //пуля
            Action action = () =>
            {
                System.Windows.Point pt;//точки-ледары
                System.Windows.Point pt2;
                bool haveHit; //флаг того есть ли попадание по объекту окружения

                switch (_vector)
                {
                    //ВЕРХ
                    case VectorEnum.Top:
                        pt = new System.Windows.Point(_ePos.X, _ePos.Y);
                        pt2 = new System.Windows.Point(_ePos.X, _ePos.Y + this.Width -1);
                        haveHit = HaveShot(pt, pt2);

                        if ((_ePos.X >= 1.5) && (haveHit == false))
                            Canvas.SetTop(this, _ePos.X -= 5);
                        else
                        {
                            Task.Factory.StartNew(DistroyMy);
                        }
                        break;
                    //НИЗ
                    case VectorEnum.Down:
                        pt = new System.Windows.Point(_ePos.X + this.Height -1, _ePos.Y);
                        pt2 = new System.Windows.Point(_ePos.X + this.Height - 1, _ePos.Y + this.Width - 1);
                        haveHit = HaveShot(pt, pt2);
                        if ((_ePos.X <= GlobalDataStatic.cnvMap1.ActualHeight - 11.5) && (haveHit == false))
                            Canvas.SetTop(this, _ePos.X += 5);
                        else
                        {
                            Task.Factory.StartNew(DistroyMy);
                        }
                        break;
                    //ЛЕВО
                    case VectorEnum.Left:
                        pt = new System.Windows.Point(_ePos.X , _ePos.Y);
                        pt2 = new System.Windows.Point(_ePos.X + this.Height -1  , _ePos.Y);
                        haveHit = HaveShot(pt, pt2); 
                        if ((_ePos.Y >= 1.5) && (haveHit == false))
                            Canvas.SetLeft(this, _ePos.Y -= 5);
                        else
                        {
                            Task.Factory.StartNew(DistroyMy);
                        }
                        break;
                    //ПРАВО
                    case VectorEnum.Right:
                        pt = new System.Windows.Point(_ePos.X, _ePos.Y + this.Width - 1);
                        pt2 = new System.Windows.Point(_ePos.X + this.Height - 1, _ePos.Y +   this.Width - 1);
                        haveHit = HaveShot(pt, pt2);
                        if ((_ePos.Y <= GlobalDataStatic.cnvMap1.ActualWidth - 11.5) && (haveHit == false))
                            Canvas.SetLeft(this, _ePos.Y += 5);
                        else
                        {
                            Task.Factory.StartNew(DistroyMy);
                        }
                        break;
                }                
            };

            GlobalDataStatic.MainDispatcher.Invoke(action);
        }

        //если пуля попала в объект, то сообщаем етому обьекту что в него попали
        public bool HaveShot(System.Windows.Point posLedarL, System.Windows.Point posLedarR)
        {
            UIElementCollection collection = GlobalDataStatic.cnvMap1.Children;

                var subset = from UIElement s in collection
                             where (s as WorldElement) != null
                             select s;

                //если есть попадние, то двигаться нельзя
                foreach (WorldElement s in subset)
                {
                    bool result = s.HaveHit(posLedarL, posLedarR);

                    if (result)
                    {
                        //если было попадание по этому объекту коллекции(и это не лут) то сообщаем объкту, что он получил урон
                        if ((s as Loot) == null)
                        {


                            s.GetDamage(_damage);
                            switch (s)
                            {
                                case Tank:
                                if (s.HP <= 0)
                                GlobalDataStatic.lblStatisticTank.Content = int.Parse(GlobalDataStatic.lblStatisticTank.Content.ToString()) + 1;
                                   // Action action = () => { };
                                   //GlobalDataStatic.MainDispatcher.Invoke(action) ;
                                    _player.Open(GlobalDataStatic.shotTargetSound);
                                    break;
                                case LocationGun:
                                    _player.Open(GlobalDataStatic.shotTargetSound);                                   
                                    break;
    
                                case BlockFerum:                                
                                case TankOfDistroy:
                                    _player.Open(GlobalDataStatic.ferumSoung);                                    
                                    break;
    
                                 case Block:
                                    _player.Open(GlobalDataStatic.rockSound);                                   
                                    break;
                            }
                        _player.Play();
                        return true;
                        }               
                    }
                }
                return false;
            

        }

        //уничтожение пули при попадание
        protected void DistroyMy()
        {
           Action action = () =>
           {
               tTimerToFire.Stop();
               GlobalDataStatic.cnvMap1.Children.Remove(this);               
           };
           GlobalDataStatic.MainDispatcher.Invoke(action);
//           Thread.Sleep(1000); //логает пипец
//           _player.Close();
           
            
        }
    }
}
