
namespace Server.Model
{
    public class Block : HPElement
    {               
        protected Block() { }
        public Block(MyPoint Pos)
        {           
            _width = 40;
            _height = 40;
            ePos = Pos;

        }

        //получение урона объктом
        public override void GetDamage(int damage) 
        {            
            HP -= damage;
            GetDamageView();           
        }

        protected virtual void GetDamageView(){}
        
    }
}
