using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tanks.Model
{
    public class Bunker : Block
    {
        public event Action BunkerDestroy;

        protected Bunker() { }
        public Bunker(Point ePos):base(ePos)
        {
            Source = Map.PictureBunker;
            IsPlayer = true;
        }

        public override void GetDamage(int damage)
        {


            HP -= damage;

            Task.Factory.StartNew(() => GetDamageView());
        }

        protected override void GetDamageView()
        {
            Action action = () =>
            {
                switch (HP)
                {
                    case 2:
                        Source = Map.PictureBunker2;
                        break;

                    case 1:
                        Source = Map.PictureBunker3;
                        break;

                    case (<= 0): //если нет хп, то объект уничтожается
                        DistroyMy();
                        BunkerDestroy();//сообщает в Main о своем разрушение
                        break;
                }
            };
            GlobalDataStatic.MainDispatcher.Invoke(action);
        }

    }
}
