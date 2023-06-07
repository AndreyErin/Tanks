using System;

//ред
namespace Server.Model
{
    //железный блок
    public class BlockFerum : Block
    {
        public BlockFerum()
        {
            //прописать добавление в стак
        }

        //инициализация
        public void InitElement(MyPoint ePos) 
        {
            InitElementBase(ePos);
            Skin = SkinsEnum.PictureBlockFerum1;
            HP = 90;
            AddMe();
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
                if (HP <= 0) {DistroyMy();} //если нет хп, то объект уничтожается
        }
    }
}
