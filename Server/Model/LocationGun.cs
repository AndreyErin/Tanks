using System.Linq;



namespace Server.Model
{
    public class LocationGun : HPElement, ISoundsObjects
    {
        //интерфейс ISoundsObjects
        public SoundsEnum sound { get; set; }
        public event ISoundsObjects.SoundDeleg? SoundEvent;

        protected VectorEnum Vec = VectorEnum.Top;
        protected System.Timers.Timer timerRotation = new System.Timers.Timer();
        protected int _damage;
        protected HPElement? target = null;

        public LocationGun(MyPoint pos, int damage)
        {
            ePos = pos;
            _width = 30;
            _height = 30;
            _damage = damage;
            Skin = SkinsEnum.PictureLocationGun1;

            timerRotation.Interval = 500;
            timerRotation.Elapsed += GunAutoRotation;
            timerRotation.EndInit();
            timerRotation.Start();            
        }

        //Таймер повороты - поиск врага
        protected void GunAutoRotation(object sender, System.Timers.ElapsedEventArgs e)
        {
            //если объект удален с карты, то останавливаем таймер
            if (!GlobalDataStatic.BattleGroundCollection.Contains(this))
            {
                timerRotation.Stop();
            }

                //стрельба(ограничение видимости 120)
                MyPoint pt;
                MyPoint pt2;
                bool enemy = false;
            switch (Vec)
            {
                    //ВЕРХ
                case VectorEnum.Top:
                    pt = new MyPoint(ePos.X - 29, ePos.Y + 9);
                    pt2 = new MyPoint(ePos.X - 29, ePos.Y + 19);

                    //если нет попадания продолжаем перечислять
                    while ((CanTarget(pt, pt2) == false) && (pt.X > 29) && (pt.X > (ePos.X - 120)))
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
                    pt = new MyPoint(ePos.X + 58, ePos.Y + 9);
                    pt2 = new MyPoint(ePos.X + 58, ePos.Y + 19);

                    while ((CanTarget(pt, pt2) == false) && (pt.X < (720 - 29)) && (pt.X < (ePos.X + 120)))
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
                    pt = new MyPoint(ePos.X + 9, ePos.Y - 29);
                    pt2 = new MyPoint(ePos.X + 19, ePos.Y - 29);

                    while ((CanTarget(pt, pt2) == false) && (pt.Y > 29) && (pt.Y > (ePos.Y - 120)))
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
                    pt = new MyPoint(ePos.X + 9, ePos.Y + 58);
                    pt2 = new MyPoint(ePos.X + 19, ePos.Y + 58);

                    while ((CanTarget(pt, pt2) == false) && (pt.Y < (1320 - 29)) && (pt.Y < (ePos.Y + 120)))
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

                //если врага нет, то вращаем пушку
            if (enemy == false)
            {                
                //вращение пушки
                switch (Vec)
                {
                    case VectorEnum.Top:
                        Vec = VectorEnum.Right;                            
                        break;
                    case VectorEnum.Down:
                        Vec = VectorEnum.Left;                            
                        break;
                    case VectorEnum.Left:
                        Vec = VectorEnum.Top;
                        break;
                    case VectorEnum.Right:
                        Vec = VectorEnum.Down;
                        break;
                }
            }

        }

        //выстрел
        protected void ToFire()
        {
            //огонь. пуля стреляет сразу при создание объекта
            Bullet bullet = new Bullet(Vec, ePos, _damage);
            sound = SoundsEnum.shotSoung;
            if (SoundEvent != null) SoundEvent(sound);

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

        protected bool CanTargetEnemy(MyPoint posLedarL, MyPoint posLedarR)
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

        //получение урона
        public override void GetDamage(int damage)
        {
            HP -= damage;

            GetDamageView(HP);
        }

        protected void GetDamageView(int hp) 
        {
                switch (HP)
                {
                    case 2:
                        Skin = SkinsEnum.PictureLocationGun2;
                        break;

                    case 1:
                        Skin = SkinsEnum.PictureLocationGun3;
                        break;

                    case (<= 0): //если нет хп, то объект уничтожается
                        DistroyMy();
                        break;
                }
        }

        //уничтожение этого экземпляра
        protected override void DistroyMy()
        {
            timerRotation.Stop();
            base.DistroyMy();
        }
    }
}
