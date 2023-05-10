using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Server.Model
{
    public class Bunker : Block
    {
        public event Action BunkerDestroy;

        protected Bunker() { }
        public Bunker(Point ePos):base(ePos)
        {
            _skin = SkinsEnum.PictureBunker;
            IsPlayer = true;
        }

        public override void GetDamage(int damage)
        {
            HP -= damage;

            Task.Factory.StartNew(() => GetDamageView());
        }

        protected override void GetDamageView()
        {
                switch (HP)
                {
                    case 2:
                        _skin = SkinsEnum.PictureBunker2;
                        break;

                    case 1:
                        _skin = SkinsEnum.PictureBunker3;
                        break;

                    case (<= 0): //если нет хп, то объект уничтожается
                        DistroyMy();
                        BunkerDestroy();//сообщает в Main о своем разрушение
                        break;
                }

        }

    }
}
