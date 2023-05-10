using System;
using System.Linq;
using System.Threading.Tasks;
using Server.Model;

//ред
namespace Server.Model
{
    public class Bullet :  WorldElement,  ISoundsObjects
    {
        //protected System.Windows.Point _ePos;
        protected VectorEnum _vector;
        protected int _damage;       
        protected System.Timers.Timer tTimerToFire = new System.Timers.Timer();        
        //protected bool _isPlayer = false;
        protected SoundsEnum _sound; //звук

        public event ISoundsObjects.SoundDeleg? SoundEvent;

        protected Bullet() { }
        //конструктор
        public Bullet(VectorEnum vector, System.Windows.Point tpos, int damage)
        {           
            _damage = damage;
            _vector = vector;
            ePos = tpos;

            //задаем начальное положение пули
            switch (vector)
            {
                case VectorEnum.Top:
                    ePos.X = ePos.X- 10;
                    ePos.Y += 10;
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
            _width = 10;
            _height = 10;
 
            switch (damage)
            { 
                case 1:
                    _skin = SkinsEnum.PictureBullet1;
                    break;
                case 2:
                    _skin = SkinsEnum.PictureBullet2;
                    break;
                case 3:
                    _skin = SkinsEnum.PictureBullet3;
                    break;
                case > 3:
                    _skin = SkinsEnum.PictureBullet4;                   
                    break;
            }

            GlobalDataStatic.BattleGroundCollection.Add(this);

            tTimerToFire.Start();

        }
        //таймер
        protected void tTimerToFire_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //пуля
            System.Windows.Point pt;//точки-ледары
            System.Windows.Point pt2;
            bool haveHit; //флаг того есть ли попадание по объекту окружения

            switch (_vector)
            {
                //ВЕРХ
                case VectorEnum.Top:
                    pt = new System.Windows.Point(ePos.X, ePos.Y);
                    pt2 = new System.Windows.Point(ePos.X, ePos.Y + this._width -1);
                    haveHit = HaveShot(pt, pt2);

                    if ((ePos.X >= 1.5) && (haveHit == false))
                        ePos.X -= 5;
                    else
                    {
                        Task.Factory.StartNew(DistroyMy);
                    }
                    break;
                //НИЗ
                case VectorEnum.Down:
                    pt = new System.Windows.Point(ePos.X + this._height -1, ePos.Y);
                    pt2 = new System.Windows.Point(ePos.X + this._height - 1, ePos.Y + this._width - 1);
                    haveHit = HaveShot(pt, pt2);
                    if ((ePos.X <= 720 - 11.5) && (haveHit == false))
                        ePos.X += 5;
                    else
                    {
                        Task.Factory.StartNew(DistroyMy);
                    }
                    break;
                //ЛЕВО
                case VectorEnum.Left:
                    pt = new System.Windows.Point(ePos.X , ePos.Y);
                    pt2 = new System.Windows.Point(ePos.X + this._height -1  , ePos.Y);
                    haveHit = HaveShot(pt, pt2); 
                    if ((ePos.Y >= 1.5) && (haveHit == false))
                        ePos.Y -= 5;
                    else
                    {
                        Task.Factory.StartNew(DistroyMy);
                    }
                    break;
                //ПРАВО
                case VectorEnum.Right:
                    pt = new System.Windows.Point(ePos.X, ePos.Y + this._width - 1);
                    pt2 = new System.Windows.Point(ePos.X + this._height - 1, ePos.Y +   this._width - 1);
                    haveHit = HaveShot(pt, pt2);
                    if ((ePos.Y <= 1320 - 11.5) && (haveHit == false))
                        ePos.Y += 5;
                    else
                    {
                        Task.Factory.StartNew(DistroyMy);
                    }
                    break;
            }                
        }

        //если пуля попала в объект, то сообщаем етому обьекту что в него попали
        public bool HaveShot(System.Windows.Point posLedarL, System.Windows.Point posLedarR)
        {
                var subset = from s in GlobalDataStatic.BattleGroundCollection
                             where (s as WorldElement) != null
                             select s;

                //если есть попадние, то двигаться нельзя
                foreach (HPElement s in subset)
                {
                    bool result = s.HaveHit(posLedarL, posLedarR);

                    if (result)
                    {
                        //если было попадание по этому объекту коллекции(и это не лут) то сообщаем объкту, что он получил урон
//                        if ((s as Loot) == null)
//                        {
                            s.GetDamage(_damage);
                            switch (s)
                            {
                                case Tank:
                                if (s.HP <= 0)

                                    _sound = SoundsEnum.shotTargetSound;
                                    break;
                                case LocationGun:
                                    _sound = SoundsEnum.shotTargetSound;
                                break;
    
                                case BlockFerum:                                
                                case TankOfDistroy:
                                    _sound = SoundsEnum.ferumSoung;                                    
                                    break;
    
                                 case Block:
                                    _sound = SoundsEnum.rockSound;                                   
                                    break;
                            }

                        if (SoundEvent != null) SoundEvent(_sound);//играть звук

                        return true;
//                        }               
                    }
                }
                return false;            
        }

        //уничтожение пули при попадание
        protected void DistroyMy()
        {
               tTimerToFire.Stop();
               GlobalDataStatic.BattleGroundCollection.Remove(this);                                   
        }
    }
}
