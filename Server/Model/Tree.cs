

namespace Server.Model
{
    public class Tree : WorldElement
    {
        protected Tree() { }
        public Tree(MyPoint pos)
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
