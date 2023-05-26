using System;
using System.Windows;

namespace Server.Model
{
    public class Bunker : Block
    {
        public event Action BunkerDestroy;

        protected Bunker() { }
        public Bunker(MyPoint ePos):base(ePos)
        {
            HP = 3;
            Skin = SkinsEnum.PictureBunker;
            IsPlayer = true;
            if(this as BunkerEnamy == null) AddMe();
        }

        public override void GetDamage(int damage)
        {
            HP -= damage;
            GetDamageView();
        }

        protected override void GetDamageView()
        {
                switch (HP)
                {
                    case 2:
                        Skin = SkinsEnum.PictureBunker2;
                        break;

                    case 1:
                        Skin = SkinsEnum.PictureBunker3;
                        break;

                    case (<= 0): //если нет хп, то объект уничтожается
                        DistroyMy();
                        BunkerDestroy?.Invoke();//сообщает в Main о своем разрушение
                        break;
                }

        }

    }
}
