using System;
using System.Linq;
using System.Windows;

namespace Server.Model
{
    public class LocationGun : HPElement, ISoundsObjects
    {
        //интерфейс ISoundsObjects
        public SoundsEnum sound { get; set; }
        public event ISoundsObjects.SoundDeleg? SoundEvent;

        
        protected System.Timers.Timer timerRotation = new System.Timers.Timer(500);
        protected int _damage;
        protected HPElement? target = null;

        public LocationGun()
        {
            //добавлен в стек
            timerRotation.Elapsed += GunAutoRotation;
            SoundEvent += GlobalDataStatic.Controller.SoundOfElement;
            
        }

        public void InitElement(MyPoint pos, int damage)
        {
            HP = 3;
            VectorElement = VectorEnum.Top;
            X = pos.X;
            Y = pos.Y;
            _width = 30;
            _height = 30;
            _damage = damage;
            Skin = SkinsEnum.PictureLocationGun1;
            
                    
            timerRotation.Start();
            AddMe();
        }

        //Таймер повороты - поиск врага
        protected void GunAutoRotation(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Action action = () =>
            //{

                //если объект удален с карты, то останавливаем таймер
                if (!GlobalDataStatic.BattleGroundCollection.ContainsKey(ID))
                {
                    timerRotation.Stop();
                    return;
                }

                //стрельба(ограничение видимости 120)
                MyPoint pt;
                MyPoint pt2;
                bool enemy = false;
                switch (VectorElement)
                {
                    //ВЕРХ
                    case VectorEnum.Top:
                        pt = new MyPoint(X - 29, Y + 9);
                        pt2 = new MyPoint(X - 29, Y + 19);

                        //если нет попадания продолжаем перечислять
                        while ((CanTarget(pt, pt2) == false) && (pt.X > 29) && (pt.X > (X - 120)))
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
                        pt = new MyPoint(X + 58, Y + 9);
                        pt2 = new MyPoint(X + 58, Y + 19);

                        while ((CanTarget(pt, pt2) == false) && (pt.X < (720 - 29)) && (pt.X < (X + 120)))
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
                        pt = new MyPoint(X + 9, Y - 29);
                        pt2 = new MyPoint(X + 19, Y - 29);

                        while ((CanTarget(pt, pt2) == false) && (pt.Y > 29) && (pt.Y > (Y - 120)))
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
                        pt = new MyPoint(X + 9, Y + 58);
                        pt2 = new MyPoint(X + 19, Y + 58);

                        while ((CanTarget(pt, pt2) == false) && (pt.Y < (1320 - 29)) && (pt.Y < (Y + 120)))
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
                    switch (VectorElement)
                    {
                        case VectorEnum.Top:
                            VectorElement = VectorEnum.Right;
                            break;
                        case VectorEnum.Down:
                            VectorElement = VectorEnum.Left;
                            break;
                        case VectorEnum.Left:
                            VectorElement = VectorEnum.Top;
                            break;
                        case VectorEnum.Right:
                            VectorElement = VectorEnum.Down;
                            break;
                    }
                }
            //};
            //GlobalDataStatic.Controller.Dispatcher.Invoke(action);
            //MessageBox.Show(VectorElement.ToString());
        }

        //выстрел
        protected void ToFire()
        {
            //огонь. пуля стреляет сразу при создание объекта
             GlobalDataStatic.StackBullet.Pop().InitElement(VectorElement, new MyPoint(X, Y), _damage);
             

            sound = SoundsEnum.shotSoung;
            SoundEvent?.Invoke(sound);

        }
              
        //проверяем ближайшую цель на пути
        protected bool CanTarget(MyPoint posLedarL, MyPoint posLedarR)
        {            
            var subset = from s in GlobalDataStatic.BattleGroundCollection
                         where (s.Value as HPElement) != null
                         select s;

            foreach (var s in subset)
            {
                bool result = ((HPElement)s.Value).HaveHit(posLedarL, posLedarR);

                if (result)
                {
                    target = (HPElement)s.Value; //цель найдена
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
            //timerRotation.Elapsed -= GunAutoRotation;
            base.DistroyMy();
            
        }
    }
}
