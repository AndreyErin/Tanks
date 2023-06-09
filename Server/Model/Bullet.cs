﻿using System.Linq;

//ред
namespace Server.Model
{
    public class Bullet :  WorldElement,  ISoundsObjects
    {
        //интерфейс ISoundsObjects
        public SoundsEnum sound { get; set; }
        public event ISoundsObjects.SoundDeleg? SoundEvent;
        
        protected VectorEnum _vector;
        protected int _damage;
        protected TankPlayer? _owner; //хозяин снаряда
        
        public Bullet() 
        {
            //добавлен в стек
            SoundEvent += GlobalDataStatic.Controller.SoundOfElement;
        }
        //конструктор
        public void InitElement(VectorEnum vector, MyPoint tpos, int damage, TankPlayer? owner = null)
        {
            _owner = owner;

            _damage = damage;
            _vector = vector;
            X = tpos.X;
            Y = tpos.Y;
            
            //задаем начальное положение пули
            switch (vector)
            {
                case VectorEnum.Top:
                    X -= 10; 
                    Y += 10;
                    break;
                case VectorEnum.Down:
                    X += 30;
                    Y += 10;
                    break;
                case VectorEnum.Left:
                    X += 10;
                    Y -= 10;
                    break;
                case VectorEnum.Right:
                    X += 10;
                    Y += 30;
                    break;
            }

            //отрисовка снаряда            
            _width = 10;
            _height = 10;
 
            switch (damage)
            { 
                case 1:
                    Skin = SkinsEnum.PictureBullet1;
                    break;
                case 2:
                    Skin = SkinsEnum.PictureBullet2;
                    break;
                case 3:
                    Skin = SkinsEnum.PictureBullet3;
                    break;
                case > 3:
                    Skin = SkinsEnum.PictureBullet4;                   
                    break;
            }
            AddMe();

            //tTimerToFire.Start();
            GlobalDataStatic.Controller.GlobalTimerMove.Elapsed += tTimerToFire_Elapsed;

        }
        //таймер
        protected void tTimerToFire_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!GlobalDataStatic.BattleGroundCollection.ContainsKey(ID)) 
            {
                GlobalDataStatic.Controller.GlobalTimerMove.Elapsed -= tTimerToFire_Elapsed;
                return;
            } 
                

            //пуля
            MyPoint pt;//точки-ледары
            MyPoint pt2;
            bool haveHit; //флаг того есть ли попадание по объекту окружения

            switch (_vector)
            {
                //ВЕРХ
                case VectorEnum.Top:
                    pt = new MyPoint(X, Y);
                    pt2 = new MyPoint(X, Y + this._width -1);
                    haveHit = HaveShot(pt, pt2);

                    if ((X >= 1.5) && (haveHit == false))
                        X -= 7;
                    else
                    {
                        DistroyMy();
                    }
                    break;
                //НИЗ
                case VectorEnum.Down:
                    pt = new MyPoint(X + this._height -1, Y);
                    pt2 = new MyPoint(X + this._height - 1, Y + this._width - 1);
                    haveHit = HaveShot(pt, pt2);
                    if ((X <= 720 - 11.5) && (haveHit == false))
                        X += 7;
                    else
                    {                        
                        DistroyMy();
                    }
                    break;
                //ЛЕВО
                case VectorEnum.Left:
                    pt = new MyPoint(X , Y);
                    pt2 = new MyPoint(X + this._height -1  , Y);
                    haveHit = HaveShot(pt, pt2); 
                    if ((Y >= 1.5) && (haveHit == false))
                        Y -= 7;
                    else
                    {
                        DistroyMy();
                    }
                    break;
                //ПРАВО
                case VectorEnum.Right:
                    pt = new MyPoint(X, Y + this._width - 1);
                    pt2 = new MyPoint(X + this._height - 1, Y +   this._width - 1);
                    haveHit = HaveShot(pt, pt2);
                    if ((Y <= 1320 - 11.5) && (haveHit == false))
                        Y += 7;
                    else
                    {
                        DistroyMy();
                    }
                    break;
            }                
        }

        //если пуля попала в объект, то сообщаем етому обьекту что в него попали
        public bool HaveShot(MyPoint posLedarL, MyPoint posLedarR)
        {
            var subset = from s in GlobalDataStatic.BattleGroundCollection
                         where (s.Value as HPElement) != null
                         select s;

            //если есть попадние, то двигаться нельзя
            foreach (var s in subset)
            {
                bool result = ((HPElement)s.Value).HaveHit(posLedarL, posLedarR);

                if (result)
                {                                                                   
                    switch (s.Value)
                    {
                        case Tank:
                            //если танк уничтожен
                            if ((((HPElement)s.Value).HP - _damage) <= 0)
                            {
                                sound = SoundsEnum.shotTargetSound;
                                //если хозяин танка был игроком, тогда записываем фраг                          
                                _owner?.Frag(((Tank)s.Value).lvlTank, ((Tank)s.Value).speedTank);
                            }
                            break;
                        case LocationGun:
                            if ((((HPElement)s.Value).HP - _damage) <= 0)
                            {
                                sound = SoundsEnum.shotTargetSound;
                                _owner?.FragLocationGun();
                            }
                            break;
    
                        case BlockFerum:                                
                        case TankOfDistroy:
                            sound = SoundsEnum.ferumSoung;                                    
                            break;
    
                        case Block:
                            sound = SoundsEnum.rockSound;                                   
                            break;
                    }

                    ((HPElement)s.Value).GetDamage(_damage);

                    //при попадание по любому тобъекту будем обновлять HP танков игроков в клиенте
                    PartyPlayers.One?.UploadHpPlayers();

                    if (GlobalDataStatic.Controller.IsMultiPlayer)
                        PartyPlayers.Two?.UploadHpPlayers();

                    SoundEvent?.Invoke(sound);//играть звук
                    return true;                                       
                }
            }
            return false;            
        }

        //уничтожение пули при попадание
        protected void DistroyMy()
        {           
            //SoundEvent = null;
            RemoveMe();           
        }
    }
}
