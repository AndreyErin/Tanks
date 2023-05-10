using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Server.Model
{
    public class TankBot : Tank
    {
        //делегат сообщающий в Main что танк был уничтожен
        //нужен для того, чтобы определять уничтоженна ли группировка танков-ботов или нет
        public event Action<TankBot> DistroyEvent;
        //таймер автопилота
        protected System.Timers.Timer tAutoMove = new System.Timers.Timer();

        protected TankBot() { }
        public TankBot(System.Windows.Point tPos) : base(tPos)
        {
            Source = Map.PictureTankBot1; 

            tAutoMove.Interval = 1000;
            tAutoMove.Elapsed += AutoMove;
            tAutoMove.EndInit();
            tAutoMove.Start();
            
        }
        //таймер для автоматического передвижения
        protected void AutoMove(object sender, System.Timers.ElapsedEventArgs e) 
        {
            //если объект удален с карты, то останавливаем таймер
            if (this.Parent != GlobalDataStatic.cnvMap1)
            {
                tAutoMove.Stop();
            }

            Action action = () =>
            {

                //стрельба
                System.Windows.Point pt;
                System.Windows.Point pt2;
                bool enemy = false;
                switch (tVec)
                {
                    //ВЕРХ
                    case VectorEnum.Top:
                        //если уперлись в разбитый танк, то стреляем
                        pt = new System.Windows.Point(_ePos.X - 3, _ePos.Y + 9);
                        pt2 = new System.Windows.Point(_ePos.X - 3, _ePos.Y + 19);
                        if (CanTarget(pt, pt2))
                        {
                            enemy = CanTargetDestroyTank(pt, pt2);
                            if (enemy)
                            {
                                ToFire();
                                break;
                            }
                        }

                        

                        pt = new System.Windows.Point(_ePos.X - 29, _ePos.Y + 9);
                        pt2 = new System.Windows.Point(_ePos.X - 29, _ePos.Y + 19);

                        //если нет попадания продолжаем перечислять
                        while ((CanTarget(pt, pt2) == false) && (pt.X > 29))
                        {
                            pt.X -= 29;
                            pt2.X -= 29;
                        }

                        //если враг есть
                        enemy = CanTargetEnemy(pt, pt2);
                        if (enemy)
                            ToFire();
                        break;
                    //НИЗ
                    case VectorEnum.Down:

                        //если уперлись в разбитый танк, то стреляем
                        pt = new System.Windows.Point(_ePos.X + 32, _ePos.Y + 9);
                        pt2 = new System.Windows.Point(_ePos.X + 32, _ePos.Y + 19);
                        if (CanTarget(pt, pt2))
                        {
                            enemy = CanTargetDestroyTank(pt, pt2);
                            if (enemy)
                            {
                                ToFire();
                                break;
                            }
                        }

                        pt = new System.Windows.Point(_ePos.X + 58, _ePos.Y + 9);
                        pt2 = new System.Windows.Point(_ePos.X + 58, _ePos.Y + 19);

                        while ((CanTarget(pt, pt2) == false) && (pt.X < (720 - 29)))
                        {
                            pt.X += 29;
                            pt2.X += 29;
                        }

                        //если враг есть
                        enemy = CanTargetEnemy(pt, pt2);
                        if (enemy)
                            ToFire();
                        break;
                    //ЛЕВО
                    case VectorEnum.Left:

                        //если уперлись в разбитый танк, то стреляем
                        pt = new System.Windows.Point(_ePos.X + 9, _ePos.Y - 3);
                        pt2 = new System.Windows.Point(_ePos.X + 19, _ePos.Y - 3);
                        if (CanTarget(pt, pt2))
                        {
                            enemy = CanTargetDestroyTank(pt, pt2);
                            if (enemy)
                            {
                                ToFire();
                                break;
                            }
                        }

                        pt = new System.Windows.Point(_ePos.X + 9, _ePos.Y - 29);
                        pt2 = new System.Windows.Point(_ePos.X + 19, _ePos.Y - 29);

                        while ((CanTarget(pt, pt2) == false) && (pt.Y > 29))
                        {
                            pt.Y -= 29;
                            pt2.Y -= 29;
                        }

                        //если враг есть
                        enemy = CanTargetEnemy(pt, pt2);
                        if (enemy)
                            ToFire();
                        break;
                    //ПРАВО
                    case VectorEnum.Right:

                        //если уперлись в разбитый танк, то стреляем
                        pt = new System.Windows.Point(_ePos.X + 9, _ePos.Y + 32);
                        pt2 = new System.Windows.Point(_ePos.X + 19, _ePos.Y + 32);
                        if (CanTarget(pt, pt2))
                        {
                            enemy = CanTargetDestroyTank(pt, pt2);
                            if (enemy)
                            {
                                ToFire();
                                break;
                            }
                        }

                        pt = new System.Windows.Point(_ePos.X + 9, _ePos.Y + 58);
                        pt2 = new System.Windows.Point(_ePos.X + 19, _ePos.Y + 58);

                        while ((CanTarget(pt, pt2) == false) && (pt.Y < (1320 - 29)))
                        {
                            pt.Y += 29;
                            pt2.Y += 29;
                        }

                        //если враг есть
                        enemy = CanTargetEnemy(pt, pt2);
                        if (enemy)
                            ToFire();
                        break;

                }

                if (!enemy)
                {
                    //движение
                    Random random = new Random();
                    if (cMove == false || noEndMap == false)
                    {
                        switch (tVec)
                        {
                            case VectorEnum.Top:
                                switch (random.Next(0, 3))
                                {
                                    case 0:
                                        tVec = VectorEnum.Left;
                                        break;
                                    case 1:
                                        tVec = VectorEnum.Right;
                                        break;
                                    case 2:
                                        tVec = VectorEnum.Down;
                                        break;
                                    default:
                                        break;
                                }
                                break;

                            case VectorEnum.Down:
                                switch (random.Next(0, 3))
                                {
                                    case 0:
                                        tVec = VectorEnum.Left;
                                        break;
                                    case 1:
                                        tVec = VectorEnum.Right;
                                        break;
                                    case 2:
                                        tVec = VectorEnum.Top;
                                        break;
                                    default:
                                        break;
                                }
                                break;

                            case VectorEnum.Left:
                                switch (random.Next(0, 3))
                                {
                                    case 0:
                                        tVec = VectorEnum.Top;
                                        break;
                                    case 1:
                                        tVec = VectorEnum.Right;
                                        break;
                                    case 2:
                                        tVec = VectorEnum.Down;
                                        break;
                                    default:
                                        break;
                                }
                                break;

                            case VectorEnum.Right:
                                switch (random.Next(0, 3))
                                {
                                    case 0:
                                        tVec = VectorEnum.Left;
                                        break;
                                    case 1:
                                        tVec = VectorEnum.Top;
                                        break;
                                    case 2:
                                        tVec = VectorEnum.Down;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                        }
                        Move(tVec);

                    }
                }
            };
            GlobalDataStatic.MainDispatcher.Invoke(action);

        }

        protected WorldElement target = null;
        //проверяем ближайшую цель на пути
        protected bool CanTarget(System.Windows.Point posLedarL, System.Windows.Point posLedarR)
        {
            UIElementCollection collection = GlobalDataStatic.cnvMap1.Children;
            var subset = from UIElement s in collection
                         where ((s as WorldElement) != null) && ((s as Loot) == null)
                         select s;

            foreach (WorldElement s in subset)
            {
                bool result = s.HaveHit(posLedarL, posLedarR);

                if (result)
                {
                    target = s; //цель найдена
                    return true;
                }                       
            }
            target = null;
            return false;
        }
        
        protected bool CanTargetEnemy(System.Windows.Point posLedarL, System.Windows.Point posLedarR)
        {
            //если ближайшая цель является врагом
            if (target != null)
            {
                if (target.IsPlayer)
                {
                    return true;
                }
            }
                           
            return false;
        }

        protected bool CanTargetDestroyTank(System.Windows.Point posLedarL, System.Windows.Point posLedarR)
        {
            //если ближайшая цель является врагом
            if (target != null)
            {
                if (target is TankOfDistroy)
                {
                    return true;
                }
            }

            return false;
        }
        //уничтожение объекта
        protected override void DistroyMy()
        {            
            base.DistroyMy();
            tAutoMove.Stop();
            DistroyEvent(this); //передаем информацию о разрущшение танка
        }
        //повышение уровня танка
        protected override void UpgradeWiewTank(int teer)
        {
            Action action = () =>
            {
                switch (teer)
                {
                    case 2:
                        Source = Map.PictureTankBot2;
                        break;
                    case 3:
                        Source = Map.PictureTankBot3;
                        break;
                    case 4:
                        //Source = Map.PictureTankBot3;
                        break;
                }
            };
            GlobalDataStatic.MainDispatcher.Invoke(action);
        }

        protected override void UpgradeWiewTankSpeed(double speed)
        {
            Action action = () =>
            {
                switch (speed)
                {
                    case 2.0:
                        Source = Map.PictureTankSpeedBot;
                        break;
                    case 2.5:
                        Source = Map.PictureTankSpeedBot2;
                        break;
                }
            };
            GlobalDataStatic.MainDispatcher.Invoke(action);
        }
    }
}
