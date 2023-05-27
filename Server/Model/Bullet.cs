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
            if (!GlobalDataStatic.BattleGroundCollection.Contains(this)) tTimerToFire.Stop();

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
                        X -= 5;
                    else
                    {
                        Task.Factory.StartNew(DistroyMy);
                    }
                    break;
                //НИЗ
                case VectorEnum.Down:
                    pt = new MyPoint(X + this._height -1, Y);
                    pt2 = new MyPoint(X + this._height - 1, Y + this._width - 1);
                    haveHit = HaveShot(pt, pt2);
                    if ((X <= 720 - 11.5) && (haveHit == false))
                        X += 5;
                    else
                    {
                        Task.Factory.StartNew(DistroyMy);
                    }
                    break;
                //ЛЕВО
                case VectorEnum.Left:
                    pt = new MyPoint(X , Y);
                    pt2 = new MyPoint(X + this._height -1  , Y);
                    haveHit = HaveShot(pt, pt2); 
                    if ((Y >= 1.5) && (haveHit == false))
                        Y -= 5;
                    else
                    {
                        Task.Factory.StartNew(DistroyMy);
                    }
                    break;
                //ПРАВО
                case VectorEnum.Right:
                    pt = new MyPoint(X, Y + this._width - 1);
                    pt2 = new MyPoint(X + this._height - 1, Y +   this._width - 1);
                    haveHit = HaveShot(pt, pt2);
                    if ((Y <= 1320 - 11.5) && (haveHit == false))
                        Y += 5;
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

                        SoundEvent?.Invoke(sound);//играть звук

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
            StopEvent();
            SoundEvent = null;
            GlobalDataStatic.BattleGroundCollection.Remove(this);                                   
        }
    }
}
