using System;


namespace Server.Model
{
    public class BlockRock : Block
    {
        public BlockRock() 
        {
            //прописать добавление в стак
        }

        public void InitElement(MyPoint ePos , bool isfriend = false) 
        {
            InitElementBase(ePos);

            Skin = SkinsEnum.PictureBlock1;
            IsPlayer = isfriend;

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
