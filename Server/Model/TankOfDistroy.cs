using System.Windows.Media;

namespace Server.Model
{
    public class TankOfDistroy : Block
    {
        public VectorEnum Vector { get; set; }

        public TankOfDistroy(MyPoint pos, VectorEnum vector, int teer, double speed) :base(pos)
        {
            Vector = vector;
            _height = 30;
            _width = 30;

            switch (teer)
            {
                case 1:
                    Skin = SkinsEnum.PictureTankOfDestroy1;
                    break;
                case 2:
                    Skin = SkinsEnum.PictureTankOfDestroy2;
                    break;
                case 3:
                case 4:
                    Skin = SkinsEnum.PictureTankOfDestroy3;
                    break;
            }

            switch (speed)
            {
                case 2.0:
                    Skin = SkinsEnum.PictureTankOfDestroySpeed1;
                    break;
                case 2.5:
                    Skin = SkinsEnum.PictureTankOfDestroySpeed2;
                    break;
            }
            AddMe();
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
            
            Loot loot = new Loot(new MyPoint(X, Y));
            base.DistroyMy();
        }
    }
}
