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
            _skin = SkinsEnum.PictureBunker;
            IsPlayer = true;
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
                        _skin = SkinsEnum.PictureBunker2;
                        break;

                    case 1:
                        _skin = SkinsEnum.PictureBunker3;
                        break;

                    case (<= 0): //если нет хп, то объект уничтожается
                        DistroyMy();
                        BunkerDestroy();//сообщает в Main о своем разрушение
                        break;
                }

        }

    }
}
