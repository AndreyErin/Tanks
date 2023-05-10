using System;


//ред
namespace Server.Model
{
    public class Block : WorldElement
    {               
        protected Block() { }
        public Block(System.Windows.Point ePos)
        {           
            Source = SkinsEnum.PictureBlock1;

            Width = 40;
            Height = 40;
            _ePos = ePos;
            HP = 3;

            GlobalDataStatic.BattleGroundCollection.Add(this);
        }

        //получение урона объктом
        public override void GetDamage(int damage) 
        {            
            HP -= damage;
            GetDamageView();           
        }

        protected virtual void GetDamageView() 
        {
            switch (HP)
            {
                case 2:
                    Source = SkinsEnum.PictureBlock2;
                    break;
            
                case 1:
                    Source = SkinsEnum.PictureBlock3;
                    break;
            
                case (<= 0): //если нет хп, то объект уничтожается
                    DistroyMy();            
                    break;
            }
        }
        
    }
}
