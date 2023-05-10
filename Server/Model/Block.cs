using System;


//ред
namespace Server.Model
{
    public class Block : HPElement
    {               
        protected Block() { }
        public Block(System.Windows.Point Pos)
        {           
            _skin = SkinsEnum.PictureBlock1;

            _width = 40;
            _height = 40;
            ePos = Pos;
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
                    _skin = SkinsEnum.PictureBlock2;
                    break;
            
                case 1:
                    _skin = SkinsEnum.PictureBlock3;
                    break;
            
                case (<= 0): //если нет хп, то объект уничтожается
                    DistroyMy();            
                    break;
            }
        }
        
    }
}
