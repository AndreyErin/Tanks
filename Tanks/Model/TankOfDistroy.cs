using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tanks.Model
{
    public class TankOfDistroy : Block
    {
        public TankOfDistroy(System.Windows.Point pos, VectorEnum vector, int teer, double speed) :base(pos)
        {
            //IsPlayer = true;
            //Source = Map.PictureTankOfDestroy1;

            switch (teer)
            {
                case 1:
                    Source = Map.PictureTankOfDestroy1;
                    break;
                case 2:
                    Source = Map.PictureTankOfDestroy2;
                    break;
                case 3:
                case 4:
                    Source = Map.PictureTankOfDestroy3;
                    break;
            }

            switch (speed)
            {
                case 2.0:
                    Source = Map.PictureTankOfDestroySpeed1;
                    break;
                case 2.5:
                    Source = Map.PictureTankOfDestroySpeed2;
                    break;
            }





            Height = 30;
            Width = 30;

            switch (vector)
            {
                case VectorEnum.Top:
                    this.LayoutTransform = new RotateTransform(0); //лишнее, просто для порядка
                    break;
                case VectorEnum.Down:
                    this.LayoutTransform = new RotateTransform(180);
                    break;
                case VectorEnum.Left:
                    this.LayoutTransform = new RotateTransform(270);
                    break;
                case VectorEnum.Right:
                    this.LayoutTransform = new RotateTransform(90);
                    break;
            }
        }

        //получение урона объктом
        public override void GetDamage(int damage)
        {
            HP -= damage;
            if (HP <= 0)
            {
                DistroyMy();
            }
//            switch (HP)
//            {
//                case 2:
//                    Source = Map.PictureTankOfDestroy2;
//                    break;
//                case 1:
//                    Source = Map.PictureTankOfDestroy3;
//                    break;
//                case (<= 0):
//                    DistroyMy();
//                    break;
//            }
        }


        protected override void DistroyMy()
        {
            base.DistroyMy();
            Loot loot = new Loot(_ePos);
        }
    }
}
