using System.Linq;
using System.Threading.Tasks;
using Server.Model;

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
        protected System.Timers.Timer tTimerToFire = new System.Timers.Timer();             


        protected Bullet() { }
        //конструктор
        public Bullet(VectorEnum vector, MyPoint tpos, int damage)
        {    
            _damage = damage;
            _vector = vector;
            ePos = tpos;
            
            //задаем начальное положение пули
            switch (vector)
            {
                case VectorEnum.Top:
                    ePos.X -= 10; 
                    ePos.Y += 10;
                    break;
                case VectorEnum.Down:
                    ePos.X += 30;
                    ePos.Y += 10;
                    break;
                case VectorEnum.Left:
                    ePos.X += 10;
                    ePos.Y -= 10;
                    break;
                case VectorEnum.Right:
                    ePos.X += 10;
                    ePos.Y += 30;
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

            GlobalDataStatic.BattleGroundCollection.Add(this);

            tTimerToFire.Start();

        }
        //таймер
        protected void tTimerToFire_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //пуля
            MyPoint pt;//точки-ледары
            MyPoint pt2;
            bool haveHit; //флаг того есть ли попадание по объекту окружения

            switch (_vector)
            {
                //ВЕРХ
                case VectorEnum.Top:
                    pt = new MyPoint(ePos.X, ePos.Y);
                    pt2 = new MyPoint(ePos.X, ePos.Y + this._width -1);
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
                    pt = new MyPoint(ePos.X + this._height -1, ePos.Y);
                    pt2 = new MyPoint(ePos.X + this._height - 1, ePos.Y + this._width - 1);
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
                    pt = new MyPoint(ePos.X , ePos.Y);
                    pt2 = new MyPoint(ePos.X + this._height -1  , ePos.Y);
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
                    pt = new MyPoint(ePos.X, ePos.Y + this._width - 1);
                    pt2 = new MyPoint(ePos.X + this._height - 1, ePos.Y +   this._width - 1);
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
        public bool HaveShot(MyPoint posLedarL, MyPoint posLedarR)
        {
                var subset = from s in GlobalDataStatic.BattleGroundCollection
                             where (s as HPElement) != null
                             select s;

                //если есть попадние, то двигаться нельзя
                foreach (HPElement s in subset)
                {
                    bool result = s.HaveHit(posLedarL, posLedarR);

                    if (result)
                    {                
                            s.GetDamage(_damage);
                            switch (s)
                            {
                                case Tank:
                                if (s.HP <= 0)

                                    sound = SoundsEnum.shotTargetSound;
                                    break;
                                case LocationGun:
                                    sound = SoundsEnum.shotTargetSound;
                                break;
    
                                case BlockFerum:                                
                                case TankOfDistroy:
                                    sound = SoundsEnum.ferumSoung;                                    
                                    break;
    
                                 case Block:
                                    sound = SoundsEnum.rockSound;                                   
                                    break;
                            }

                        if (SoundEvent != null) SoundEvent(sound);//играть звук

                        return true;
//                                       
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
