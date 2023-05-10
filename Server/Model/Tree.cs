
//ред
namespace Server.Model
{
    public class Tree : Img
    {
        public System.Windows.Point Pos;

        protected Tree() { }
        public Tree(System.Windows.Point ePos)
        {
            Pos = ePos;
            Width = 40;
            Height = 40;           
            Source = SkinsEnum.PictureWood1;

            GlobalDataStatic.BattleGroundCollection.Add(this);          
        }
    }
}
