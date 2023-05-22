using System;


namespace Server.Model
{
    public class BunkerEnamy : Bunker
    {
        public event Action BunkerDestroy;

        protected BunkerEnamy() { }
        public BunkerEnamy (MyPoint ePos) : base(ePos)
        {
            Skin = SkinsEnum.PictureBunkerEnamy;
            IsPlayer = false;
            AddMe();
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
                        Skin = SkinsEnum.PictureBunkerEnamy2;
                        break;

                    case 1:
                        Skin = SkinsEnum.PictureBunkerEnamy3;
                        break;

                    case (<= 0): //если нет хп, то объект уничтожается
                        DistroyMy();
                        BunkerDestroy();//сообщает в Main о своем разрушение
                        break;
                }
        }
    }
}

