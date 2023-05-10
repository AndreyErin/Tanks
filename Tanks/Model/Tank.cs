using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;

using System.Numerics;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace Tanks.Model
{
    public class Tank : WorldElement
    {
        protected MediaPlayer _player = new MediaPlayer();


        //уровень танка
        protected int lvlTank = 1;
        //дамаг танка
        protected int damageTank = 1;
        //скорость танка
        protected double speedTank = 1.5;
        //НЕ уперлись в край карты
        protected bool noEndMap = false;
        //таймер передвижения танка
        protected System.Timers.Timer tTimerMove = new System.Timers.Timer();
        //направление
        protected VectorEnum tVec { get; set; } = VectorEnum.Top;
        //флаг того можно ли двигаться(нет препятствий)
        protected bool cMove = false;

        protected Tank() { }
        
        //конструктор
        public Tank(System.Windows.Point tPos)
        {
            

            _ePos = tPos;

            //создаем внешний вид танка
            CreatViewTank();

            //отрисовка
            GlobalDataStatic.cnvMap1.Children.Add(this);

            //настройка таймера движения
            tTimerMove.Interval = 10;
            tTimerMove.Elapsed += tTimerMove_Elapsed;
            tTimerMove.EndInit();
        }
        
        //функция таймера для движения танка
        protected void tTimerMove_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Action action = () =>
            {
                System.Windows.Point pt;//точки-ледары
                System.Windows.Point pt2;

            switch (tVec)
            {
                //ВЕРХ
                case VectorEnum.Top:
                    this.LayoutTransform = new RotateTransform(0);//поворот в направление езды
                        //проверяем угловые верхние точки танка
                        pt = new System.Windows.Point(_ePos.X - 3, _ePos.Y);
                        pt2 = new System.Windows.Point(_ePos.X - 3, _ePos.Y + 29);
                        //проверяем нет ли препятствий для движения
                        cMove = CanMove(pt, pt2);
                        //если не достигли границ  поля боя и перед носом у танка нет преграды (дочерних эелметнов поля боя)
                        noEndMap = (_ePos.X >= speedTank + 1);
                        if (noEndMap && cMove)                        
                            Canvas.SetTop(this, _ePos.X -= speedTank); 
                        
                        break;
                    //НИЗ
                    case VectorEnum.Down:
                        this.LayoutTransform = new RotateTransform(180);
                        //проверяем угловые нижние точки танка
                         pt = new System.Windows.Point(_ePos.X + 32, _ePos.Y);
                         pt2 = new System.Windows.Point(_ePos.X + 32, _ePos.Y + 29);
                        //проверяем нет ли препятствий для движения
                        cMove = CanMove(pt, pt2);
                        noEndMap = (_ePos.X <= GlobalDataStatic.cnvMap1.ActualHeight - 31 - speedTank);
                        if (noEndMap && cMove)
                            Canvas.SetTop(this, _ePos.X += speedTank);
                        break;
                    //ЛЕВО
                    case VectorEnum.Left:
                        this.LayoutTransform = new RotateTransform(270);
                        //проверяем угловые левые точки танка
                        pt = new System.Windows.Point(_ePos.X, _ePos.Y - 3);
                        pt2 = new System.Windows.Point(_ePos.X + 29, _ePos.Y - 3);
                        //проверяем нет ли препятствий для движения
                        cMove = CanMove(pt, pt2);
                        noEndMap = (_ePos.Y >= speedTank + 1);
                        if (noEndMap && cMove)
                            Canvas.SetLeft(this, _ePos.Y -= speedTank);
                        break;
                    //ПРАВО
                    case VectorEnum.Right:
                        this.LayoutTransform = new RotateTransform(90);                      
                        //проверяем угловые правые точки танка
                        pt = new System.Windows.Point(_ePos.X, _ePos.Y + 32);
                        pt2 = new System.Windows.Point(_ePos.X + 29, _ePos.Y + 32);
                        //проверяем нет ли препятствий для движения
                        cMove = CanMove(pt, pt2);
                        noEndMap = (_ePos.Y <= GlobalDataStatic.cnvMap1.ActualWidth - 31 - speedTank);
                        if (noEndMap && cMove)
                            Canvas.SetLeft(this, _ePos.Y += speedTank);
                        break;
                }
            };
            GlobalDataStatic.MainDispatcher.Invoke(action);
        }
        
        //начать движение танка(запустить таймер)
        public void Move(VectorEnum vector)
        {
            //если танк не уничтожен, то едем
            if (GlobalDataStatic.cnvMap1.Children.Contains(this))
            {
                //направление движения
                tVec = vector;
                tTimerMove.Start();
            }
        }
        
        //остановыть движение(остановить таймер)
        public void Stop()
        {
            //останавливаем движение танка
            tTimerMove.Stop();
        }
        
        //рисуем изображение танка
        protected void CreatViewTank()
        {
            this.Width = 30;
            this.Height = 30;

            Canvas.SetTop(this, _ePos.X);
            Canvas.SetLeft(this, _ePos.Y);    
        }
        
        //выстрел
        public void ToFire()
        {
//            Task.Factory.StartNew( () =>
//            {
//
//                Action action = () =>
//                {
//                    //если танк не уничтожен, то стреляем
                    if (GlobalDataStatic.cnvMap1.Children.Contains(this))
                    {
                        //огонь. пуля стреляет сразу при создание объекта
                        Bullet bullet = new Bullet(tVec, _ePos, damageTank);
                        _player.Open(GlobalDataStatic.shotSoung);
                        _player.Position = TimeSpan.Zero;
                        _player.Play();
                    }
//                };
//
//                GlobalDataStatic.MainDispatcher.Invoke(action);
//            });

        }
        
        //если ледары насчупали препятсвие то двигаться дальше нельзя
        protected bool CanMove(System.Windows.Point posLedarL, System.Windows.Point posLedarR)
        {
            UIElementCollection collection = GlobalDataStatic.cnvMap1.Children;

            var subset = from UIElement s in collection
                             where (s as WorldElement) != null
                             select s;
            
            //если есть препятствие, то двигаться нельзя
            foreach ( WorldElement s in subset ) 
            {               
                bool result = s.HaveHit(posLedarL, posLedarR);

                if (result) 
                {
                    if (s is Loot) //лут не помеха для движения
                    {
                        GlobalDataStatic.cnvMap1.Children.Remove(s);
                        GetLoot((Loot)s);                                               
                        return true;
                    }
                    else
                        return false;
                }                     
            }
            return true;
        }
        
        //получение дамага
        public override void GetDamage(int damage)
        {
            HP -= damage;
            if (HP < 1)
            {
                DistroyMy();
            }
        }
        
        //уничтожение объекта
        protected override void DistroyMy()
        {
            base.DistroyMy();
            tTimerMove.Stop();
            TankOfDistroy tankOfDistroy = new TankOfDistroy(_ePos, tVec, lvlTank, speedTank);
        }

        //обработка получения лута
        protected void GetLoot(Loot loot) 
        {
            switch (loot.typeUpgrade)
            {
                case BonusEnum.Teer:
                    UpgradeTeer();
                    speedTank = 1.5;
                    break;

                case BonusEnum.Speed:
                    speedTank += 0.5;
                    if (speedTank > 2.5)
                        speedTank = 2.5;
                    Task.Factory.StartNew( ()=> UpgradeWiewTankSpeed(speedTank));
                    HP = 1;
                    damageTank = 1;
                    lvlTank = 1;
                    break;
            }
            _player.Open(GlobalDataStatic.bonusSound);
            _player.Position = TimeSpan.Zero;
            _player.Play();
        }
        
        //поднимаем уровень танка
        protected void UpgradeTeer() 
        {
            lvlTank++;
            if (lvlTank > 4) 
                lvlTank = 4;

            switch (lvlTank)
            {
                case 2:                    
                    HP = 2;
                    damageTank = 2;
                    break;
                case 3:
                    HP = 3;
                    damageTank = 3;
                    break;
                case 4:
                    damageTank = 100;
                    break;      
            }
            Task.Factory.StartNew(() => UpgradeWiewTank(lvlTank));
        }
        
        //повышение уровня танка - визуализация -------------------
        protected virtual void UpgradeWiewTank(int teer) 
        {
//            switch (teer)
//            {
//                case 2:
//                    Source = Map.PictureTank2;
//                    break;
//                case 3:
//                    Source = Map.PictureTank3;
//                    break;
//                case 4:
//                    //Source = Map.PictureTank3;
//                    break;
//            }           
        }

        protected virtual void UpgradeWiewTankSpeed(double speed)
        {
            Action action = () =>
            {
                switch (speed)
                {
                    case 2.0:
                        Source = Map.PictureTankSpeed;
                        break;
                    case 2.5:
                        Source = Map.PictureTankSpeed2;
                        break;
                }
            };
            GlobalDataStatic.MainDispatcher.Invoke(action);
        }
    }
}
