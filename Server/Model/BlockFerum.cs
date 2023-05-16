using System;

//ред
namespace Server.Model
{
    //железный блок
    internal class BlockFerum : Block
    {
        public BlockFerum(MyPoint ePos) : base(ePos)
        {
            Skin = SkinsEnum.PictureBlockFerum1;
            HP = 90;
        }

        //получение урона объктом
        public override void GetDamage(int damage) 
        {
            HP -= damage;
            GetDamageView();
        }

        protected override void GetDamageView()
        {
                if (HP <= 60 && HP > 30) { Skin = SkinsEnum.PictureBlockFerum2; }
                if (HP <= 30 && HP > 0) { Skin = SkinsEnum.PictureBlockFerum3; }
                if (HP <= 0) { DistroyMy(); } //если нет хп, то объект уничтожается
        }
    }
}
