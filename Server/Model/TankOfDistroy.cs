
namespace Server.Model
{
    public class TankOfDistroy : Block
    {
        public TankOfDistroy()
        {
            //добавлен в стек
        }

        public void InitElement(MyPoint pos, VectorEnum vector, int teer, double speed)
        {
            InitElementBase(pos);
            
            _height = 30;
            _width = 30;

            VectorElement = vector;

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
            
            GlobalDataStatic.StackLoot.Pop().InitElement(new MyPoint(X, Y));
            base.DistroyMy();
        }
    }
}
