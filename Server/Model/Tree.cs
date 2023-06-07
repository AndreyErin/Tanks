

namespace Server.Model
{
    public class Tree : WorldElement
    {
        public Tree() 
        {
            //добавлен в стек
        }
        public void InitElement(MyPoint pos)
        {
            X = pos.X;
            Y = pos.Y;
            _width = 40;
            _height = 40;           
            Skin = SkinsEnum.PictureWood1;

            AddMe();          
        }
    }
}
