using System.Windows.Media;

namespace Server.Model
{
    public class TankOfDistroy : Block
    {
        public VectorEnum Vector { get; set; }

        public TankOfDistroy(System.Windows.Point pos, VectorEnum vector, int teer, double speed) :base(pos)
        {
            Vector = vector;
            _height = 30;
            _width = 30;

            switch (teer)
            {
                case 1:
                    _skin = SkinsEnum.PictureTankOfDestroy1;
                    break;
                case 2:
                    _skin = SkinsEnum.PictureTankOfDestroy2;
                    break;
                case 3:
                case 4:
                    _skin = SkinsEnum.PictureTankOfDestroy3;
                    break;
            }

            switch (speed)
            {
                case 2.0:
                    _skin = SkinsEnum.PictureTankOfDestroySpeed1;
                    break;
                case 2.5:
                    _skin = SkinsEnum.PictureTankOfDestroySpeed2;
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
        }


        protected override void DistroyMy()
        {
            base.DistroyMy();
            Loot loot = new Loot(ePos);
        }
    }
}
