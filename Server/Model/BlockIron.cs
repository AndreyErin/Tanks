using System;


namespace Server.Model
{
    public class BlockIron : Block
    {
        protected BlockIron() { }
        public BlockIron(MyPoint pos):base(pos)
        {
            
            Skin = SkinsEnum.PictureBlock1;
            AddMe();
        }

        protected override void GetDamageView()
        {
            switch (HP)
            {
                case 2:
                    Skin = SkinsEnum.PictureBlock2;
                    break;

                case 1:
                    Skin = SkinsEnum.PictureBlock3;
                    break;

                case (<= 0): //если нет хп, то объект уничтожается
                    DistroyMy();
                    break;
            }
        }
    }
}
