using System;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace Server.Model
{
    public abstract class Tank : HPElement, ISoundsObjects
    {
        //интерфейс
        public SoundsEnum sound { get; set; }
        public event ISoundsObjects.SoundDeleg? SoundEvent;

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
        public Tank(MyPoint tPos)
        {            
            X = tPos.X;
            Y = tPos.Y;
            _height = 30;
            _width = 30;
            
            //настройка таймера движения
            tTimerMove.Interval = 10;
            tTimerMove.Elapsed += tTimerMove_Elapsed;
            tTimerMove.EndInit();
        }
        
        //функция таймера для движения танка
        protected void tTimerMove_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //tTimerMove.Interval = 100000;
            Action action = () =>
            {
            MyPoint pt;//точки-ледары
                MyPoint pt2;

                switch (tVec)
                {
                    //ВЕРХ
                    case VectorEnum.Top:
                        //проверяем угловые верхние точки танка
                        pt = new MyPoint(X - 3, Y);
                        pt2 = new MyPoint(X - 3, Y + 29);
                        //проверяем нет ли препятствий для движения
                        cMove = CanMove(pt, pt2);
                        //если не достигли границ  поля боя и перед носом у танка нет преграды (дочерних эелметнов поля боя)
                        noEndMap = (X >= speedTank + 1);
                        if (noEndMap && cMove)
                            X -= speedTank;
                        break;
                    //НИЗ
                    case VectorEnum.Down:
                        //проверяем угловые нижние точки танка
                        pt = new MyPoint(X + 32, Y);
                        pt2 = new MyPoint(X + 32, Y + 29);
                        //проверяем нет ли препятствий для движения
                        cMove = CanMove(pt, pt2);
                        noEndMap = (X <= 720 - 31 - speedTank);
                        if (noEndMap && cMove)
                            X += speedTank;
                        break;
                    //ЛЕВО
                    case VectorEnum.Left:
                        //проверяем угловые левые точки танка
                        pt = new MyPoint(X, Y - 3);
                        pt2 = new MyPoint(X + 29, Y - 3);
                        //проверяем нет ли препятствий для движения
                        cMove = CanMove(pt, pt2);
                        noEndMap = (Y >= speedTank + 1);
                        if (noEndMap && cMove)
                            Y -= speedTank;
                        break;
                    //ПРАВО
                    case VectorEnum.Right:
                        //проверяем угловые правые точки танка
                        pt = new MyPoint(X, Y + 32);
                        pt2 = new MyPoint(X + 29, Y + 32);
                        //проверяем нет ли препятствий для движения
                        cMove = CanMove(pt, pt2);
                        noEndMap = (Y <= 1320 - 31 - speedTank);
                        if (noEndMap && cMove)
                            Y += speedTank;
                        break;
                }
            };
            GlobalDataStatic.Controller.Dispatcher.Invoke(action);
        }
        
        //начать движение танка(запустить таймер)
        public void Move(VectorEnum vector)
        {
            //если танк не уничтожен, то едем
            if (GlobalDataStatic.BattleGroundCollection.Contains(this))
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
        
        
        //выстрел
        public void ToFire()
        {
             //если танк не уничтожен, то стреляем
             if (GlobalDataStatic.BattleGroundCollection.Contains(this))
             {
                 //огонь. пуля стреляет сразу при создание объекта
                 Bullet bullet = new Bullet(tVec, new MyPoint(X, Y), damageTank);
                 sound = SoundsEnum.shotSoung;
             }
        }
        
        //если ледары насчупали препятсвие то двигаться дальше нельзя
        protected bool CanMove(MyPoint posLedarL, MyPoint posLedarR)
        {            
            var subset = from s in GlobalDataStatic.BattleGroundCollection
                         where ((s as HPElement) != null) || ((s as Loot) != null)
                         select s;

            //если мы уперлись в лут то получаем его и едем дальше
            foreach (WorldElement s in subset)
            {
                bool result;
                switch (s)
                {
                    case Loot:
                        result = ((Loot)s).HaveHit(posLedarL, posLedarR);

                        if (result)
                        {
                            GetLoot((Loot)s);
                            GlobalDataStatic.BattleGroundCollection.Remove(s);                            
                            return true;
                        }
                        break;
                    case HPElement:
                        result = ((HPElement)s).HaveHit(posLedarL, posLedarR);

                        if (result)
                        {
                            return false;//двигаться нельзя
                        }
                        break;
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
            tTimerMove.Stop();
            base.DistroyMy();            
            TankOfDistroy tankOfDistroy = new TankOfDistroy(new MyPoint(X, Y), tVec, lvlTank, speedTank);
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
                    UpgradeWiewTankSpeed(speedTank);
                    HP = 1;
                    damageTank = 1;
                    lvlTank = 1;
                    break;
            }
            sound = SoundsEnum.bonusSound;
            if (SoundEvent != null) SoundEvent(sound);
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
            UpgradeWiewTank(lvlTank);
        }

        //повышение уровня танка - визуализация -------------------
        protected abstract void UpgradeWiewTank(int teer);
        protected abstract void UpgradeWiewTankSpeed(double speed);

    }
}
