using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Tanks.Model
{
    public class LocationGun : Model.Block
    {
        protected MediaPlayer _player = new MediaPlayer();

        protected VectorEnum Vec = VectorEnum.Top;
        protected System.Timers.Timer timerRotation = new System.Timers.Timer();
        protected int _damage;
        protected DoubleAnimation _spinerAnimation;
        protected RotateTransform rt = new RotateTransform();

        public LocationGun(System.Windows.Point pos, int damage) : base(pos)
        {
            Width = 30;
            Height = 30;
            _damage = damage;

            Source = Map.PictureLocationGun1;
            _spinerAnimation = new DoubleAnimation();
            _spinerAnimation.Duration = TimeSpan.FromMilliseconds(500);

            
            rt.Angle = 90;
            rt.CenterX = 15;
            rt.CenterY = 15;
            this.RenderTransform = rt;


            timerRotation.Interval = 500;
            timerRotation.Elapsed += GunAutoRotation;
            timerRotation.EndInit();
            timerRotation.Start();
            _damage = damage;
        }

        //Таймер повороты - поиск врага
        protected void GunAutoRotation(object sender, System.Timers.ElapsedEventArgs e)
        {
            //если объект удален с карты, то останавливаем таймер
            if (this.Parent != GlobalDataStatic.cnvMap1)
            {
                timerRotation.Stop();
            }

            Action action = () =>
            {

                //стрельба(ограничение видимости 120)
                System.Windows.Point pt;
                System.Windows.Point pt2;
                bool enemy = false;
                switch (Vec)
                {
                    //ВЕРХ
                    case VectorEnum.Top:
                        pt = new System.Windows.Point(_ePos.X - 29, _ePos.Y + 9);
                        pt2 = new System.Windows.Point(_ePos.X - 29, _ePos.Y + 19);

                        //если нет попадания продолжаем перечислять
                        while ((CanTarget(pt, pt2) == false) && (pt.X > 29) && (pt.X > (_ePos.X - 120)))
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
                        pt = new System.Windows.Point(_ePos.X + 58, _ePos.Y + 9);
                        pt2 = new System.Windows.Point(_ePos.X + 58, _ePos.Y + 19);

                        while ((CanTarget(pt, pt2) == false) && (pt.X < (720 - 29)) && (pt.X < (_ePos.X + 120)))
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
                        pt = new System.Windows.Point(_ePos.X + 9, _ePos.Y - 29);
                        pt2 = new System.Windows.Point(_ePos.X + 19, _ePos.Y - 29);

                        while ((CanTarget(pt, pt2) == false) && (pt.Y > 29) && (pt.Y > (_ePos.Y - 120)))
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
                        pt = new System.Windows.Point(_ePos.X + 9, _ePos.Y + 58);
                        pt2 = new System.Windows.Point(_ePos.X + 19, _ePos.Y + 58);

                        while ((CanTarget(pt, pt2) == false) && (pt.Y < (1320 - 29)) && (pt.Y < (_ePos.Y + 120)))
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
                            _spinerAnimation.From = 0;
                            _spinerAnimation.To = 90;                                                                                   
                            rt.BeginAnimation(RotateTransform.AngleProperty , _spinerAnimation);
                            break;
                        case VectorEnum.Down:
                            Vec = VectorEnum.Left;                            
                            _spinerAnimation.From = 180;
                            _spinerAnimation.To = 270;
                            rt.BeginAnimation(RotateTransform.AngleProperty, _spinerAnimation);
                            break;
                        case VectorEnum.Left:
                            Vec = VectorEnum.Top;
                            _spinerAnimation.From = - 90;
                            _spinerAnimation.To = 0;
                            rt.BeginAnimation(RotateTransform.AngleProperty, _spinerAnimation);
                            break;
                        case VectorEnum.Right:
                            Vec = VectorEnum.Down;
                            _spinerAnimation.From = 90;
                            _spinerAnimation.To = 180;
                            rt.BeginAnimation(RotateTransform.AngleProperty, _spinerAnimation);
                            break;
                    }
                }
            };
            GlobalDataStatic.MainDispatcher.Invoke(action);
        }

        //выстрел
        protected void ToFire()
        {
                //огонь. пуля стреляет сразу при создание объекта
                Bullet bullet = new Bullet(Vec, _ePos, _damage);
            _player.Open(GlobalDataStatic.shotSoung);
            _player.Position = TimeSpan.Zero;
            _player.Play();
        }
        protected WorldElement target = null;

        //проверяем ближайшую цель на пути
        protected bool CanTarget(System.Windows.Point posLedarL, System.Windows.Point posLedarR)
        {
            UIElementCollection collection = GlobalDataStatic.cnvMap1.Children;
            var subset = from UIElement s in collection
                         where ((s as WorldElement) != null)
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

        //получение урона
        public override void GetDamage(int damage)
        {
            HP -= damage;

            Task.Factory.StartNew(() => GetDamageView(HP));
        }

        protected void GetDamageView(int hp) 
        {
            Action action = () =>
            {
                switch (HP)
                {
                    case 2:
                        Source = Map.PictureLocationGun2;
                        break;

                    case 1:
                        Source = Map.PictureLocationGun3;
                        break;

                    case (<= 0): //если нет хп, то объект уничтожается
                        DistroyMy();
                        break;
                }
            };
            GlobalDataStatic.MainDispatcher.Invoke(action);
        }

        //уничтожение этого экземпляра
        protected override void DistroyMy()
        {
            timerRotation.Stop();
            GlobalDataStatic.cnvMap1.Children.Remove(this);
            _player.Close();
        }
    }
}
