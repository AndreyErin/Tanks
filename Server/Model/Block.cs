using System;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace Server.Model
{
    public class Block : WorldElement
    {               
        //protected MediaPlayer _player;

        protected Block() { }
        public Block(System.Windows.Point ePos)
        {
            

            Source = Map.PictureBlock1;

            Width = 40;
            Height = 40;
            _ePos = ePos;
            HP = 3;

            Canvas.SetTop(this, _ePos.X);
            Canvas.SetLeft(this, _ePos.Y);

            GlobalDataStatic.cnvMap1.Children.Add(this );
        }

        //получение урона объктом
        public override void GetDamage(int damage) 
        {            
            HP -= damage;

            Task.Factory.StartNew(() => GetDamageView());
            
        }

        protected virtual void GetDamageView() 
        {
            Action action = () =>
            {
                switch (HP)
                {
                    case 2:
                        Source = Map.PictureBlock2;
                        break;

                    case 1:
                        Source = Map.PictureBlock3;
                        break;

                    case (<= 0): //если нет хп, то объект уничтожается
                        DistroyMy();

                        break;
                }
            };
            GlobalDataStatic.MainDispatcher.Invoke(action);
        }
        
    }
}
