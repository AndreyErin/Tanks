

namespace Server.Model
{
    public class Tree : WorldElement
    {
        public System.Windows.Point Pos;

        protected Tree() { }
        public Tree(System.Windows.Point ePos)
        {
            Pos = ePos;
            _width = 40;
            _height = 40;           
            _skin = SkinsEnum.PictureWood1;

            GlobalDataStatic.BattleGroundCollection.Add(this);          
        }
    }
}
