using System;
using System.Linq;

namespace Server.Model
{
    public abstract class Tank : HPElement, ISoundsObjects
    {
        protected bool timerON = false;

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
  
        //флаг того можно ли двигаться(нет препятствий)
        protected bool cMove = false;
        
        protected Tank() 
        {
            //подписываемся на отправку звуков
            SoundEvent += GlobalDataStatic.Controller.SoundOfElement;
        }
        
        //конструктор
        public void InitElementBase(MyPoint tPos)
        {            
            X = tPos.X;
            Y = tPos.Y;
            _height = 30;
            _width = 30;
            lvlTank = 1;
            damageTank = 1;
            speedTank = 1.5;
        }
        
        //функция таймера для движения танка
        protected void tTimerMove_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!GlobalDataStatic.BattleGroundCollection.ContainsKey(ID)) return;

                //tTimerMove.Interval = 100000;
                Action action = () =>
            {
            MyPoint pt;//точки-ледары
                MyPoint pt2;

                switch (VectorElement)
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
            ////если танк не уничтожен, то едем
            if (!GlobalDataStatic.BattleGroundCollection.ContainsKey(ID)) return;
            //{
            //направление движения
            VectorElement = vector;
            //подрубаемся к общиму таймеру
            if (timerON == false)
            {
                GlobalDataStatic.Controller.GlobalTimerMove.Elapsed += tTimerMove_Elapsed;
                timerON = true;
            }

        }
        
        //остановыть движение(остановить таймер)
        public void Stop()
        {
            //отключаемся от общего таймера
            if (timerON)
            {
                GlobalDataStatic.Controller.GlobalTimerMove.Elapsed -= tTimerMove_Elapsed;
                timerON = false;
            }
        }
                
        //выстрел
        public void ToFire()
        {
             //если танк не уничтожен, то стреляем
             if (GlobalDataStatic.BattleGroundCollection.ContainsKey(ID))
             {
                 //огонь. пуля стреляет сразу при создание объекта
                 GlobalDataStatic.StackBullet.Pop().InitElement(VectorElement, new MyPoint(X, Y), damageTank);
                 sound = SoundsEnum.shotSoung;
                 SoundEvent?.Invoke(sound);
             }
        }
        
        //если ледары насчупали препятсвие то двигаться дальше нельзя
        protected bool CanMove(MyPoint posLedarL, MyPoint posLedarR)
        {            
            var subset = from s in GlobalDataStatic.BattleGroundCollection
                         where ((s.Value as HPElement) != null) || ((s.Value as Loot) != null)
                         select s;

            //если мы уперлись в лут то получаем его и едем дальше
            foreach (var s in subset)
            {
                bool result;
                switch (s.Value)
                {
                    case Loot:
                        result = ((Loot)s.Value).HaveHit(posLedarL, posLedarR);

                        if (result)
                        {
                            GetLoot((Loot)s.Value);                           
                            ((Loot)s.Value).RemoveMe();
                                                       
                            return true;
                        }
                        break;
                    case HPElement:
                        result = ((HPElement)s.Value).HaveHit(posLedarL, posLedarR);

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
            //отключаемся от общего таймера
            GlobalDataStatic.Controller.GlobalTimerMove.Elapsed -= tTimerMove_Elapsed;
            timerON = false;
            base.DistroyMy();            
            GlobalDataStatic.StackTankOfDistroy.Pop().InitElement(new MyPoint(X, Y), VectorElement, lvlTank, speedTank);
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
            SoundEvent?.Invoke(sound);
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
                    damageTank = 30;
                    break;      
            }
            UpgradeWiewTank(lvlTank);
        }

        //повышение уровня танка - визуализация -------------------
        protected abstract void UpgradeWiewTank(int teer);
        protected abstract void UpgradeWiewTankSpeed(double speed);

    }
}
