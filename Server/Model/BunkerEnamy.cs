using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Server.Model
{
    public class BunkerEnamy : Bunker
    {
        public event Action BunkerDestroy;

        protected BunkerEnamy() { }
        public BunkerEnamy (Point ePos) : base(ePos)
        {
            Source = Map.PictureBunkerEnamy;
            IsPlayer = false;
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
                        Source = Map.PictureBunkerEnamy2;
                        break;

                    case 1:
                        Source = Map.PictureBunkerEnamy3;
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

