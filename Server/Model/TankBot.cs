using System;
using System.Linq;

namespace Server.Model
{
    public class TankBot : Tank
    {
        //делегат сообщающий в Main, что танк был уничтожен
        //нужен для того, чтобы определять уничтоженна ли группировка танков-ботов или нет
        public event Action<TankBot> DistroyEvent;
        //таймер автопилота
        protected System.Timers.Timer tAutoMove = new System.Timers.Timer();

        protected HPElement? target = null;

        protected TankBot() { }
        public TankBot(MyPoint tPos) : base(tPos)
        {
            Skin = SkinsEnum.PictureTankBot1; 

            tAutoMove.Interval = 1000;
            tAutoMove.Elapsed += AutoMove;
            tAutoMove.EndInit();
            tAutoMove.Start();
            AddMe();
        }
        //таймер для автоматического передвижения
        protected void AutoMove(object sender, System.Timers.ElapsedEventArgs e) 
        {
            //если объект удален с карты, то останавливаем таймер
            if (!GlobalDataStatic.BattleGroundCollection.Contains(this))
            {
                tAutoMove.Stop();
            }


            //стрельба
            MyPoint pt;
            MyPoint pt2;
            bool enemy = false;
            switch (tVec)
            {
                //ВЕРХ
                case VectorEnum.Top:
                    //если уперлись в разбитый танк, то стреляем
                    pt = new MyPoint(ePos.X - 3, ePos.Y + 9);
                    pt2 = new MyPoint(ePos.X - 3, ePos.Y + 19);
                    if (CanTarget(pt, pt2))
                    {
                        enemy = CanTargetDestroyTank();
                        if (enemy)
                        {
                            ToFire();
                            break;
                        }
                    }

                    

                    pt = new MyPoint(ePos.X - 29, ePos.Y + 9);
                    pt2 = new MyPoint(ePos.X - 29, ePos.Y + 19);

                    //если нет попадания продолжаем перечислять
                    while ((CanTarget(pt, pt2) == false) && (pt.X > 29))
                    {
                        pt.X -= 29;
                        pt2.X -= 29;
                    }

                    //если враг есть
                    enemy = CanTargetEnemy();
                    if (enemy)
                        ToFire();
                    break;
                //НИЗ
                case VectorEnum.Down:

                    //если уперлись в разбитый танк, то стреляем
                    pt = new MyPoint(ePos.X + 32, ePos.Y + 9);
                    pt2 = new MyPoint(ePos.X + 32, ePos.Y + 19);
                    if (CanTarget(pt, pt2))
                    {
                        enemy = CanTargetDestroyTank();
                        if (enemy)
                        {
                            ToFire();
                            break;
                        }
                    }

                    pt = new MyPoint(ePos.X + 58, ePos.Y + 9);
                    pt2 = new MyPoint(ePos.X + 58, ePos.Y + 19);

                    while ((CanTarget(pt, pt2) == false) && (pt.X < (720 - 29)))
                    {
                        pt.X += 29;
                        pt2.X += 29;
                    }

                    //если враг есть
                    enemy = CanTargetEnemy();
                    if (enemy)
                        ToFire();
                    break;
                //ЛЕВО
                case VectorEnum.Left:

                    //если уперлись в разбитый танк, то стреляем
                    pt = new MyPoint(ePos.X + 9, ePos.Y - 3);
                    pt2 = new MyPoint(ePos.X + 19, ePos.Y - 3);
                    if (CanTarget(pt, pt2))
                    {
                        enemy = CanTargetDestroyTank();
                        if (enemy)
                        {
                            ToFire();
                            break;
                        }
                    }

                    pt = new MyPoint(ePos.X + 9, ePos.Y - 29);
                    pt2 = new MyPoint(ePos.X + 19, ePos.Y - 29);

                    while ((CanTarget(pt, pt2) == false) && (pt.Y > 29))
                    {
                        pt.Y -= 29;
                        pt2.Y -= 29;
                    }

                    //если враг есть
                    enemy = CanTargetEnemy();
                    if (enemy)
                        ToFire();
                    break;
                //ПРАВО
                case VectorEnum.Right:

                    //если уперлись в разбитый танк, то стреляем
                    pt = new MyPoint(ePos.X + 9, ePos.Y + 32);
                    pt2 = new MyPoint(ePos.X + 19, ePos.Y + 32);
                    if (CanTarget(pt, pt2))
                    {
                        enemy = CanTargetDestroyTank();
                        if (enemy)
                        {
                            ToFire();
                            break;
                        }
                    }

                    pt = new MyPoint(ePos.X + 9, ePos.Y + 58);
                    pt2 = new MyPoint(ePos.X + 19, ePos.Y + 58);

                    while ((CanTarget(pt, pt2) == false) && (pt.Y < (1320 - 29)))
                    {
                        pt.Y += 29;
                        pt2.Y += 29;
                    }

                    //если враг есть
                    enemy = CanTargetEnemy();
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

        }

        
        //проверяем ближайшую цель на пути
        protected bool CanTarget(MyPoint posLedarL, MyPoint posLedarR)
        {
            var subset = from s in GlobalDataStatic.BattleGroundCollection
                         where (s as HPElement) != null
                         select s;

            foreach (HPElement s in subset)
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
        
        protected bool CanTargetEnemy()
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

        protected bool CanTargetDestroyTank()
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
            tAutoMove.Stop();
            base.DistroyMy();            
            DistroyEvent(this); //передаем информацию о разрущшение танка
        }
        //повышение уровня танка
        protected override void UpgradeWiewTank(int teer)
        {
            switch (teer)
            {
                case 2:
                    Skin = SkinsEnum.PictureTankBot2;
                    break;
                case 3:
                case 4:
                    Skin = SkinsEnum.PictureTankBot3;
                    break;
            }
        }

        protected override void UpgradeWiewTankSpeed(double speed)
        {
            switch (speed)
            {
                case 2.0:
                    Skin = SkinsEnum.PictureTankSpeedBot;
                    break;
                case 2.5:
                    Skin = SkinsEnum.PictureTankSpeedBot2;
                    break;
            }

        }
    }
}
